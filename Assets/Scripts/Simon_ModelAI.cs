using UnityEngine;
using AIGame.Core;

namespace Simon.AI
{

    /// <summary>
    /// Simon_ModelAI AI implementation.
    /// TODO: Describe your AI strategy here.
    /// </summary>
    public class Simon_ModelAI : BaseAI
    {

        private const float MODEL_UPDATE_INTERVAL = 0.5f;
        private const float ARRIVAL_THRESHOLD = 0.5f;
        private WorldModel worldModel = new WorldModel();
        private float lastModelUpdate;
        private float stateChangeTime;
        private States currentState;
        private Vector3 currentDestination;

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
            
            stateChangeTime = Time.time;

        }

        /// <summary>
        /// Called every frame to make decisions.
        /// Implement your AI logic here.
        /// </summary>
        protected override void ExecuteAI()
        {
            
            if (Time.time - lastModelUpdate > MODEL_UPDATE_INTERVAL)
            {

                lastModelUpdate = Time.time;
                UpdateWorldModel();

            }

        }


        private void UpdateWorldModel()
        {

            var powerUpSnapshot = GetVisiblePowerUpsSnapshot();
            foreach (var powerUp in powerUpSnapshot)
                worldModel.UpdatePowerUp(powerUp.Id, powerUp.Position);

            var enemiesSnapshot = GetVisibleEnemiesSnapshot();
            foreach(var enemy in enemiesSnapshot)
                worldModel.UpdateEnemy(enemy.Id, enemy.Position);

            worldModel.CleanUpData();

        }


        private void MakeDecision()
        {



        }


        private void ExecuteCurrentState()
        {



        }


        private void ExecuteHunting()
        {



        }


        private void ExecutePowerUpPursuit()
        {



        }


        private void ExecuteExploration()
        {



        }


        private void ExecuteWandering()
        {



        }


        private void GetExplorationTarget()
        {



        }


        private bool HasReachedDestination()
        {

            if (!NavMeshAgent.pathPending && !NavMeshAgent.hasPath && Vector3.Distance(transform.position, currentDestination) <= ARRIVAL_THRESHOLD)
            {

                //Debug.Log($"Reached Destination, currentstate is {currentState}");
                return true;

            }

            return false;

        }

    }
}