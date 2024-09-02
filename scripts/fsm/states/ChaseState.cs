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
    }

    public override void Enter()
    {
        GD.Print("on chase state");
        StateMachine.TryTransitionToDeath(Character);
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
}