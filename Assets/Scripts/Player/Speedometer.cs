using TMPro;
using UnityEngine;
using UnityEngine.VFX;

public class Speedometer : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public VisualEffect speedEffect;
    public TMP_Text speedText;

    public float displayMultiplier = 100f;

    public float radiusAtZero = 3f;
    public float radiusMin = 0.6f;
    public float speedThreshold = 1500f;
    public float halfSpeed = 6000f;

    void Update()
    {
        if (playerMovement == null || speedText == null)
        {
            return;
        }

        if(playerMovement.HorizontalSpeed < 0.1f)
        {
            speedEffect.enabled = false;
            return;
        }
        else
        {
            speedEffect.enabled = true;
        }

        float speed = playerMovement.HorizontalSpeed * displayMultiplier;

        float adjustedSpeed = Mathf.Max(0f, speed - speedThreshold);
        float k = Mathf.Log(2f) / halfSpeed;
        float t = Mathf.Exp(-k * adjustedSpeed);
        float radius = Mathf.Lerp(radiusMin, radiusAtZero, t);

        speedEffect.SetFloat("Radius", radius);
        speedText.text = Mathf.RoundToInt(speed).ToString();
    }
}
