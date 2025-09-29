using UnityEngine;
using UnityEngine.AI;
using AIGame.Core;
using System.Collections;

namespace Simon.AI
{
    /// <summary>
    /// Simon_AI_Test AI implementation.
    /// TODO: Describe your AI strategy here.
    /// </summary>
    public class Simon_AI_Test : BaseAI
    {

        /// <summary>
        /// How many times will we try to find a  random location before we give up
        /// </summary>
        private int maxAttempts = 30;

        /// <summary>
        /// The radius we will wander
        /// </summary>
        private float wanderRadius = 200f;

        /// <summary>
        /// How far away from your destination we will accept as arrived
        /// </summary>
        private const float ARRIVAL_THRESHOLD = 0.5f;

        /// <summary>
        /// The current destination this agent is walking towards
        /// </summary>
        private Vector3 currentDestination;

        private bool isAttacking = false;

        private bool isGettingPowerUp = false;

        private bool isGettingFlag = false;

        private bool foundFlag = false;

        private bool pickingUpFlag = false;

        private bool hasFlag = false;

        private const float pickupTimer = 5f;

        /// <summary>
        /// Configure the agent's stats (speed, health, etc.).
        /// </summary>
        protected override void ConfigureStats()
        {

            AllocateStat(StatType.Speed, 3);
            AllocateStat(StatType.VisionRange, 7);
            AllocateStat(StatType.ProjectileRange, 7);
            AllocateStat(StatType.ReloadSpeed, 1);
            AllocateStat(StatType.DodgeCooldown, 2);

        }

        /// <summary>
        /// Called once when the agent starts.
        /// Use this for initialization.
        /// </summary>
        protected override void StartAI()
        {
            // TODO: Initialize your AI here
        }

        /// <summary>
        /// Called every frame to make decisions.
        /// Implement your AI logic here.
        /// </summary>
        protected override void ExecuteAI()
        {

            if (pickingUpFlag)
                NavMeshAgent.isStopped = true;

            isAttacking = Attack();

            if (!isAttacking)
                isGettingFlag = GetFlag();

            if (!isAttacking && !isGettingFlag && GetVisiblePowerUpsSnapshot().Count > 0)
            {

            }
            else
            {

                isGettingPowerUp = false;

            }

            if (!isAttacking && !isGettingFlag && !isGettingPowerUp)
            {

                NavMeshAgent.isStopped = false;

                if (HasReachedDestination())
                {

                    currentDestination = PickRandomDestination();
                    MoveTo(currentDestination);

                }


            }

        }


        private bool GetFlag()
        {

            var detectFlag = GetVisibleFlagsSnapShot();

            if (detectFlag.Count > 0)
            {
                foreach (var flag in detectFlag)
                {
                    if (flag.Team == MyDetectable.TeamID)
                    {
                        Debug.Log("Own flag detected");
                        continue;
                    }
                    else
                    {
                        Debug.Log("Enemy flag detected");
                        currentDestination = flag.Position;
                        MoveTo(currentDestination);
                        foundFlag = true;
                        return true;
                    }
                }
            }
            
            return false;

        }

        private bool Attack()
        {

            if (GetVisibleEnemiesSnapshot().Count > 0)
            {
                NavMeshAgent.isStopped = true;
                RefreshOrAcquireTarget();
                if (TryGetTarget(out PerceivedAgent target))
                {
                    FaceTarget(target.Position);
                    ThrowBallAt(target);
                    return true;
                }

                return false;

            }
            else
            {

                return false;

            }

        }

        /// <summary>
        /// Picks a random destination that the AI can walk to using NavMesh.
        /// </summary>
        /// <returns>A random walkable position, or the current position if no valid destination found</returns>
        private Vector3 PickRandomDestination()
        {

            Vector3 currentPosition = transform.position;

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

        /// <summary>
        /// Checks if we have arrived at our destination
        /// </summary>
        /// <returns>true if we have arrived</returns>
        private bool HasReachedDestination()
        {

            if (NavMeshAgent.remainingDistance <= ARRIVAL_THRESHOLD && foundFlag)
            {

                if (!pickingUpFlag)
                {

                    pickingUpFlag = true;
                    StartCoroutine(PickupFlag());

                }

            }
            else if (NavMeshAgent.remainingDistance <= ARRIVAL_THRESHOLD && !foundFlag)
            {

                return true;

            }
            else if (!NavMeshAgent.pathPending && !NavMeshAgent.hasPath && Vector3.Distance(transform.position, currentDestination) <= ARRIVAL_THRESHOLD)
            {

                return true;

            }

            return false;

        }


        private IEnumerator PickupFlag()
        {

            yield return new WaitForSeconds(pickupTimer);

            foundFlag = false;

            Vector3? flagPosition = CaptureTheFlag.Instance.GetOwnFlagPosition(this);
            currentDestination = flagPosition.Value;
            NavMeshAgent.isStopped = false;
            pickingUpFlag = false;
            hasFlag = true;
            MoveTo(currentDestination);

        }

    }
}