using System.Collections;
using NUnit.Framework;
using RG.EscapeRoom.Puzzles.SingleLever;
using UnityEditor;
using UnityEngine.TestTools;

public class FactoryTest
{

    public SingleLeverSettings singleLeverSettings;
    private SingleLeverFactory singleLeverFactory;

    [SetUp]
    public void SetUp()
    {
        
        singleLeverSettings =
            AssetDatabase.LoadAssetAtPath<SingleLeverSettings>("Assets/RG/EscapeRoom/Puzzles/SingleLever/SingleLeverSettings.asset");
        singleLeverFactory = new SingleLeverFactory(singleLeverSettings);
    }
    
    [UnityTest]
    public IEnumerator FactoryWillCreatePuzzle()
    {
        var puzzle = singleLeverFactory.CreatePuzzle(new PuzzleDefinition());
        Assert.NotNull(puzzle.gameObject);
        yield return null;
    }
}
