using System;


[Serializable]
public class MonsterStat
{
    public int Health;
    public int MaxHealth = 200;

    public float PatrolTime = 3f;
    public float MovementRange = 10f;
    public float TraceDistance = 10f;
    public float ReturnDistance = 20f;

    public float AttackDistance = 2f;
    public float AttackDelay = 3f;
    public int Damage = 10;

    public void Init()
    {
        Health = MaxHealth;
    }
}