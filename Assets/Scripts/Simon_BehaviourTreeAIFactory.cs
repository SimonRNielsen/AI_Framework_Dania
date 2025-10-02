using System;
using AIGame.Core;
using UnityEngine;

namespace Simon.AI
{
    /// <summary>
    /// Factory that spawns Simon_BehaviourTreeAIFactory agents.
    /// Creates a full team of agents using custom AI behaviour.
    /// </summary>
    [RegisterFactory("Simon Behaviour")]
    public class Simon_BehaviourTreeAIFactory : AgentFactory
    {
        /// <summary>
        /// Returns the agent types this factory wants to spawn.
        /// </summary>
        /// <returns>An array containing the AI types to spawn.</returns>
        protected override System.Type[] GetAgentTypes()
        {
            return new System.Type[] { typeof(SimonBehaviourAI) };
        }
    }
}