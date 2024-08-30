using System.Collections.Generic;
using Godot;

public partial class AttackState : State, IInteractableState<ICharacter>, IAttackableState
{
    [Export]
    public AnimatedSprite2D AnimationPlayer;

    [Export]
    public CharacterBody2D Character;

    [Export]
    public int AttackPower;

    private Timer _attackCooldownTimer;

    private ICharacter _attackingObject;

    private bool _attackCooldownFinished;

    private string _attackDirection;

    private bool _isAttacking;

    private Dictionary<string, string> _attackDict = new ()
    {
        { DirectionNames.RIGHT, AnimationNames.SIDE_ATTACK },
        { DirectionNames.LEFT, AnimationNames.SIDE_ATTACK },
        { DirectionNames.DOWN, AnimationNames.FRONT_ATTACK },
        { DirectionNames.UP, AnimationNames.BACK_ATTACK },  
    };

    public override void _Ready()
    {
        _attackCooldownTimer = GetNode<Timer>(StateNodeNames.AttackCooldownTimer);
        _attackDirection = DirectionNames.DOWN;
    }

    public override void Enter()
    {
        _attackCooldownFinished = true;
        _isAttacking = true;
    }

    public override void _PhysicsProcess(double delta)
    {
        if(_attackCooldownFinished && _attackingObject != null && _attackingObject.IsAlive() && _isAttacking)
        {
            SetAttackDirection();
            AnimationPlayer.Play(_attackDict[_attackDirection]);

            _attackingObject.SetHealth(_attackingObject.GetHealth() - AttackPower);
            _attackCooldownFinished = false;
            _attackCooldownTimer.Start();
            GD.Print($"Attacking character health: {_attackingObject.GetHealth()}");
            if(_attackingObject.IsAlive() == false)
            {
                _attackingObject = null;
            }
        }
    }

    public override void Exit() => _attackCooldownTimer.Stop();

    public void OnFsmAttackCooldownTimerTimeout()
    {
        _attackCooldownTimer.Stop();
        if(_attackingObject.IsAlive())
        {
            _attackCooldownFinished = true;
        }
        else
        {
            Exit();
            StateMachine.TransitionTo(StateNames.Wander);
        }
    }

    public ICharacter GetInteractableObject()  => _attackingObject;

    public void SetInteractableObject(ICharacter interactableObject)  => _attackingObject = interactableObject;

    private void SetAttackDirection()
    {
        var attackingObjectPosition = _attackingObject.GetCurrentPosition();

        float deltaX = attackingObjectPosition.X - Character.Position.X;
        float deltaY = attackingObjectPosition.Y - Character.Position.Y;

        if (Mathf.Abs(deltaX) > Mathf.Abs(deltaY))
        {
            AnimationPlayer.FlipH = deltaX < 0;
            _attackDirection = AnimationPlayer.FlipH ? DirectionNames.RIGHT : DirectionNames.LEFT;
        }
        else
        {
            _attackDirection = deltaY < 0 ? DirectionNames.UP : DirectionNames.DOWN;
        }      
    }

    public bool GetIsAttacking() => _isAttacking;

    public void SetIsAttacking(bool isAttacking) => _isAttacking = isAttacking;
}
