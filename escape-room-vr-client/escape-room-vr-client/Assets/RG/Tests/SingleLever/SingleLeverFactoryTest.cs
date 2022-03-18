using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using RG.EscapeRoom.Interaction.Scripts;
using RG.EscapeRoom.Puzzles.SingleLever;
using RG.Tests;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

public class SingleLeverFactoryTest
{

    private SingleLeverSettings singleLeverSettings;
    private SingleLeverFactory singleLeverFactory;
    private XRPlayerReference playerReference;
    private PuzzleDefinition puzzleDefinition;
    private RoomFactory roomFactory;
    private InteractionHandlers interactionHandlers;
    private ControllerButtonData controllerButtonData;
    private TestMotionHelper testMotionHelper;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        roomFactory = AssetDatabase.LoadAssetAtPath<RoomFactory>("Assets/RG/EscapeRoom/Factories/RoomFactory.asset");
        var createRoomTask = roomFactory.CreateRoomWithPlayerInit("TestScene");
        testMotionHelper = new TestMotionHelper();
        yield return testMotionHelper.Await(createRoomTask);
        
        playerReference = createRoomTask.Result;
        
        singleLeverSettings =
            AssetDatabase.LoadAssetAtPath<SingleLeverSettings>("Assets/RG/EscapeRoom/Puzzles/SingleLever/SingleLeverSettings.asset");
        singleLeverFactory = new SingleLeverFactory(singleLeverSettings);
        puzzleDefinition = new PuzzleDefinition();
        puzzleDefinition.position = new Vector3(0,1.5f, 0);
        puzzleDefinition.rotation = Quaternion.Euler(-90, -90, 0);  
        playerReference.head.transform.SetPositionAndRotation(new Vector3(0,1.6f,-1), playerReference.head.transform.rotation);
        puzzleDefinition.position = new Vector3(0,1.5f, 0);


        controllerButtonData = new ControllerButtonData();
        interactionHandlers = new InteractionHandlers(controllerButtonData, playerReference.leftHand, playerReference.rightHand);
        interactionHandlers.Initialize();
        testMotionHelper.TickInBackground(interactionHandlers);
    }
    
    [UnityTest]
    public IEnumerator FactoryWillCreatePuzzle()
    {
        var puzzle = singleLeverFactory.CreatePuzzle(puzzleDefinition);
        Assert.NotNull(puzzle.gameObject);
        var leverEnd = puzzle.gameObject.transform.Find("Joint/Lever/LeverEnd");
        var leverEndTransform = leverEnd;
        playerReference.head.transform.LookAt(leverEndTransform);
        
        yield return testMotionHelper.Await(testMotionHelper.MoveGameObjectToPositionOverTime(playerReference.rightHand.gameObject,
            leverEndTransform.position,
            leverEndTransform.rotation, 2));
        
        yield return testMotionHelper.Await(testMotionHelper.Idle(1));
        controllerButtonData.NotifyButtonPressed(IControllerButtonData.Controller.Right,IControllerButtonData.Button.Grip);
        yield return testMotionHelper.Await(testMotionHelper.Idle(1));
        
        yield return testMotionHelper.Await(testMotionHelper.MoveGameObjectOverTime(playerReference.rightHand.gameObject,
            Vector3.down * 0.2f, 1));
        controllerButtonData.NotifyButtonReleased(IControllerButtonData.Controller.Right,IControllerButtonData.Button.Grip);

        yield return testMotionHelper.Await(testMotionHelper.Idle(10));
    }
   
    
}
