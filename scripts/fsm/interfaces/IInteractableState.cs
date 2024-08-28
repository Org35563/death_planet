using Godot;

public interface IInteractableState
{
    public CharacterBody2D GetInteractable();

    public void SetInteractable(CharacterBody2D interactable);
}