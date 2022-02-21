using System;
using System.Collections;
using System.Threading.Tasks;
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
    private Transform head;
    private GameObject rightHand;
    private GameObject leftHand;
    private PuzzleDefinition puzzleDefinition;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        SceneManager.LoadScene("TestScene");
        singleLeverSettings =
            AssetDatabase.LoadAssetAtPath<SingleLeverSettings>("Assets/RG/EscapeRoom/Puzzles/SingleLever/SingleLeverSettings.asset");
        singleLeverFactory = new SingleLeverFactory(singleLeverSettings);
        while (head == null)
        {
            if (Camera.allCameras.Length > 0)
            {
                var camera = Camera.allCameras[0];
                head = camera.transform;
            }
            yield return null;
        }
        while (rightHand == null || leftHand== null)
        {
            rightHand = GameObject.Find("Right Hand");
            leftHand = GameObject.Find("Left Hand");
            yield return null;
        }
        puzzleDefinition = new PuzzleDefinition();
        puzzleDefinition.position = new Vector3(0,1.5f, 0);
        puzzleDefinition.rotation = Quaternion.Euler(-90, -90, 0);
    }
    
    [UnityTest]
    public IEnumerator FactoryWillCreatePuzzle()
    {
        var puzzle = singleLeverFactory.CreatePuzzle(puzzleDefinition);
        Assert.NotNull(puzzle.gameObject);
        var leverEnd = puzzle.gameObject.transform.Find("Joint/Lever/LeverEnd");
        var leverEndTransform = leverEnd;
        head.LookAt(leverEndTransform);
        
        yield return TestMotionHelper.Await(TestMotionHelper.MoveGameObjectToPositionOverTime(rightHand,
            leverEndTransform.position,
            leverEndTransform.rotation, 3));
    }
}
