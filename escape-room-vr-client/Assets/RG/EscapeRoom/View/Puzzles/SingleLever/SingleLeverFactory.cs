using System.Collections.Generic;
using RG.EscapeRoom.Controller.Interaction;
using RG.EscapeRoom.Controller.Player;
using RG.EscapeRoom.Controller.Puzzles.SingleLever;
using RG.EscapeRoom.Model.Puzzles;
using RG.EscapeRoom.Model.Puzzles.SingleLever;
using RG.EscapeRoom.Puzzles.SingleLever;
using RG.EscapeRoom.ViewController.Puzzles.SingleLever;
using UnityEngine;

public class SingleLeverFactory : PuzzleFactory<SingleLeverModel>
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
        gameObject.transform.SetPositionAndRotation(MathUtils.UnityVector3(puzzleDefinition.position), MathUtils.UnityQuaternion(puzzleDefinition.rotation));
        SingleLeverReference singleLeverReference = gameObject.GetComponent<SingleLeverReference>();
        singleLeverReference.leverEnd.SetNetworkId($"{singleLeverModel.GetId()}-leverEnd");
        var grabbables = new Dictionary<string, HandInteractableItemReference>();
        grabbables[singleLeverReference.leverEnd.NetworkId()] = singleLeverReference.leverEnd;
        
        var puzzle = new Puzzle(new SingleLeverController(pullData, singleLeverReference, singleLeverModel),
            singleLeverReference,
            new SingleLeverViewController(singleLeverModel, singleLeverReference), 
            grabbables);
        return puzzle;
    }
}

public class MathUtils
{
    public static Vector3 UnityVector3(RG.EscapeRoom.Model.Math.Vector3 vector)
    {
        return new Vector3(vector.x, vector.y, vector.z);
    }
    public static Quaternion UnityQuaternion(RG.EscapeRoom.Model.Math.Quaternion q)
    {
        return new Quaternion(q.x, q.y, q.z, q.w);
    }

    public static RG.EscapeRoom.Model.Math.Vector3 InternalVector3(Vector3 v)
    {
        return new RG.EscapeRoom.Model.Math.Vector3(v.x, v.y, v.z);
    }
    public static RG.EscapeRoom.Model.Math.Quaternion InternalQuaternion(Quaternion q)
    {
        return new RG.EscapeRoom.Model.Math.Quaternion(q.x, q.y, q.z, q.w);
    }
}