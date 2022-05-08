using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using RG.EscapeRoom.Controller.Player;
using RG.EscapeRoom.Wiring;
using UnityEngine;

namespace RG.Tests
{
    public class TestMotionHelper
    {
        private readonly List<ITickable> tickablesToTick = new List<ITickable>();
        private float testSpeed = 5f;
        private XRPlayerReference playerReference;

        public IEnumerator Await(Task t)
        {
            while (!t.IsCompleted)
            {
                TickAllBackgroundTickables();
                if (playerReference != null)
                {
                    Vector3 pointBetweenHands = (playerReference.leftHand.transform.position + playerReference.rightHand.transform.position) / 2;
                    playerReference.head.transform.LookAt(pointBetweenHands);
                }
                yield return null;
            }
        }

        public async Task MoveGameObjectOverTime(GameObject objectToMove, Vector3 pathToFollow,
            float timeToMoveFullDistance)
        {
            await MoveGameObjectToPositionOverTime(objectToMove,
                objectToMove.transform.position + pathToFollow,
                objectToMove.transform.rotation,
                timeToMoveFullDistance
            );
        }

        public async Task MoveGameObjectToPositionOverTime(GameObject objectToMove, Vector3 positionToMoveTo,
            Quaternion rotationToMoveTo,
            float timeToMoveFullDistance)
        {
            var scaledTime = timeToMoveFullDistance / testSpeed;
            var startTime = GetTime();
            var startPosition = objectToMove.transform.position;
            var startRotation = objectToMove.transform.rotation;
            var currentTime = GetTime();
            while (currentTime - startTime < scaledTime)
            {
                currentTime = GetTime();
                var t = (currentTime - startTime) / scaledTime;
                var pos = Vector3.Lerp(startPosition, positionToMoveTo, t);
                var rot = Quaternion.Lerp(startRotation, rotationToMoveTo, t);
                objectToMove.transform.SetPositionAndRotation(pos, rot);
                await Task.Yield();
            }
        }

        public async Task Idle(float timeToIdle)
        {
            var scaledTime = timeToIdle / testSpeed;
            var startTime = GetTime();
            var currentTime = GetTime();
            while (currentTime - startTime < scaledTime)
            {
                currentTime = GetTime();
                await Task.Yield();
            }
        }

        private void TickAllBackgroundTickables()
        {
            for (var i = 0; i < tickablesToTick.Count; i++) tickablesToTick[i].Tick();
        }

        private float GetTime()
        {
            return Time.time;
        }

        public void TickInBackground(ITickable tickable)
        {
            tickablesToTick.Add(tickable);
        }

        public void SetPlayerReference(XRPlayerReference playerReference)
        {
            this.playerReference = playerReference;
        }
    }
}