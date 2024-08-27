using System.Collections.Generic;
using Godot;

public partial class StateMachine : Node
{
    [Export]
    public NodePath InitialState;

    private Dictionary<string, State> _states;

    private State _currentState;

    public override void _Ready()
    {
        _states = new Dictionary<string, State>();
        foreach(Node node in GetChildren())
        {
            if(node is State s)
            {
                _states.Add(node.Name, s);
                s.StateMachine = this;
                s.Exit(); // сброс состояния
            }
        }

        _currentState = GetNode<State>(InitialState);
        _currentState.Enter();
    }

    public override void _Process(double delta)
    {
        _currentState.Update((float) delta);
    }

    public override void _PhysicsProcess(double delta)
    {
        _currentState.PhysicsUpdate((float) delta);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        _currentState.HandleInput(@event);
    }

    public void TransitionTo(string key)
    {
        if(!_states.ContainsKey(key) || _currentState == _states[key])
        {
            return;
        }

        _currentState.Exit();
        _currentState = _states[key];
        _currentState.Enter();
    }
}
