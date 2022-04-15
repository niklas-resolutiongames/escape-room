    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using RG.EscapeRoomProtocol;
using RG.EscapeRoomProtocol.Messages;
using RG.EscapeRoomServer.Server;
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
            escapeRoomSocketServer = serverFactory.CreateServer(port);
            Task.Run(() => escapeRoomSocketServer.RunListener());
            Task.Run(() => escapeRoomSocketServer.RunReceiver());
            serverEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
            clientEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port+1);
            yield return null;
        }
        [UnityTest]
        public IEnumerator TestClientHello()
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
            Assert.AreEqual("abc123", loadRoomMessage.roomId);
        }

        private IEnumerator SendMessage(UdpClient client, ClientConnectMessage message)
        {
            byte[] data = new byte[1024];
            var memoryStream = new MemoryStream(data);
            var numberOfBytes = protocolSerializer.SerializeMessage(message, memoryStream);
            unityTestLogger.Info($"Sending {ByteUtil.ByteArrayToString(data, 0, numberOfBytes)} to server");
            var sendMessageTask = client.SendAsync(data, numberOfBytes, serverEndPoint);
            while (sendMessageTask.Status < TaskStatus.RanToCompletion) { yield return null; }
        }

        private IEnumerator ReceiveMessage(byte[] buffer)
        {
            var stream = new MemoryStream(buffer);
            var receiveTask = protocolSerializer.DeserializeNextMessage(stream, testMessageReceiver);
            while (receiveTask.Status < TaskStatus.RanToCompletion) { yield return null; }
        }
    }

}