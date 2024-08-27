using System;
using Godot;

public partial class IdleState : State
{
    [Export]
    public AnimatedSprite2D _animationPlayer;

    [Export]
    public CharacterBody2D _enemy;

    private Timer _fsmIdleTimer;

    private Random _random;

    private string _idleName;

    public override void _Ready()
    {
        _fsmIdleTimer = GetNode<Timer>("fsm_idle_timer");
        _random = new Random();
        _idleName = AnimationNames.FRONT_IDLE;
    }

    public override void PhysicsUpdate(float delta)
    {
        _enemy.Velocity = new Vector2();
    }

    public override void Enter()
    {       
        _idleName = (string) _enemy.GetMeta("MoveDirection");
        GD.Print(_idleName);
        _fsmIdleTimer.WaitTime = _random.Next(1, 4);
        if(_animationPlayer != null)
        {
            _animationPlayer.Play(_idleName);
        }

        _fsmIdleTimer.Start();  
    }

    public override void Exit()
    {
        _fsmIdleTimer.Stop();
    }

    public void OnFsmIdleTimerTimeout()
    {
        StateMachine.TransitionTo("move");
    }
}