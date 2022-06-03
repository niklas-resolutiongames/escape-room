using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using RG.EscapeRoom.Model.Rooms;
using RG.EscapeRoomProtocol;
using RG.EscapeRoomServer.Server;

public class EscapeRoomSocketServer
{
    private readonly int port;
    private readonly string pathToRoomDefinition;
    private readonly RoomDefinitionParser roomDefinitionParser;
    private readonly ILogger logger;
    private readonly CancellationTokenSource cancellationTokenSource;
    private readonly ProtocolSerializer protocolSerializer;
    private readonly MessageSender messageSender;
    private readonly HashSet<Client> allConnectedClients;
    private int timeoutsUntilStop;
    private bool isRunning = false;
    private Dictionary<SocketAddress, Client> clients = new Dictionary<SocketAddress, Client>();
    private Dictionary<Client, ServerMessageReceiver> receivers = new Dictionary<Client, ServerMessageReceiver>();
    private Queue<Client> clientsToProcess = new Queue<Client>();
    private RoomDefinition roomDefintion;

    public EscapeRoomSocketServer(int port, string pathToRoomDefinition, RoomDefinitionParser roomDefinitionParser,
        ILogger logger, CancellationTokenSource cancellationTokenSource, ProtocolSerializer protocolSerializer,
        MessageSender messageSender, int timeoutsUntilStop, HashSet<Client> allConnectedClients)
    {
        this.port = port;
        this.pathToRoomDefinition = pathToRoomDefinition;
        this.roomDefinitionParser = roomDefinitionParser;
        this.logger = logger;
        this.cancellationTokenSource = cancellationTokenSource;
        this.protocolSerializer = protocolSerializer;
        this.messageSender = messageSender;
        this.timeoutsUntilStop = timeoutsUntilStop;
        this.allConnectedClients = allConnectedClients;
    }

    public async Task Start()
    {
        try
        {
            ReadConfig();
            var listener = Task.Run(() => RunListener());
            var receiver = Task.Run(() => RunReceiver());
        }
        catch (Exception e)
        {
            logger.Error("Main loop got error", e);
        }
    }

    private void ReadConfig()
    {
        string json = File.ReadAllText(pathToRoomDefinition);
        roomDefintion = roomDefinitionParser.Parse(json);
    }

    public void RunListener()
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
                int numberOfReceivedBytes = 0;
                try
                {
                    numberOfReceivedBytes = socket.ReceiveFrom(data, ref remote);
                }
                catch (SocketException e)
                {
                    if (e.SocketErrorCode == SocketError.TimedOut)
                    {
                        if (timeoutsUntilStop == 1)
                        {
                            cancellationTokenSource.Cancel();
                        }

                        timeoutsUntilStop--;
                    } else
                    {
                        throw e;
                    }
                }

                if (numberOfReceivedBytes > 0)
                {
                    var socketAddress = remote.Serialize();
                    Client client;
                    if (clients.ContainsKey(socketAddress))
                    {
                        client = clients[socketAddress];
                    }
                    else
                    {
                        client = new Client(remote);
                        var clientMessageReceiver = new ServerMessageReceiver(client, messageSender, roomDefintion);
                        client.Init();
                        clients[socketAddress] = client;
                        receivers[client] = clientMessageReceiver;
                        allConnectedClients.Add(client);
                        remote = CreateEndPoint();
                    }
                    logger.Info($"Client {socketAddress} sent {numberOfReceivedBytes} bytes: {ByteUtil.ByteArrayToString(data, 0, numberOfReceivedBytes)}");

                    client.ReceiveData(data,numberOfReceivedBytes);
                    clientsToProcess.Enqueue(client);
                }

                Task.Yield();
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

    public void RunReceiver()
    {
        while (!cancellationTokenSource.IsCancellationRequested)
        {
            if (clientsToProcess.Count > 0)
            {
                var client = clientsToProcess.Dequeue();
                MessageReceiver messageReceiver = receivers[client];
                protocolSerializer.DeserializeNextMessage(client.byteBuffer, messageReceiver);
            }

            Task.Yield();
        }
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

public class Client
{
    public readonly EndPoint endPoint;
    public ByteFifoBuffer byteBuffer;

    public Client(EndPoint endPoint)
    {
        this.endPoint = endPoint;
    }

    public void Init()
    {
        byteBuffer = new ByteFifoBuffer(1024);
    }

    public void ReceiveData(byte[] data, int numberOfBytes)
    {
        byteBuffer.Write(data, 0, numberOfBytes);
    }
}

public interface ILogger
{
    void Info(string message);
    void Error(string error, Exception exception);
}
