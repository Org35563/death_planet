using Godot;

public interface ILivingCreature
{
    public int GetHealth();

    public void SetHealth(int newHealth);

    public bool IsAlive();

    public Vector2 GetCurrentPosition();

    public float GetMoveSpeed();

    public float SetMoveSpeed(float newSpeed);
}