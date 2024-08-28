using Godot;

public partial class State : Node
{
    public StateMachine StateMachine { get ; set; }

    public virtual void Enter() {}

    public virtual void Exit() {}

    public virtual void PhysicsUpdate(float delta) {}
}