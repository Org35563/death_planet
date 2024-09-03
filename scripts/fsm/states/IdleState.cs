using System;
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

    private string _currentDirection;

    public override void _Ready()
    {
        _fsmIdleTimer = GetNode<Timer>(StateNodeNames.IdleTimer);
        _random = new Random();
        _chaseNode = Global.GetNodeByName(Character, StateNodeNames.StateMachine, StateNames.Chase);
        _currentDirection = DirectionNames.DOWN;
        ChaseArea.BodyEntered += OnChaseAreaBodyEntered;
        ChaseArea.BodyExited += OnChaseAreaBodyExited;
    }

    public override void Enter()
    {   
        StateMachine.TryTransitionToDeath(Character);

        PlayIdleAnimation();

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
        if(Global.IsCreatureAlive(Character) == false)
        {
            return;
        }

        if(body != null && Global.IsCreatureAlive(body))
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
        if(Global.IsCreatureAlive(Character) == false)
        {
            return;
        }

        if(_chaseNode != null && _chaseNode is IInteractableState<CharacterBody2D> interactableState)
        {
            interactableState.SetInteractableObject(null);
        }

        if(_chaseNode != null && _chaseNode is IMovableState movableState)
        {
            _currentDirection = movableState.GetCurrentDirection();
        }

        Exit();
        StateMachine.TransitionTo(StateNames.Wander);
    }

    public string GetCurrentDirection() => _currentDirection;

    public void SetCurrentDirection(string direction) => _currentDirection = direction;

    private void PlayIdleAnimation()
    {
        var idleAnimationName = AnimationHelper.GetIdleAnimationNameByDirection(_currentDirection);
        if(AnimationPlayer != null)
        {
            AnimationPlayer.Play(idleAnimationName);
        }
    }
}