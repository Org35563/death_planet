using Godot;

public static class StateMachineExtensions
{
    public static void TryTransitionToDeath(this StateMachine stateMachine, CharacterBody2D character)
    {
        if(!Global.IsCreatureAlive(character))
        {
            stateMachine.TransitionTo(StateNames.Death);
        }
    }

    public static void TryTransitionToWander(this StateMachine stateMachine, CharacterBody2D character)
    {
        if(character == null || !Global.IsCreatureAlive(character))
        {
            stateMachine.TransitionTo(StateNames.Wander);
        }
    }
}