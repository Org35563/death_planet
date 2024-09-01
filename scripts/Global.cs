using System.Collections.Generic;
using System.Linq;
using Godot;

public static class Global
{
    private static Dictionary<string, string> _attackAnimations = new ()
    {
        { DirectionNames.RIGHT, AnimationNames.SIDE_ATTACK },
        { DirectionNames.LEFT, AnimationNames.SIDE_ATTACK },
        { DirectionNames.DOWN, AnimationNames.FRONT_ATTACK },
        { DirectionNames.UP, AnimationNames.BACK_ATTACK },  
    };

    private static Dictionary<string, string> _moveAnimations = new ()
    {
        { DirectionNames.RIGHT, AnimationNames.SIDE_WALK },
        { DirectionNames.LEFT, AnimationNames.SIDE_WALK },
        { DirectionNames.DOWN, AnimationNames.FRONT_WALK },
        { DirectionNames.UP, AnimationNames.BACK_WALK },  
    };

    private static Dictionary<string, string> _idleAnimations = new ()
    {
        { DirectionNames.RIGHT, AnimationNames.SIDE_IDLE },
        { DirectionNames.LEFT, AnimationNames.SIDE_IDLE },
        { DirectionNames.DOWN, AnimationNames.FRONT_IDLE },
        { DirectionNames.UP, AnimationNames.BACK_IDLE },  
    };

    public static string GetAttackAnimationNameByDirection(string direction) =>
      GetAnimationNameByDirection(_attackAnimations, direction);

    public static string GetMoveAnimationNameByDirection(string direction) =>
      GetAnimationNameByDirection(_moveAnimations, direction);

    public static string GetIdleAnimationNameByDirection(string direction) =>
      GetAnimationNameByDirection(_idleAnimations, direction);

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

    private static string GetAnimationNameByDirection(Dictionary<string, string> animations, string direction)
    {
        if(animations.ContainsKey(direction))
        {
          return animations[direction];
        }

        return AnimationNames.NONE;
    }
  }