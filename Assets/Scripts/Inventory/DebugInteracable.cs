using UnityEngine;

public class DebugInteracable : MonoBehaviour, IInteractable
{
    public bool CanInteractWith(GameObject interactor)
    {
        return true;
    }

    public void Interact(GameObject interactor)
    {
        Debug.Log($"Interacted with {gameObject.name} by {interactor.name}");
    }
}
