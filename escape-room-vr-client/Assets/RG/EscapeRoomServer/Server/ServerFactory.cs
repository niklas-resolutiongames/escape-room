using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using RG.EscapeRoom.Model.Rooms;
using RG.EscapeRoomProtocol;

namespace RG.EscapeRoomServer.Server
{
    public class ServerFactory
    {
        private ILogger logger;
        private CancellationTokenSource cancellationTokenSource;

        public ServerFactory(ILogger logger, CancellationTokenSource cancellationTokenSource)
        {
            this.logger = logger;
            this.cancellationTokenSource = cancellationTokenSource;
        }

        public EscapeRoomSocketServer CreateServer(int port, string pathToRoomDefinition, int timeoutsUntilStop = 0)
        {
            var protocolSerializer = new ProtocolSerializer(new PrimitiveSerializer());
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            var allConnectedClients = new HashSet<Client>();
            var udpMessageSender = new UdpMessageSender(socket, protocolSerializer, allConnectedClients);
            udpMessageSender.Init();
            RoomDefinitionParser roomDefinitionParser = new RoomDefinitionParser();
            return new EscapeRoomSocketServer(port, pathToRoomDefinition, roomDefinitionParser, logger, cancellationTokenSource, protocolSerializer, udpMessageSender, timeoutsUntilStop, allConnectedClients);

        }
    }
}