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

            MKSequence offenceSequence = new MKSequence(blackboard);
            rootSelector.children.Add(offenceSequence);

            AquireTarget aquireTarget = new AquireTarget(blackboard, this);
            MoveInRange moveInRange = new MoveInRange(blackboard, this);
            ShootThatBunny shootThatBunny = new ShootThatBunny(blackboard, this);
            offenceSequence.children.Add(aquireTarget);
            offenceSequence.children.Add(moveInRange);
            offenceSequence.children.Add(shootThatBunny);

            //Defender Move to controlpoint sequence
            MKSequence controlPointCheck = new MKSequence(blackboard);
            AmOutsideCp amOutsideCp = new AmOutsideCp(blackboard, this);
            MoveToCP moveToCP = new MoveToCP(blackboard, this);
            controlPointCheck.children.Add(amOutsideCp);
            controlPointCheck.children.Add(moveToCP);

            //Defender moving around CP sequence
            MKSequence moveAroundCPSequence = new MKSequence(blackboard);
            AmIInCP amIInCP = new AmIInCP(blackboard, this);
            MoveAroundCP moveAroundCP = new MoveAroundCP(blackboard, this);
            moveAroundCPSequence.children.Add(amIInCP);
            moveAroundCPSequence.children.Add(moveAroundCP);

            MKSelector defenderSelector = new MKSelector(blackboard);
            defenderSelector.children.Add(controlPointCheck);
            defenderSelector.children.Add(moveAroundCP);
            rootSelector.children.Add(defenderSelector);

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