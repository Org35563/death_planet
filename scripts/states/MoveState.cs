using System;
using Godot;

public partial class MoveState : State
{
    [Export]
    public AnimatedSprite2D _animationPlayer;

    [Export]
    public CharacterBody2D _enemy;

    private Timer _fsmMoveTimer;

    private Random _random;

    private Vector2 _moveDirection;

    private float _moveSpeed = 30.0f;

    private bool _isMove = false;

    private string _currentIdleName;

    public override void _Ready()
    {
        _fsmMoveTimer = GetNode<Timer>("fsm_move_timer");
        _random = new Random();
    }

    public override void PhysicsUpdate(float delta)
    {
        var newVelocity = _moveDirection * _moveSpeed;;
        if((_enemy.Position.X + newVelocity.X) > 0 && (_enemy.Position.Y + newVelocity.Y) > 0)
        {
            _enemy.Velocity = newVelocity;
        } 
    }

    public override void Enter()
    {  
        _moveDirection = new Vector2(_random.Next(-1, 1), _random.Next(-1, 1)).Normalized();
        _fsmMoveTimer.WaitTime = _random.Next(2, 5);

        var animationName = string.Empty;
        if (Mathf.Abs(_moveDirection.X) > Mathf.Abs(_moveDirection.Y))
        {
            animationName = AnimationNames.SIDE_WALK;
            _animationPlayer.FlipH = _moveDirection.X < 0;

            _currentIdleName = AnimationNames.SIDE_IDLE;
        }
        else
        {
            if (_moveDirection.Y < 0)
            {
                animationName = AnimationNames.BACK_WALK;
                _currentIdleName = AnimationNames.BACK_IDLE;
            }
            else
            {
                animationName = AnimationNames.FRONT_WALK;
                _currentIdleName = AnimationNames.FRONT_IDLE;
            }
        }     

        if(_animationPlayer != null)
        {
            _animationPlayer.Play(animationName);;
        }

        _fsmMoveTimer.Start();  
    }

    public override void Exit()
    {
        _enemy.SetMeta("MoveDirection", _currentIdleName);
        _fsmMoveTimer.Stop();
    }

    public void OnFsmMoveTimerTimeout()
    {
        StateMachine.TransitionTo("idle");
    }
}