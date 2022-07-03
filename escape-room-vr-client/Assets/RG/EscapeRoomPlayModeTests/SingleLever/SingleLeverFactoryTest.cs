using System.Collections;
using System.Threading.Tasks;
using NUnit.Framework;
using RG.EscapeRoom.Interaction.Scripts;
using RG.EscapeRoom.Model.Puzzles.SingleLever;
using RG.EscapeRoom.Puzzles.SingleLever;
using RG.EscapeRoom.Wiring;
using RG.Tests;
using UnityEngine;
using UnityEngine.TestTools;

public class SingleLeverFactoryTest
{
    private TestMotionHelper testMotionHelper;
    private GameClient localGameClient;
    private EscapeRoomSocketServer escapeRoomSocketServer;
    private int serverPort = 12345;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        testMotionHelper = new TestMotionHelper();
        escapeRoomSocketServer = ServerTestUtils.StartServer(serverPort, 30);
        var gameClientFactory = new GameClientFactory();
        GameClientFactory.TESTING_ROOM_JSON = TestUtil.ReadTextFile(
            "Assets/RG/EscapeRoomPlayModeTests/SingleLever/SingleLeverFactoryTestRoomDefinition.json");
        localGameClient = gameClientFactory.CreateGameClient();
        localGameClient.Connect(serverPort, "127.0.0.1");
        testMotionHelper.TickInBackground(localGameClient);

        yield return testMotionHelper.Await(Task.Run(async () =>
        {
            while (!localGameClient.roomIsLoaded)
            {
                await Task.Yield();
            }
        }));
        testMotionHelper.SetPlayerReference(localGameClient.playerReference);
        yield return null;
    }

    [UnityTearDown]
    public IEnumerator StopServer()
    {
        ServerTestUtils.StopServer();
        localGameClient.Dispose();
        while (ServerTestUtils.ServerIsRunning())
        {
            yield return null;
        }
    }

    [UnityTest]
    public IEnumerator PullLeverWillSetValue()
    {
        for (int i = 0; i < localGameClient.room.puzzles.Count; i++)
        {
            var puzzle = localGameClient.room.puzzles[i];
            var puzzleDefinition = localGameClient.roomDefinition.puzzles[i];
            var singleLeverModel = (SingleLeverModel) localGameClient.room.roomModel.puzzles[puzzleDefinition.id];
            var isDown = i % 2 == 0;
            var direction = isDown ? Vector3.down: Vector3.up;
            var expectedValue = isDown ? -1 : 1;
            var activeHand = isDown? localGameClient.playerReference.rightHand: localGameClient.playerReference.leftHand;
            var activeController = isDown? IControllerButtonData.Controller.Right: IControllerButtonData.Controller.Left;
            var leverEnd = ((SingleLeverReference) puzzle.view).leverEnd;
            var leverEndTransform = leverEnd.transform;
            
            yield return testMotionHelper.Await(testMotionHelper.MoveGameObjectToPositionOverTime(
                activeHand.gameObject,
                leverEndTransform.position,
                leverEndTransform.rotation, 
                2));

            yield return testMotionHelper.Await(testMotionHelper.Idle(1));
            localGameClient.controllerButtonData.NotifyButtonPressed(activeController,
                IControllerButtonData.Button.Grip);
            yield return testMotionHelper.Await(testMotionHelper.Idle(1));

            yield return testMotionHelper.Await(testMotionHelper.MoveGameObjectOverTime(
                activeHand.gameObject,
                direction * 0.3f, 2));
            localGameClient.controllerButtonData.NotifyButtonReleased(activeController,
                IControllerButtonData.Button.Grip);

            Assert.AreEqual(expectedValue, singleLeverModel.GetValue());
        }
    }

    [UnityTest]
    public IEnumerator PlayerMovementsAreNetworked()
    {
        
        yield return testMotionHelper.Await(testMotionHelper.MoveGameObjectToPositionOverTime(
            localGameClient.playerReference.head.gameObject,
            new Vector3(1,2,3),
            Quaternion.Euler(5,6,7), 
            1));
        yield return testMotionHelper.Await(testMotionHelper.MoveGameObjectToPositionOverTime(
            localGameClient.playerReference.leftHand.gameObject,
            new Vector3(2,2,3),
            Quaternion.Euler(5,6,7), 
            1));
        yield return testMotionHelper.Await(testMotionHelper.MoveGameObjectToPositionOverTime(
            localGameClient.playerReference.rightHand.gameObject,
            new Vector3(-2,2,3),
            Quaternion.Euler(5,6,7), 
            1));

        yield return testMotionHelper.Await(testMotionHelper.Idle(1));
        
        var localPlayerNetworkState = localGameClient.receivedNetworkStateData.receivedPlayerPositions[localGameClient.receivedNetworkStateData.localPlayerNetworkId];
        var localPlayerHeadTransform = localGameClient.playerReference.head.transform;
        Assert.AreEqual(MathUtils.InternalVector3(localPlayerHeadTransform.position), localPlayerNetworkState.headPosition);
        Assert.AreEqual(MathUtils.InternalQuaternion(localPlayerHeadTransform.rotation), localPlayerNetworkState.headRotation);

        var localPlayerLeftHandTransform = localGameClient.playerReference.leftHand.transform;
        var localPlayerRightHandTransform = localGameClient.playerReference.rightHand.transform;
        Assert.AreEqual(MathUtils.InternalVector3(localPlayerLeftHandTransform.position), localPlayerNetworkState.leftHandPosition);
        Assert.AreEqual(MathUtils.InternalVector3(localPlayerRightHandTransform.position), localPlayerNetworkState.rightHandPosition);
        Assert.AreEqual(MathUtils.InternalQuaternion(localPlayerLeftHandTransform.rotation), localPlayerNetworkState.leftHandRotation);
        Assert.AreEqual(MathUtils.InternalQuaternion(localPlayerRightHandTransform.rotation), localPlayerNetworkState.rightHandRotation);
    }
}