using System;
using NUnit.Framework;
using RG.EscapeRoom.Model.Math;
using RG.EscapeRoom.Model.Puzzles;
using RG.EscapeRoom.Model.Rooms;

public class RoomDefinitionParserTest
{
    private RoomDefinitionParser roomDefinitionParser;
    private RoomDefinition roomDefinition;

    [SetUp]
    public void SetUp()
    {
        roomDefinitionParser = new RoomDefinitionParser();
        roomDefinition = new RoomDefinition();
        roomDefinition.scene = "TestScene";
        for (int i = 0; i < 360; i += 30)
        {
            var puzzle = new PuzzleDefinition();
            puzzle.id = $"lever_{i}";
            puzzle.position = new Vector3((float)Math.Cos(i), 1.5f, (float)Math.Sin(i));
            puzzle.rotation = MathUtils.InternalQuaternion(UnityEngine.Quaternion.Euler(-90 + i, -90, 0));
            puzzle.position = new Vector3();
            roomDefinition.puzzles.Add(puzzle);
        }
    }

    [Test]
    public void ParseFileWillCreatePuzzleDefintions()
    {
        
    }
}