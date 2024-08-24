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
		if(Input.IsActionJustPressed(ButtonNames.Attack))
		{	
			if(direction == MoveDirectionNames.RIGHT)
			{
				animation.FlipH = false;
				animation.Play(AnimationNames.SIDE_ATTACK);	
				attackTimer.Start();
			}
			else if(direction == MoveDirectionNames.LEFT)
			{
				animation.FlipH = true;
				animation.Play(AnimationNames.SIDE_ATTACK);
				attackTimer.Start();
			}
			else if(direction == MoveDirectionNames.DOWN)
			{
				animation.Play(AnimationNames.FRONT_ATTACK);
				attackTimer.Start();
			}
			else if(direction == MoveDirectionNames.UP)
			{
				animation.Play(AnimationNames.BACK_ATTACK);
				attackTimer.Start();
			}		
		}
	}
}