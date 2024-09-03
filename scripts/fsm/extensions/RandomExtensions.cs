using System;
using Godot;

public static class RandomExtensions
{
    public static Vector2 GetNewRandomMoveDirection(this Random random)
    {
        var newMoveDirection = Vector2.Zero;
        while(newMoveDirection.X == 0 && newMoveDirection.Y == 0)
        {
            newMoveDirection = new Vector2(random.Next(-1, 2), random.Next(-1, 2)).Normalized();
        }

        return newMoveDirection;
    }
}