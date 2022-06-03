using UnityEngine;

namespace RG.EscapeRoom.Wiring
{
    public class RealTimeProvider: ITimeProvider
    {
        public float GetTime()
        {
            return Time.time;
        }
    }

    public interface ITimeProvider
    {
        public float GetTime();
    }
}