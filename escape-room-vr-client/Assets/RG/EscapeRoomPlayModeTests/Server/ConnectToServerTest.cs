using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using RG.EscapeRoomProtocol;
using RG.EscapeRoomProtocol.Messages;
using UnityEngine;
using UnityEngine.TestTools;

namespace RG.Tests.Server
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

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            port = 12345;
            unityTestLogger = new UnityTestLogger();
            cancellationTokenSource = new CancellationTokenSource();
            escapeRoomSocketServer = new EscapeRoomSocketServer(port, unityTestLogger, cancellationTokenSource);
            Task.Run(() => escapeRoomSocketServer.Start());
            serverEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
            clientEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port+1);
            protocolSerializer = new ProtocolSerializer();
            yield return null;
        }
        [UnityTest]
        public IEnumerator TestEcho()
        {
            var timeAtStart = Time.time;
            UdpClient client = new UdpClient(clientEndPoint);
            var testString = "testing testing";
            byte[] testMessage = Encoding.Unicode.GetBytes(testString);

            yield return SendMessage(client, new ClientHelloMessage(Encoding.Unicode.GetBytes("abc123")));
            
            var sendTask = client.SendAsync(testMessage, testMessage.Length, serverEndPoint);
            var receiveTask = client.ReceiveAsync();
            while (sendTask.Status < TaskStatus.RanToCompletion) { yield return null; }
            while (receiveTask.Status < TaskStatus.RanToCompletion) { yield return null; }

            unityTestLogger.Info($"[Client] Sent {sendTask.Result} bytes");
            var resultBuffer = receiveTask.Result.Buffer;
            var receivedString = Encoding.Unicode.GetString(resultBuffer);
            unityTestLogger.Info($"[Client] Received {resultBuffer.Length} bytes: '{receivedString}'");
            unityTestLogger.OutputLog();
            cancellationTokenSource.Cancel();
            Assert.AreEqual(testString, receivedString);
            while (escapeRoomSocketServer.IsRunning() && Time.time - timeAtStart < 10) { yield return null; }
            Assert.IsFalse(escapeRoomSocketServer.IsRunning());
        }

        [UnityTest]
        public IEnumerator SocketTest()
        {
            var timeAtStart = Time.time;
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Udp);
            socket.Connect(serverEndPoint);
            var networkStream = new NetworkStream(socket);
            protocolSerializer.SerializeMessage(new ClientHelloMessage(Encoding.UTF8.GetBytes("abc123")),
                networkStream);
            networkStream.Flush();
            unityTestLogger.OutputLog();
            cancellationTokenSource.Cancel();
            while (escapeRoomSocketServer.IsRunning() && Time.time - timeAtStart < 10) { yield return null; }
        }

        private IEnumerator SendMessage(UdpClient client, ClientHelloMessage message)
        {
            byte[] data = new byte[1024];
            var memoryStream = new MemoryStream(data);
            var numberOfBytes = protocolSerializer.SerializeMessage(message, memoryStream);
            memoryStream.Flush();
            var sendMessageTask = client.SendAsync(data, numberOfBytes, serverEndPoint);
            while (sendMessageTask.Status < TaskStatus.RanToCompletion) { yield return null; }
        }
    }
}