using System.Collections.Generic;
using Godot;

public partial class ChaseState : State, IInteractableState, IMovableState
{
    [Export]
    public AnimatedSprite2D _animationPlayer;

    [Export]
    public CharacterBody2D _enemy;

    [Export]
    public float MoveSpeed;

    private CharacterBody2D _chasingObject;

    private string _currentDirection;

    private Dictionary<string, string> _chasesDict = new ()
    {
        { MoveDirectionNames.RIGHT, AnimationNames.SIDE_WALK },
        { MoveDirectionNames.LEFT, AnimationNames.SIDE_WALK },
        { MoveDirectionNames.DOWN, AnimationNames.FRONT_WALK },
        { MoveDirectionNames.UP, AnimationNames.BACK_WALK },  
    };

    public override void PhysicsUpdate(float delta)
    {
        var direction = (_chasingObject.Position - _enemy.Position).Normalized();
        _enemy.Position += direction * MoveSpeed * delta;

        if (Mathf.Abs(direction.X) > Mathf.Abs(direction.Y))
        {
            _animationPlayer.FlipH = direction.X < 0;

            _currentDirection = _animationPlayer.FlipH ? MoveDirectionNames.RIGHT : MoveDirectionNames.LEFT;
        }
        else
        {
            if (direction.Y < 0)
            {
                _currentDirection = MoveDirectionNames.UP;
            }
            else
            {
                _currentDirection = MoveDirectionNames.DOWN;
            }
        }     

        if(_animationPlayer != null)
        {
            _animationPlayer.Play(_chasesDict[_currentDirection]);
        }  

        _enemy.MoveAndSlide();
    }

    public CharacterBody2D GetInteractable() => _chasingObject;

    public void SetInteractable(CharacterBody2D interactable) => _chasingObject = interactable;

    public string GetCurrentDirection() => _currentDirection;

    public void SetCurrentDirection(string direction) => _currentDirection = direction;
}