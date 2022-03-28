using System;

namespace RG.EscapeRoom.Model.Puzzles.SingleLever
{
    public class SingleLeverModel: IIntegerOutputPuzzleModel
    {
        private readonly string id;
        private readonly PuzzleTypes type;

        public SingleLeverModel(string id, PuzzleTypes type)
        {
            this.id = id;
            this.type = type;
        }

        public float leverPosition;

        public int GetValue()
        {
            if (leverPosition < -45)
            {
                return -1;
            }
            if (leverPosition > 45)
            {
                return 1;
            }

            return 0;
        }

        public string GetId()
        {
            return id;
        }

        public PuzzleTypes GetType()
        {
            return type;
        }
    }

    public interface IIntegerOutputPuzzleModel: PuzzleModel
    {
        public int GetValue();
    }

    public interface PuzzleModel
    {
        public string GetId();
        public PuzzleTypes GetType();
    }
}