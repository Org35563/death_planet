using Godot;

public static class HeroMovement
{
    public static HeroMovementDto Move(HeroMovementDto movementDto)
	{
		var newVelocity = default(Vector2);
        var newDirection = MoveDirectionNames.NONE;
        var isWalking = false;
		if(Input.IsActionPressed("ui_right"))
		{
			newVelocity = GetNewVelocity(movementDto.Velocity, movementDto.Speed, 0);
            newDirection = MoveDirectionNames.RIGHT;
            isWalking = true;
		}
		else if (Input.IsActionPressed("ui_left"))
		{
			newVelocity = GetNewVelocity(movementDto.Velocity, -movementDto.Speed, 0);
            newDirection = MoveDirectionNames.LEFT;
            isWalking = true;
		}
		else if (Input.IsActionPressed("ui_down"))
		{
			newVelocity = GetNewVelocity(movementDto.Velocity, 0, movementDto.Speed);
            newDirection = MoveDirectionNames.DOWN;
            isWalking = true;
		}
		else if (Input.IsActionPressed("ui_up"))
		{
			newVelocity = GetNewVelocity(movementDto.Velocity, 0, -movementDto.Speed);
            newDirection = MoveDirectionNames.UP;
            isWalking = true;
		}
		else 
		{
			newVelocity = GetNewVelocity(movementDto.Velocity, 0, 0);
		}

        if(newDirection != MoveDirectionNames.NONE)
        {        
            movementDto.CurrentDirection = newDirection;
        }

        movementDto.Velocity = newVelocity;
        movementDto.IsWalking = isWalking;

		return movementDto;
	}

    private static Vector2 GetNewVelocity(Vector2 initialVelocity, float xAxisSpeed, float yAxisSpeed)
	{
		Vector2 newVelocity = initialVelocity;

		newVelocity.X = xAxisSpeed;
		newVelocity.Y = yAxisSpeed;

		return newVelocity;
	}
}