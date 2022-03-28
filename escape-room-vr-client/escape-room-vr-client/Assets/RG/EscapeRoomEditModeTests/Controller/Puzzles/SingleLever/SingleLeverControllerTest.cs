using NUnit.Framework;
using RG.EscapeRoom.Controller.Interaction;
using RG.EscapeRoom.Controller.Player;
using RG.EscapeRoom.Model.Puzzles;
using RG.EscapeRoom.Model.Puzzles.SingleLever;
using RG.EscapeRoom.Puzzles.SingleLever;
using UnityEngine;

namespace RG.EscapeRoom.Controller.Puzzles.SingleLever
{
    public class SingleLeverControllerTest
    {
        private SingleLeverController singleLeverController;
        private PullData pullData;
        private SingleLeverReference singleLeverReference;
        private SingleLeverModel singleLeverModel;

        [SetUp]
        public void SetUp()
        {
            pullData = new PullData();
            singleLeverReference = new SingleLeverReference();
            singleLeverReference.origin = CreateGameObject(0,0,0);
            singleLeverReference.leverJoint = CreateGameObject(0,0,0);
            singleLeverReference.leverEnd = new HandInteractableItemReference();
            singleLeverModel = new SingleLeverModel("", PuzzleTypes.SingleLever);
            singleLeverController = new SingleLeverController(pullData, singleLeverReference, singleLeverModel);
        }

        [Test]
        public void PullLeverDownOnNonRotatedLeverWillLowerValue()
        {
            pullData.handPull[singleLeverReference.leverEnd] = CreateGameObject(0,-.1f,.1f).transform;
            singleLeverController.Tick();
            Assert.AreEqual(-45, singleLeverModel.leverPosition);
        }

        [Test]
        public void PullLeverUpOnNonRotatedLeverWillIncreaseValue()
        {
            pullData.handPull[singleLeverReference.leverEnd] = CreateGameObject(0,.1f,.1f).transform;
            singleLeverController.Tick();
            Assert.AreEqual(45, singleLeverModel.leverPosition);
        }

        private static GameObject CreateGameObject(float x,float y,float z)
        {
            var gameObject = new GameObject();
            gameObject.transform.position = new Vector3(x, y, z);
            
            return gameObject;
        }
    }
}