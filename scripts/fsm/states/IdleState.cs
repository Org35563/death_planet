using System;
using System.Collections.Generic;
using Godot;

public partial class IdleState : State, IMovableState
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

    private Timer _fsmIdleTimer;

    private Random _random;

    private string _currentIdle;

    private string _currentDirection;

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
        _currentDirection = MoveDirectionNames.DOWN;
        ChaseArea.BodyEntered += OnBodyEntered;
        ChaseArea.BodyExited += OnBodyExited;
    }

    public override void PhysicsUpdate(float delta) => Character.Velocity = Vector2.Zero;

    public override void Enter()
    {   
        _currentIdle = _idlesDict[_currentDirection];
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

    public void OnFsmIdleTimerTimeout() => StateMachine.TransitionTo(StateNames.Wander);

    public void OnBodyEntered(Node2D body)
    {
        if(Global.IsGameUnitType<IHero>(body))
        {
            Exit();
            var chaseNode = GetChaseNode();
            if(chaseNode != null && chaseNode is IInteractableState interactableState)
            {
                interactableState.SetInteractable((CharacterBody2D)body);
            }

            if(chaseNode != null && chaseNode is IMovableState movableState)
            {
                movableState.SetCurrentDirection(_currentDirection);
            }

            StateMachine.TransitionTo(StateNames.Chase);
        }
    }

    public void OnBodyExited(Node2D body)
    {
        Exit();
        var chaseNode = GetChaseNode();
        if(chaseNode != null && chaseNode is IMovableState movableState)
        {
            _currentDirection = movableState.GetCurrentDirection();
        }

        StateMachine.TransitionTo(StateNames.Idle);
    }

    public string GetCurrentDirection() => _currentDirection;

    public void SetCurrentDirection(string direction) => _currentDirection = direction;

    private Node GetChaseNode() => Global.GetNodeByName(Character, StateNodeNames.StateMachine, StateNames.Chase);
}