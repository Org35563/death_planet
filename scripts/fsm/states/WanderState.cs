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
    public int MinTimerValue;

    [Export]
    public int MaxTimerValue;

    private Timer _fsmWanderTimer;

    private Random _random;

    private Vector2 _currentMoveDirection;

    private string _currentMoveDirectionName;

    private Node _idleNode;

    private Node _chaseNode;

    private float _wanderingSpeed;

    private float _rayCastLength = 20.0f;

    private Area2D _validMoveArea;

    public override void _Ready()
    {
        if(Character is ICombatCreature combatCreature)
        {
            _wanderingSpeed = combatCreature.GetMoveSpeed();
        }
        
        _validMoveArea = GetNode("../../../").GetNode<Area2D>("slime_area");

        _fsmWanderTimer = GetNode<Timer>(StateNodeNames.WanderTimer);
        _random = new Random();
        _idleNode = Global.GetNodeByName(Character, StateNodeNames.StateMachine, StateNames.Idle);
        _chaseNode = Global.GetNodeByName(Character, StateNodeNames.StateMachine, StateNames.Chase);
        ChaseArea.BodyEntered += OnChaseCollision;
    }

    public override void PhysicsUpdate(float delta)
    {
        CheckCollisionsWithBarriers();

        var newPosition = _currentMoveDirection * _wanderingSpeed * delta;
        if(IsCharacterOnValidArea(newPosition))
        {
            // GD.Print("valid area!");
            Character.Position += newPosition;
        }
        else
        {
            // GD.Print("wrong area!");
            TransitionToChaseToValidArea();
        }

        Character.MoveAndSlide();
    }

    public override void Enter()
    {  
        // GD.Print("on wander state");

        StateMachine.TryTransitionToDeath(Character);

        _currentMoveDirection = GetNewRandomMoveDirection();

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

    public void OnFsmWanderTimerTimeout() => TransitionToIdle();

    public string GetCurrentDirection() => _currentMoveDirectionName;

    public void SetCurrentDirection(string direction) => _currentMoveDirectionName = direction;

    public void OnChaseCollision(Node2D body)
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
                movableState.SetCurrentDirection(_currentMoveDirectionName);
            }

            Exit();
            StateMachine.TransitionTo(StateNames.Chase);
        }
        else
        {
            Exit();
            StateMachine.TransitionTo(StateNames.Idle);
        }
    }

    private Vector2 GetNewRandomMoveDirection()
    {
        var newMoveDirection = Vector2.Zero;
        while(newMoveDirection.X == 0 || newMoveDirection.Y == 0)
        {
            newMoveDirection = new Vector2(_random.Next(-1, 2), _random.Next(-1, 2)).Normalized();
        }

        return newMoveDirection;
    }

    private void SetNewRayCastDirection(Vector2 moveDirection)
    {
        var velocity = moveDirection * _wanderingSpeed;

        int xDirection = velocity.X > 0 ? 1 : (velocity.X < 0 ? -1 : 0);
        int yDirection = velocity.Y > 0 ? 1 : (velocity.Y < 0 ? -1 : 0);

        Vector2 targetPosition = new (_rayCastLength * xDirection, _rayCastLength * yDirection);

        RayCast.TargetPosition = targetPosition;
    }

    private void TransitionToIdle()
    {
        if(_idleNode != null && _idleNode is IMovableState movableState)
        {
            movableState.SetCurrentDirection(_currentMoveDirectionName);
        }

        Exit();
        StateMachine.TransitionTo(StateNames.Idle);
    }

    private void TransitionToChaseToValidArea()
    {
        if(_chaseNode != null && _chaseNode is IInteractableState<CharacterBody2D> interactableState)
        {
            interactableState.SetInteractableObject(new CharacterBody2D(){ Name = "area2d", Position = _validMoveArea.Position });
        }

        if(_chaseNode != null && _chaseNode is IMovableState movableState)
        {
            movableState.SetCurrentDirection(_currentMoveDirectionName);
        }

        Exit();
        StateMachine.TransitionTo(StateNames.Chase);
    }

    private void CheckCollisionsWithBarriers()
    {
        if(RayCast != null)
        {
            SetNewRayCastDirection(_currentMoveDirection);

            if(RayCast.IsColliding())
            {
                TransitionToIdle();
            }         
        }
    }

    private bool IsCharacterOnValidArea(Vector2 positionToCheck)
    {
        if(_validMoveArea != null)
        {
            var areaBounds = GetWorldBoundaries(_validMoveArea, "slime_area_collision_shape");
            if (areaBounds.HasPoint(Character.Position + positionToCheck))
            {
                return true;              
            }
        }

        return false;
    }

    private Rect2 GetWorldBoundaries(Area2D area, string areaCollisionShapeName)
    {
        // Получаем позицию и размер Area2D
        var child = area.GetNode<CollisionShape2D>(areaCollisionShapeName);
        Vector2 position = child.Position;
        Vector2 size = child.Shape.GetRect().Size;

        // Создаем Rect2, представляющий границы Area2D
        return new Rect2(position - size / 2, size);
    }
}