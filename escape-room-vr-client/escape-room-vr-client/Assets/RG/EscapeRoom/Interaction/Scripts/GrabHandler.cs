using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace RG.EscapeRoom.Interaction.Scripts
{

    public class GrabData
    {
        public HandInteractableItem itemHeldInLeftHand;
        public HandInteractableItem itemHeldInRightHand;
    }
    
    public class GrabHandler: ITickable
    {
        private readonly ControllerButtonData controllerButtonData;
        private readonly GrabData grabData;
        private readonly XRPlayerHandReference leftHand;
        private readonly XRPlayerHandReference rightHand;

        public GrabHandler(ControllerButtonData controllerButtonData, GrabData grabData, XRPlayerHandReference leftHand, XRPlayerHandReference rightHand)
        {
            this.controllerButtonData = controllerButtonData;
            this.grabData = grabData;
            this.leftHand = leftHand;
            this.rightHand = rightHand;
        }

        public void Tick()
        {
            if (controllerButtonData.IsButtonPressed(IControllerButtonData.Controller.Left,
                    IControllerButtonData.Button.Grip))
            {
                if (grabData.itemHeldInLeftHand == null)
                {
                    if (leftHand.interactableItemsInContactWithHand.Count > 0)
                    {
                        grabData.itemHeldInLeftHand = leftHand.interactableItemsInContactWithHand.First();
                        if (grabData.itemHeldInLeftHand != null)
                        {
                            Debug.Log($"Left hand grabbed {grabData.itemHeldInLeftHand}");
                        }
                    }
                }
            }
            else
            {
                if (grabData.itemHeldInLeftHand != null)
                {
                    Debug.Log($"Left hand released {grabData.itemHeldInLeftHand}");
                }
                grabData.itemHeldInLeftHand = null;
            }
            if (controllerButtonData.IsButtonPressed(IControllerButtonData.Controller.Right,
                    IControllerButtonData.Button.Grip))
            {
                if (grabData.itemHeldInRightHand == null)
                {
                    if (rightHand.interactableItemsInContactWithHand.Count > 0)
                    {
                        grabData.itemHeldInRightHand = rightHand.interactableItemsInContactWithHand.First();
                        if (grabData.itemHeldInRightHand != null)
                        {
                            Debug.Log($"Right hand grabbed {grabData.itemHeldInRightHand}");
                        }
                    }
                }
            }
            else
            {
                if (grabData.itemHeldInRightHand != null)
                {
                    Debug.Log($"Right hand released {grabData.itemHeldInRightHand}");
                }
                grabData.itemHeldInRightHand = null;
            }
        }
    }
}