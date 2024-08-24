using Godot;

public partial class Hero : CharacterBody2D
{
	private string _currentDirection;

	private AnimatedSprite2D _animation;

	private bool _enemyInAttackRange = false;

	private bool _enemyAttackCooldown = true;

	private bool _attackIp = false;

	#region Hero Parameters

	private float _speed;

	private int _health;

	private bool _isAlive;

	#endregion

    public override void _Ready()
    {
		SetHeroParams();

		_isAlive = true;
		_currentDirection = MoveDirectionNames.DOWN;
		
		_animation = GetNode<AnimatedSprite2D>(HeroNodeNames.HeroAnimation);
        _animation.Play(AnimationNames.FRONT_IDLE);		
    }

    public override void _PhysicsProcess(double delta)
	{
		if(_health <= 0)
		{
			_isAlive = false;
			_health = 0;
			_animation.Play("death_idle");
		}
		else
		{
			HeroMove(delta);
			EnemyAttack();
			Attack();
		}
	}

	public void HeroMove(double delta)
	{
		Vector2 velocity = default;
		if(Input.IsActionPressed("ui_right"))
		{
			velocity = GetHeroMovement(MoveDirectionNames.RIGHT, true, _speed, 0);
		}
		else if (Input.IsActionPressed("ui_left"))
		{
			velocity = GetHeroMovement(MoveDirectionNames.LEFT, true, -_speed, 0);
		}
		else if (Input.IsActionPressed("ui_down"))
		{
			velocity = GetHeroMovement(MoveDirectionNames.DOWN, true, 0, _speed);
		}
		else if (Input.IsActionPressed("ui_up"))
		{
			velocity = GetHeroMovement(MoveDirectionNames.UP, true, 0, -_speed);
		}
		else 
		{
			velocity = GetHeroMovement(MoveDirectionNames.NONE, false, 0, 0);
		}

		Velocity = velocity;
		MoveAndSlide();
	}

	public void OnDealAttackTimerTimeout()
	{
		var timer = GetNode<Timer>("deal_attack_timer") as Timer;
		timer.Stop();
		Global.HeroCurrentAttack = false;
		_attackIp = false;
	}

	public void OnHeroHitboxBodyEntered(Node2D body)
	{
		if(body.Name == "enemy" && body.HasMethod("Attack"))
		{
			_enemyInAttackRange = true;
		}
	}

	public void OnHeroHitboxBodyExited(Node2D body)
	{
		if(body.Name == "enemy" && body.HasMethod("Attack"))
		{
			_enemyInAttackRange = false;
		}	
	}

	public void OnAttackCooldownTimeout()
	{
		_enemyAttackCooldown = true;
	}

	private void EnemyAttack()
	{
		if(_enemyInAttackRange && _enemyAttackCooldown && _isAlive)
		{
			_health -= Global.EnemyAttackValue;
			_enemyAttackCooldown = false;
			var timer = GetNode<Timer>("attack_cooldown");
			timer.Start(); 
			GD.Print(_health);
		}
	}

	private Vector2 GetHeroMovement(string direction, bool isWalking, float xSpeed, float ySpeed)
	{
		Vector2 newVelocity = Velocity;
		if(direction != MoveDirectionNames.NONE)
		{
			_currentDirection = direction;
		}

		if(_attackIp == false)
		{
			PlayAnimation(isWalking);
		}

		newVelocity.X = xSpeed;
		newVelocity.Y = ySpeed;

		return newVelocity;
	}

	private void PlayAnimation(bool isWalking)
	{
		var anim = string.Empty;
		var direction = _currentDirection;
		if(direction == MoveDirectionNames.RIGHT)
		{
			_animation.FlipH = false;
			anim = isWalking ? AnimationNames.SIDE_WALK : AnimationNames.SIDE_IDLE;	
		}
		
		if(direction == MoveDirectionNames.LEFT)
		{
			_animation.FlipH = true;
			anim = isWalking ? AnimationNames.SIDE_WALK : AnimationNames.SIDE_IDLE;
		}
		
		if(direction == MoveDirectionNames.DOWN)
		{
			_animation.FlipH = true;
			anim = isWalking ? AnimationNames.FRONT_WALK : AnimationNames.FRONT_IDLE;
		}
		
		if(direction == MoveDirectionNames.UP)
		{
			_animation.FlipH = true;
			anim = isWalking ? AnimationNames.BACK_WALK : AnimationNames.BACK_IDLE;
		}

		_animation.Play(anim);
	}

	private void Attack()
	{
		if(Input.IsActionJustPressed("attack"))
		{
			Global.HeroCurrentAttack = true;
			_attackIp = true;	
			if(_currentDirection == MoveDirectionNames.RIGHT)
			{
				_animation.FlipH = false;
				_animation.Play("side_attack");
				var timer = GetNode<Timer>("deal_attack_timer");		
				timer.Start();
			}
			else if(_currentDirection == MoveDirectionNames.LEFT)
			{
				_animation.FlipH = true;
				_animation.Play("side_attack");
				var timer = GetNode<Timer>("deal_attack_timer");
				timer.Start();
			}
			else if(_currentDirection == MoveDirectionNames.DOWN)
			{
				_animation.Play("front_attack");
				var timer = GetNode<Timer>("deal_attack_timer");
				timer.Start();
			}
			else if(_currentDirection == MoveDirectionNames.UP)
			{
				_animation.Play("back_attack");
				var timer = GetNode<Timer>("deal_attack_timer");
				timer.Start();
			}		
		}
	}

	private void SetHeroParams()
	{
		var newSpeed = (float) GetMeta(HeroMetadataNames.Speed);
		if(_speed != newSpeed)
		{
			_speed = newSpeed;
		}

		var newHealth = (int) GetMeta(HeroMetadataNames.Health);
		if(_health != newHealth)
		{
			_health = newHealth;
		}
	}
}
