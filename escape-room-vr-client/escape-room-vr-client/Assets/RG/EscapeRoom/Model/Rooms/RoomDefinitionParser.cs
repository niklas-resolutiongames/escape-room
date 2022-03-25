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

    public class RoomDefinition
    {
        public string scene;
        public List<PuzzleDefinition> puzzles;
    }
}