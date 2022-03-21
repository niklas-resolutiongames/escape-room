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
                var leverPosition = singleLeverReference.gameObject.transform.position;
                var handPosition = handTransform.position;
                var dy = handPosition.y - leverPosition.y;
                var dz = handPosition.z - leverPosition.z;
                var angle = Mathf.Atan2(dz, dy) * Mathf.Rad2Deg;
                singleLeverModel.leverPosition = angle + 90;
            }
        }
    }
}