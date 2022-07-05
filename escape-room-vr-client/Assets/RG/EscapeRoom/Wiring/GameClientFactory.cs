using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RG.EscapeRoom.Controller.Player;
using RG.EscapeRoom.Interaction;
using RG.EscapeRoom.Model.Rooms;
using RG.EscapeRoom.Networking;
using RG.EscapeRoom.ViewController.Player;
using RG.EscapeRoom.Wiring.Factories;
using RG.EscapeRoomProtocol;
using RG.EscapeRoomProtocol.Messages;
using UnityEditor;
using UnityEngine;

namespace RG.EscapeRoom.Wiring
{
    public class GameClientFactory
    {
        public static string TESTING_ROOM_JSON; // remove later

        public GameClient CreateGameClient()
        {
            var incomingMessagesData = new IncomingMessagesData();
            var interactionDatas = InteractionHandlers.CreateDatas();
            var receivedNetworkStateData = new ReceivedNetworkStateData();
            var roomDefinitionParser = new RoomDefinitionParser();
            ProtocolSerializer protocolSerializer = new ProtocolSerializer(new PrimitiveSerializer());
            var messageSender = new MessageSender(protocolSerializer, incomingMessagesData, receivedNetworkStateData);
            messageSender.Init();

            var clientNetworkHandler = new ClientNetworkConnectionHandler(messageSender, protocolSerializer, new ClientMessageReceiver(incomingMessagesData));
            var roomFactory =
                AssetDatabase.LoadAssetAtPath<RoomFactory>("Assets/RG/EscapeRoom/Wiring/Factories/RoomFactory.asset");
            var networkedPlayerFactory =
                AssetDatabase.LoadAssetAtPath<NetworkedPlayerFactory>("Assets/RG/EscapeRoom/Wiring/Factories/NetworkedPlayerFactory.asset");
            var networkedPlayerViewController = new NetworkedPlayerViewController(networkedPlayerFactory, receivedNetworkStateData);
            var controllerButtonData = new ControllerButtonData();

            return new GameClient(interactionDatas, roomDefinitionParser, roomFactory, controllerButtonData, clientNetworkHandler, incomingMessagesData, messageSender, receivedNetworkStateData, networkedPlayerViewController);
        }
    }

    public class GameClient: ITickable, IDisposable
    {
        private readonly InteractionDatas interactionDatas;
        private readonly RoomDefinitionParser roomDefinitionParser;
        private readonly RoomFactory roomFactory;
        public readonly ControllerButtonData controllerButtonData;
        private readonly ClientNetworkConnectionHandler clientNetworkConnectionHandler;
        private readonly IncomingMessagesData incomingMessagesData;
        private readonly MessageSender messageSender;
        public readonly ReceivedNetworkStateData receivedNetworkStateData;
        private readonly NetworkedPlayerViewController networkedPlayerViewController;

        public RoomDefinition roomDefinition;
        public Room room;
        public XRPlayerReference playerReference;
        private InteractionHandlers interactionHandlers;
        public bool roomIsLoaded = false;
        public ClientPositionHandler clientPositionHandler;

        public GameClient(InteractionDatas interactionDatas, RoomDefinitionParser roomDefinitionParser,
            RoomFactory roomFactory, ControllerButtonData controllerButtonData,
            ClientNetworkConnectionHandler clientNetworkConnectionHandler, IncomingMessagesData incomingMessagesData,
            MessageSender messageSender, ReceivedNetworkStateData receivedNetworkStateData,
            NetworkedPlayerViewController networkedPlayerViewController)
        {
            this.interactionDatas = interactionDatas;
            this.roomDefinitionParser = roomDefinitionParser;
            this.roomFactory = roomFactory;
            this.controllerButtonData = controllerButtonData;
            this.clientNetworkConnectionHandler = clientNetworkConnectionHandler;
            this.incomingMessagesData = incomingMessagesData;
            this.messageSender = messageSender;
            this.receivedNetworkStateData = receivedNetworkStateData;
            this.networkedPlayerViewController = networkedPlayerViewController;
        }

        public void Connect(int serverPort, string serverIp)
        {
            clientNetworkConnectionHandler.Connect(serverPort, serverIp);
        }

        private async Task LoadRoom(string json)
        {
            roomDefinition = roomDefinitionParser.Parse(json);

            var createRoomTask = roomFactory.CreateRoomWithPlayerInIt(roomDefinition, interactionDatas);
            await createRoomTask;

            room = createRoomTask.Result;
            playerReference = room.playerReference;
            interactionHandlers =
                new InteractionHandlers(controllerButtonData, playerReference.leftHand, playerReference.rightHand, interactionDatas, new RealTimeProvider(), messageSender, incomingMessagesData);

            clientPositionHandler = new ClientPositionHandler(messageSender, playerReference, incomingMessagesData, receivedNetworkStateData);

            playerReference.head.transform.position = new Vector3(0f, 1.8f, 0f);
            playerReference.leftHand.transform.position = new Vector3(.2f, 1.5f, -.2f);
            playerReference.rightHand.transform.position = new Vector3(.2f, 1.5f, .2f);
        
            interactionHandlers.InitializeHandlers(room);
            roomIsLoaded = true;
        }

        public void Tick()
        {
            if (roomIsLoaded)
            {
                interactionHandlers.Tick();
                TickPuzzles();
                clientPositionHandler.Tick();
                networkedPlayerViewController.Tick();
            }

            clientNetworkConnectionHandler.Tick();
            CheckIncomingMessages();
        }

        private async Task CheckIncomingMessages()
        {
            if (incomingMessagesData.loadRoomMessages.Count > 0)
            {
                var loadRoomMessage = incomingMessagesData.loadRoomMessages.Dequeue();
                await LoadRoom(GameClientFactory.TESTING_ROOM_JSON);
            }
        }

        private void TickPuzzles()
        {
            for (int i = 0; i < roomDefinition.puzzles.Length; i++)
            {
                var puzzle = room.puzzles[i];
                puzzle.controller.Tick();
                puzzle.viewController.Tick();
            }
        }

        public void Dispose()
        {
            clientNetworkConnectionHandler.Dispose();
        }
    }

    public class ReceivedNetworkStateData
    {
        public int localPlayerNetworkId;
        public Dictionary<int, PlayerPositionMessage> receivedPlayerPositions = new Dictionary<int, PlayerPositionMessage>();
    }
}