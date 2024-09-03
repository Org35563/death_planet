using Godot;

public partial class AttackState : State, IInteractableState<ILivingCreature>
{
    [Export]
    public AnimatedSprite2D AnimationPlayer;

    [Export]
    public CharacterBody2D Attacker;

    private Timer _attackCooldownTimer;

    private ILivingCreature _attackedObject;

    private bool _attackCooldownFinished;

    private int _attackPower;

    public override void _Ready()
    {
        if(Attacker is ICombatCreature combatCreature)
        {
            _attackPower = combatCreature.GetAttackPower();
        }

        _attackCooldownTimer = GetNode<Timer>(StateNodeNames.AttackCooldownTimer);
        _attackCooldownFinished = true;
    }

    public override void Enter()
    {
        StateMachine.TryTransitionToDeath(Attacker);
    }

    public override void PhysicsUpdate(float delta)
    {
        if(_attackCooldownFinished && _attackedObject != null && _attackedObject.IsAlive())
        {
            AnimationPlayer.PlayAttackAnimation(Attacker.Position, _attackedObject.GetCurrentPosition());

            _attackedObject.SetHealth(_attackedObject.GetHealth() - _attackPower);
            _attackCooldownFinished = false;
            _attackCooldownTimer.Start();
            GD.Print($"Attacking character health: {_attackedObject.GetHealth()}");
        }
    }

    public void OnFsmAttackCooldownTimerTimeout()
    {
        _attackCooldownTimer.Stop();

        StateMachine.TryTransitionToDeath(Attacker);

        if(_attackedObject.IsAlive())
        {
            _attackCooldownFinished = true;
        }
        else
        {
            _attackedObject = null;
            StateMachine.TransitionTo(StateNames.Wander);
        }
    }

    public ILivingCreature GetInteractableObject()  => _attackedObject;

    public void SetInteractableObject(ILivingCreature interactableObject)  => _attackedObject = interactableObject;
}
