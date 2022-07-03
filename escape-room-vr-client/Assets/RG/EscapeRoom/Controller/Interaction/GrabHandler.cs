using System.Linq;
using RG.EscapeRoom.Controller.Player;
using RG.EscapeRoom.Interaction;
using RG.EscapeRoom.Interaction.Scripts;
using RG.EscapeRoom.Networking;
using RG.EscapeRoom.Wiring;
using RG.EscapeRoom.Wiring.Factories;
using RG.EscapeRoomProtocol.Messages;

namespace RG.EscapeRoom.Controller.Interaction
{
    public class GrabData
    {
        public HandInteractableItem itemHeldInLeftHand;
        public HandInteractableItem itemHeldInRightHand;
    }

    public class GrabHandler : ITickable
    {
        private readonly ControllerButtonData controllerButtonData;
        private readonly GrabData grabData;
        private readonly XRPlayerHandReference leftHand;
        private readonly XRPlayerHandReference rightHand;
        private readonly MessageSender messageSender;
        private readonly ITimeProvider timeProvider;
        private readonly Room room;
        private readonly IncomingMessagesData incomingMessagesData;
        
        private float timeForLastLeftGrabAttempt;
        private float timeForLastRightGrabAttempt;

        public GrabHandler(ControllerButtonData controllerButtonData, GrabData grabData, XRPlayerHandReference leftHand,
            XRPlayerHandReference rightHand, ITimeProvider timeProvider, MessageSender messageSender, Room room,
            IncomingMessagesData incomingMessagesData)
        {
            this.controllerButtonData = controllerButtonData;
            this.grabData = grabData;
            this.leftHand = leftHand;
            this.rightHand = rightHand;
            this.timeProvider = timeProvider;
            this.messageSender = messageSender;
            this.room = room;
            this.incomingMessagesData = incomingMessagesData;
        }

        public void Tick()
        {
            CheckIncomingGrabMessages();
            timeForLastLeftGrabAttempt = CheckGrab(IControllerButtonData.Controller.Left, grabData.itemHeldInLeftHand,
                leftHand, timeForLastLeftGrabAttempt, RequestGrabMessage.LeftHand);
            timeForLastRightGrabAttempt = CheckGrab(IControllerButtonData.Controller.Right, grabData.itemHeldInRightHand,
                rightHand, timeForLastRightGrabAttempt, RequestGrabMessage.RightHand);
        }

        private void CheckIncomingGrabMessages()
        {
            while (incomingMessagesData.incomingGrabResults.Count > 0)
            {
                var message = incomingMessagesData.incomingGrabResults.Dequeue();
                var item = room.FindHandInteractableItemReference(message.requestMessage.grabbableId);
                if (item != null)
                {
                    switch (message.requestMessage.hand)
                    {
                        case RequestGrabMessage.LeftHand:
                            if (message.requestMessage.isGrab)
                            {
                                grabData.itemHeldInLeftHand = item;
                            }
                            else
                            {
                                grabData.itemHeldInLeftHand = null;
                            }
                            break;
                        case RequestGrabMessage.RightHand:
                            if (message.requestMessage.isGrab)
                            {
                                grabData.itemHeldInRightHand = item;
                            }
                            else
                            {
                                grabData.itemHeldInRightHand = null;
                            }
                            break;
                    }
                }
            }
        }

        private float CheckGrab(IControllerButtonData.Controller controller, HandInteractableItem itemHeldInHand,
            XRPlayerHandReference handReference, float timeForLastGrabAttempt, byte hand)
        {
            if (controllerButtonData.IsButtonPressed(controller,
                    IControllerButtonData.Button.Grip))
            {
                if (itemHeldInHand == null && MessageSendingCurfewPassed(timeForLastGrabAttempt)) 
                {
                    if (handReference.interactableItemsInContactWithHand.Count > 0)
                    {
                        var grabbedItem = handReference.interactableItemsInContactWithHand.First();
                        timeForLastGrabAttempt = timeProvider.GetTime();
                        messageSender.SendRequestGrabMessage(hand, grabbedItem.NetworkId(), true);
                    }
                }
            }
            else
            {
                if (itemHeldInHand != null && MessageSendingCurfewPassed(timeForLastGrabAttempt))
                {
                    timeForLastGrabAttempt = timeProvider.GetTime();
                    messageSender.SendRequestGrabMessage(hand, itemHeldInHand.NetworkId(), false);
                }
            }
            return timeForLastGrabAttempt;
        }

        private bool MessageSendingCurfewPassed(float timeForLastGrabAttempt)
        {
            return timeProvider.GetTime() - timeForLastGrabAttempt > 1;
        }
    }
}