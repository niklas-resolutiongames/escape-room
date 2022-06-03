using System.Threading.Tasks;
using RG.EscapeRoom.Controller.Player;
using RG.EscapeRoom.Interaction;
using RG.EscapeRoom.Model.Rooms;
using RG.EscapeRoom.Networking;
using RG.EscapeRoom.Wiring.Factories;
using RG.EscapeRoomProtocol;
using UnityEditor;
using UnityEngine;

namespace RG.EscapeRoom.Wiring
{
    public class GameClientFactory
    {
        public static  string TESTING_ROOM_JSON; // remove later

        public GameClient CreateGameClient()
        {
            var incomingMessagesData = new IncomingMessagesData();
            var interactionDatas = InteractionHandlers.CreateDatas();
            var roomDefinitionParser = new RoomDefinitionParser();
            ProtocolSerializer protocolSerializer = new ProtocolSerializer();
            var messageSender = new MessageSender(protocolSerializer, incomingMessagesData);
            messageSender.Init();
            var clientNetworkHandler = new ClientNetworkHandler(messageSender, protocolSerializer, new ClientMessageReceiver(incomingMessagesData));
            var roomFactory =
                AssetDatabase.LoadAssetAtPath<RoomFactory>("Assets/RG/EscapeRoom/Wiring/Factories/RoomFactory.asset");
            var controllerButtonData = new ControllerButtonData();

            return new GameClient(interactionDatas, roomDefinitionParser, roomFactory, controllerButtonData, clientNetworkHandler, incomingMessagesData, messageSender);
        }
    }

    public class GameClient: ITickable
    {
        private readonly InteractionDatas interactionDatas;
        private readonly RoomDefinitionParser roomDefinitionParser;
        private readonly RoomFactory roomFactory;
        public readonly ControllerButtonData controllerButtonData;
        private readonly ClientNetworkHandler clientNetworkHandler;
        private readonly IncomingMessagesData incomingMessagesData;
        private readonly MessageSender messageSender;

        public RoomDefinition roomDefinition;
        public Room room;
        public XRPlayerReference playerReference;
        public InteractionHandlers interactionHandlers;
        public bool roomIsLoaded = false;

        public GameClient(InteractionDatas interactionDatas, RoomDefinitionParser roomDefinitionParser, RoomFactory roomFactory, ControllerButtonData controllerButtonData, ClientNetworkHandler clientNetworkHandler, IncomingMessagesData incomingMessagesData, MessageSender messageSender)
        {
            this.interactionDatas = interactionDatas;
            this.roomDefinitionParser = roomDefinitionParser;
            this.roomFactory = roomFactory;
            this.controllerButtonData = controllerButtonData;
            this.clientNetworkHandler = clientNetworkHandler;
            this.incomingMessagesData = incomingMessagesData;
            this.messageSender = messageSender;
        }

        public void Connect(int serverPort, string serverIp)
        {
            clientNetworkHandler.Connect(serverPort, serverIp);
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
            }

            clientNetworkHandler.Tick();
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
    }
}