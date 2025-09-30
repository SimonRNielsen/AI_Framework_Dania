using AIGame.Core;
using Simon.AI;
using UnityEngine;
using UnityEngine.AI;

public enum States_Simon
{

    None,
    AttackState,
    RoamState,
    DodgeState,
    CaptureFlagState,
    SaveFlagState,
    GetPowerUpState

}

public class AttackState : IState_Simon<Simon_AI_Test>
{

    public void Enter(Simon_AI_Test obj)
    {



    }

    public void Exit(Simon_AI_Test obj)
    {

        

    }

    public void Update(Simon_AI_Test obj)
    {

        if (obj.GetVisibleEnemiesSnapshot().Count > 0)
        {

            obj.NavMeshAgent.isStopped = true;
            obj.RefreshOrAcquireTarget();
            if (obj.TryGetTarget(out PerceivedAgent target))
            {
                obj.FaceTarget(target.Position);
                obj.ThrowBallAt(target);
                obj.IsAttacking = true;
            }

            obj.IsAttacking = false;

        }
        else
        {

            obj.IsAttacking = false;

        }

    }
}

public class RoamState : IState_Simon<Simon_AI_Test>
{

    private int maxAttempts = 30;
    private float wanderRadius = 200f;

    public void Enter(Simon_AI_Test obj)
    {
        


    }

    public void Exit(Simon_AI_Test obj)
    {
        


    }

    public void Update(Simon_AI_Test obj)
    {

        obj.NavMeshAgent.isStopped = false;

        if (obj.HasReachedDestination())
        {

            obj.CurrentDestination = PickRandomDestination(obj);
            obj.MoveTo(obj.CurrentDestination);

        }

    }

    private Vector3 PickRandomDestination(Simon_AI_Test obj)
    {

        Vector3 currentPosition = obj.transform.position;

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

public class DodgeState : IState_Simon<Simon_AI_Test>
{

    public void Enter(Simon_AI_Test obj)
    {
        throw new System.NotImplementedException();
    }

    public void Exit(Simon_AI_Test obj)
    {
        throw new System.NotImplementedException();
    }

    public void Update(Simon_AI_Test obj)
    {
        throw new System.NotImplementedException();
    }
}

public class CaptureFlagState : IState_Simon<Simon_AI_Test>
{

    public void Enter(Simon_AI_Test obj)
    {
        throw new System.NotImplementedException();
    }

    public void Exit(Simon_AI_Test obj)
    {
        throw new System.NotImplementedException();
    }

    public void Update(Simon_AI_Test obj)
    {
        throw new System.NotImplementedException();
    }
}

public class SaveFlagState : IState_Simon<Simon_AI_Test>
{

    public void Enter(Simon_AI_Test obj)
    {
        throw new System.NotImplementedException();
    }

    public void Exit(Simon_AI_Test obj)
    {
        throw new System.NotImplementedException();
    }

    public void Update(Simon_AI_Test obj)
    {
        throw new System.NotImplementedException();
    }
}

public class GetPowerUpState : IState_Simon<Simon_AI_Test>
{

    public void Enter(Simon_AI_Test obj)
    {
        throw new System.NotImplementedException();
    }

    public void Exit(Simon_AI_Test obj)
    {
        throw new System.NotImplementedException();
    }

    public void Update(Simon_AI_Test obj)
    {
        throw new System.NotImplementedException();
    }
}