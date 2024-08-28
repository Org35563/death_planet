using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class IdleState : State
{
    [Export]
    public AnimatedSprite2D AnimationPlayer;

    [Export]
    public CharacterBody2D Enemy;

    [Export]
    public int MinTimerValue;

    [Export]
    public int MaxTimerValue;

    private Timer _fsmIdleTimer;

    private Random _random;

    private string _currentIdle;

    private Vector2 _idleVelocity;

    private Dictionary<string, string> _idlesDict = new ()
    {
        { MoveDirectionNames.RIGHT, AnimationNames.SIDE_IDLE },
        { MoveDirectionNames.LEFT, AnimationNames.SIDE_IDLE },
        { MoveDirectionNames.DOWN, AnimationNames.FRONT_IDLE },
        { MoveDirectionNames.UP, AnimationNames.BACK_IDLE },  
    };

    public override void _Ready()
    {
        _fsmIdleTimer = GetNode<Timer>(StateNodeNames.IdleTimer);
        _random = new Random();
        _currentIdle = AnimationNames.FRONT_IDLE;
        _idleVelocity = Vector2.Zero;
    }

    public override void PhysicsUpdate(float delta) => Enemy.Velocity = _idleVelocity;

    public override void Enter()
    {   
        var moveStateNode = Enemy
            .GetNode<Node>(StateNodeNames.StateMachine)
            .GetChildren()
            .FirstOrDefault(x => x.Name == StateNames.Wander);
        
        var currentDirection = MoveDirectionNames.DOWN;
        if(moveStateNode != null && moveStateNode is IMovableState movableState)
        {
            currentDirection = movableState.GetCurrentDirection() ?? currentDirection;
        }

        _currentIdle = _idlesDict[currentDirection];
        if(AnimationPlayer != null)
        {
            AnimationPlayer.Play(_currentIdle);
        }

        if(_fsmIdleTimer != null)
        {
            _fsmIdleTimer.WaitTime = _random.Next(MinTimerValue, MaxTimerValue);
            _fsmIdleTimer.Start(); 
        }
    }

    public override void Exit() => _fsmIdleTimer.Stop();

    public void OnFsmIdleTimerTimeout()
    {
        StateMachine.TransitionTo(StateNames.Wander);
    }
}