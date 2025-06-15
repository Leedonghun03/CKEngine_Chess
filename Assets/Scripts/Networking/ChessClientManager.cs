using System;
using System.Collections.Generic;
using System.Net;
using EndoAshu.Chess.Client;
using EndoAshu.Chess.Client.InGame;
using EndoAshu.Chess.InGame;
using EndoAshu.Chess.Room;
using EndoAshu.Chess.User;
using Runetide.Packet;
using Runetide.Util.Logging;
using UnityEngine;

public static class ChessClientManager {
    private static readonly IPEndPoint _host = new IPEndPoint(IPAddress.Loopback, 1557);
    private static ChessClient _client = new ChessClient(_host);

#nullable enable
    public static ChessClient? UnsafeClient {
        get => _client.IsConnected ? _client : null;
    }

    public static bool IsWinnerScene => GameEndData != null;

    public static ClientSideChessGameEndPacket? GameEndData { get; set; }
#nullable disable

    public static ChessClient Client
    {
        get
        {
            if (!_client.IsConnected)
            {
                _client.RemoveOnReceivePacketListener(OnGameEnd);
                _client.Logger.OnLogging -= OnLog;
                _client.Dispose();
                _client = new ChessClient(_host);
#if UNITY_EDITOR
                _client.Logger.MinLevel = LogLevel.TRACE;
#else
                _client.Logger.MinLevel = LogLevel.INFO;
#endif
                _client.Logger.OnLogging += OnLog;
                _client.AddOnReceivePacketListener(OnGameEnd);
                _client.Start();
            }
            return _client;
        }
    }

    private static void OnGameEnd(IPacket t)
    {
        if (t is ClientSideChessGameEndPacket pk)
        {
            GameEndData = pk;
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