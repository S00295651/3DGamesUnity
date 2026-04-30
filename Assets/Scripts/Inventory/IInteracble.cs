using UnityEngine;

public interface IInteractable
{
    public bool CanInteractWith(GameObject interactor);
    public void Interact(GameObject interactor);
}
