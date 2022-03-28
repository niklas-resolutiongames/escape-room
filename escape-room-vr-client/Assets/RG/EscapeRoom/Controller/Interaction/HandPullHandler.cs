using System.Collections.Generic;
using RG.EscapeRoom.Controller.Player;
using RG.EscapeRoom.Wiring;
using UnityEngine;

namespace RG.EscapeRoom.Controller.Interaction
{
    public class HandPullHandler : ITickable
    {
        private readonly PullData pullData;
        private readonly GrabData grabData;
        private readonly XRPlayerHandReference leftHand;
        private readonly XRPlayerHandReference rightHand;

        public HandPullHandler(PullData pullData, GrabData grabData, XRPlayerHandReference leftHand, XRPlayerHandReference rightHand)
        {
            this.pullData = pullData;
            this.grabData = grabData;
            this.leftHand = leftHand;
            this.rightHand = rightHand;
        }

        public void Tick()
        {
            pullData.handPull.Clear();
            CheckPullObjectInHand(grabData.itemHeldInLeftHand, leftHand);
            CheckPullObjectInHand(grabData.itemHeldInRightHand, rightHand);
        }

        private void CheckPullObjectInHand(HandInteractableItem heldItem, XRPlayerHandReference hand)
        {
            if (heldItem != null)
            {
                var handTransform = hand.transform;
                pullData.handPull[heldItem] = handTransform;
            }
        }
    }

    public class PullData
    {
        public Dictionary<HandInteractableItem, Transform> handPull = new Dictionary<HandInteractableItem, Transform>();
    }
}