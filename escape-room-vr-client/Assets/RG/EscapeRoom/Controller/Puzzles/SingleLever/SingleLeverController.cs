using RG.EscapeRoom.Controller.Interaction;
using RG.EscapeRoom.Model.Puzzles.SingleLever;
using RG.EscapeRoom.Puzzles.SingleLever;
using UnityEngine;

namespace RG.EscapeRoom.Controller.Puzzles.SingleLever
{
    public class SingleLeverController : IController
    {
        private readonly PullData pullData;
        private readonly SingleLeverReference singleLeverReference;
        private readonly SingleLeverModel singleLeverModel;

        public SingleLeverController(PullData pullData, SingleLeverReference singleLeverReference, SingleLeverModel singleLeverModel)
        {
            this.pullData = pullData;
            this.singleLeverReference = singleLeverReference;
            this.singleLeverModel = singleLeverModel;
        }


        public void Tick()
        {
            Transform handTransform;
            if (pullData.handPull.TryGetValue(singleLeverReference.leverEnd, out handTransform))
            {
                var leverOrigin = singleLeverReference.origin.transform;
                var handPosition = handTransform.position;
                var fromOriginToHand = leverOrigin.position - handPosition;
                var leverNormal = -leverOrigin.forward;
                var angle = Vector3.SignedAngle(leverNormal, fromOriginToHand, leverOrigin.right);
                singleLeverModel.leverPosition = -angle;
            }
        }
    }
}