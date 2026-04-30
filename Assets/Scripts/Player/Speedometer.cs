using TMPro;
using UnityEngine;
using UnityEngine.VFX;

public class Speedometer : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public VisualEffect speedEffect;
    public TMP_Text speedText;
    public TMP_Text maxSpeedText;

    public float displayMultiplier = 100f;

    public float radiusAtZero = 3f;
    public float radiusMin = 0.6f;
    public float speedThreshold = 1500f;
    public float halfSpeed = 6000f;
    public float maxSpeed = -1f;

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

        if (maxSpeed < Mathf.RoundToInt(speed)) // save max speed
        {
            maxSpeed = Mathf.RoundToInt(speed);
        }

        string suffix = "Current Speed : ";
        speedText.text = suffix + Mathf.RoundToInt(speed).ToString();

        suffix = "Max Speed : ";
        maxSpeedText.text = suffix + Mathf.RoundToInt(maxSpeed).ToString();
    }
}
