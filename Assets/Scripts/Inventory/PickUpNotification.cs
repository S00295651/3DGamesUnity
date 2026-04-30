using System.Collections;
using UnityEngine;
using TMPro;

public class PickupNotification : MonoBehaviour
{
    public static PickupNotification Instance;

    [SerializeField] private TextMeshProUGUI notificationText;
    [SerializeField] private float displayDuration = 2f;
    [SerializeField] private float fadeDuration = 0.5f;

    private Coroutine currentCoroutine;

    void Awake()
    {
        Instance = this;
        notificationText.alpha = 0f;
    }

    public void Show(ItemData item)
    {
        string message = GetMessage(item);
        if (message == null) return;

        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        currentCoroutine = StartCoroutine(DisplayMessage(message));
    }

    private string GetMessage(ItemData item)
    {
        return item.affectedStat switch
        {
            StatType.groundSpeed => "+1% Ground Speed",
            StatType.airSpeed => "+1% Air Speed",
            StatType.JumpForce => "+1% Jump Force",
            _ => null
        };
    }

    private IEnumerator DisplayMessage(string message)
    {
        notificationText.text = message;

        yield return Fade(0f, 1f, fadeDuration);

        yield return new WaitForSeconds(displayDuration);

        yield return Fade(1f, 0f, fadeDuration);
    }

    private IEnumerator Fade(float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            notificationText.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }
        notificationText.alpha = to;
    }
}