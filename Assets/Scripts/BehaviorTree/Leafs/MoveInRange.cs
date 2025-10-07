using UnityEngine;
using MortensKombat;

public class MoveInRange : MKNode
{

    private SuperMorten ai;

    private bool test = false;

    public MoveInRange(MKBlackboard blackboard, SuperMorten parent) : base(blackboard)
    {

        ai = parent;

    }

    public override NodeState Evaluate()
    {

        if (Vector3.Distance(ai.Target.position, ai.transform.position) <= ai.ProjectileRange)
        {

            if (!test)
            {

                test = true;
                Debug.Log("Enemy deemed in range (runs once)");

            }

            ai.StopMoving();
            return NodeState.Success;

        }

        ai.MoveTo(ai.Target.position);

        return NodeState.Failure;

    }
}
