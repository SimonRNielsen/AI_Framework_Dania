using UnityEngine;
using MortensKombat;
using System.Linq;

public class IsEnemyInVisibleRange : MKNode
{

    private SuperMorten ai;
    private readonly float recentData;

    public IsEnemyInVisibleRange(MKBlackboard blackboard, SuperMorten parent) : base(blackboard)
    {

        ai = parent;

    }

    public override NodeState Evaluate()
    {

        ai.Target = null;

        var snapshot = ai.GetVisibleEnemiesSnapshot();

        if (snapshot.Count > 0)
        {

            blackboard.SetValue(ai.MyDetectable.TeamID + blackboard.enemy, snapshot[0]);
            ai.Target = blackboard.GetValue<EnemyData>(ai.MyDetectable.TeamID + blackboard.enemy + snapshot[0].Id);

            return NodeState.Success;

        }

        EnemyData temp = blackboard.GetEnemies(ai).Where(x => Time.time - x.timestamp < recentData).OrderBy(x => Vector3.Distance(ai.transform.position, x.position)).FirstOrDefault();

        if (temp != null && Vector3.Distance(temp.position, ai.transform.position) <= ai.ProjectileRange)
        {

            ai.Target = temp;

            return NodeState.Success;

        }

        return NodeState.Failure;

    }
}
