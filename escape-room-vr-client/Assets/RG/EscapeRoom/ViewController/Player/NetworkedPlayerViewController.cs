using System.Collections.Generic;
using System.Threading.Tasks;
using RG.EscapeRoom.Controller.Player;
using RG.EscapeRoom.Wiring;
using RG.EscapeRoom.Wiring.Factories;
using RG.EscapeRoomProtocol.Messages;
using UnityEngine;

namespace RG.EscapeRoom.ViewController.Player
{
    public class NetworkedPlayerViewController: ITickable
    {
        private readonly NetworkedPlayerFactory networkedPlayerFactory;
        private readonly ReceivedNetworkStateData receivedNetworkStateData;

        private Dictionary<int, NetworkedPlayerReference> networkedPlayerReferences = new Dictionary<int, NetworkedPlayerReference>();
        private Task<NetworkedPlayerReference> pendingCreatePlayerTask;

        public NetworkedPlayerViewController(NetworkedPlayerFactory networkedPlayerFactory, ReceivedNetworkStateData receivedNetworkStateData)
        {
            this.networkedPlayerFactory = networkedPlayerFactory;
            this.receivedNetworkStateData = receivedNetworkStateData;
        }

        public void Tick()
        {
            var playerIdsInModel = receivedNetworkStateData.receivedPlayerPositions.Keys;
            SpawnFirstMissingPlayer(playerIdsInModel);

            foreach (var playerId in playerIdsInModel)
            {
                if (networkedPlayerReferences.ContainsKey(playerId))
                {
                    var networkPosition = receivedNetworkStateData.receivedPlayerPositions[playerId];
                    var playerReference = networkedPlayerReferences[playerId];
                    playerReference.head.transform.position = MathUtils.UnityVector3(networkPosition.headPosition);
                    playerReference.head.transform.rotation = MathUtils.UnityQuaternion(networkPosition.headRotation);
                    playerReference.leftHand.transform.position =
                        MathUtils.UnityVector3(networkPosition.leftHandPosition);
                    playerReference.leftHand.transform.rotation =
                        MathUtils.UnityQuaternion(networkPosition.leftHandRotation);
                    playerReference.rightHand.transform.position =
                        MathUtils.UnityVector3(networkPosition.rightHandPosition);
                    playerReference.rightHand.transform.rotation =
                        MathUtils.UnityQuaternion(networkPosition.rightHandRotation);
                }
            }

        }

        private void SpawnFirstMissingPlayer(Dictionary<int, PlayerPositionMessage>.KeyCollection playerIdsInModel)
        {
            if (pendingCreatePlayerTask is {IsCompleted: true})
            {
                if (!pendingCreatePlayerTask.IsFaulted)
                {
                    var networkedPlayerReference = pendingCreatePlayerTask.Result;
                    networkedPlayerReferences[networkedPlayerReference.playerId] = networkedPlayerReference;
                }
                else
                {
                    Debug.Log(pendingCreatePlayerTask.Exception);
                }

                pendingCreatePlayerTask = null;
            }
            if (pendingCreatePlayerTask == null)
            {
                foreach (var playerId in playerIdsInModel)
                {
                    if (!networkedPlayerReferences.ContainsKey(playerId))
                    {
                        pendingCreatePlayerTask = networkedPlayerFactory.CreateNetworkedPlayerReference(playerId);
                        return;
                    }
                }
            }
        }
    }
}