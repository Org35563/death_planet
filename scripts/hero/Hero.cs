using Godot;

public partial class Hero : CharacterBody2D
{
	private IEnemy _closestEnemy;

	private AnimatedSprite2D _animationPlayer;

	private Timer _heroAttackTimer;

	private Timer _heroAttackCooldownTimer;

	private bool _enemyInAttackRange = false;

	private bool _heroAttackCooldownFinished = true;

	private bool _isHeroAttacking = false;

	private HeroMovementDto _movementDto;

	private int _health;

	private bool _isAlive;

	private int _attackPower;


    public override void _Ready()
    {
		_health = (int) GetMeta(HeroMetadataNames.Health);
		_attackPower = (int) GetMeta(HeroMetadataNames.AttackPower); 
		_isAlive = true;

		_movementDto = new HeroMovementDto
		{
			CurrentDirection = MoveDirectionNames.DOWN,
			Speed = (float) GetMeta(HeroMetadataNames.Speed)
		};
		
		_heroAttackTimer = GetNode<Timer>(HeroNodeNames.AttackTimer);
		_heroAttackCooldownTimer = GetNode<Timer>(HeroNodeNames.AttackCooldownTimer); 

		_animationPlayer = GetNode<AnimatedSprite2D>(HeroNodeNames.Animation);
        _animationPlayer.Play(AnimationNames.FRONT_IDLE);		
    }

    public override void _PhysicsProcess(double delta)
	{
		if(_health <= 0)
		{
			// TODO: сделать отдельный метод на проверку жив ли герой
			_isAlive = false;
			_health = 0;
			_animationPlayer.Play("death_idle");
		}
		else
		{
			Move();
			Attack();
		}
	}

	public void Move()
	{
		var newMovement = HeroMovement.Move(_movementDto);
		if(_isHeroAttacking == false)
		{
			HeroAnimation.PlayMovement(_movementDto.CurrentDirection, _movementDto.IsWalking, _animationPlayer);
		}

		Velocity = newMovement.Velocity;
		MoveAndSlide();
	}

	private void Attack()
	{
		// TODO: переделать систему управления, атака с помощью мыши
		if(Input.IsActionJustPressed(ButtonNames.Attack))
		{
			HeroAnimation.PlayAttack(_movementDto.CurrentDirection, _animationPlayer, _heroAttackTimer);
			_isHeroAttacking = true;

			if(_enemyInAttackRange && _closestEnemy != null && _heroAttackCooldownFinished)
			{
				_closestEnemy.SetHealth(_closestEnemy.GetHealth() - _attackPower);
				_closestEnemy.SetAttack(true);
				_heroAttackCooldownFinished = false;
				_heroAttackCooldownTimer.Start();
				GD.Print($"Enemy health: {_closestEnemy.GetHealth()}");
			}
		}
	}

	#region Обработчики событий

	public void OnHeroAttackAreaBodyEntered(Node2D body)
	{
		if(Global.IsEnemy(body))
		{
			_enemyInAttackRange = true;
			_closestEnemy = (IEnemy)body;
		}
	}

	public void OnHeroAttackAreaBodyExited(Node2D body)
	{
		if(Global.IsEnemy(body))
		{
			_enemyInAttackRange = false;
			_closestEnemy = null;
		}	
	}

	public void OnHeroAttackTimerTimeout()
	{
		_heroAttackTimer.Stop();
		Global.HeroCurrentAttack = false;
		_isHeroAttacking = false;
	}

	public void OnHeroAttackCooldownTimerTimeout()
	{
		_heroAttackCooldownFinished = true;
	}

	#endregion
}
