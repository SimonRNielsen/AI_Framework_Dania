using UnityEngine;
using MortensKombat;

public class MoveInRange : MKNode
{

    private SuperMorten ai;

    public MoveInRange(MKBlackboard blackboard, SuperMorten parent) : base(blackboard)
    {

        ai = parent;

    }

    public override NodeState Evaluate()
    {

        if (Vector3.Distance(ai.Target.position, ai.transform.position) <= ai.ProjectileRange)
        {

            ai.StopMoving();
            return NodeState.Success;

        }

        ai.MoveTo(ai.Target.position);

        return NodeState.Failure;

    }
}
