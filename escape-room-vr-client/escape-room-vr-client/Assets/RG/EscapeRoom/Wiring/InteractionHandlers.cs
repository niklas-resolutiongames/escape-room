using RG.EscapeRoom.Controller.Interaction;
using RG.EscapeRoom.Controller.Player;
using RG.EscapeRoom.Interaction;

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
        private HandPullHandler handPullHandler;

        private GrabHandler grabHandler;

        public InteractionHandlers(ControllerButtonData controllerButtonData, XRPlayerHandReference leftHandReference,
            XRPlayerHandReference rightHandReference)
        {
            this.controllerButtonData = controllerButtonData;
            this.leftHandReference = leftHandReference;
            this.rightHandReference = rightHandReference;
        }

        public void Tick()
        {
            grabHandler.Tick();
            handPullHandler.Tick();
        }

        public InteractionDatas Initialize()
        {
            var interactionDatas = new InteractionDatas();
            interactionDatas.grabData = new GrabData();
            interactionDatas.pullData = new PullData();
            grabHandler = new GrabHandler(controllerButtonData, interactionDatas.grabData, leftHandReference,
                rightHandReference);
            handPullHandler = new HandPullHandler(interactionDatas.pullData, interactionDatas.grabData, leftHandReference, rightHandReference);
            return interactionDatas;
        }
    }
}