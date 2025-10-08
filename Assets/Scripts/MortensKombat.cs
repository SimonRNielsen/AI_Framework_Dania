using System;
using AIGame.Core;
using UnityEngine;

namespace MortensKombat
{
    /// <summary>
    /// Factory that spawns MortensKombat agents.
    /// Creates a full team of agents using custom AI behaviour.
    /// </summary>
    [RegisterFactory("Mortens Kombat")]
    public class MortensKombat : AgentFactory
    {
        /// <summary>
        /// Returns the agent types this factory wants to spawn.
        /// </summary>
        /// <returns>An array containing the AI types to spawn.</returns>
        protected override System.Type[] GetAgentTypes()
        {
            return new System.Type[] { /*typeof(Defender), typeof(Attacker), */typeof(Scout)/*, typeof(Defender), typeof(Attacker) */};
        }
    }
}