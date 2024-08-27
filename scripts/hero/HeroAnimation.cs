using Godot;

public static class HeroAnimation
{
    public static void PlayMovement(string direction, bool isWalking, AnimatedSprite2D animation)
	{
		var animationName = string.Empty;
		if(direction == MoveDirectionNames.RIGHT)
		{
			animation.FlipH = false;
			animationName = isWalking ? AnimationNames.SIDE_WALK : AnimationNames.SIDE_IDLE;	
		}
		
		if(direction == MoveDirectionNames.LEFT)
		{
			animation.FlipH = true;
			animationName = isWalking ? AnimationNames.SIDE_WALK : AnimationNames.SIDE_IDLE;
		}
		
		if(direction == MoveDirectionNames.DOWN)
		{
			animation.FlipH = true;
			animationName = isWalking ? AnimationNames.FRONT_WALK : AnimationNames.FRONT_IDLE;
		}
		
		if(direction == MoveDirectionNames.UP)
		{
			animation.FlipH = true;
			animationName = isWalking ? AnimationNames.BACK_WALK : AnimationNames.BACK_IDLE;
		}

		animation.Play(animationName);
	}

	public static void PlayAttack(string direction, AnimatedSprite2D animation, Timer attackTimer)
	{
		var animationName = string.Empty;
		if(direction == MoveDirectionNames.RIGHT)
		{
			animation.FlipH = false;
			animationName = AnimationNames.SIDE_ATTACK;	
		}
		else if(direction == MoveDirectionNames.LEFT)
		{
			animation.FlipH = true;
			animationName = AnimationNames.SIDE_ATTACK;
		}
		else if(direction == MoveDirectionNames.DOWN)
		{
			animationName = AnimationNames.FRONT_ATTACK;
		}
		else if(direction == MoveDirectionNames.UP)
		{
			animationName = AnimationNames.BACK_ATTACK;
		}

		animation.Play(animationName);	
		attackTimer.Start();
	}
}