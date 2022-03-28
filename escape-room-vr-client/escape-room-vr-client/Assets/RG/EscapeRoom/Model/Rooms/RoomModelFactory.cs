using System;
using System.Collections.Generic;
using RG.EscapeRoom.Model.Puzzles;
using RG.EscapeRoom.Model.Puzzles.SingleLever;

namespace RG.EscapeRoom.Model.Rooms
{
    public class RoomModelFactory
    {
        public RoomModel CreateRoomModel(RoomDefinition roomDefinition)
        {
            var roomModel = new RoomModel();
            roomModel.puzzles = new Dictionary<string, PuzzleModel>();
            for (int i = 0; i < roomDefinition.puzzles.Length; i++)
            {
                var puzzleDefinition = roomDefinition.puzzles[i];
                PuzzleModel puzzleModel = CreatePuzzleModel(puzzleDefinition);
                roomModel.puzzles[puzzleModel.GetId()] = puzzleModel;
            }
            return roomModel;
        }

        private static PuzzleModel CreatePuzzleModel(PuzzleDefinition puzzleDefinition)
        {
            PuzzleTypes type;
            Enum.TryParse(puzzleDefinition.type, out type);
            switch (type)
            {
                case PuzzleTypes.SingleLever:
                    return new SingleLeverModel(puzzleDefinition.id, type);
            }

            return null;
        }
    }

    public class RoomModel
    {
        public Dictionary<string,PuzzleModel> puzzles;
    }
}