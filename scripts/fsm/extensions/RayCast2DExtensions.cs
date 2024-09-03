using System.Collections.Generic;
using Godot;

public static class RayCast2DExtensions
{
    public static void SetNewRayCastDirection(this RayCast2D rayCast, string directionName, float rayLength)
    {
        if(rayCast != null)
        {
            rayCast.TargetPosition = GetRayByDirectionName(directionName, rayLength);
        }
    }

    public static bool HasCollisionsWithBarriers(this RayCast2D rayCast, Vector2 position, float rayLength)
    {
        if(rayCast != null)
        {
            rayCast.SetNewRayCastDirection(position, rayLength);
            if(rayCast.IsColliding())
            {
                return true;
            }         
        }

        return false;
    }

    private static Vector2 GetRayByDirectionName(string directionName, float rayLength)
    {
        var raysDict = new Dictionary<string, Vector2>()
        {
            { DirectionNames.RIGHT, new Vector2(-rayLength, 0) },
            { DirectionNames.LEFT, new Vector2(rayLength ,0) },
            { DirectionNames.DOWN, new Vector2(0, rayLength) },
            { DirectionNames.UP, new Vector2(0, -rayLength) },  
        };

        if(raysDict.ContainsKey(directionName))
        {
            return raysDict[directionName];
        }

        return Vector2.Zero;
    }

    private static void SetNewRayCastDirection(this RayCast2D rayCast, Vector2 position, float rayLength)
    {
        int xDirection = position.X > 0 ? 1 : (position.X < 0 ? -1 : 0);
        int yDirection = position.Y > 0 ? 1 : (position.Y < 0 ? -1 : 0);

        Vector2 targetPosition = new (rayLength * xDirection, rayLength * yDirection);

        rayCast.TargetPosition = targetPosition;
    }
}