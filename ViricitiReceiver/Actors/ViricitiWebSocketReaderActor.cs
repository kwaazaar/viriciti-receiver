using Akka.Actor;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ViricitiReceiver.Messages;

namespace ViricitiReceiver.Actors
{
    class ViricitiWebSocketReaderActor : ReceiveActor
    {
        private readonly Uri _url;
        private ClientWebSocket _cws;
        private byte[] _recvBuffer;
        private ArraySegment<byte> _receiveSegment;
        private IActorRef _messageProcessorActor;

        public ViricitiWebSocketReaderActor(Uri url, IActorRef messageProcessorActor)
        {
            _url = url;
            _messageProcessorActor = messageProcessorActor;

            ReceiveAsync<StartProcessing>((msg) => StartProcessing(msg));
            ReceiveAsync<ContinueProcessing>((msg) => ContinueProcessing(msg));
            ReceiveAsync<StopProcessing>((msg) => StopProcessing(msg));
        }

        private async Task<bool> StartProcessing(StartProcessing msg)
        {
            if (_cws == null)
            {
                var cancellationTokenSource = new CancellationTokenSource(msg.ConnectionTimeout);
                _cws = new ClientWebSocket();

                try
                {
                    await _cws.ConnectAsync(_url, cancellationTokenSource.Token);
                }
                catch (OperationCanceledException opEx)
                {
                    throw new TimeoutException($"No connection withing {msg.ConnectionTimeout.TotalSeconds} seconds", opEx);
                }

                _recvBuffer = new byte[1024 * 4];
                _receiveSegment = new ArraySegment<byte>(_recvBuffer);

                // Start reading messages
                Self.Tell(new ContinueProcessing());
            }

            return true;
        }

        private async Task<bool> StopProcessing(StopProcessing msg)
        {
            if (_cws != null)
            {
                if (_cws.State == WebSocketState.Open)
                {
                    var cancellationTokenSource = new CancellationTokenSource(msg.ConnectionTimeout);
                    await _cws.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, cancellationTokenSource.Token);
                }
                _cws.Dispose();
                _cws = null;
                _recvBuffer = null;
                _receiveSegment = null;
            }

            return true;
        }

        private async Task<bool> ContinueProcessing(ContinueProcessing msg)
        {
            WebSocketReceiveResult res = null;
            try
            {
                var cancellationTokenSource = new CancellationTokenSource(msg.ReceiveTimeout);
                res = await _cws.ReceiveAsync(_receiveSegment, cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                Continue();
                return true;
            }

            if (res.CloseStatus != null)
            {
                Continue();
                return true;
            }
            else if (res.Count > 0)
            {
                var message = Encoding.UTF8.GetString(_receiveSegment.Array, _receiveSegment.Offset, res.Count);
                _messageProcessorActor.Tell(new MessageReceived(message, DateTime.UtcNow));
            }
            Continue();
            return true;
        }

        private void Continue()
        {
            Self.Tell(new ContinueProcessing());
        }
    }
}
