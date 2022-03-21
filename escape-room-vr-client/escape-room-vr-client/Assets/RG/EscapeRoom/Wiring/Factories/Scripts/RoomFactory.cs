using System.Threading.Tasks;
using RG.EscapeRoom.Controller.Player;
using RG.EscapeRoom.Player.Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "Factories/RoomFactory", fileName = "RoomFactory")]
public class RoomFactory : ScriptableObject
{
    public XRPlayerFactory xrPlayerFactory;

    public async Task<XRPlayerReference> CreateRoomWithPlayerInit(string sceneName)
    {
        var loadAction = SceneManager.LoadSceneAsync(sceneName);

        var stillLoading = true;
        var totalWait = 0;
        while (stillLoading)
        {
            if (loadAction.isDone) stillLoading = false;

            if (totalWait > 1000)
            {
                stillLoading = false;
                Debug.LogError("Waited for scene for too long");
            }

            await Task.Delay(10);
            totalWait += 10;
        }

        var playerReference = xrPlayerFactory.GetUniquePlayerReference();
        return await playerReference;
    }
}