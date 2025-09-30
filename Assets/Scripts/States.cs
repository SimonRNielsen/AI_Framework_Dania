using AIGame.Core;
using Simon.AI;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class AttackState : IState<Simon_AI>
{

    public void Enter(Simon_AI obj)
    {

        Debug.Log("AttackState Enter Ran");
        obj.NavMeshAgent.isStopped = true;

    }

    public void Exit(Simon_AI obj)
    {

        Debug.Log("AttackState Exit Ran");
        obj.NavMeshAgent.isStopped = false;
        obj.MoveTo(obj.CurrentDestination);

    }

    public void Update(Simon_AI obj)
    {

        if (obj.GetVisibleEnemiesSnapshot().Count > 0)
        {

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

public class RoamState : IState<Simon_AI>
{

    private int maxAttempts = 30;
    private float wanderRadius = 200f;

    public void Enter(Simon_AI obj)
    {

        Debug.Log("RoamState Enter Ran");

    }

    public void Exit(Simon_AI obj)
    {

        Debug.Log("RoamState Exit Ran");

    }

    public void Update(Simon_AI obj)
    {

        if (obj.NavMeshAgent.isStopped)
            obj.NavMeshAgent.isStopped = false;

        if (obj.HasReachedDestination())
        {

            obj.CurrentDestination = PickRandomDestination(obj);
            obj.MoveTo(obj.CurrentDestination);

        }

        if (obj.OwnFlagCarried)
        {

            obj.StateBeforeFlagCapture = States.Roam;
            obj.CurrentState = States.SaveFlag;

        }


    }

    private Vector3 PickRandomDestination(Simon_AI obj)
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

public class DodgeState : IState<Simon_AI>
{

    public void Enter(Simon_AI obj)
    {

        Debug.Log("DodgeState Enter Ran");
        obj.StartDodge(Random.Range(0, 2) == 1 ? Vector3.left : Vector3.right);

    }

    public void Exit(Simon_AI obj)
    {

        Debug.Log("DodgeState Exit Ran");
        if (obj.PreviousState == States.CaptureFlag && obj.PointOfInterest != Vector3.zero)
            obj.FaceTarget(obj.PointOfInterest);

    }

    public void Update(Simon_AI obj)
    {

        if (!obj.IsDodging)
            obj.CurrentState = obj.PreviousState;

    }

}

public class CaptureFlagState : IState<Simon_AI>
{

    public void Enter(Simon_AI obj)
    {

        Debug.Log("CaptureFlagState Enter Ran");
        Vector3? flagLocation = CaptureTheFlag.Instance.GetOwnFlagPosition(obj);
        obj.CurrentDestination = flagLocation.Value;
        obj.MoveTo(obj.CurrentDestination);

    }

    public void Exit(Simon_AI obj)
    {

        Debug.Log("CaptureFlagState Exit Ran");

    }

    public void Update(Simon_AI obj)
    {

        if (!obj.EnemyFlagCarried)
            obj.CurrentState = obj.StateBeforeFlagCapture;

    }

}

public class SaveFlagState : IState<Simon_AI>
{

    public void Enter(Simon_AI obj)
    {

        Debug.Log("SaveFlagState Enter Ran");
        Vector3? flagLocation = CaptureTheFlag.Instance.GetOwnFlagPosition(obj);
        obj.CurrentDestination = flagLocation.Value;
        obj.MoveTo(obj.CurrentDestination);

    }

    public void Exit(Simon_AI obj)
    {

        Debug.Log("SaveFlagState Exit Ran");

    }

    public void Update(Simon_AI obj)
    {

        if (obj.HasReachedDestination())
            Enter(obj);

        if (!obj.OwnFlagCarried)
            obj.CurrentState = obj.StateBeforeFlagCapture;

    }

}

public class GetPowerUpState : IState<Simon_AI>
{

    public void Enter(Simon_AI obj)
    {

        Debug.Log("GetPowerUpState Enter Ran");
        var powerUps = obj.GetVisiblePowerUpsSnapshot();
        float maxDistance = 1000;
        Vector3 newDestination = Vector3.zero;

        if (powerUps.Count > 0)
            foreach (var power in powerUps)
            {
                float distance = Vector3.Distance(obj.gameObject.transform.position, power.Position);
                if (distance < maxDistance)
                {
                    newDestination = power.Position;
                    maxDistance = distance;
                    obj.TargetPowerUp = power;
                }
            }

        if (newDestination != Vector3.zero)
        {

            obj.NavMeshAgent.isStopped = false;
            obj.CurrentDestination = newDestination;
            obj.MoveTo(newDestination);

        }

    }

    public void Exit(Simon_AI obj)
    {

        Debug.Log("GetPowerUpState Exit Ran");
        if (obj.TryConsumePowerupAction?.Invoke(obj.TargetPowerUp.Id) ?? false)
        {

            obj.TimeSinceLastPowerUp = 0;
            Simon_AI.CheckOthersTargetPowerUp?.Invoke(obj.TargetPowerUp.Id, obj.AgentID);

        }
        obj.IsGettingPowerUp = false;

    }

    public void Update(Simon_AI obj)
    {

        if (obj.HasReachedDestination())
            obj.CurrentState = obj.PreviousState;

    }

}

public class GetFlagState : IState<Simon_AI>
{
    public void Enter(Simon_AI obj)
    {

        Debug.Log("GetFlagState Enter Ran");
        var detectFlag = obj.GetVisibleFlagsSnapShot();

        if (detectFlag.Count > 0)
        {

            foreach (var flag in detectFlag)
            {

                if (flag.Team != obj.MyDetectable.TeamID)
                {

                    obj.CurrentDestination = flag.Position;
                    obj.MoveTo(obj.CurrentDestination);
                    Debug.Log("Enemy flag detected");

                }
                else
                    Debug.Log("Own flag detected");

            }

        }

    }

    public void Exit(Simon_AI obj)
    {

        Debug.Log("GetFlagState Exit Ran");
        obj.NavMeshAgent.isStopped = false;
        obj.MoveTo(obj.CurrentDestination);

    }

    public void Update(Simon_AI obj)
    {

        if (obj.GetVisibleEnemiesSnapshot().Count > 0)
        {

            obj.RefreshOrAcquireTarget();
            if (obj.TryGetTarget(out PerceivedAgent target))
            {

                obj.NavMeshAgent.isStopped = true;
                obj.FaceTarget(target.Position);
                obj.ThrowBallAt(target);
                obj.IsAttacking = true;

            }

            obj.IsAttacking = false;

        }
        else
        {

            obj.NavMeshAgent.isStopped = false;
            obj.IsAttacking = false;

        }

        if (obj.HasReachedDestination())
            obj.NavMeshAgent.isStopped = true;

        if (obj.EnemyFlagCarried && obj.HasFlag)
        {

            obj.StateBeforeFlagCapture = obj.PreviousState;
            obj.CurrentState = States.CaptureFlag;

        }
        else if (obj.EnemyFlagCarried)
            obj.CurrentState = obj.PreviousState;

    }

}

public class CheckDirectionState : IState<Simon_AI>
{

    private const float checkForSeconds = 0.02f;

    public void Enter(Simon_AI obj)
    {

        Debug.Log("CheckDirectionState Enter Ran");
        obj.NavMeshAgent.isStopped = true;

        if (obj.PointOfInterest != Vector3.zero)
            obj.FaceTarget(obj.PointOfInterest);

        obj.CheckedFor = 0f;

    }

    public void Exit(Simon_AI obj)
    {

        Debug.Log("CheckDirectionState Exit Ran");
        obj.NavMeshAgent.isStopped = false;
        obj.MoveTo(obj.CurrentDestination);
        obj.PointOfInterest = Vector3.zero;

    }

    public void Update(Simon_AI obj)
    {

        if (obj.CheckedFor >= checkForSeconds)
            obj.CurrentState = obj.PreviousState;

        obj.CheckedFor += Time.deltaTime;

        if (obj.GetVisibleEnemiesSnapshot().Count > 0)
            obj.CurrentState = States.Attack;

    }

}