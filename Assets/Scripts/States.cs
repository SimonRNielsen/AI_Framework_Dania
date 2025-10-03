using AIGame.Core;
using Simon.AI;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class AttackState : IState<Simon_AI>
{

    public void Enter(Simon_AI obj)
    {

        obj.SetStopped(true);
        //Debug.Log("AttackState Enter Ran");

    }

    public void Exit(Simon_AI obj)
    {

        obj.SetStopped(false);
        obj.MoveTo(obj.CurrentDestination);
        //Debug.Log("AttackState Exit Ran");

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

        if (obj.IsStopped())
            obj.SetStopped(false);
        //Debug.Log("RoamState Enter Ran");

    }

    public void Exit(Simon_AI obj)
    {

        //Debug.Log("RoamState Exit Ran");

    }

    public void Update(Simon_AI obj)
    {

        if (obj.HasReachedDestination())
        {

            obj.CurrentDestination = PickRandomDestination(obj);
            obj.MoveTo(obj.CurrentDestination);

        }

        if (obj.OwnFlagCarried)
        {

            obj.StateBeforeFlagCapture = obj.CurrentState;
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

        Vector3 random = Vector3.zero;

        switch (Random.Range(0, 4))
        {
            case 0:
                random = Vector3.back;
                break;
            case 1: 
                random = Vector3.forward; 
                break;
            case 2:
                random = Vector3.left;
                break;
            case 3:
                random = Vector3.right;
                break;
        }

        obj.StartDodge(random);
        //Debug.Log("DodgeState Enter Ran");

    }

    public void Exit(Simon_AI obj)
    {

        if (obj.PreviousState == States.CaptureFlag && obj.PointOfInterest != Vector3.zero)
            obj.FaceTarget(obj.PointOfInterest);
        //Debug.Log("DodgeState Exit Ran");

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

        Vector3? flagLocation = CaptureTheFlag.Instance.GetOwnFlagPosition(obj);
        obj.CurrentDestination = flagLocation.Value;
        obj.MoveTo(obj.CurrentDestination);
        //Debug.Log("CaptureFlagState Enter Ran");

    }

    public void Exit(Simon_AI obj)
    {

        //Debug.Log("CaptureFlagState Exit Ran");

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

        if (obj.IsStopped())
            obj.SetStopped(false);

        GoToOwnFlag(obj);
        //Debug.Log("SaveFlagState Enter Ran");

    }

    public void Exit(Simon_AI obj)
    {

        //Debug.Log("SaveFlagState Exit Ran");

    }

    public void Update(Simon_AI obj)
    {

        if (obj.HasReachedDestination())
            GoToOwnFlag(obj);

        if (!obj.OwnFlagCarried)
            obj.CurrentState = obj.StateBeforeFlagCapture;

    }

    private void GoToOwnFlag(Simon_AI obj)
    {

        Vector3? flagLocation = CaptureTheFlag.Instance.GetOwnFlagPosition(obj);
        obj.CurrentDestination = flagLocation.Value;
        obj.MoveTo(obj.CurrentDestination);

    }

}

public class GetPowerUpState : IState<Simon_AI>
{

    public void Enter(Simon_AI obj)
    {

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

            obj.SetStopped(false);
            obj.CurrentDestination = newDestination;
            obj.MoveTo(newDestination);

        }
        //Debug.Log("GetPowerUpState Enter Ran");

    }

    public void Exit(Simon_AI obj)
    {

        if (obj.TryConsumePowerupAction?.Invoke(obj.TargetPowerUp.Id) ?? false)
        {

            obj.TimeSinceLastPowerUp = 0;
            Simon_AI.ClaimedTargetPowerUp?.Invoke(obj.TargetPowerUp.Id, obj.AgentID);

        }
        obj.IsGettingPowerUp = false;
        //Debug.Log("GetPowerUpState Exit Ran");

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

        var detectFlag = obj.GetVisibleFlagsSnapShot();

        if (detectFlag.Count > 0)
        {

            foreach (var flag in detectFlag)
            {

                if (flag.Team != obj.MyDetectable.TeamID)
                {

                    obj.CurrentDestination = flag.Position;
                    obj.MoveTo(obj.CurrentDestination);
                    //Debug.Log("Enemy flag detected");

                }
                //else
                    //Debug.Log("Own flag detected");

            }

        }
        //Debug.Log("GetFlagState Enter Ran");

    }

    public void Exit(Simon_AI obj)
    {

        obj.SetStopped(false);
        obj.MoveTo(obj.CurrentDestination);
        //Debug.Log("GetFlagState Exit Ran");

    }

    public void Update(Simon_AI obj)
    {

        if (obj.GetVisibleEnemiesSnapshot().Count > 0)
        {

            obj.RefreshOrAcquireTarget();
            if (obj.TryGetTarget(out PerceivedAgent target))
            {

                obj.SetStopped(true);
                obj.FaceTarget(target.Position);
                obj.ThrowBallAt(target);
                obj.IsAttacking = true;

            }

            obj.IsAttacking = false;

        }
        else
        {

            obj.SetStopped(false);
            obj.IsAttacking = false;

        }

        if (obj.HasReachedDestination())
            obj.SetStopped(true);

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

        obj.SetStopped(true);

        if (obj.PointOfInterest != Vector3.zero)
            obj.FaceTarget(obj.PointOfInterest);

        obj.CheckedFor = 0f;
        //Debug.Log("CheckDirectionState Enter Ran");

    }

    public void Exit(Simon_AI obj)
    {

        obj.SetStopped(false);
        obj.MoveTo(obj.CurrentDestination);
        obj.PointOfInterest = Vector3.zero;
        //Debug.Log("CheckDirectionState Exit Ran");

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