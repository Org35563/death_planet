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
    public float MoveSpeed;

    private CharacterBody2D _chasingObject;

    private string _currentDirection;

    private bool _isCharacterStopped;

    private Node _attackNode;

    private Dictionary<string, string> _chasesDict = new ()
    {
        { DirectionNames.RIGHT, AnimationNames.SIDE_WALK },
        { DirectionNames.LEFT, AnimationNames.SIDE_WALK },
        { DirectionNames.DOWN, AnimationNames.FRONT_WALK },
        { DirectionNames.UP, AnimationNames.BACK_WALK },  
    };

    public override void _Ready()
    {
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
        Character.Position += direction * MoveSpeed * delta;

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

        Character.MoveAndSlide();
    }

    public CharacterBody2D GetInteractableObject() => _chasingObject;

    public void SetInteractableObject(CharacterBody2D interactable) => _chasingObject = interactable;

    public string GetCurrentDirection() => _currentDirection;

    public void SetCurrentDirection(string direction) => _currentDirection = direction;

    public void OnAttackAreaBodyEntered(Node2D body)
    {
        if(Global.IsCreatureAlive(Character) == false)
        {
            return;
        }

        if(body is ILivingCreature character)
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
        if(Global.IsCreatureAlive(Character) == false)
        {
            return;
        }

        if(body is ILivingCreature && (CharacterBody2D)body == _chasingObject)
        {
            StateMachine.TransitionTo(StateNames.Chase);
        }
    }
}