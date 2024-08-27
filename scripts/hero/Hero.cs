using Godot;

public partial class Hero : CharacterBody2D, IHero
{
	private IEnemy _closestEnemy;

	private AnimatedSprite2D _animationPlayer;

	private Timer _heroAttackTimer;

	private Timer _heroAttackCooldownTimer;

	private Timer _heroDeathTimer;

	private HeroMovementDto _movementDto;

	#region Свойства атаки героя

	private bool _enemyInAttackRange = false;

	private bool _heroAttackCooldownFinished = true;

	private bool _isHeroAttacking = false;

	private bool _isHeroUnderAttack = false;

	#endregion

	#region Характеристики героя
	
	private int _health;

	private bool _isAlive;

	private int _attackPower;

	#endregion

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
		_heroDeathTimer = GetNode<Timer>(HeroNodeNames.DeathTimer);

		_animationPlayer = GetNode<AnimatedSprite2D>(HeroNodeNames.Animation);
        _animationPlayer.Play(AnimationNames.FRONT_IDLE);		
    }

    public override void _PhysicsProcess(double delta)
	{
		CheckIsHeroAlive();

		if(_isAlive)
		{
			Attack();
			Move();
		}
	}

	public int GetHealth() => _health;

    public void SetHealth(int newHealthValue) => _health = newHealthValue;

    public void SetIsUnderAttack(bool isUnderAttack) => _isHeroUnderAttack = isUnderAttack;

	public Vector2 GetCurrentPosition() => Position;

	public bool IsAlive() => _isAlive;

	#region Обработчики событий

	public void OnHeroAttackAreaBodyEntered(Node2D body)
	{
		if(Global.IsGameUnitType<IEnemy>(body))
		{
			_enemyInAttackRange = true;
			_closestEnemy = (IEnemy)body;
		}
	}

	public void OnHeroAttackAreaBodyExited(Node2D body)
	{
		if(Global.IsGameUnitType<IEnemy>(body))
		{
			_enemyInAttackRange = false;
			_closestEnemy = null;
		}	
	}

	public void OnHeroAttackTimerTimeout()
	{
		_heroAttackTimer.Stop();
		_isHeroAttacking = false;
	}

	public void OnHeroAttackCooldownTimerTimeout()
	{
		_heroAttackCooldownTimer.Stop();
		_heroAttackCooldownFinished = true;
	}

	public void OnHeroDeathTimerTimeout()
	{
        _heroDeathTimer.Stop();
		_animationPlayer.Play(AnimationNames.DEATH_IDLE);
	}

    #endregion

	private void Move()
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
				_closestEnemy.SetIsUnderAttack(true);
				_heroAttackCooldownFinished = false;
				_heroAttackCooldownTimer.Start();
				GD.Print($"Enemy health: {_closestEnemy.GetHealth()}");
			}
		}
	}

	private void CheckIsHeroAlive()
	{
		if(_isHeroUnderAttack)
        {   
            if(_health <= 0)
            {
                _animationPlayer.Play(AnimationNames.DEATH); 
                _heroDeathTimer.Start();
                _isHeroUnderAttack = false;
				_isAlive = false;
            }
        }
	}
}
