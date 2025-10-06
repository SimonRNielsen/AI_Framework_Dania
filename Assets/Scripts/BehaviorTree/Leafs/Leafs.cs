using AIGame.Core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IsEnemyVisible : Node
{

    private BaseAI ai;
    public const string enemyPosition = "LastSeenEnemyPosition";
    public const string enemyTime = "LastSeenEnemyTime";

    public IsEnemyVisible(MKBlackboard blackboard, BaseAI ai) : base(blackboard)
    {

        this.ai = ai;

    }

    public override NodeState Evaluate()
    {

        var enemies = ai.GetVisibleEnemiesSnapshot();
        if (enemies.Count > 0)
        {

            ai.RefreshOrAcquireTarget();

            if (blackboard != null)
            {

                blackboard.SetValue(ai.MyDetectable.TeamID + enemyPosition, enemies[0].Position);
                blackboard.SetValue(ai.MyDetectable.TeamID + enemyTime, Time.time);

            }

            return NodeState.Success;

        }

        return NodeState.Failure;

    }

}


public class AttackEnemy : Node
{

    private BaseAI ai;

    public AttackEnemy(MKBlackboard blackboard, BaseAI ai) : base(blackboard)
    {

        this.ai = ai;

    }

    public override NodeState Evaluate()
    {

        if (ai.TryGetTarget(out PerceivedAgent target))
        {

            ai.FaceTarget(target.Position);
            ai.ThrowBallAt(target);

            return NodeState.Success;

        }

        return NodeState.Failure;

    }

}

public class InvestigateLastSeenEnemy : Node
{

    private BaseAI ai;

    public InvestigateLastSeenEnemy(MKBlackboard blackboard, BaseAI ai) : base(blackboard)
    {

        this.ai = ai;

    }


    public override NodeState Evaluate()
    {

        if (blackboard != null)
        {

            // Check if we have recent enemy intel using generic methods
            if (blackboard.HasKey(ai.MyDetectable.TeamID + IsEnemyVisible.enemyPosition) && blackboard.HasKey(ai.MyDetectable.TeamID + IsEnemyVisible.enemyTime))
            {

                float lastSeenTime = blackboard.GetValue<float>(ai.MyDetectable.TeamID + IsEnemyVisible.enemyTime);

                // Only investigate if intel is recent (within 10 seconds)
                if (Time.time - lastSeenTime < 10f)
                {

                    Vector3 lastSeenPosition = blackboard.GetValue<Vector3>(ai.MyDetectable.TeamID + IsEnemyVisible.enemyPosition);
                    ai.MoveTo(lastSeenPosition);

                    return NodeState.Success;

                }
                else
                {

                    // Clean up old data using generic methods
                    blackboard.RemoveKey(ai.MyDetectable.TeamID + IsEnemyVisible.enemyPosition);
                    blackboard.RemoveKey(ai.MyDetectable.TeamID + IsEnemyVisible.enemyTime);

                }

            }

        }

        return NodeState.Failure;

    }

}


public class MoveToPoint : Node
{

    private BaseAI ai;
    private Vector3 destination;
    private const int maxAttempts = 30;
    private const float wanderRadius = 200f;

    public MoveToPoint(MKBlackboard blackboard, BaseAI ai, Vector3 destination) : base(blackboard)
    {

        this.ai = ai;
        this.destination = destination;

    }


    public override NodeState Evaluate()
    {

        ai.MoveTo(destination);

        if (Vector3.Distance(ai.transform.position, destination) < 2f)
        {

            destination = PickRandomDestination();
            return NodeState.Success;

        }

        return NodeState.Running;

    }

    private Vector3 PickRandomDestination()
    {

        Vector3 currentPosition = ai.transform.position;

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            // Generate a random direction
            Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
            randomDirection += currentPosition;

            // Try to find a valid NavMesh position near the random point
            if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, wanderRadius, NavMesh.AllAreas))
            {
                // Additional check: make sure we can actually path to this destination
                NavMeshPath path = new NavMeshPath();
                if (NavMesh.CalculatePath(currentPosition, hit.position, NavMesh.AllAreas, path))
                {

                    if (path.status == NavMeshPathStatus.PathComplete)
                    {

                        return hit.position;

                    }

                }

            }

        }

        return currentPosition;

    }

}