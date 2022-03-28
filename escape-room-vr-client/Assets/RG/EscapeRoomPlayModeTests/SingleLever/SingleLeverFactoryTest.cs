using System.Collections;
using NUnit.Framework;
using RG.EscapeRoom.Controller.Player;
using RG.EscapeRoom.Interaction;
using RG.EscapeRoom.Interaction.Scripts;
using RG.EscapeRoom.Model.Puzzles.SingleLever;
using RG.EscapeRoom.Model.Rooms;
using RG.EscapeRoom.Puzzles.SingleLever;
using RG.EscapeRoom.Wiring;
using RG.EscapeRoom.Wiring.Factories;
using RG.Tests;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

public class SingleLeverFactoryTest
{
    private ControllerButtonData controllerButtonData;
    private InteractionHandlers interactionHandlers;
    private XRPlayerReference playerReference;
    private RoomFactory roomFactory;
    private TestMotionHelper testMotionHelper;
    private Room room;
    private RoomDefinition roomDefinition;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        var interactionDatas = InteractionHandlers.CreateDatas();
        var roomDefinitionParser = new RoomDefinitionParser();
        var json = TestUtil.ReadTextFile(
            "Assets/RG/EscapeRoomPlayModeTests/SingleLever/SingleLeverFactoryTestRoomDefinition.json");
        
        roomDefinition = roomDefinitionParser.Parse(json);

        testMotionHelper = new TestMotionHelper();

        roomFactory =
            AssetDatabase.LoadAssetAtPath<RoomFactory>("Assets/RG/EscapeRoom/Wiring/Factories/RoomFactory.asset");
        var createRoomTask = roomFactory.CreateRoomWithPlayerInIt(roomDefinition, interactionDatas);
        yield return testMotionHelper.Await(createRoomTask);

        room = createRoomTask.Result;
        playerReference = room.playerReference;

        testMotionHelper.SetPlayerReference(playerReference);

        playerReference.head.transform.position = new Vector3(0f, 1.8f, 0f);
        playerReference.leftHand.transform.position = new Vector3(.2f, 1.5f, -.2f);
        playerReference.rightHand.transform.position = new Vector3(.2f, 1.5f, .2f);
        
        controllerButtonData = new ControllerButtonData();
        interactionHandlers =
            new InteractionHandlers(controllerButtonData, playerReference.leftHand, playerReference.rightHand, interactionDatas);
        interactionHandlers.InitializeHandlers();
        testMotionHelper.TickInBackground(interactionHandlers);

        for (int i = 0; i < roomDefinition.puzzles.Length; i++)
        {
            var puzzle = room.puzzles[i];
            testMotionHelper.TickInBackground(puzzle.controller);
            testMotionHelper.TickInBackground(puzzle.viewController);
        }
    }

    [UnityTest]
    public IEnumerator PullLeverWillSetValue()
    {
        for (int i = 0; i < room.puzzles.Count; i++)
        {
            var puzzle = room.puzzles[i];
            var puzzleDefinition = roomDefinition.puzzles[i];
            var singleLeverModel = (SingleLeverModel) room.roomModel.puzzles[puzzleDefinition.id];
            var isDown = i % 2 == 0;
            var direction = isDown ? Vector3.down: Vector3.up;
            var expectedValue = isDown ? -1 : 1;
            var activeHand = isDown? playerReference.rightHand: playerReference.leftHand;
            var activeController = isDown? IControllerButtonData.Controller.Right: IControllerButtonData.Controller.Left;
            var leverEnd = ((SingleLeverReference) puzzle.view).leverEnd;
            var leverEndTransform = leverEnd.transform;
            
            yield return testMotionHelper.Await(testMotionHelper.MoveGameObjectToPositionOverTime(
                activeHand.gameObject,
                leverEndTransform.position,
                leverEndTransform.rotation, 
                2));

            yield return testMotionHelper.Await(testMotionHelper.Idle(1));
            controllerButtonData.NotifyButtonPressed(activeController,
                IControllerButtonData.Button.Grip);
            yield return testMotionHelper.Await(testMotionHelper.Idle(1));

            yield return testMotionHelper.Await(testMotionHelper.MoveGameObjectOverTime(
                activeHand.gameObject,
                direction * 0.3f, 2));
            controllerButtonData.NotifyButtonReleased(activeController,
                IControllerButtonData.Button.Grip);

            Assert.AreEqual(expectedValue, singleLeverModel.GetValue());
        }
    }

    [UnityTest]
    public IEnumerator Play10SecondsWithInputEnabled()
    {
        ControllerButtonListener controllerButtonListener = new ControllerButtonListener(controllerButtonData);
        controllerButtonListener.Initialize();
        testMotionHelper.TickInBackground(controllerButtonListener);
        yield return testMotionHelper.Await(testMotionHelper.Idle(10));
    }
}