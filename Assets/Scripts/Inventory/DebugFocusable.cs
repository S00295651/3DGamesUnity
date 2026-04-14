using UnityEngine;
using UnityEngine.UI;

public class DebugFocusable : MonoBehaviour, IFocusable
{
    public Color OutlineColor = Color.white;
    public float OutlineWidth = 5.0f;
    private Color originalColor;
    private Material material; 
    private Outline outline;

    private void Awake()
    {
        outline = GetComponent<Outline>();

        if (outline != null)
        {
            outline.OutlineColor = OutlineColor;
            outline.OutlineWidth = 5.0f; // Adjust width as needed
            outline.enabled = false; // Start with outline off
        }
        else
        {
            Debug.LogError("No Outline component found on " + gameObject.name + ". Please add one.");
        }
    }

    public void Focus(GameObject interactor)
    {
        if (outline != null)
        {
            outline.enabled = true;
        }
    }

    public void UnFocus(GameObject interactor)
    {
        if (outline != null)
        {
            outline.enabled = false;
        }
    }
}
