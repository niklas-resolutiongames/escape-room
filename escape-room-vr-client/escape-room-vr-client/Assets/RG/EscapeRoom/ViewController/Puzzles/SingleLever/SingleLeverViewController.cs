using RG.EscapeRoom.Model.Puzzles.SingleLever;
using RG.EscapeRoom.Puzzles.SingleLever;
using UnityEngine;

namespace RG.EscapeRoom.ViewController.Puzzles.SingleLever
{
    public class SingleLeverViewController : IViewController
    {
        private readonly SingleLeverModel model;
        private readonly SingleLeverReference view;
        private double lastLeverPositon;

        public SingleLeverViewController(SingleLeverModel model, SingleLeverReference view)
        {
            this.model = model;
            this.view = view;
        }

        public void Tick()
        {
            view.leverJoint.transform.localEulerAngles = new Vector3(0,this.model.leverPosition,0);
        }
    }
}