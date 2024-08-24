using Godot;

public class HeroMovementDto
{
    public string CurrentDirection { get; set; }

    public Vector2 Velocity { get; set; }

    public float Speed { get; set; }

    public bool IsWalking { get; set; }
}