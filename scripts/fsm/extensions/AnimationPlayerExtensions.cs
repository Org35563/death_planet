using Godot;

public static class AnimationPlayerExtensions
{
    public static void PlayMoveAnimation(this AnimatedSprite2D animationPlayer, string moveDirectionName, bool flipH)
    {     
        if(animationPlayer != null)
        {
            var moveAnimationName = AnimationHelper.GetMoveAnimationNameByDirection(moveDirectionName);

            animationPlayer.FlipH = flipH;
            
            animationPlayer.Play(moveAnimationName);
        }
    }

    public static void PlayAttackAnimation(this AnimatedSprite2D animationPlayer, Vector2 attackerPosition, Vector2 attackedPosition)
    {
        float deltaX = attackedPosition.X - attackerPosition.X;
        float deltaY = attackedPosition.Y - attackerPosition.Y;

        var attackDirection = DirectionNames.NONE;
        if (Mathf.Abs(deltaX) > Mathf.Abs(deltaY))
        {
            animationPlayer.FlipH = deltaX < 0;
            attackDirection = animationPlayer.FlipH ? DirectionNames.RIGHT : DirectionNames.LEFT;
        }
        else
        {
            attackDirection = deltaY < 0 ? DirectionNames.UP : DirectionNames.DOWN;
        }

        var attackAnimationName = AnimationHelper.GetAttackAnimationNameByDirection(attackDirection);

        animationPlayer.Play(attackAnimationName);
    }

    public static void PlayIdleAnimation(this AnimatedSprite2D animationPlayer, string directionName)
    {
        var idleAnimationName = AnimationHelper.GetIdleAnimationNameByDirection(directionName);

        animationPlayer?.Play(idleAnimationName);
    }
}