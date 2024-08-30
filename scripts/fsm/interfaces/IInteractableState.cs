public interface IInteractableState<T>
{
    public T GetInteractableObject();

    public void SetInteractableObject(T interactableObject);
}