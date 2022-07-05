using System.Threading.Tasks;
using RG.EscapeRoom.Controller.Player;
using UnityEngine;

namespace RG.EscapeRoom.Wiring.Factories
{
    [CreateAssetMenu(menuName = "Factories/Networked Player Factory", fileName = "NetworkedPlayerFactory")]
    public class NetworkedPlayerFactory: ScriptableObject
    {
        public NetworkedPlayerReference networkedPlayerReference;

        public async Task<NetworkedPlayerReference> CreateNetworkedPlayerReference(int playerId)
        {
            var player = Instantiate(networkedPlayerReference);
            player.name = $"NetworkedPlayerReference_{playerId}";
            player.playerId = playerId;
            return player;
        }
    }
}