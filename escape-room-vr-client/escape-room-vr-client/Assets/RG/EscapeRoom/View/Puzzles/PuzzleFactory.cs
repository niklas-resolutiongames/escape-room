using RG.EscapeRoom.Controller;
using RG.EscapeRoom.Model.Puzzles.SingleLever;
using RG.EscapeRoom.ViewController;
using UnityEngine;

public interface PuzzleFactory
{
    public Puzzle CreatePuzzle(PuzzleDefinition puzzleDefinition, SingleLeverModel singleLeverModel);
}

public struct PuzzleDefinition
{
    public Vector3 position;
    public Quaternion rotation;
}

public class Puzzle
{
    public readonly IController controller;
    public readonly Component view;
    public readonly IViewController viewController;

    public Puzzle(Component view, IController controller, IViewController viewController)
    {
        this.view = view;
        this.controller = controller;
        this.viewController = viewController;
    }
}