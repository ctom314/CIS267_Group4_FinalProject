using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeManager : MonoBehaviour
{
    public static FadeManager instance; // Singleton instance for global access
    private Image fadeImage;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else
        {
            Destroy(gameObject);
        }

        fadeImage = GetComponentInChildren<Image>();
    }

    public IEnumerator FadeOut(float duration)
    {
        float elapsedTime = 0f;
        Color color = fadeImage.color;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsedTime / duration); // Increase alpha
            fadeImage.color = color;
            yield return null;
        }

        color.a = 1f;
        fadeImage.color = color; // Ensure fully opaque
    }

    public IEnumerator FadeIn(float duration)
    {
        float elapsedTime = 0f;
        Color color = fadeImage.color;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            color.a = 1f - Mathf.Clamp01(elapsedTime / duration); // Decrease alpha
            fadeImage.color = color;
            yield return null;
        }

        color.a = 0f;
        fadeImage.color = color; // Ensure fully transparent
    }
}
