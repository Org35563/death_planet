using System.Collections.Generic;
using Godot;

public partial class AttackState : State, IInteractableState<ILivingCreature>
{
    [Export]
    public AnimatedSprite2D AnimationPlayer;

    [Export]
    public CharacterBody2D Character;

    [Export]
    public int AttackPower;

    private Timer _attackCooldownTimer;

    private ILivingCreature _attackingObject;

    private bool _attackCooldownFinished;

    private string _attackDirection;

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
        _attackCooldownFinished = true;
    }

    public override void Enter()
    {
        StateMachine.TryTransitionToDeath(Character);
    }

    public override void PhysicsUpdate(float delta)
    {
        if(_attackCooldownFinished && _attackingObject != null && _attackingObject.IsAlive())
        {
            SetAttackDirection();
            AnimationPlayer.Play(_attackDict[_attackDirection]);

            _attackingObject.SetHealth(_attackingObject.GetHealth() - AttackPower);
            _attackCooldownFinished = false;
            _attackCooldownTimer.Start();
            GD.Print($"Attacking character health: {_attackingObject.GetHealth()}");
        }
    }

    public void OnFsmAttackCooldownTimerTimeout()
    {
        _attackCooldownTimer.Stop();

        StateMachine.TryTransitionToDeath(Character);

        if(_attackingObject.IsAlive())
        {
            _attackCooldownFinished = true;
        }
        else
        {
            _attackingObject = null;
            StateMachine.TransitionTo(StateNames.Wander);
        }
    }

    public ILivingCreature GetInteractableObject()  => _attackingObject;

    public void SetInteractableObject(ILivingCreature interactableObject)  => _attackingObject = interactableObject;

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
}
