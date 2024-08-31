using Godot;

public partial class DeathState : State
{
    [Export]
    public AnimatedSprite2D AnimationPlayer;

    [Export]
    public CharacterBody2D Character;

    private Timer _fsmDeathTimer;

    private bool _alreadyDead;

    public override void _Ready()
    {
        _fsmDeathTimer = GetNode<Timer>(StateNodeNames.DeathTimer);
        _alreadyDead = false;
    }

    public override void Enter()
    {      
        if(_fsmDeathTimer != null && !_alreadyDead)
        {
            AnimationPlayer.Play(AnimationNames.DEATH);
            _fsmDeathTimer.Start();
            _alreadyDead = true;
        }
    }

    public void OnFsmDeathTimerTimeout()
    {
        Character.QueueFree();
        Character.Dispose();
    }
}
