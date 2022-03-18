using UnityEditor;
using UnityEngine;

namespace RG.EscapeRoom.Puzzles.SingleLever
{
    [CreateAssetMenu(fileName = "SingleLeverSettings", menuName = "Puzzles/SingleLeverSettings", order = 1)]
    public class SingleLeverSettings: ScriptableObject
    {
        public GameObject prefab;
    }
}