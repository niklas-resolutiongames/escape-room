using UnityEngine;

public interface PuzzleFactory
{
    public Puzzle CreatePuzzle(PuzzleDefinition puzzleDefinition);
}

public struct PuzzleDefinition
{
    public Vector3 position;
    public Quaternion rotation;
}

public class Puzzle
{
    public GameObject gameObject;

    public Puzzle(GameObject gameObject)
    {
        this.gameObject = gameObject;
    }
}