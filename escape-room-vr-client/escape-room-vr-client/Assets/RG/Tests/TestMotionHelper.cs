using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace RG.Tests
{
    public class TestMotionHelper
    {

        public static IEnumerator Await(Task t)
        {
            while (!t.IsCompleted) { yield return null; }
        }

        public static async Task MoveGameObjectOverTime(GameObject objectToMove, Vector3 pathToFollow, float timeToMoveFullDistance)
        {
            await MoveGameObjectToPositionOverTime(objectToMove, 
                objectToMove.transform.position + pathToFollow, 
                objectToMove.transform.rotation,
                timeToMoveFullDistance
            );
        }

        public static async Task MoveGameObjectToPositionOverTime(GameObject objectToMove, Vector3 positionToMoveTo, Quaternion rotationToMoveTo,
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
        
        public static async Task Idle(float timeToIdle)
        {
            
            var startTime = GetTime();
            var currentTime = GetTime();
            while (currentTime - startTime < timeToIdle)
            {
                currentTime = GetTime();
                await Task.Yield();
            }
        }

        private static void _Move()
        {
            throw new System.NotImplementedException();
        }

        private static float GetTime()
        {
            return Time.time;
        }

    }
}