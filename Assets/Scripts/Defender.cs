using UnityEngine;
using AIGame.Core;

namespace MortensKombat
{
    /// <summary>
    /// CrusaderMorten AI implementation.
    /// TODO: Describe your AI strategy here.
    /// </summary>
    public class Defender : SuperMorten
    {
        /// <summary>
        /// Configure the agent's stats (speed, health, etc.).
        /// </summary>
        protected override void ConfigureStats()
        {

            AllocateStat(StatType.Speed, 10);
            AllocateStat(StatType.ProjectileRange, 0);
            AllocateStat(StatType.VisionRange, 3);
            AllocateStat(StatType.DodgeCooldown, 7);
            AllocateStat(StatType.ReloadSpeed, 0);

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
            // TODO: Implement your AI decision-making logic here
        }
    }
}