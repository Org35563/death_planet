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
}