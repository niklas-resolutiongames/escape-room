namespace RG.EscapeRoom.Model.Puzzles.SingleLever
{
    public class SingleLeverModel: IIntegerOutputPuzzleModel
    {
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
    }

    public interface IIntegerOutputPuzzleModel
    {
        public int GetValue();
    }
}