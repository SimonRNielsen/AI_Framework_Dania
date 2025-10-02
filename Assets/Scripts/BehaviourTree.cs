using UnityEngine;
using System.Collections.Generic;
using AIGame.Core;
using Simon.AI;


public enum NodeState
{

    Running,
    Success,
    Failure

}


public class Blackboard
{

    private static Blackboard sharedInstance;
    private Dictionary<string, object> data = new Dictionary<string, object>();

    private Blackboard() { }

    public static Blackboard GetShared(BaseAI caller)
    {

        if (caller is SimonBehaviourAI)
        {

            if (sharedInstance == null)
                sharedInstance = new Blackboard();

            return sharedInstance;

        }

        return null;

    }


    public void SetValue<T>(string key, T value) => data[key] = value;


    public T GetValue<T>(string key)
    {

        if (data.TryGetValue(key, out object result) && result is T type) return type;

        return default;

    }


    public bool HasKey(string key) => data.ContainsKey(key);


    public void RemoveKey(string key) => data.Remove(key);

}


public abstract class Node
{

    protected Blackboard blackboard;
    public List<Node> children = new List<Node>();

    public Node(Blackboard blackboard)
    {

        this.blackboard = blackboard;

    }

    public abstract NodeState Evaluate();

}

public class Tree
{

    private Node root;
    private Blackboard blackboard;

    public Tree(Node root, Blackboard blackboard)
    {

        this.root = root;
        this.blackboard = blackboard;

    }

    public NodeState Tick() => root.Evaluate();

}


public class Selector : Node
{

    public Selector(Blackboard blackboard) : base(blackboard) {  }


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

    public Sequence(Blackboard blackboard) : base(blackboard) { }


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
