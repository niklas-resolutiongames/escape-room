using System.Collections;
using NUnit.Framework;
using RG.EscapeRoom.Puzzles.SingleLever;
using RG.Tests;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class SingleLeverFactoryTest
{

    private SingleLeverSettings singleLeverSettings;
    private SingleLeverFactory singleLeverFactory;
    private XRPlayerReference playerReference;
    private PuzzleDefinition puzzleDefinition;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        SceneManager.LoadScene("TestScene");
        singleLeverSettings =
            AssetDatabase.LoadAssetAtPath<SingleLeverSettings>("Assets/RG/EscapeRoom/Puzzles/SingleLever/SingleLeverSettings.asset");
        singleLeverFactory = new SingleLeverFactory(singleLeverSettings);
        while (playerReference == null) {
            playerReference = GameObject.FindObjectOfType<XRPlayerReference>();
            yield return null;
        }
        puzzleDefinition = new PuzzleDefinition();
        puzzleDefinition.position = new Vector3(0,1.5f, 0);
        puzzleDefinition.rotation = Quaternion.Euler(-90, -90, 0);  
        playerReference.head.transform.SetPositionAndRotation(new Vector3(0,1.6f,-1), playerReference.head.transform.rotation);
    }
    
    [UnityTest]
    public IEnumerator FactoryWillCreatePuzzle()
    {
        var puzzle = singleLeverFactory.CreatePuzzle(puzzleDefinition);
        Assert.NotNull(puzzle.gameObject);
        var leverEnd = puzzle.gameObject.transform.Find("Joint/Lever/LeverEnd");
        var leverEndTransform = leverEnd;
        playerReference.head.transform.LookAt(leverEndTransform);
        
        yield return TestMotionHelper.Await(TestMotionHelper.MoveGameObjectToPositionOverTime(playerReference.rightHand.gameObject,
            leverEndTransform.position,
            leverEndTransform.rotation, 2));
        
        yield return TestMotionHelper.Await(TestMotionHelper.Idle(2));
        
        yield return TestMotionHelper.Await(TestMotionHelper.MoveGameObjectOverTime(playerReference.rightHand.gameObject,
            Vector3.down * 0.2f, 1));
        
    }
}
