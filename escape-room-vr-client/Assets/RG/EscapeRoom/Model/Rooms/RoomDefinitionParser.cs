using System;
using System.Collections.Generic;
using RG.EscapeRoom.Model.Puzzles;
using UnityEngine;

namespace RG.EscapeRoom.Model.Rooms
{
    public class RoomDefinitionParser
    {
        public RoomDefinition Parse(string json)
        {
            return JsonUtility.FromJson<RoomDefinition>(json);
        }

        public string ToJson(RoomDefinition roomDefinition)
        {
            return JsonUtility.ToJson(roomDefinition);
        }
    }

    [Serializable]
    public struct RoomDefinition
    {
        public string scene;
        public PuzzleDefinition[] puzzles;
    }
}