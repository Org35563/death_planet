using Godot;

public interface ICharacter
{
    public int GetHealth();

    public void SetHealth(int newHealth);

    public bool IsAlive();

    public Vector2 GetCurrentPosition();
}