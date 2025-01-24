using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagable
{
    public abstract void InternalIncHp(int value);
    public abstract void InternalDecHp(int value);
}
