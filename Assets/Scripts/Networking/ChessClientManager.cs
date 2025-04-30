using System.Net;
using EndoAshu.Chess.Client;
using EndoAshu.Chess.User;

public static class ChessClientManager {
    private static readonly IPEndPoint _host = new IPEndPoint(IPAddress.Loopback, 1557);
    private static ChessClient _client = new ChessClient(_host);

#nullable enable
    public static ChessClient? UnsafeClient {
        get => _client.IsConnected ? _client : null;
    }
#nullable disable

    public static ChessClient Client {
        get {
            if (!_client.IsConnected) {
                _client.Dispose();
                _client = new ChessClient(_host);
                _client.Start();
            }
            return _client;
        }
    }
}