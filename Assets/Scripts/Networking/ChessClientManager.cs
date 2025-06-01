using System;
using System.Net;
using EndoAshu.Chess.Client;
using EndoAshu.Chess.User;
using Runetide.Util.Logging;
using UnityEngine;

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
                _client.Logger.OnLogging -= OnLog;
                _client.Dispose();
                _client = new ChessClient(_host);
#if UNITY_EDITOR
                _client.Logger.MinLevel = LogLevel.DEBUG;
#else
                _client.Logger.MinLevel = LogLevel.INFO;
#endif
                _client.Logger.OnLogging += OnLog;
                _client.Start();
            }
            return _client;
        }
    }

    private static void OnLog(ILogItem item)
    {
        switch(item.Level) {
            case LogLevel.ERROR:
                Debug.LogError(item.ToString());
                break;
            case LogLevel.WARN:
                Debug.LogWarning(item.ToString());
                break;
            default:
                Debug.Log(item.ToString());
                break;
        }
    }
}