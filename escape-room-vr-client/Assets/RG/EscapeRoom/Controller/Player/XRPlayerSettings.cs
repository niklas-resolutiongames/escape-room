using UnityEngine;

namespace RG.EscapeRoom.Controller.Player
{
    [CreateAssetMenu(menuName = "Player/XRPlayerSetting", fileName = "XRPlayerSetting")]
    public class XRPlayerSettings : ScriptableObject
    {
        public XRPlayerReference playerReference;
    }
}