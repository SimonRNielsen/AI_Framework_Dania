using UnityEngine;

public interface IState_Simon<T>
{
    public abstract void Enter(T obj);

    public abstract void Exit(T obj);

    public abstract void Update(T obj);
}
