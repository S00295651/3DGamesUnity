using TMPro;
using UnityEngine;

public class Speedometer : MonoBehaviour
{
    public PlayerMovement playerMovement;

    public TMP_Text speedText;

    public float displayMultiplier = 100f;

    void Update()
    {
        if (playerMovement == null || speedText == null) return;

        float speed = playerMovement.HorizontalSpeed * displayMultiplier;
        speedText.text = Mathf.RoundToInt(speed).ToString();
    }
}
