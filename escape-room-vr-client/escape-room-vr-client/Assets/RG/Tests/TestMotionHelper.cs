using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RG.EscapeRoom.Interaction.Scripts;
using UnityEngine;

namespace RG.Tests
{
    public class TestMotionHelper
    {
        private List<ITickable> tickablesToTick = new List<ITickable>();

        public IEnumerator Await(Task t)
        {
            while (!t.IsCompleted)
            {
                TickAllBackgroundTickables();
                yield return null;
            }
        }

        public async Task MoveGameObjectOverTime(GameObject objectToMove, Vector3 pathToFollow, float timeToMoveFullDistance)
        {
            await MoveGameObjectToPositionOverTime(objectToMove, 
                objectToMove.transform.position + pathToFollow, 
                objectToMove.transform.rotation,
                timeToMoveFullDistance
            );
        }

        public async Task MoveGameObjectToPositionOverTime(GameObject objectToMove, Vector3 positionToMoveTo, Quaternion rotationToMoveTo,
                float timeToMoveFullDistance)
            {
            var startTime = GetTime();
            var startPosition = objectToMove.transform.position;
            var startRotation = objectToMove.transform.rotation;
            var currentTime = GetTime();
            while (currentTime - startTime < timeToMoveFullDistance)
            {
                currentTime = GetTime();
                var t = (currentTime - startTime) / timeToMoveFullDistance;
                Vector3 pos = Vector3.Lerp(startPosition, positionToMoveTo, t);
                Quaternion rot = Quaternion.Lerp(startRotation, rotationToMoveTo, t);
                objectToMove.transform.SetPositionAndRotation(pos, rot);
                await Task.Yield();
            }
        }
        
        public async Task Idle(float timeToIdle)
        {
            var startTime = GetTime();
            var currentTime = GetTime();
            while (currentTime - startTime < timeToIdle)
            {
                currentTime = GetTime();
                await Task.Yield();
            }
        }

        private void TickAllBackgroundTickables()
        {
            for (int i = 0; i < tickablesToTick.Count; i++)
            {
                tickablesToTick[i].Tick();
            }
        }

        private float GetTime()
        {
            return Time.time;
        }

        public void TickInBackground(ITickable tickable)
        {
            tickablesToTick.Add(tickable);
        }
    }
}