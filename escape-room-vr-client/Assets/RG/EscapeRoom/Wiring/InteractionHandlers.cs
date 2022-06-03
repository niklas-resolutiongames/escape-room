using RG.EscapeRoom.Controller.Interaction;
using RG.EscapeRoom.Controller.Player;
using RG.EscapeRoom.Interaction;
using RG.EscapeRoom.Networking;
using RG.EscapeRoom.Wiring.Factories;

namespace RG.EscapeRoom.Wiring
{
    public class InteractionDatas
    {
        public GrabData grabData;
        public PullData pullData;
    }

    public class InteractionHandlers : ITickable
    {
        private readonly ControllerButtonData controllerButtonData;
        private readonly XRPlayerHandReference leftHandReference;
        private readonly XRPlayerHandReference rightHandReference;
        private readonly InteractionDatas interactionDatas;
        private readonly ITimeProvider timeProvider;
        private readonly MessageSender messageSender;
        private readonly IncomingMessagesData incomingMessagesData;

        private HandPullHandler handPullHandler;
        private GrabHandler grabHandler;

        public InteractionHandlers(ControllerButtonData controllerButtonData, XRPlayerHandReference leftHandReference, XRPlayerHandReference rightHandReference, InteractionDatas interactionDatas, ITimeProvider timeProvider, MessageSender messageSender, IncomingMessagesData incomingMessagesData)
        {
            this.controllerButtonData = controllerButtonData;
            this.leftHandReference = leftHandReference;
            this.rightHandReference = rightHandReference;
            this.interactionDatas = interactionDatas;
            this.timeProvider = timeProvider;
            this.messageSender = messageSender;
            this.incomingMessagesData = incomingMessagesData;
        }

        public void Tick()
        {
            grabHandler.Tick();
            handPullHandler.Tick();
        }

        public static InteractionDatas CreateDatas()
        {
            var interactionDatas = new InteractionDatas();
            interactionDatas.grabData = new GrabData();
            interactionDatas.pullData = new PullData();
            return interactionDatas;
        }

        public void InitializeHandlers(Room room)
        {
            grabHandler = new GrabHandler(controllerButtonData, interactionDatas.grabData, leftHandReference,
                rightHandReference, timeProvider, messageSender, room, incomingMessagesData);
            handPullHandler = new HandPullHandler(interactionDatas.pullData, interactionDatas.grabData, leftHandReference,
                rightHandReference);
        }
    }
}