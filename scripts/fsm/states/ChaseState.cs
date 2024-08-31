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

    private string _currentDirection;

    private bool _isCharacterStopped;

    private Node _attackNode;

    private float _chasingSpeed;

    private Dictionary<string, string> _chasesDict = new ()
    {
        { DirectionNames.RIGHT, AnimationNames.SIDE_WALK },
        { DirectionNames.LEFT, AnimationNames.SIDE_WALK },
        { DirectionNames.DOWN, AnimationNames.FRONT_WALK },
        { DirectionNames.UP, AnimationNames.BACK_WALK },  
    };

    private Dictionary<string, Vector2> _chasesRayCastDict = new ()
    {
        { DirectionNames.RIGHT, new Vector2(-15, 0) },
        { DirectionNames.LEFT, new Vector2(15 ,0) },
        { DirectionNames.DOWN, new Vector2(0, 15) },
        { DirectionNames.UP, new Vector2(0, -15) },  
    };

    public override void _Ready()
    {
        if(Character is ICombatCreature combatCreature)
        {
            _chasingSpeed = combatCreature.GetMoveSpeed();
        }

        AttackArea.BodyEntered += OnAttackAreaBodyEntered;
        AttackArea.BodyExited += OnAttackAreaBodyExited;
        _attackNode = Global.GetNodeByName(Character, StateNodeNames.StateMachine, StateNames.Attack);
    }

    public override void Enter()
    {
        StateMachine.TryTransitionToDeath(Character);

        StateMachine.TryTransitionToWander(_chasingObject);
    }

    public override void PhysicsUpdate(float delta)
    {
        var direction = (_chasingObject.Position - Character.Position).Normalized();
        Character.Position += direction * _chasingSpeed * delta;

        if (Mathf.Abs(direction.X) > Mathf.Abs(direction.Y))
        {
            AnimationPlayer.FlipH = direction.X < 0;
            _currentDirection = AnimationPlayer.FlipH ? DirectionNames.RIGHT : DirectionNames.LEFT;
        }
        else if (direction.Y < 0)
        {
            _currentDirection = DirectionNames.UP;
        }
        else
        {
            _currentDirection = DirectionNames.DOWN;
        }

        if(AnimationPlayer != null)
        {
            AnimationPlayer.Play(_chasesDict[_currentDirection]);
        }  

        if(RayCast != null)
        {
            RayCast.TargetPosition = _chasesRayCastDict[_currentDirection];
        }

        Character.MoveAndSlide();
    }

    public CharacterBody2D GetInteractableObject() => _chasingObject;

    public void SetInteractableObject(CharacterBody2D interactable) => _chasingObject = interactable;

    public string GetCurrentDirection() => _currentDirection;

    public void SetCurrentDirection(string direction) => _currentDirection = direction;

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
}