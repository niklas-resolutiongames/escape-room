using RG.EscapeRoom.Controller;
using RG.EscapeRoom.Model.Puzzles;
using RG.EscapeRoom.ViewController;
using UnityEngine;

public interface PuzzleFactory<ModelType>
{
    public Puzzle CreatePuzzle(PuzzleDefinition puzzleDefinition, ModelType model);
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