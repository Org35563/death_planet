using Godot;

public partial class AttackState : State, IInteractableState<ILivingCreature>
{
    [Export]
    public AnimatedSprite2D AnimationPlayer;

    [Export]
    public CharacterBody2D Character;

    private Timer _attackCooldownTimer;

    private ILivingCreature _attackingObject;

    private bool _attackCooldownFinished;

    private int _attackPower;

    public override void _Ready()
    {
        if(Character is ICombatCreature combatCreature)
        {
            _attackPower = combatCreature.GetAttackPower();
        }

        _attackCooldownTimer = GetNode<Timer>(StateNodeNames.AttackCooldownTimer);
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
            PlayAttackAnimation();

            _attackingObject.SetHealth(_attackingObject.GetHealth() - _attackPower);
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

    private void PlayAttackAnimation()
    {
        var attackingObjectPosition = _attackingObject.GetCurrentPosition();

        float deltaX = attackingObjectPosition.X - Character.Position.X;
        float deltaY = attackingObjectPosition.Y - Character.Position.Y;

        var attackDirection = DirectionNames.NONE;
        if (Mathf.Abs(deltaX) > Mathf.Abs(deltaY))
        {
            AnimationPlayer.FlipH = deltaX < 0;
            attackDirection = AnimationPlayer.FlipH ? DirectionNames.RIGHT : DirectionNames.LEFT;
        }
        else
        {
            attackDirection = deltaY < 0 ? DirectionNames.UP : DirectionNames.DOWN;
        }

        var attackAnimationName = AnimationHelper.GetAttackAnimationNameByDirection(attackDirection);

        AnimationPlayer.Play(attackAnimationName);
    }
}
