using System.Collections.Generic;

public static class AnimationHelper
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

    private static string GetAnimationNameByDirection(Dictionary<string, string> animations, string direction)
    {
        if(animations.ContainsKey(direction))
        {
            return animations[direction];
        }

        return AnimationNames.NONE;
    }
}