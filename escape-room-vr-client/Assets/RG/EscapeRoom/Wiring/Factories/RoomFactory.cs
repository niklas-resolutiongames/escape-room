using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RG.EscapeRoom.Controller.Player;
using RG.EscapeRoom.Model.Puzzles;
using RG.EscapeRoom.Model.Puzzles.SingleLever;
using RG.EscapeRoom.Model.Rooms;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RG.EscapeRoom.Wiring.Factories
{
    [CreateAssetMenu(menuName = "Factories/RoomFactory", fileName = "RoomFactory")]
    public class RoomFactory : ScriptableObject
    {
        public XRPlayerFactory xrPlayerFactory;
        public PuzzlesFactoryFactory puzzlesFactoryFactory;

        public RoomModelFactory roomModelFactory = new RoomModelFactory();

        public async Task<Room> CreateRoomWithPlayerInIt(RoomDefinition roomDefinition, InteractionDatas interactionDatas)
        {
            await LoadScene(roomDefinition);

            var playerReference = await xrPlayerFactory.GetUniquePlayerReference();
            
            RoomModel roomModel = roomModelFactory.CreateRoomModel(roomDefinition);

            var puzzles = CreatePuzzles(roomDefinition, roomModel, interactionDatas);
            
            return new Room(playerReference, roomModel, puzzles);
        }

        private List<Puzzle> CreatePuzzles(RoomDefinition roomDefinition, RoomModel roomModel,
            InteractionDatas interactionDatas)
        {
            var puzzles = new List<Puzzle>();

            for (int i = 0; i < roomDefinition.puzzles.Length; i++)
            {
                var puzzleDefinition = roomDefinition.puzzles[i];
                PuzzleTypes puzzleDefinitionType;
                Enum.TryParse(puzzleDefinition.type, true, out puzzleDefinitionType);
                switch (puzzleDefinitionType)
                {
                    case PuzzleTypes.SingleLever:
                        SingleLeverModel model = (SingleLeverModel) roomModel.puzzles[puzzleDefinition.id];
                        puzzles.Add(puzzlesFactoryFactory.GetSingleLeverFactory(interactionDatas).CreatePuzzle(puzzleDefinition, model));
                        break;
                }
            }
            
            return puzzles;
        }


        private static async Task LoadScene(RoomDefinition roomDefinition)
        {
            var loadAction = SceneManager.LoadSceneAsync(roomDefinition.scene);

            var stillLoading = true;
            var totalWait = 0;
            while (stillLoading)
            {
                if (loadAction.isDone) stillLoading = false;

                if (totalWait > 1000)
                {
                    stillLoading = false;
                    Debug.LogError("Waited for scene for too long");
                }

                await Task.Delay(10);
                totalWait += 10;
            }
        }
    }

    public class Room
    {
        public XRPlayerReference playerReference;
        public RoomModel roomModel;
        public List<Puzzle> puzzles;

        public Room(XRPlayerReference playerReference, RoomModel roomModel, List<Puzzle> puzzles)
        {
            this.playerReference = playerReference;
            this.roomModel = roomModel;
            this.puzzles = puzzles;
        }

        public HandInteractableItemReference FindHandInteractableItemReference(string networkId)
        {
            foreach (var puzzle in puzzles)
            {
                if (puzzle.grabbables.ContainsKey(networkId))
                {
                    return puzzle.grabbables[networkId];
                }
            }

            return null;
        }
    }
}