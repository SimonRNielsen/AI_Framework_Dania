using AIGame.Core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using MortensKombat;

// baseai.position - find egen position

//Set destination 0,0 CP

//Find path

//Move

//Check - succes


public class MoveToCP : MKNode
{
    #region Fields
    private SuperMorten mortenAI;
    private bool inCP;

    #endregion
    #region Properties
    #endregion
    #region Construtor
    public MoveToCP(MKBlackboard blackboard, SuperMorten mortenAI) : base(blackboard)
    {
        this.mortenAI = mortenAI;
    }
    #endregion
    #region Methods
    public override NodeState Evaluate()
    {
        if(inCP == true)
        {
            return NodeState.Success;
        }
        

        return NodeState.Failure;
    }
    #endregion
}
