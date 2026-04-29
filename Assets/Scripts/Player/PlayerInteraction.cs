using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    public Transform CameraTransform;
    public float InteractionDistance = 100f;
    public float detectionRadius = 0.5f;
    public LayerMask InteractionLayers;

    private RaycastHit raycastHit;
    private IInteractable currentInteractable;
    private IFocusable currentFocusable;

    private void OnEnable()
    {
        InputManager.Actions.Game.Interact.performed += OnInteract;
    }

    private void OnDisable()
    {
        InputManager.Actions.Game.Interact.performed -= OnInteract;
        ClearFocus();
    }

    private void Update()
    {
        CastRay();
    }

    private void CastRay()
    {
        if (Physics.SphereCast(
            CameraTransform.position,
            detectionRadius,
            CameraTransform.forward,
            out raycastHit,
            InteractionDistance,
            InteractionLayers))
        {
            raycastHit.collider.gameObject.TryGetComponent<IInteractable>(out currentInteractable);
            raycastHit.collider.gameObject.TryGetComponent<IFocusable>(out IFocusable newFocusable);

            if(newFocusable != currentFocusable)
            {
                ClearFocus();

                currentFocusable = newFocusable;

                if (currentFocusable != null)
                {
                    currentFocusable.Focus(gameObject);
                }
            }
        }
        else
        {
            ClearFocus();
        }
    }

    private void ClearFocus()
    {
        if(currentFocusable != null && (currentFocusable as Object) != null)
        {
            currentFocusable.UnFocus(gameObject);
        }
        currentFocusable = null;
        currentInteractable = null;
    }

    public void OnInteract(InputAction.CallbackContext obj)
    {
        if (currentInteractable != null)
        {
            if (currentInteractable.CanInteractWith(gameObject))
            {
                currentInteractable.Interact(gameObject);
            }
        }
    }
}
