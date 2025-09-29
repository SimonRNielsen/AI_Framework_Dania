using UnityEngine;
using UnityEngine.AI;
using AIGame.Core;

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

        /// <summary>
        /// Configure the agent's stats (speed, health, etc.).
        /// </summary>
        protected override void ConfigureStats()
        {

            AllocateStat(StatType.Speed, 5);
            AllocateStat(StatType.VisionRange, 15);

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

            if (HasReachedDestination())
            {

                currentDestination = PickRandomDestination();
                MoveTo(currentDestination);

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

            if (NavMeshAgent.remainingDistance <= ARRIVAL_THRESHOLD)
            {
                return true;
            }
            else if (!NavMeshAgent.pathPending && !NavMeshAgent.hasPath && Vector3.Distance(transform.position, currentDestination) <= ARRIVAL_THRESHOLD)
            {
                return true;
            }

            return false;

        }

    }
}