using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using RG.EscapeRoomProtocol;
using RG.EscapeRoomProtocol.Messages;
using RG.EscapeRoomServer.Server;
using RG.Tests;
using UnityEngine;
using UnityEngine.TestTools;

namespace RG.EscapeRoomPlayModeTests.Server
{
    public class ConnectToServerTest
    {
        private EscapeRoomSocketServer escapeRoomSocketServer;
        private int port;
        private IPEndPoint clientEndPoint;
        private IPEndPoint serverEndPoint;
        private UnityTestLogger unityTestLogger;
        private CancellationTokenSource cancellationTokenSource;
        private ProtocolSerializer protocolSerializer;
        private TestMessageReceiver testMessageReceiver;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            port = 12345;
            unityTestLogger = new UnityTestLogger();
            cancellationTokenSource = new CancellationTokenSource();
            protocolSerializer = new ProtocolSerializer();
            testMessageReceiver = new TestMessageReceiver();
            var serverFactory = new ServerFactory(unityTestLogger, cancellationTokenSource);
            escapeRoomSocketServer = serverFactory.CreateServer(port, TestUtil.PathToFile("Assets/RG/EscapeRoomPlayModeTests/SingleLever/SingleLeverFactoryTestRoomDefinition.json"));
            escapeRoomSocketServer.Start();
            serverEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
            clientEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port+1);
            yield return null;
        }
        [UnityTest]
        public IEnumerator TestClientConnectReceivesLoadRoom()
        {
            var timeAtStart = Time.time;
            UdpClient client = new UdpClient(clientEndPoint);
            yield return SendMessage(client, new ClientConnectMessage(Encoding.Unicode.GetBytes("abc123")));
            var receiveTask = client.ReceiveAsync();
            while (receiveTask.Status < TaskStatus.RanToCompletion) { yield return null; }

            var resultBuffer = receiveTask.Result.Buffer;
            yield return ReceiveMessage(resultBuffer);
            cancellationTokenSource.Cancel();
            while (escapeRoomSocketServer.IsRunning() && Time.time - timeAtStart < 10) { yield return null; }
            Assert.IsFalse(escapeRoomSocketServer.IsRunning());
            Assert.AreEqual(1, testMessageReceiver.messages.Count);
            LoadRoomMessage loadRoomMessage = (LoadRoomMessage) testMessageReceiver.messages[0];
            Assert.AreEqual("SingleLeverTestScene", loadRoomMessage.roomDefinitionId);
        }

        private IEnumerator SendMessage(UdpClient client, ClientConnectMessage message)
        {
            ByteFifoBuffer buffer = new ByteFifoBuffer(1024);
            var numberOfBytes = protocolSerializer.SerializeMessage(message, buffer);
            var sendMessageTask = client.SendAsync(buffer.ReadAllAsArray(), numberOfBytes, serverEndPoint);
            while (sendMessageTask.Status < TaskStatus.RanToCompletion) { yield return null; }
        }

        private IEnumerator ReceiveMessage(byte[] buffer)
        {
            ByteFifoBuffer byteFifoBuffer = new ByteFifoBuffer(buffer.Length);
            byteFifoBuffer.Write(buffer, 0, buffer.Length);
            var receiveTask = protocolSerializer.DeserializeNextMessage(byteFifoBuffer, testMessageReceiver);
            while (receiveTask.Status < TaskStatus.RanToCompletion) { yield return null; }
        }
    }

}