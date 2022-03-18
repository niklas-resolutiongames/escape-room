using UnityEngine;

namespace RG.EscapeRoom.Player.Scripts
{
    
    [CreateAssetMenu(menuName = "Player/XRPlayerSetting", fileName = "XRPlayerSetting")]
    public class XRPlayerSettings: ScriptableObject
    {
        public XRPlayerReference playerReference;
    }
}