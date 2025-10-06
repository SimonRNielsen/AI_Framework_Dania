using UnityEngine;
using AIGame.Core;

namespace MortensKombat
{
    /// <summary>
    /// SuperMorten AI implementation.
    /// TODO: Describe your AI strategy here.
    /// </summary>
    public class SuperMorten : BaseAI
    {

        protected MKBlackboard blackboard;
        private const float SUPPLYDATA = 0.1f;
        private float timeSinceDataUpdate = SUPPLYDATA;

        /// <summary>
        /// Configure the agent's stats (speed, health, etc.).
        /// </summary>
        protected override void ConfigureStats() { }

        /// <summary>
        /// Called once when the agent starts.
        /// Use this for initialization.
        /// </summary>
        protected override void StartAI()
        {

            blackboard = MKBlackboard.GetShared(this);
            FlagEnter += SpottedFlag;

        }

        private void OnDisable()
        {

            FlagEnter -= SpottedFlag;

        }

        /// <summary>
        /// Called every frame to make decisions.
        /// Implement your AI logic here.
        /// </summary>
        protected override void ExecuteAI()
        {

            timeSinceDataUpdate += Time.deltaTime;
            if (timeSinceDataUpdate >= SUPPLYDATA)
            {

                timeSinceDataUpdate = 0f;
                SupplyData();

            }

        }


        private void SupplyData()
        {

            var enemies = GetVisibleEnemiesSnapshot();

            if (enemies.Count > 0)
                foreach (var enemy in enemies)
                    blackboard.SetValue(MyDetectable.TeamID + blackboard.enemy, enemy);

        }


        private void SpottedFlag(Team id)
        {

            if (id == MyDetectable.TeamID || blackboard.HasKey(MyDetectable.TeamID + blackboard.flag)) return;

            var flags = GetVisibleFlagsSnapShot();

            if (flags.Count > 0)
                foreach (var flag in flags)
                    blackboard.SetValue(MyDetectable.TeamID + blackboard.flag, flag);

        }

    }
}