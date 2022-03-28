using System;
using RG.EscapeRoom.Model.Math;

namespace RG.EscapeRoom.Model.Puzzles
{
    [Serializable]
    public struct PuzzleDefinition
    {
        public string type;
        public string id;
        public Vector3 position;
        public Quaternion rotation;
        public string customData;

        public PuzzleDefinition(string type, string id, Vector3 position, Quaternion rotation, string customData)
        {
            this.type = type;
            this.id = id;
            this.position = position;
            this.rotation = rotation;
            this.customData = customData;
        }
    }

    public enum PuzzleTypes
    {
        SingleLever
    }
}