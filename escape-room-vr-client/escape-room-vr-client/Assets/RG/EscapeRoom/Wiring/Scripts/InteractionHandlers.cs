namespace RG.EscapeRoom.Interaction.Scripts
{
    public class InteractionDatas
    {
        public GrabData grabData;
    }
    public class InteractionHandlers: ITickable
    {
        
        private readonly ControllerButtonData controllerButtonData;
        private readonly XRPlayerHandReference leftHandReference;
        private readonly XRPlayerHandReference rightHandReference;
        
        private GrabHandler grabHandler;

        public InteractionHandlers(ControllerButtonData controllerButtonData, XRPlayerHandReference leftHandReference, XRPlayerHandReference rightHandReference)
        {
            this.controllerButtonData = controllerButtonData;
            this.leftHandReference = leftHandReference;
            this.rightHandReference = rightHandReference;
        }

        public InteractionDatas Initialize()
        {
            var interactionDatas = new InteractionDatas();
            interactionDatas.grabData = new GrabData();
            grabHandler = new GrabHandler(controllerButtonData, interactionDatas.grabData, leftHandReference, rightHandReference);
            return interactionDatas;
        }

        public void Tick()
        {
            grabHandler.Tick();
        }
    }
}