using System;
using System.Collections.Generic;
using Godot;

public partial class Enemy : CharacterBody2D
{
    public const float Speed = 45.0f;

    private bool _heroChase = false;

    private Node2D _hero = null;

    private AnimatedSprite2D _enemyAnimation;

    private bool _stopMovement = false;

    private const string EnemyAnimationNodeName = "enemy_animation";

    private string _currentDirection = MoveDirectionNames.DOWN;

    private bool _heroInAttackZone = false;

    private int _health = 100;

    private Dictionary<string, string> _idlesDict = new ()
    {
        { MoveDirectionNames.RIGHT, AnimationNames.SIDE_IDLE },
        { MoveDirectionNames.LEFT, AnimationNames.SIDE_IDLE },
        { MoveDirectionNames.DOWN, AnimationNames.FRONT_IDLE },
        { MoveDirectionNames.UP, AnimationNames.BACK_IDLE },  
    };

    public override void _Ready()
    {
		_enemyAnimation = GetNode(EnemyAnimationNodeName) as AnimatedSprite2D;
        _enemyAnimation.Play(AnimationNames.FRONT_IDLE);
    }

    public override void _PhysicsProcess(double delta)
    {
        DealWithAttack();

        if(_health <= 0)
        {
            return;
        }

        if(_heroChase && !_stopMovement)
        {
            Vector2 direction = (_hero.Position - Position).Normalized();
            Position += direction * Speed * (float)delta;
            if (Mathf.Abs(direction.X) > Mathf.Abs(direction.Y))
            {
                // Движение влево или вправо
                _enemyAnimation.Play(AnimationNames.SIDE_WALK);
                _enemyAnimation.FlipH = direction.X < 0;

                _currentDirection = _enemyAnimation.FlipH ? MoveDirectionNames.RIGHT : MoveDirectionNames.LEFT;
            }
            else
            {
                if (direction.Y < 0)
                {
                    // Движение вверх
                    _enemyAnimation.Play(AnimationNames.BACK_WALK);
                    _currentDirection = MoveDirectionNames.UP;
                }
                else
                {
                    // Движение вниз
                    _enemyAnimation.Play(AnimationNames.FRONT_WALK);
                    _currentDirection = MoveDirectionNames.DOWN;
                }
            }

            MoveAndSlide();
        }
        else
        {
            _enemyAnimation.Play(_idlesDict[_currentDirection]);
        }
    }

    public void OnDetectionAreaBodyEntered(Node2D body)
    {
        MakeOnHeroAction(body, () =>
        {
            _hero = body;
            _heroChase = true;
        });
    }

    public void OnDetectionAreaBodyExited(Node2D body)
    {
        MakeOnHeroAction(body, () =>
        {
            _hero = null;
            _heroChase = false;
        });
    }

    public void  OnStopAreaBodyEntered(Node2D body)
    {
        MakeOnHeroAction(body, () => {_stopMovement = true;});      
    }

    public void OnStopAreaBodyExited(Node2D body)
    {
        MakeOnHeroAction(body, () => {_stopMovement = false;});
    }

    public void MakeOnHeroAction(Node2D body, Action action)
    {
        if(string.Equals(body.Name, nameof(Hero), StringComparison.OrdinalIgnoreCase))
        {
            action();
        }
    }

    public void OnEnemyHitboxBodyEntered(Node2D body)
    {
        if(body.Name == "hero")
        {
            _heroInAttackZone = true;
        }
    }

    public void OnEnemyHitboxBodyExited(Node2D body)
    {
        if(body.Name == "hero")
        {
            _heroInAttackZone = false;
        }
    }

    public void Attack()
    {
        GD.Print("Attacking!");
    }

    public void OnDeathTimerTimeout()
    {
        var deathTimer = GetNode<Timer>("death_timer") as Timer;
        deathTimer.Stop();
        this.QueueFree();
    }

    private void DealWithAttack()
    {
        if(_heroInAttackZone && Global.HeroCurrentAttack && _health > 0)
        {
            _health -= Global.HeroAttackValue;
            GD.Print($"Slime health: " + _health);
            if(_health <= 0)
            {
                var deathTimer = GetNode<Timer>("death_timer") as Timer;
                _enemyAnimation.Play("death"); 
                deathTimer.Start();                 
            }

            Global.HeroCurrentAttack = false;
        }
    }
}


