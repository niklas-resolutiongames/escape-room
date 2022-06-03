using System.Collections.Generic;
using RG.EscapeRoom.Controller;
using RG.EscapeRoom.Controller.Player;
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
    public readonly Dictionary<string, HandInteractableItemReference> grabbables;

    public Puzzle(IController controller, Component view, IViewController viewController, Dictionary<string, HandInteractableItemReference> grabbables)
    {
        this.controller = controller;
        this.view = view;
        this.viewController = viewController;
        this.grabbables = grabbables;
    }
}