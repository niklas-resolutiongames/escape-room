using System.Threading.Tasks;
using RG.EscapeRoom.Controller.Player;
using UnityEngine;

namespace RG.EscapeRoom.Wiring.Factories
{
    [CreateAssetMenu(menuName = "Factories/XRPlayerFactory", fileName = "XRPlayerFactory")]
    public class XRPlayerFactory : ScriptableObject
    {
        public XRPlayerSettings xrPlayerSettings;

        private XRPlayerReference cachedPlayerReference;

        public async Task<XRPlayerReference> GetUniquePlayerReference()
        {
            if (cachedPlayerReference != null) return cachedPlayerReference;
            cachedPlayerReference = Instantiate(xrPlayerSettings.playerReference);
            while (cachedPlayerReference == null) await Task.Yield();
            return cachedPlayerReference;
        }
    }
}