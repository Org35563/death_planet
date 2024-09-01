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
}