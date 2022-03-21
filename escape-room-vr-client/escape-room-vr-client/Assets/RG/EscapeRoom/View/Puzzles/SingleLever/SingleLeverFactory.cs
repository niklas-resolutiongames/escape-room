using RG.EscapeRoom.Controller.Interaction;
using RG.EscapeRoom.Controller.Puzzles.SingleLever;
using RG.EscapeRoom.Model.Puzzles.SingleLever;
using RG.EscapeRoom.Puzzles.SingleLever;
using RG.EscapeRoom.ViewController.Puzzles.SingleLever;
using UnityEngine;

public class SingleLeverFactory : PuzzleFactory
{
    private readonly PullData pullData;
    private readonly SingleLeverSettings singleLeverSettings;

    public SingleLeverFactory(SingleLeverSettings singleLeverSettings, PullData pullData)
    {
        this.singleLeverSettings = singleLeverSettings;
        this.pullData = pullData;
    }

    public Puzzle CreatePuzzle(PuzzleDefinition puzzleDefinition, SingleLeverModel singleLeverModel)
    {
        var gameObject = Object.Instantiate(singleLeverSettings.prefab.gameObject);
        gameObject.transform.SetPositionAndRotation(puzzleDefinition.position, puzzleDefinition.rotation);
        SingleLeverReference singleLeverReference = gameObject.GetComponent<SingleLeverReference>();
        return new Puzzle(singleLeverReference, new SingleLeverController(pullData, singleLeverReference, singleLeverModel),
            new SingleLeverViewController(singleLeverModel, singleLeverReference));
    }
}