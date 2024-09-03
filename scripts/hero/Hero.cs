using Godot;

// TODO: переработать с учётом новой механики AI врага
public partial class Hero : CharacterBody2D, ICombatCreature
{
	[Export]
	public int Health;

	[Export]
	public float MoveSpeed;

	[Export]
	public int AttackPower;

	private ILivingCreature _closestEnemy;

	private AnimatedSprite2D _animationPlayer;

	private Timer _heroAttackTimer;

	private Timer _heroAttackCooldownTimer;

	private Timer _heroDeathTimer;

	private HeroMovementDto _movementDto;

	private bool _enemyInAttackRange = false;

	private bool _heroAttackCooldownFinished = true;

	private bool _isHeroAttacking = false;

	private bool _isAlive { get => Health > 0; }

	private bool _alreadyDeath;

    public override void _Ready()
    {
		_alreadyDeath = false;

		_movementDto = new HeroMovementDto
		{
			CurrentDirection = DirectionNames.DOWN,
			Speed = MoveSpeed
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

	#region Обработчики событий

	public void OnHeroAttackAreaBodyEntered(Node2D body)
	{
		if(Global.IsGameUnitType<ILivingCreature>(body))
		{	
			_enemyInAttackRange = true;
			_closestEnemy = (ILivingCreature)body;
		}
	}

	public void OnHeroAttackAreaBodyExited(Node2D body)
	{
		if(Global.IsGameUnitType<ILivingCreature>(body))
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
				_closestEnemy.SetHealth(_closestEnemy.GetHealth() - AttackPower);
				_heroAttackCooldownFinished = false;
				_heroAttackCooldownTimer.Start();
				GD.Print($"Enemy health: {_closestEnemy.GetHealth()}");
			}
		}
	}

	private void CheckIsHeroAlive()
	{
		if(_isAlive == false && _alreadyDeath == false)
		{
			var collisionShape = GetNode<CollisionShape2D>("hero_collision_shape");
            collisionShape.Disabled = true;

			_animationPlayer.Play(AnimationNames.DEATH);
			_alreadyDeath = true;
			_heroDeathTimer.Start();
		}
	}

    public bool IsAlive() => _isAlive;

    public int GetHealth() => Health;

    public void SetHealth(int newHealth) => Health = newHealth;

    public Vector2 GetCurrentPosition() => Position;

    public float GetMoveSpeed() => MoveSpeed;

    public float SetMoveSpeed(float newSpeed) => MoveSpeed = newSpeed;

    public int GetAttackPower() => AttackPower;

    public int SetAttackPower(int newAttackPower) => AttackPower = newAttackPower;
}
