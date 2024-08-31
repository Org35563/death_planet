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
        if(RayCast != null)
        {
            SetRayCastDirection();

            if(RayCast.IsColliding())
            {
                GenerateRandomCoords();
            }          
        }
        
        if(IsOnArea(_moveDirection * _wanderingSpeed))
        {
            Character.Velocity = _moveDirection * _wanderingSpeed;
            Character.MoveAndSlide();
        }
    }

    public override void Enter()
    {  
        StateMachine.TryTransitionToDeath(Character);

        GenerateRandomCoords();

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

    private void GenerateRandomCoords()
    {
        while(true)
        {
            _moveDirection = new Vector2(_random.Next(-1, 2), _random.Next(-1, 2)).Normalized();
            if(_moveDirection.X != 0 || _moveDirection.Y != 0)
            {
                break;
            }
        }
    }

    // TODO: отрефакторить
    private void SetRayCastDirection()
    {
        var x = 15;
        var y = 15;
        var velocity = Character.Velocity;
        if (velocity.X != 0 && velocity.Y != 0)
        {
            if (velocity.X > 0 && velocity.Y > 0)
            {
                // print("Движется по диагонали вправо вниз");
                x *= 1;
                y *= 1;
            }
            else if(velocity.X > 0 && velocity.Y < 0)
            {
                // print("Движется по диагонали вправо вверх");
                x *= 1;
                y *= -1;
            }
            else if(velocity.X < 0 && velocity.Y > 0)
            {
                // print("Движется по диагонали влево вниз");
                x *= -1;
                y *= 1;
            }            
            else if(velocity.X < 0 && velocity.Y < 0)
            {
                // print("Движется по диагонали влево вверх");
                x *= -1;
                y *= -1;
            }  
        }        
        else
        {
            if(velocity.X > 0)
            {
                // print("Движется вправо");
                x *= 1;
                y *= 0;
            }
            else if(velocity.X < 0)
            {
                // print("Движется влево");
                x *= -1;
                y *= 0;
            }   
            else if(velocity.Y > 0)
            {
                // print("Движется вниз");
                x *= 0;
                y *= 1;
            }          
            else if(velocity.Y < 0)
            {
                // print("Движется вверх");
                x *= 0;
                y *= -1;
            }             
        }

        RayCast.TargetPosition = new Vector2(x, y);
    }

    private bool IsOnArea(Vector2 positionToCheck)
    {
        var node1 = GetParent();
        var node2 = node1.GetParent();
        var node3 = node2.GetParent();

        var area = node3.GetNode<Area2D>("slime_area");

        if(area != null)
        {
            var areaBounds = GetWorldBoundaries(area, "slime_area_collision_shape");
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