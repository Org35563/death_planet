using System;
using System.Linq;
using Godot;

public static class Global
{
    public static bool IsGameUnitType<T>(Node2D body) =>
      body.GetType()
          .GetInterfaces()
          .Contains(typeof(T));

    public static Node GetNodeByName(Node source, string parentNodeName, string searchNodeName) =>
      source.GetNode<Node>(parentNodeName)
            .GetChildren()
            .FirstOrDefault(x => x.Name == searchNodeName);

    public static bool IsCreatureAlive(Node body) => body is ILivingCreature creature && creature.IsAlive();

    public static string GetNewMoveDirectionName(Vector2 direction)
    {
        var newDirection = string.Empty;
        if (Mathf.Abs(direction.X) > Mathf.Abs(direction.Y))
        {
            newDirection = direction.X < 0 ? DirectionNames.RIGHT : DirectionNames.LEFT;
        }
        else if (direction.Y < 0)
        {
            newDirection = DirectionNames.UP;
        }
        else
        {
            newDirection = DirectionNames.DOWN;
        }

        return newDirection;
    }

    public static Rect2 GetWorldBoundaries(Area2D area)
    {
        // Получаем позицию и размер Area2D
        var areaShape = area.GetChild<CollisionShape2D>(0);
        Vector2 position = areaShape.Position;
        Vector2 size = areaShape.Shape.GetRect().Size;

        // Создаем Rect2, представляющий границы Area2D
        return new Rect2(position - size / 2, size);
    }

    public static bool IsCharacterOnArea(Area2D area, Vector2 positionToCheck)
    {
        if(area != null)
        {
            var areaBounds = GetWorldBoundaries(area);
            if (areaBounds.HasPoint(positionToCheck))
            {
                return true;              
            }
        }

        return false;
    }
}