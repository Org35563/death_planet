using Godot;

public partial class Enemy : CharacterBody2D, ICombatCreature
{
    [Export]
    public int Health;

    [Export]
    public float MoveSpeed;

    [Export]
    public int AttackPower;

    public bool IsAlive() => Health > 0;

    public Vector2 GetCurrentPosition() => Position;

    public int GetHealth() => Health;

    public void SetHealth(int newHealth) => Health = newHealth;

    public float GetMoveSpeed() => MoveSpeed;

    public float SetMoveSpeed(float newSpeed) => MoveSpeed = newSpeed;

    public int GetAttackPower() => AttackPower;

    public int SetAttackPower(int newAttackPower) => AttackPower = newAttackPower;
}
