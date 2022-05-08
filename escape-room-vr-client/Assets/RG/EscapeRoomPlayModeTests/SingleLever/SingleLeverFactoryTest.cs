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
    private GameClient gameClient;
    private EscapeRoomSocketServer escapeRoomSocketServer;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        testMotionHelper = new TestMotionHelper();            
        escapeRoomSocketServer = ServerTestUtils.StartServer(12345, 30);
        var gameClientFactory = new GameClientFactory();
        GameClientFactory.TESTING_ROOM_JSON = TestUtil.ReadTextFile(
            "Assets/RG/EscapeRoomPlayModeTests/SingleLever/SingleLeverFactoryTestRoomDefinition.json");
        gameClient = gameClientFactory.CreateGameClient();
        gameClient.Connect(12345, "127.0.0.1");
        
        testMotionHelper.TickInBackground(gameClient);

        yield return testMotionHelper.Await(Task.Run(async () =>
        {
            while (!gameClient.roomIsLoaded)
            {
                await Task.Yield();
            }
        }));
        testMotionHelper.SetPlayerReference(gameClient.playerReference);
        yield return null;
    }

    [UnityTearDown]
    public void StopServer()
    {
        ServerTestUtils.StopServer();
    }

    [UnityTest]
    public IEnumerator PullLeverWillSetValue()
    {
        for (int i = 0; i < gameClient.room.puzzles.Count; i++)
        {
            var puzzle = gameClient.room.puzzles[i];
            var puzzleDefinition = gameClient.roomDefinition.puzzles[i];
            var singleLeverModel = (SingleLeverModel) gameClient.room.roomModel.puzzles[puzzleDefinition.id];
            var isDown = i % 2 == 0;
            var direction = isDown ? Vector3.down: Vector3.up;
            var expectedValue = isDown ? -1 : 1;
            var activeHand = isDown? gameClient.playerReference.rightHand: gameClient.playerReference.leftHand;
            var activeController = isDown? IControllerButtonData.Controller.Right: IControllerButtonData.Controller.Left;
            var leverEnd = ((SingleLeverReference) puzzle.view).leverEnd;
            var leverEndTransform = leverEnd.transform;
            
            yield return testMotionHelper.Await(testMotionHelper.MoveGameObjectToPositionOverTime(
                activeHand.gameObject,
                leverEndTransform.position,
                leverEndTransform.rotation, 
                2));

            yield return testMotionHelper.Await(testMotionHelper.Idle(1));
            gameClient.controllerButtonData.NotifyButtonPressed(activeController,
                IControllerButtonData.Button.Grip);
            yield return testMotionHelper.Await(testMotionHelper.Idle(1));

            yield return testMotionHelper.Await(testMotionHelper.MoveGameObjectOverTime(
                activeHand.gameObject,
                direction * 0.3f, 2));
            gameClient.controllerButtonData.NotifyButtonReleased(activeController,
                IControllerButtonData.Button.Grip);

            Assert.AreEqual(expectedValue, singleLeverModel.GetValue());
        }
    }

    [UnityTest]
    public IEnumerator Play10SecondsWithInputEnabled()
    {
        ControllerButtonListener controllerButtonListener = new ControllerButtonListener(gameClient.controllerButtonData);
        controllerButtonListener.Initialize();
        testMotionHelper.TickInBackground(controllerButtonListener);
        yield return testMotionHelper.Await(testMotionHelper.Idle(10));
    }
}