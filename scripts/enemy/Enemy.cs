using Godot;

public partial class Enemy : CharacterBody2D, ILivingCreature
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
}
