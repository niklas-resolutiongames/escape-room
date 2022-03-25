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
    }

    [Test]
    public void ParseFileWillCreatePuzzleDefintions()
    {
        
    }
}