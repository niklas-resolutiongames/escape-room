using RG.EscapeRoom.Model.Math;

namespace RG.EscapeRoom.Model.Puzzles
{
    public struct PuzzleDefinition
    {
        public string type;
        public string id;
        public Vector3 position;
        public Quaternion rotation;
        public string customData;
    }
}