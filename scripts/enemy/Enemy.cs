using System.Collections.Generic;
using Godot;

public partial class Enemy : CharacterBody2D, IEnemy
{
    
    private IHero _closestHero;

    private AnimatedSprite2D _animationPlayer;

    private Timer _enemyDeathTimer;

    private Timer _enemyAttackCooldownTimer;

    #region Свойства передвижения врага

    private string _currentDirection;

    private bool _stopMovement = false;

    #endregion

    #region Свойства атаки врага

    private bool _heroChase = false;

    private bool _heroInAttackZone = false;

    private bool _isEnemyUnderAttack = false;

    private bool _enemyAttackCooldownFinished = true;

    private bool _isEnemyAttacking = false;

    #endregion

    #region Характеристики врага

    private float _speed;

    private int _health;

    private bool _isAlive;

    private int _attackPower;

    #endregion

    private Dictionary<string, string> _idlesDict = new ()
    {
        { MoveDirectionNames.RIGHT, AnimationNames.SIDE_IDLE },
        { MoveDirectionNames.LEFT, AnimationNames.SIDE_IDLE },
        { MoveDirectionNames.DOWN, AnimationNames.FRONT_IDLE },
        { MoveDirectionNames.UP, AnimationNames.BACK_IDLE },  
    };

    public override void _Ready()
    {
		_health = (int) GetMeta(HeroMetadataNames.Health);
		_attackPower = (int) GetMeta(HeroMetadataNames.AttackPower);
        _speed = (float) GetMeta(HeroMetadataNames.Speed);

        _isAlive = true;
        _isEnemyUnderAttack = false;
        _currentDirection = MoveDirectionNames.DOWN;

        _enemyDeathTimer = GetNode<Timer>(EnemyNodeNames.DeathTimer);
        _enemyAttackCooldownTimer = GetNode<Timer>(EnemyNodeNames.AttackCooldownTimer);

		_animationPlayer = GetNode(EnemyNodeNames.Animation) as AnimatedSprite2D;
        _animationPlayer.Play(AnimationNames.FRONT_IDLE);
    }

    public override void _PhysicsProcess(double delta)
    {
        CheckIsEnemyAlive();

        if(_isAlive)
        {
            Move((float)delta);
            Attack();
        }
    }

    public int GetHealth() => _health;

    public void SetHealth(int newHealthValue) => _health = newHealthValue;

    public void SetIsUnderAttack(bool isUnderAttack) => _isEnemyUnderAttack = isUnderAttack;

    #region Обработчики событий

    public void OnHeroDetectionAreaBodyEntered(Node2D body)
    {
        if(Global.IsGameUnitType<IHero>(body))
        {
            _closestHero = (IHero)body;
            _heroChase = true;
        }
    }

    public void OnHeroDetectionAreaBodyExited(Node2D body)
    {
        if(Global.IsGameUnitType<IHero>(body))
        {
            _closestHero = null;
            _heroChase = false;
        }
    }

    public void  OnStopAreaBodyEntered(Node2D body)
    {
        if(Global.IsGameUnitType<IHero>(body))
        {
            _stopMovement = true;
        }    
    }

    public void OnStopAreaBodyExited(Node2D body)
    {
        if(Global.IsGameUnitType<IHero>(body))
        {
            _stopMovement = false;
        } 
    }

    public void OnEnemyHitboxBodyEntered(Node2D body)
    {
        if(Global.IsGameUnitType<IHero>(body))
        {
            _heroInAttackZone = true;
        }
    }

    public void OnEnemyHitboxBodyExited(Node2D body)
    {
        if(Global.IsGameUnitType<IHero>(body))
        {
            _heroInAttackZone = false;
        }
    }

    public void OnEnemyDeathTimerTimeout()
    {
        _enemyDeathTimer.Stop();
        this.QueueFree();
    }

    public void OnEnemyAttackCooldownTimerTimeout()
    {
        _enemyAttackCooldownTimer.Stop();
        _enemyAttackCooldownFinished = true;
        _closestHero.SetIsUnderAttack(false);
    }

    #endregion

    private void Move(float delta)
    {
        if(_heroChase && !_stopMovement)
        {
            Vector2 direction = (_closestHero.GetCurrentPosition() - Position).Normalized();
            Position += direction * _speed * delta;
            if (Mathf.Abs(direction.X) > Mathf.Abs(direction.Y))
            {
                // Движение влево или вправо
                _animationPlayer.Play(AnimationNames.SIDE_WALK);
                _animationPlayer.FlipH = direction.X < 0;

                _currentDirection = _animationPlayer.FlipH ? MoveDirectionNames.RIGHT : MoveDirectionNames.LEFT;
            }
            else
            {
                if (direction.Y < 0)
                {
                    // Движение вверх
                    _animationPlayer.Play(AnimationNames.BACK_WALK);
                    _currentDirection = MoveDirectionNames.UP;
                }
                else
                {
                    // Движение вниз
                    _animationPlayer.Play(AnimationNames.FRONT_WALK);
                    _currentDirection = MoveDirectionNames.DOWN;
                }
            }

            MoveAndSlide();   
        }
        else
        {
            _animationPlayer.Play(_idlesDict[_currentDirection]);
        }
    }

    private void Attack()
    {
        if(_heroInAttackZone && _closestHero != null && _closestHero.IsAlive() && _enemyAttackCooldownFinished)
        {
            _closestHero.SetHealth(_closestHero.GetHealth() - _attackPower);
            _closestHero.SetIsUnderAttack(true);
            _enemyAttackCooldownFinished = false;
            _enemyAttackCooldownTimer.Start();
            GD.Print($"Hero health: {_closestHero.GetHealth()}");
        }
    }

    private void CheckIsEnemyAlive()
    {
        if(_isEnemyUnderAttack)
        {   
            if(_health <= 0)
            {
                _animationPlayer.Play(AnimationNames.DEATH); 
                _enemyDeathTimer.Start();
                _isEnemyUnderAttack = false;
                _isAlive = false;
            }
        }
    }
}


