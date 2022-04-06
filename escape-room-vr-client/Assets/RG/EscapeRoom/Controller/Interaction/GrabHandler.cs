using System.Linq;
using RG.EscapeRoom.Controller.Player;
using RG.EscapeRoom.Interaction;
using RG.EscapeRoom.Interaction.Scripts;
using RG.EscapeRoom.Wiring;
using UnityEngine;

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

        public GrabHandler(ControllerButtonData controllerButtonData, GrabData grabData, XRPlayerHandReference leftHand,
            XRPlayerHandReference rightHand)
        {
            this.controllerButtonData = controllerButtonData;
            this.grabData = grabData;
            this.leftHand = leftHand;
            this.rightHand = rightHand;
        }

        public void Tick()
        {
            grabData.itemHeldInLeftHand = CheckGrab(IControllerButtonData.Controller.Left, grabData.itemHeldInLeftHand,
                leftHand);
            grabData.itemHeldInRightHand = CheckGrab(IControllerButtonData.Controller.Right, grabData.itemHeldInRightHand,
                rightHand);
        }

        private HandInteractableItem CheckGrab(IControllerButtonData.Controller controller, HandInteractableItem itemHeldInHand, XRPlayerHandReference handReference)
        {
            if (controllerButtonData.IsButtonPressed(controller,
                    IControllerButtonData.Button.Grip))
            {
                if (itemHeldInHand == null)
                {
                    if (handReference.interactableItemsInContactWithHand.Count > 0)
                    {
                        var grabbedItem = handReference.interactableItemsInContactWithHand.First();
                        if (grabbedItem != null)
                            Debug.Log($"{controller} grabbed {grabbedItem}");
                        return grabbedItem;
                    }
                }

                return itemHeldInHand;
            }
            else
            {
                if (itemHeldInHand != null)
                    Debug.Log($"{controller} released {itemHeldInHand}");
                return null;
            }
        }
    }
}