using TMPro;
using UnityEngine;
using UnityEngine.VFX;

public class Speedometer : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public VisualEffect speedEffect;
    public TMP_Text speedText;

    public float displayMultiplier = 100f;

    void Update()
    {
        if (playerMovement == null || speedText == null)
        {
            return;
        }

        float speed = playerMovement.HorizontalSpeed * displayMultiplier;
        float ajustedRadius = 1 / Mathf.Log(speed / 250, 12);
        float clampedRadius = Mathf.Clamp(ajustedRadius, 0.6f, 3f);
        speedEffect.SetFloat("Radius", clampedRadius);
        speedText.text = Mathf.RoundToInt(speed).ToString();
    }
}
