using NUnit;
using UnityEngine;

public class Item : MonoBehaviour,IInteractable
{
    [SerializeField] private string itemName;
    public ParticleSystem pickupEffect;
    public ItemData itemToGive;

    public bool CanInteractWith(GameObject interactor)
    {
        return true; 
    }

    public void Interact(GameObject interactor)
    {
        Debug.Log($"try to pickup {gameObject.name} by {interactor.name}");

        if (interactor.TryGetComponent<Inventory>(out Inventory inventory))
        {
            inventory.AddItem(itemToGive);

            pickupEffect.transform.parent = null;
            pickupEffect.Play();

            var main = pickupEffect.main;
            main.stopAction = ParticleSystemStopAction.Destroy;

            Destroy(gameObject);
        }
    }
}
