using System.Collections.Generic;
using RG.EscapeRoom.Model.Puzzles;
using RG.EscapeRoom.Model.Puzzles.SingleLever;
using RG.EscapeRoom.Puzzles.SingleLever;
using UnityEngine;

namespace RG.EscapeRoom.Wiring.Factories
{
    
    [CreateAssetMenu(menuName = "Factories/PuzzlesFactoryFactory", fileName = "PuzzlesFactoryFactory")]
    public class PuzzlesFactoryFactory:ScriptableObject
    {
        public SingleLeverSettings singleLeverSettings;

        public PuzzleFactory<SingleLeverModel> GetSingleLeverFactory(InteractionDatas interactionDatas)
        {
            return new SingleLeverFactory(singleLeverSettings, interactionDatas.pullData);
        }
    }
}