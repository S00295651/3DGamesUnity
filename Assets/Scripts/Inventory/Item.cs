using NUnit;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private string itemName;
    public ParticleSystem pickupEffect;
    public ItemData itemToGive;

    private Transform player;
    public float minAngle = -30f;
    public float maxAngle = 30f;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogWarning("no object with tag player found");
        }
    }

    public bool CanInteractWith(GameObject interactor)
    {
        return true; 
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        Debug.Log($"Trying to pick up {gameObject.name} by {other.name}");

        if (other.TryGetComponent<Inventory>(out Inventory inventory))
        {
            inventory.AddItem(itemToGive);

            if (pickupEffect != null)
            {
                pickupEffect.transform.parent = null;
                pickupEffect.Play();
                var main = pickupEffect.main;
                main.stopAction = ParticleSystemStopAction.Destroy;
            }

            Destroy(gameObject);
        }
    }

    void LateUpdate()
    {
        if (player == null) return;

        Vector3 direction = player.position - transform.position;

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        Vector3 angles = targetRotation.eulerAngles;
        float x = angles.x;
        if (x > 180) x -= 360;

        x = Mathf.Clamp(x, minAngle, maxAngle);

        transform.rotation = Quaternion.Euler(x, angles.y, 0);
    }
}
