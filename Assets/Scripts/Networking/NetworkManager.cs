using System;
using System.Net;
using EndoAshu.Net.Omen;
using EndoAshu.Net.Omen.Packet;

public class NetworkManager {
    public static EventHandler<IPacketTransformer> InitTransformer;

    public static SimpleClientRunner CreateClient(IPEndPoint remotePoint) {
        var client = new SimpleClientRunner(remotePoint);
        InitTransformer?.Invoke(null, client.PacketTransformer);
        return client;
    }

    public static SimpleServerRunner CreateServer(int port, int maxConnections = 100) {
        var server = new SimpleServerRunner(port, maxConnections);
        InitTransformer?.Invoke(null, server.PacketTransformer);
        return server;
    }
}