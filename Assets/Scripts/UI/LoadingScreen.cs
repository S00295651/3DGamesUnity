using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadingScreen : MonoBehaviour
{
    public static LoadingScreen Instance;

    [Header("References")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI tipText;

    [Header("Settings")]
    [SerializeField] private int chunksRequiredToLoad = 4;
    [SerializeField] private float fadeDuration = 1f;

    private int chunksLoaded = 0;
    private bool isDone = false;

    void Awake()
    {
        Instance = this;
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }

    // Called by EndlessTerrain when chunks ready
    public void OnChunkReady()
    {
        if (isDone) return;

        chunksLoaded++;
        if (chunksLoaded >= chunksRequiredToLoad)
        {
            isDone = true;
            StartCoroutine(FadeOut());
        }
    }

    private IEnumerator FadeOut()
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        gameObject.SetActive(false);
    }
}
