using UnityEngine;
using AIGame.Core;

namespace MortensKombat
{
    /// <summary>
    /// HolyMorten AI implementation.
    /// TODO: Describe your AI strategy here.
    /// </summary>
    public class Attacker : SuperMorten
    {
        /// <summary>
        /// Configure the agent's stats (speed, health, etc.).
        /// </summary>
        protected override void ConfigureStats()
        {

            AllocateStat(StatType.Speed, 3);
            AllocateStat(StatType.ProjectileRange, 10);
            AllocateStat(StatType.VisionRange, 3);
            AllocateStat(StatType.DodgeCooldown, 0);
            AllocateStat(StatType.ReloadSpeed, 4);

        }

        /// <summary>
        /// Called once when the agent starts.
        /// Use this for initialization.
        /// </summary>
        protected override void StartAI()
        {

            base.StartAI();

            MKSelector rootSelector = new MKSelector(blackboard);



            behaviourTree = new MKTree(rootSelector, blackboard);

        }

        /// <summary>
        /// Called every frame to make decisions.
        /// Implement your AI logic here.
        /// </summary>
        protected override void ExecuteAI()
        {

            base.ExecuteAI();

        }

        public override string ToString() => "Attacker";

    }
}