﻿using System.Threading;
using RG.EscapeRoomServer.Server;

namespace RG.Tests
{
    public class ServerTestUtils
    {
        private static CancellationTokenSource stopServerCancellationTokenSource = new CancellationTokenSource();

        public static EscapeRoomSocketServer StartServer(int port, int timeoutsUntilStop)
        {
            stopServerCancellationTokenSource.Cancel();
            stopServerCancellationTokenSource = new CancellationTokenSource();
            ILogger unityTestLogger = new UnityTestLogger();
            var serverFactory = new ServerFactory(unityTestLogger, stopServerCancellationTokenSource);
            var escapeRoomSocketServer = serverFactory.CreateServer(port, TestUtil.PathToFile("Assets/RG/EscapeRoomPlayModeTests/SingleLever/SingleLeverFactoryTestRoomDefinition.json"), timeoutsUntilStop);
            escapeRoomSocketServer.Start();
            return escapeRoomSocketServer;
        }

        public static void StopServer()
        {
            stopServerCancellationTokenSource.Cancel();
        }
    }
}