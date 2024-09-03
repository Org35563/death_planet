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
    public RayCast2D RayCast;

    [Export]
    public string HomeAreaName;

    [Export]
    public int MinTimerValue;

    [Export]
    public int MaxTimerValue;

    private Timer _fsmWanderTimer;

    private Random _random;

    private Vector2 _currentMoveDirection;

    private string _currentMoveDirectionName;

    private Node _idleNode;

    private float _wanderingSpeed;

    private float _rayLength = 20.0f;

    private Area2D _validMoveArea;

    public override void _Ready()
    {
        if(Character is ICombatCreature combatCreature)
        {
            _wanderingSpeed = combatCreature.GetMoveSpeed();
        }
        
        _validMoveArea = GetNode("../../../").GetNode<Area2D>(HomeAreaName);

        _fsmWanderTimer = GetNode<Timer>(StateNodeNames.WanderTimer);
        _random = new Random();
        _idleNode = Global.GetNodeByName(Character, StateNodeNames.StateMachine, StateNames.Idle);
    }

    public override void PhysicsUpdate(float delta)
    {
        var expectedPosition  = _currentMoveDirection * _wanderingSpeed;
        if(RayCast.HasCollisionsWithBarriers(expectedPosition, _rayLength))
        {
            StateMachine.TransitionToIdle(_idleNode, _currentMoveDirectionName);
        }

        var newPosition = expectedPosition * delta;
        if(Global.IsCharacterOnArea(_validMoveArea, Character.Position + newPosition))
        {
            Character.Position += newPosition;
        }
        else
        {
            StateMachine.TransitionTo(StateNames.BackToHome);
        }

        Character.MoveAndSlide();
    }

    public override void Enter()
    {  
        StateMachine.TryTransitionToDeath(Character);

        _currentMoveDirection = _random.GetNewRandomMoveDirection();

        var newDirectionName = Global.GetNewMoveDirectionName(_currentMoveDirection);

        _currentMoveDirectionName = newDirectionName;

        AnimationPlayer.PlayMoveAnimation(newDirectionName, _currentMoveDirection.X < 0);

        if(_fsmWanderTimer != null)
        {
            _fsmWanderTimer.WaitTime = _random.Next(MinTimerValue, MaxTimerValue);
            _fsmWanderTimer.Start(); 
        }
    }

    public override void Exit() => _fsmWanderTimer.Stop();

    public void OnFsmWanderTimerTimeout() => StateMachine.TransitionToIdle(_idleNode, _currentMoveDirectionName);

    public string GetCurrentDirection() => _currentMoveDirectionName;

    public void SetCurrentDirection(string direction) => _currentMoveDirectionName = direction;
}