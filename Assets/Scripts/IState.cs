using UnityEngine;

public interface IState<T>
{
    public abstract void Enter(T obj);

    public abstract void Exit(T obj);

    public abstract void Update(T obj);
}
