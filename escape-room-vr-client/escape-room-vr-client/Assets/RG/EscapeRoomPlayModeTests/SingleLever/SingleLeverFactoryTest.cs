using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using RG.EscapeRoom.Controller.Player;
using RG.EscapeRoom.Interaction;
using RG.EscapeRoom.Interaction.Scripts;
using RG.EscapeRoom.Model.Puzzles;
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
    private SingleLeverFactory singleLeverFactory;

    private SingleLeverSettings singleLeverSettings;
    private TestMotionHelper testMotionHelper;
    private List<SingleLeverModel> singleLeverModels = new List<SingleLeverModel>();
    private List<Puzzle> puzzles = new List<Puzzle>();
    private List<SingleLeverReference> puzzleViews = new List<SingleLeverReference>();

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        var roomDefinitionParser = new RoomDefinitionParser();
        var json = TestUtil.ReadTextFile(
            "Assets/RG/EscapeRoomPlayModeTests/SingleLever/SingleLeverFactoryTestRoomDefinition.json");
        
        var roomDefinition = roomDefinitionParser.Parse(json);
        
        testMotionHelper = new TestMotionHelper();

        roomFactory =
            AssetDatabase.LoadAssetAtPath<RoomFactory>("Assets/RG/EscapeRoom/Wiring/Factories/RoomFactory.asset");
        var createRoomTask = roomFactory.CreateRoomWithPlayerInIt(roomDefinition);
        yield return testMotionHelper.Await(createRoomTask);

        playerReference = createRoomTask.Result;

        controllerButtonData = new ControllerButtonData();
        interactionHandlers =
            new InteractionHandlers(controllerButtonData, playerReference.leftHand, playerReference.rightHand);
        var interactionDatas = interactionHandlers.Initialize();
        testMotionHelper.TickInBackground(interactionHandlers);

        singleLeverSettings = AssetDatabase.LoadAssetAtPath<SingleLeverSettings>(
            "Assets/RG/EscapeRoom/View/Puzzles/SingleLever/SingleLeverSettings.asset");
        singleLeverFactory = new SingleLeverFactory(singleLeverSettings, interactionDatas.pullData);
        
        for (int i = 0; i < roomDefinition.puzzles.Count; i++)
        {
            var puzzleDefinition = roomDefinition.puzzles[i];
                
            var singleLeverModel = new SingleLeverModel();
            var puzzle = singleLeverFactory.CreatePuzzle(puzzleDefinition, singleLeverModel);
            var puzzleView = (SingleLeverReference) puzzle.view;
            testMotionHelper.TickInBackground(puzzle.controller);
            testMotionHelper.TickInBackground(puzzle.viewController);
            Assert.NotNull(puzzleView);
            puzzleViews.Add(puzzleView);
            singleLeverModels.Add(singleLeverModel);
        }
    }

    [UnityTest]
    public IEnumerator PullLeverWillSetValue()
    {
        playerReference.head.transform.position = new Vector3(1.2f, 2.4f, 0.25f);
        for (int i = 0; i < puzzleViews.Count; i++)
        {
            var puzzleView = puzzleViews[i];
            var singleLeverModel = singleLeverModels[i];
            var isDown = i % 2 == 0;
            var direction = isDown ? Vector3.down: Vector3.up;
            var expectedValue = isDown ? -1 : 1;
            var activeHand = isDown? playerReference.rightHand: playerReference.leftHand;
            var activeController = isDown? IControllerButtonData.Controller.Right: IControllerButtonData.Controller.Left;
            var leverEnd = puzzleView.leverEnd;
            var leverEndTransform = leverEnd.transform;
            
            playerReference.head.transform.LookAt(leverEndTransform);

            yield return testMotionHelper.Await(testMotionHelper.MoveGameObjectToPositionOverTime(
                activeHand.gameObject,
                leverEndTransform.position,
                leverEndTransform.rotation, 
                2,
                playerReference.head));

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