using System;
using Godot;

public partial class WanderState : State, IMovableState
{
    [Export]
    public AnimatedSprite2D _animationPlayer;

    [Export]
    public CharacterBody2D _enemy;

    [Export]
    public float MoveSpeed;

    private Timer _fsmMoveTimer;

    private Random _random;

    private Vector2 _moveDirection;

    private string _currentDirection;

    public override void _Ready()
    {
        _fsmMoveTimer = GetNode<Timer>(StateNodeNames.WanderTimer);
        _random = new Random();
    }

    public override void PhysicsUpdate(float delta)
    {
        _enemy.Velocity = _moveDirection * MoveSpeed;

        _enemy.MoveAndSlide();
    }

    public override void Enter()
    {  
        while(true)
        {
            _moveDirection = new Vector2(_random.Next(-1, 2), _random.Next(-1, 2)).Normalized();
            if(_moveDirection.X != 0 || _moveDirection.Y != 0)
            {
                break;
            }
        }

        var animationName = string.Empty;
        if (Mathf.Abs(_moveDirection.X) > Mathf.Abs(_moveDirection.Y))
        {
            animationName = AnimationNames.SIDE_WALK;
            _animationPlayer.FlipH = _moveDirection.X < 0;

            _currentDirection = _animationPlayer.FlipH ? MoveDirectionNames.RIGHT : MoveDirectionNames.LEFT;
        }
        else
        {
            if (_moveDirection.Y < 0)
            {
                animationName = AnimationNames.BACK_WALK;
                _currentDirection = MoveDirectionNames.UP;
            }
            else
            {
                animationName = AnimationNames.FRONT_WALK;
                _currentDirection = MoveDirectionNames.DOWN;
            }
        }     

        if(_animationPlayer != null)
        {
            _animationPlayer.Play(animationName);;
        }

        if(_fsmMoveTimer != null)
        {
            _fsmMoveTimer.WaitTime = _random.Next(1, 3);
            _fsmMoveTimer.Start(); 
        }
    }

    public override void Exit() => _fsmMoveTimer.Stop();

    public void OnFsmWanderTimerTimeout()
    {
        StateMachine.TransitionTo(StateNames.Idle);
    }

    public string GetCurrentDirection() => _currentDirection;

    public void SetCurrentDirection(string direction) => _currentDirection = direction;
}