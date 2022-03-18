﻿using System.Threading.Tasks;
using UnityEngine;

namespace RG.EscapeRoom.Player.Scripts
{
    [CreateAssetMenu(menuName = "Factories/XRPlayerFactory", fileName = "XRPlayerFactory")]
    public class XRPlayerFactory: ScriptableObject
    {
        public XRPlayerSettings xrPlayerSettings;
        
        private XRPlayerReference cachedPlayerReference;

        public async Task<XRPlayerReference> GetUniquePlayerReference()
        {
            if (cachedPlayerReference != null)
            {
                return cachedPlayerReference;
            }
            cachedPlayerReference = Instantiate(xrPlayerSettings.playerReference);
            while (cachedPlayerReference == null)
            {
                Task.Yield();
            }
            return cachedPlayerReference;
        }
    }
}