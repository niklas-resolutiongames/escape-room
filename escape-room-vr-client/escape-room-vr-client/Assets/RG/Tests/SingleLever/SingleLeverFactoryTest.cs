using System.Collections;
using NUnit.Framework;
using RG.EscapeRoom.Controller.Player;
using RG.EscapeRoom.Interaction;
using RG.EscapeRoom.Interaction.Scripts;
using RG.EscapeRoom.Model.Puzzles.SingleLever;
using RG.EscapeRoom.Puzzles.SingleLever;
using RG.EscapeRoom.Wiring;
using RG.Tests;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.TestTools;

public class SingleLeverFactoryTest
{
    private ControllerButtonData controllerButtonData;
    private InteractionHandlers interactionHandlers;
    private XRPlayerReference playerReference;
    private PuzzleDefinition puzzleDefinition;
    private RoomFactory roomFactory;
    private SingleLeverFactory singleLeverFactory;

    private SingleLeverSettings singleLeverSettings;
    private TestMotionHelper testMotionHelper;
    private SingleLeverModel singleLeverModel;
    private Puzzle puzzle;
    private SingleLeverReference puzzleView;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        roomFactory = AssetDatabase.LoadAssetAtPath<RoomFactory>("Assets/RG/EscapeRoom/Wiring/Factories/RoomFactory.asset");
        var createRoomTask = roomFactory.CreateRoomWithPlayerInit("TestScene");
        testMotionHelper = new TestMotionHelper();
        yield return testMotionHelper.Await(createRoomTask);

        playerReference = createRoomTask.Result;

        controllerButtonData = new ControllerButtonData();
        interactionHandlers =
            new InteractionHandlers(controllerButtonData, playerReference.leftHand, playerReference.rightHand);
        var interactionDatas = interactionHandlers.Initialize();
        testMotionHelper.TickInBackground(interactionHandlers);
        
        
        singleLeverModel = new SingleLeverModel();
        singleLeverSettings =
            AssetDatabase.LoadAssetAtPath<SingleLeverSettings>(
                "Assets/RG/EscapeRoom/View/Puzzles/SingleLever/SingleLeverSettings.asset");
        singleLeverFactory = new SingleLeverFactory(singleLeverSettings, interactionDatas.pullData);
        puzzleDefinition = new PuzzleDefinition();
        puzzleDefinition.position = new Vector3(0, 1.5f, 0);
        puzzleDefinition.rotation = Quaternion.Euler(-90, -90, 0);
        playerReference.head.transform.SetPositionAndRotation(new Vector3(0, 1.6f, -1),
            playerReference.head.transform.rotation);
        puzzleDefinition.position = new Vector3(0, 1.5f, 0);

        puzzle = singleLeverFactory.CreatePuzzle(puzzleDefinition, singleLeverModel);
        puzzleView = (SingleLeverReference) puzzle.view;
        Assert.NotNull(puzzleView);
        testMotionHelper.TickInBackground(puzzle.controller);
        testMotionHelper.TickInBackground(puzzle.viewController);

    }

    [UnityTest]
    public IEnumerator PullDownLeverWillSetValue()
    {
        var leverEnd = puzzleView.leverEnd;
        var leverEndTransform = leverEnd.transform;
        playerReference.head.transform.LookAt(leverEndTransform);

        yield return testMotionHelper.Await(testMotionHelper.MoveGameObjectToPositionOverTime(
            playerReference.rightHand.gameObject,
            leverEndTransform.position,
            leverEndTransform.rotation, 2));

        yield return testMotionHelper.Await(testMotionHelper.Idle(1));
        controllerButtonData.NotifyButtonPressed(IControllerButtonData.Controller.Right,
            IControllerButtonData.Button.Grip);
        yield return testMotionHelper.Await(testMotionHelper.Idle(1));

        yield return testMotionHelper.Await(testMotionHelper.MoveGameObjectOverTime(
            playerReference.rightHand.gameObject,
            Vector3.down * 0.3f, 2));
        controllerButtonData.NotifyButtonReleased(IControllerButtonData.Controller.Right,
            IControllerButtonData.Button.Grip);

        Assert.AreEqual(-1, singleLeverModel.GetValue());
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