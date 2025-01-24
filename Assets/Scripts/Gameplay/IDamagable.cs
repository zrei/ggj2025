using System.Numerics;
using UnityEngine;

public interface IDamagable
{
    public abstract Transform UnitTransform { get; }
    public abstract void InternalIncHp(int value);
    public abstract void InternalDecHp(int value);
}
