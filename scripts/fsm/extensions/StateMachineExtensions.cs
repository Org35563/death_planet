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

    public static void TransitionToIdle(this StateMachine stateMachine, Node idleNode, string currentMoveDirectionName)
    {
        if(idleNode != null && idleNode is IMovableState movableState)
        {
            movableState.SetCurrentDirection(currentMoveDirectionName);
        }

        stateMachine.TransitionTo(StateNames.Idle);
    }
}