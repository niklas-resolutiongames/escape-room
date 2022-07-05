using RG.EscapeRoom.Model.Rooms;
using UnityEngine;

namespace RG.EscapeRoom.Wiring.Factories
{
    public class RoomDefinitionParser : IRoomDefinitionParser
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
}