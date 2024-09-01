using System.Collections.Generic;
using Godot;

public partial class ChaseState : State, IInteractableState<CharacterBody2D>, IMovableState
{
    [Export]
    public AnimatedSprite2D AnimationPlayer;

    [Export]
    public CharacterBody2D Character;

    [Export]
    public Area2D AttackArea;

    [Export]
    public RayCast2D RayCast;

    private CharacterBody2D _chasingObject;

    private string _currentMoveDirectionName;

    private Node _attackNode;

    private float _moveSpeed;

    private Dictionary<string, Vector2> _chasesRayCastDict;

    private float _rayCastLength = 20.0f;

    private Area2D _sourceArea;

    private bool _isOnSourceArea;

    public override void _Ready()
    {
        if(Character is ICombatCreature combatCreature)
        {
            _moveSpeed = combatCreature.GetMoveSpeed();
        }

        _chasesRayCastDict = new ()
        {
            { DirectionNames.RIGHT, new Vector2(-_rayCastLength, 0) },
            { DirectionNames.LEFT, new Vector2(_rayCastLength ,0) },
            { DirectionNames.DOWN, new Vector2(0, _rayCastLength) },
            { DirectionNames.UP, new Vector2(0, -_rayCastLength) },  
        };

        AttackArea.BodyEntered += OnAttackAreaBodyEntered;
        AttackArea.BodyExited += OnAttackAreaBodyExited;
        _attackNode = Global.GetNodeByName(Character, StateNodeNames.StateMachine, StateNames.Attack);

        _isOnSourceArea = true;
        _sourceArea = GetNode("../../../").GetNode<Area2D>("slime_area");

        _sourceArea.BodyEntered += OnSourceAreaBodyEntered;
        _sourceArea.BodyExited += OnSourceAreaBodyExited;
    }

    public override void Enter()
    {
        // GD.Print("on chase state");
        StateMachine.TryTransitionToDeath(Character);

        TryTransitionToWander();
    }

    public override void PhysicsUpdate(float delta)
    {
        var direction = (_chasingObject.Position - Character.Position).Normalized();

        Character.Position += direction * _moveSpeed * delta;

        var newDirectionName = Global.GetNewMoveDirectionName(direction);

        _currentMoveDirectionName = newDirectionName;

        AnimationPlayer.PlayMoveAnimation(newDirectionName, direction.X < 0);

        // TODO: Добавить избегание препятствий
        SetNewRayCastDirection(newDirectionName);

        Character.MoveAndSlide();
    }

    public CharacterBody2D GetInteractableObject() => _chasingObject;

    public void SetInteractableObject(CharacterBody2D interactable) => _chasingObject = interactable;

    public string GetCurrentDirection() => _currentMoveDirectionName;

    public void SetCurrentDirection(string direction) => _currentMoveDirectionName = direction;

    public void OnAttackAreaBodyEntered(Node2D body)
    {
        StateMachine.TryTransitionToDeath(Character);

        if(body != null && body is ILivingCreature character)
        {
            if(_attackNode != null && _attackNode is IInteractableState<ILivingCreature> interactableState)
            {
                interactableState.SetInteractableObject(character);
            }
            
            StateMachine.TransitionTo(StateNames.Attack);
        }     
    }

    public void OnAttackAreaBodyExited(Node2D body)
    {
        StateMachine.TryTransitionToDeath(Character);

        if(body != null && body is ILivingCreature && (CharacterBody2D)body == _chasingObject)
        {
            StateMachine.TransitionTo(StateNames.Chase);
        }
    }

    private void SetNewRayCastDirection(string currentDirectionName)
    {
        if(RayCast != null)
        {
            RayCast.TargetPosition = _chasesRayCastDict[currentDirectionName];
        }
    }

    public void TryTransitionToWander()
    {
        var collisionShape = _sourceArea.GetNode<CollisionShape2D>("slime_area_collision_shape");
        if(_chasingObject.Name == "area2d")
        {
            _chasingObject = new CharacterBody2D() { Position = new Vector2(collisionShape.Position.X, collisionShape.Position.Y) };

            GD.Print(_sourceArea.Name);

            return;
        }

        if(_isOnSourceArea && (_chasingObject == null || !Global.IsCreatureAlive(_chasingObject)))
        {
            GD.Print("TransitionTo(StateNames.Wander)");
            _chasingObject = null;
            StateMachine.TransitionTo(StateNames.Wander);
        }
    }

    public void OnSourceAreaBodyEntered(Node2D body)
    {
        if(body is CharacterBody2D characterBody  && characterBody == Character)
        {
            GD.Print("OnSourceAreaBodyEntered");
            _isOnSourceArea = true;
        }
    }

    public void OnSourceAreaBodyExited(Node2D body)
    {
        if(body is CharacterBody2D characterBody  && characterBody == Character)
        {   
            GD.Print("OnSourceAreaBodyExited");
            _isOnSourceArea = false;
        }
    }
}