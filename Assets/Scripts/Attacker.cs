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

            //Attacker Move to controlpoint selector and sequence

            MKSequence controlPointCheckSequence = new MKSequence(blackboard); //Sequence: Outside CP? YES -> move to cp
            AmOutsideCp amOutsideCp = new AmOutsideCp(blackboard, this);
            MoveToCP moveToCP = new MoveToCP(blackboard, this);
            controlPointCheckSequence.children.Add(amOutsideCp);
            controlPointCheckSequence.children.Add(moveToCP);
            
            MKSelector contestedControlPointSelector = new MKSelector(blackboard); //Selector: Is CP ours? NO -> Run controlPointCheckSequence 
            IsCPContested isCPContested = new IsCPContested(blackboard, this);
            contestedControlPointSelector.children.Add(isCPContested);
            contestedControlPointSelector.children.Add(controlPointCheckSequence);

            //Defender moving around CP sequence
            MKSequence moveAroundCPSequence = new MKSequence(blackboard);
            AmIInCP amIInCP = new AmIInCP(blackboard, this);
            MoveAroundCP moveAroundCP = new MoveAroundCP(blackboard, this);
            moveAroundCPSequence.children.Add(amIInCP);
            moveAroundCPSequence.children.Add(moveAroundCP);

            rootSelector.children.Add(contestedControlPointSelector);
            rootSelector.children.Add(moveAroundCPSequence);

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