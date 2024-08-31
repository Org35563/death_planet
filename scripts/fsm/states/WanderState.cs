using System;
using Godot;

public partial class WanderState : State, IMovableState
{
    [Export]
    public AnimatedSprite2D AnimationPlayer;

    [Export]
    public CharacterBody2D Character;

    [Export]
    public Area2D ChaseArea;

    [Export]
    public int MinTimerValue;

    [Export]
    public int MaxTimerValue;

    private Timer _fsmWanderTimer;

    private Random _random;

    private Vector2 _moveDirection;

    private string _currentDirection;

    private Node _idleNode;

    private float _wanderingSpeed;

    public override void _Ready()
    {
        if(Character is ICombatCreature combatCreature)
        {
            _wanderingSpeed = combatCreature.GetMoveSpeed();
        }

        _fsmWanderTimer = GetNode<Timer>(StateNodeNames.WanderTimer);
        _random = new Random();
        _idleNode = Global.GetNodeByName(Character, StateNodeNames.StateMachine, StateNames.Idle);
        ChaseArea.BodyEntered += OnChaseCollision;
    }

    public override void PhysicsUpdate(float delta)
    {
        Character.Velocity = _moveDirection * _wanderingSpeed;
        Character.MoveAndSlide();
    }

    public override void Enter()
    {  
        StateMachine.TryTransitionToDeath(Character);

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
            AnimationPlayer.FlipH = _moveDirection.X < 0;

            _currentDirection = AnimationPlayer.FlipH ? DirectionNames.RIGHT : DirectionNames.LEFT;
        }
        else if (_moveDirection.Y < 0)
        {
            animationName = AnimationNames.BACK_WALK;
            _currentDirection = DirectionNames.UP;
        }
        else
        {
            animationName = AnimationNames.FRONT_WALK;
            _currentDirection = DirectionNames.DOWN;
        }

        if(AnimationPlayer != null)
        {
            AnimationPlayer.Play(animationName);
        }

        if(_fsmWanderTimer != null)
        {
            _fsmWanderTimer.WaitTime = _random.Next(MinTimerValue, MaxTimerValue);
            _fsmWanderTimer.Start(); 
        }
    }

    public override void Exit() => _fsmWanderTimer.Stop();

    public void OnFsmWanderTimerTimeout()
    {
        if(_idleNode != null && _idleNode is IMovableState movableState)
        {
            movableState.SetCurrentDirection(_currentDirection);
        }

        Exit();
        StateMachine.TransitionTo(StateNames.Idle);
    }

    public string GetCurrentDirection() => _currentDirection;

    public void SetCurrentDirection(string direction) => _currentDirection = direction;

    public void OnChaseCollision(Node2D body)
    {
        if(Global.IsCreatureAlive(Character) == false)
        {
            return;
        }

        if(body != null && Global.IsCreatureAlive(body))
        {
            Exit();
            StateMachine.TransitionTo(StateNames.Chase);
        }
        else
        {
            Exit();
            StateMachine.TransitionTo(StateNames.Idle);
        }
    }
}