using UnityEngine;
using System.Collections.Generic;
using AIGame.Core;
using Simon.AI;
using MortensKombat;


public enum NodeState
{

    Running,
    Success,
    Failure

}


public class MKBlackboard
{

    private static MKBlackboard sharedInstance;
    public readonly string enemy = "enemy";
    public readonly string flag = "flag";
    private Dictionary<string, object> data = new Dictionary<string, object>();

    private MKBlackboard() { }

    public static MKBlackboard GetShared(BaseAI caller)
    {

        if (caller is BehaviourAITest || caller is SuperMorten)
        {

            if (sharedInstance == null)
                sharedInstance = new MKBlackboard();

            return sharedInstance;

        }

        return null;

    }


    public void SetValue<T>(string key, T value) => data[key] = value;


    public T GetValue<T>(string key) => data.TryGetValue(key, out object result) && result is T type ? type : default;
    /*{

        if (data.TryGetValue(key, out object result) && result is T type) return type;

        return default;

    }*/


    public bool HasKey(string key) => data.ContainsKey(key);


    public void RemoveKey(string key) => data.Remove(key);

}


public abstract class Node
{

    protected MKBlackboard blackboard;
    public List<Node> children = new List<Node>();

    public Node(MKBlackboard blackboard)
    {

        this.blackboard = blackboard;

    }

    public abstract NodeState Evaluate();

}

public class Tree
{

    private Node root;
    private MKBlackboard blackboard;

    public Tree(Node root, MKBlackboard blackboard)
    {

        this.root = root;
        this.blackboard = blackboard;

    }

    public NodeState Tick() => root.Evaluate();

}


public class Selector : Node
{

    public Selector(MKBlackboard blackboard) : base(blackboard) {  }


    public override NodeState Evaluate()
    {

        foreach (Node child in children)
        {

            NodeState result = child.Evaluate();
            if (result != NodeState.Failure) return result;

        }

        return NodeState.Failure;

    }

}


public class Sequence : Node
{

    public Sequence(MKBlackboard blackboard) : base(blackboard) { }


    public override NodeState Evaluate()
    {

        foreach (Node child in children)
        {

            NodeState result = child.Evaluate();
            if (result != NodeState.Success) return result;

        }

        return NodeState.Success;

    }

}
