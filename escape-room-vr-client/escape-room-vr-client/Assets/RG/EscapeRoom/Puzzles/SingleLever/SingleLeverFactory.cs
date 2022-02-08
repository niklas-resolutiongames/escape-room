using RG.EscapeRoom.Puzzles.SingleLever;
using UnityEngine;

public class SingleLeverFactory: PuzzleFactory
{

    private readonly SingleLeverSettings singleLeverSettings;

    public SingleLeverFactory(SingleLeverSettings singleLeverSettings)
    {
        this.singleLeverSettings = singleLeverSettings;
    }

    public Puzzle CreatePuzzle(PuzzleDefinition puzzleDefinition)
    {
        var gameObject = Object.Instantiate(singleLeverSettings.prefab);
        return new Puzzle(gameObject);
    }
}