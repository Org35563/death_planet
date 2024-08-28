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
                s.Exit();
            }
        }

        _currentState = GetNode<State>(InitialState);
        _currentState.Enter();
    }

    public override void _PhysicsProcess(double delta)
    {
        _currentState.PhysicsUpdate((float) delta);
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
