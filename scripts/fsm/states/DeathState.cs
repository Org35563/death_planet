using Godot;

public partial class DeathState : State
{
    [Export]
    public AnimatedSprite2D AnimationPlayer;

    [Export]
    public CharacterBody2D Character;

    private Timer _fsmDeathTimer;


    public override void _Ready()
    {
        _fsmDeathTimer = GetNode<Timer>(StateNodeNames.DeathTimer);
    }

    public override void Enter()
    {
        GD.Print($"{Character.Name} DEATH!");
        AnimationPlayer.Play(AnimationNames.DEATH);
        if(_fsmDeathTimer != null)
        {
            _fsmDeathTimer.Start();
        }
    }

    public void OnFsmDeathTimerTimeout()
    {
        _fsmDeathTimer.Stop();
        Character.QueueFree();
    }
}
