using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class EscapeRoomSocketServer
{
    private readonly int port;
    private readonly ILogger logger;
    private readonly CancellationTokenSource cancellationTokenSource;
    private bool isRunning = false;
    private Dictionary<SocketAddress, Client> clients = new Dictionary<SocketAddress, Client>();

    public EscapeRoomSocketServer(int port, ILogger logger, CancellationTokenSource cancellationTokenSource)
    {
        this.port = port;
        this.logger = logger;
        this.cancellationTokenSource = cancellationTokenSource;
    }

    public async void Start()
    {   
        isRunning = true;
        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        socket.ReceiveTimeout = 1000;
        var ipEndPoint = CreateEndPoint();
        var remote = CreateEndPoint();
        byte[] data = new byte[1024];
        try
        {
            socket.Bind(ipEndPoint);
            while (!cancellationTokenSource.IsCancellationRequested)
            {
                logger.Info($"[Server] Waiting for data...");
                var numberOfReceivedBytes = socket.ReceiveFrom(data, ref remote);
                if (numberOfReceivedBytes > 0)
                {
                    var socketAddress = remote.Serialize();
                    Client client;
                    if (clients.ContainsKey(socketAddress))
                    {
                        logger.Info($"Returning client {socketAddress} sent {numberOfReceivedBytes} bytes");
                        client = clients[socketAddress];
                        socket.SendTo(data, numberOfReceivedBytes, SocketFlags.None, client.endPoint);
                        logger.Info($"[Server] sent {numberOfReceivedBytes} bytes");
                    }
                    else
                    {
                        logger.Info($"New client {socketAddress} sent {numberOfReceivedBytes} bytes");
                        client = new Client(remote);
                        client.Init();
                        clients[socketAddress] = client;
                        remote = CreateEndPoint();
                    }

                    client.ReceiveData(data,numberOfReceivedBytes);
                }

            }
        }
        catch (Exception e)
        {
            logger.Error("Error", e);
        } finally
        {
          socket.Close();  
        }

        isRunning = false;
        logger.Info($"[Server] shut down");
    }

    private EndPoint CreateEndPoint()
    {
        return new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
    }

    public bool IsRunning()
    {
        return isRunning;
    }
    
}

internal class Client
{
    public EndPoint endPoint;
    private byte[] buffer;
    private BufferedStream bufferedStream;

    public Client(EndPoint endPoint)
    {
        this.endPoint = endPoint;
    }

    public void Init()
    {
        buffer = new byte[1024];
        bufferedStream = new BufferedStream(new MemoryStream(buffer));
    }

    public void ReceiveData(byte[] data, int numberOfBytes)
    {
        bufferedStream.Write(data, 0, numberOfBytes);
    }
}

public interface ILogger
{
    void Info(string message);
    void Error(string error, Exception exception);
}
