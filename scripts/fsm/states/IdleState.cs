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

    private Node _chaseNode;

    private string _currentIdle;

    private string _currentDirection;

    private Dictionary<string, string> _idlesDict = new ()
    {
        { DirectionNames.RIGHT, AnimationNames.SIDE_IDLE },
        { DirectionNames.LEFT, AnimationNames.SIDE_IDLE },
        { DirectionNames.DOWN, AnimationNames.FRONT_IDLE },
        { DirectionNames.UP, AnimationNames.BACK_IDLE },  
    };

    public override void _Ready()
    {
        _fsmIdleTimer = GetNode<Timer>(StateNodeNames.IdleTimer);
        _random = new Random();
        _chaseNode = Global.GetNodeByName(Character, StateNodeNames.StateMachine, StateNames.Chase);
        _currentIdle = AnimationNames.FRONT_IDLE;
        _currentDirection = DirectionNames.DOWN;
        ChaseArea.BodyEntered += OnChaseAreaBodyEntered;
        ChaseArea.BodyExited += OnChaseAreaBodyExited;
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

    public void OnChaseAreaBodyEntered(Node2D body)
    {
        if(Global.IsCharacterAlive(body))
        {
            if(_chaseNode != null && _chaseNode is IInteractableState<CharacterBody2D> interactableState)
            {
                interactableState.SetInteractableObject((CharacterBody2D)body);
            }

            if(_chaseNode != null && _chaseNode is IMovableState movableState)
            {
                movableState.SetCurrentDirection(_currentDirection);
            }

            Exit();
            StateMachine.TransitionTo(StateNames.Chase);
        }
        else
        {
            Exit();
            StateMachine.TransitionTo(StateNames.Wander);
        }
    }

    public void OnChaseAreaBodyExited(Node2D body)
    {
        if(_chaseNode != null && _chaseNode is IMovableState movableState)
        {
            _currentDirection = movableState.GetCurrentDirection();
        }

        Exit();
        StateMachine.TransitionTo(StateNames.Wander);
    }

    public string GetCurrentDirection() => _currentDirection;

    public void SetCurrentDirection(string direction) => _currentDirection = direction;
}