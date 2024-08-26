using Godot;

public interface IHero : ICombatUnit
{
    public Vector2 GetCurrentPosition();

    public bool IsAlive();
}