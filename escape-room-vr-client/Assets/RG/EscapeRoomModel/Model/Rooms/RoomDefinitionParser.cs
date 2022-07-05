using System;
using RG.EscapeRoom.Model.Puzzles;
using RG.EscapeRoom.Model.Rooms;
using UnityEngine;

namespace RG.EscapeRoom.Model.Rooms
{
    public interface IRoomDefinitionParser
    {
        RoomDefinition Parse(string json);
        string ToJson(RoomDefinition roomDefinition);
    }

    [Serializable]
    public struct RoomDefinition
    {
        public string roomDefinitionId;
        public string scene;
        public PuzzleDefinition[] puzzles;
    }
}