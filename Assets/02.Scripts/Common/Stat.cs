using System;

[Serializable] // 직렬화 가능한
public class Stat
{
    public int Health;
    public int MaxHealth;

    public float Stamina;
    public float MaxStamina;
    public float RecoveryStamina;
    public float RunConsumeStamina;

    public float MoveSpeed;
    public float RunSpeed;
    public float JumpPower;
    public float JumpConsumeStamina;

    public float RotationSpeed;

    public float AttackCoolTime;
    public float AttackConsumeStamina;
    public int Damage;
    public void Init()
    {
        Health = MaxHealth;
        Stamina = MaxStamina;
    }
}
