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
    }

    public override void PhysicsUpdate(float delta)
    {
        if(_chasingObject is ICharacter character && !character.IsAlive())
        {
            StateMachine.TransitionTo(StateNames.Idle);
        }

        var direction = (_chasingObject.Position - Character.Position).Normalized();
        Character.Position += direction * MoveSpeed * delta;

        if (Mathf.Abs(direction.X) > Mathf.Abs(direction.Y))
        {
            AnimationPlayer.FlipH = direction.X < 0;

            _currentDirection = AnimationPlayer.FlipH ? DirectionNames.RIGHT : DirectionNames.LEFT;
        }
        else
        {
            if (direction.Y < 0)
            {
                _currentDirection = DirectionNames.UP;
            }
            else
            {
                _currentDirection = DirectionNames.DOWN;
            }
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
        if(body is ICharacter character)
        {
            var attackNode = Global.GetNodeByName(Character, StateNodeNames.StateMachine, StateNames.Attack);
            if(attackNode != null && attackNode is IInteractableState<ICharacter> interactableState)
            {
                interactableState.SetInteractableObject(character);
            }

            if(attackNode != null && attackNode is IAttackableState attackableState)
            {
                attackableState.SetIsAttacking(true);
            }
            
            Exit();
            StateMachine.TransitionTo(StateNames.Attack);
        }     
    }

    public void OnAttackAreaBodyExited(Node2D body)
    {
        if(body is ICharacter && (CharacterBody2D)body == _chasingObject)
        {
            var attackNode = Global.GetNodeByName(Character, StateNodeNames.StateMachine, StateNames.Attack);
            if(attackNode != null && attackNode is IAttackableState attackableState)
            {
                attackableState.SetIsAttacking(false);
            }

            Exit();
            StateMachine.TransitionTo(StateNames.Chase);
        }
    }
}