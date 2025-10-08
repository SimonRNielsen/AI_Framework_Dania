using UnityEngine;
using System.Collections.Generic;
using AIGame.Core;
using MortensKombat;
using System.Linq;


public enum NodeState
{

    Running,
    Success,
    Failure

}


public class MKBlackboard
{

    private static MKBlackboard sharedInstance;
    /// <summary>
    /// Used for getting data on enemies together with MyDetectable.TeamID
    /// </summary>
    public readonly string enemy = "enemyAgent";
    /// <summary>
    /// Used for getting location of enemyflag together with MyDetectable.TeamID
    /// </summary>
    public readonly string flag = "enemyFlag";
    private Dictionary<string, object> data = new Dictionary<string, object>();
    private readonly float DATANOTOBSOLETE = 10f;

    private MKBlackboard() { }

    /// <summary>
    /// Establishes connection to blackboard
    /// </summary>
    /// <param name="caller">AI to connect</param>
    /// <returns>Connection to blackboard</returns>
    public static MKBlackboard GetShared(BaseAI caller)
    {

        if (caller is SuperMorten)
        {

            if (sharedInstance == null)
                sharedInstance = new MKBlackboard();

            return sharedInstance;

        }

        return null;

    }

    /// <summary>
    /// Inputs data to blackboard
    /// </summary>
    /// <typeparam name="T">Datatype to be stored</typeparam>
    /// <param name="key">Where to store data</param>
    /// <param name="value">Data to be stored</param>
    public void SetValue<T>(string key, T value)
    {

        if (value is PerceivedAgent enemy)
        {

            if (data.TryGetValue(key + enemy.Id, out object result) && result is EnemyData enemyData)
            {

                if (enemyData.recordedFrame >= enemy.Frame) return;

                enemyData.position = enemy.Position;
                enemyData.timestamp = Time.time;
                enemyData.recordedFrame = enemy.Frame;
                enemyData.enemy = enemy;

            }
            else
                data[key + enemy.Id] = new EnemyData(enemy.Position, enemy.Id, Time.time, enemy.Frame, enemy);

            return;

        }

        data[key] = value;

    }


    /// <summary>
    /// Gets generic data from blackboard
    /// </summary>
    /// <typeparam name="T">Datatype expected from key</typeparam>
    /// <param name="key">Location of data</param>
    /// <returns>Default if wrong datatype or none, else data</returns>
    public T GetValue<T>(string key) => data.TryGetValue(key, out object result) && result is T type ? type : default;

    /// <summary>
    /// Method to retrieve all pertinent EnemyData
    /// </summary>
    /// <param name="caller">This</param>
    /// <returns>List of EnemyData</returns>
    public List<EnemyData> GetEnemies(BaseAI caller) => data.Where(x => x.Key.StartsWith(caller.MyDetectable.TeamID.ToString() + enemy)).Select(x => x.Value).OfType<EnemyData>().ToList();

    /// <summary>
    /// Checks if data is stored
    /// </summary>
    /// <param name="key">string to check</param>
    /// <returns>true if blackboard contains data at location</returns>
    public bool HasKey(string key) => data.ContainsKey(key);

    /// <summary>
    /// Removes data from blackboard
    /// </summary>
    /// <param name="key">Removes data at location</param>
    public void RemoveKey(string key) => data.Remove(key);

    /// <summary>
    /// Removes enemy data older than 10 seconds
    /// </summary>
    /// <returns>Timestamp (Time.time)</returns>
    public float RemoveObsoleteData()
    {

        List<string> keys = data.Where(x => x.Value is EnemyData enemyData && Time.time - enemyData.timestamp > DATANOTOBSOLETE).Select(x => x.Key).ToList();

        if (keys.Count > 0)
        {

            Debug.LogWarning($"Removed {keys.Count} obsolete enemy data from blackboard");

            foreach (string key in keys)
                RemoveKey(key);

        }

        return Time.time;

    }

}


public abstract class MKNode
{

    protected MKBlackboard blackboard;
    public List<MKNode> children = new List<MKNode>();

    public MKNode(MKBlackboard blackboard)
    {

        this.blackboard = blackboard;

    }

    public abstract NodeState Evaluate();

}

public class MKTree
{

    private MKNode root;
    private MKBlackboard blackboard;

    public MKTree(MKNode root, MKBlackboard blackboard)
    {

        this.root = root;
        this.blackboard = blackboard;

    }

    public NodeState Tick() => root.Evaluate();

}


public class MKSelector : MKNode
{

    public MKSelector(MKBlackboard blackboard) : base(blackboard) { }


    public override NodeState Evaluate()
    {

        foreach (MKNode child in children)
        {

            NodeState result = child.Evaluate();
            if (result != NodeState.Failure) return result;

        }

        return NodeState.Failure;

    }

}


public class MKSequence : MKNode
{

    public MKSequence(MKBlackboard blackboard) : base(blackboard) { }


    public override NodeState Evaluate()
    {

        foreach (MKNode child in children)
        {

            NodeState result = child.Evaluate();
            if (result != NodeState.Success) return result;

        }

        return NodeState.Success;

    }

}


public class EnemyData
{

    public Vector3 position;
    public float timestamp;
    public int id;
    public int recordedFrame;
    public PerceivedAgent enemy;

    public EnemyData(Vector3 pos, int enemyID, float time, int frame, PerceivedAgent enemy)
    {

        position = pos;
        id = enemyID;
        timestamp = time;
        recordedFrame = frame;
        this.enemy = enemy;

    }

}
