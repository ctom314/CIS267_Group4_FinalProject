using System.Collections;
using UnityEngine;
using TMPro;

public class NotificationManager : MonoBehaviour
{
    public GameObject notificationPrefab; // Assign the Notification Prefab
    public Transform notificationPanel;   // Assign the Notification Panel (parent object)
    public float notificationSpacing = 50f; // Spacing between notifications
    private float currentYOffset = 0f;    // Tracks the Y offset for placing notifications

    // Add a notification to the panel
    public void ShowNotification(string message)
    {
        // Instantiate the notification
        GameObject notification = Instantiate(notificationPrefab, notificationPanel);

        // Set the text
        TMP_Text textComponent = notification.GetComponentInChildren<TMP_Text>();
        if (textComponent != null)
        {
            textComponent.text = message;
        }

        // Adjust the notification's position
        RectTransform rectTransform = notification.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(0, -currentYOffset);

        // Update the offset for the next notification
        currentYOffset += rectTransform.sizeDelta.y + notificationSpacing;

        // Start the fade-out coroutine
        StartCoroutine(FadeAndDestroy(notification, rectTransform));
    }

    private IEnumerator FadeAndDestroy(GameObject notification, RectTransform rectTransform)
    {
        CanvasGroup canvasGroup = notification.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = notification.AddComponent<CanvasGroup>();
        }

        // Slide-in animation (from the right)
        Vector2 startPosition = rectTransform.anchoredPosition;
        rectTransform.anchoredPosition = new Vector2(500, startPosition.y); // Start off-screen to the right
        float slideInTime = 0.5f;
        float elapsedTime = 0f;

        while (elapsedTime < slideInTime)
        {
            elapsedTime += Time.deltaTime;
            rectTransform.anchoredPosition = Vector2.Lerp(new Vector2(500, startPosition.y), startPosition, elapsedTime / slideInTime);
            yield return null;
        }

        // Wait for a few seconds
        yield return new WaitForSeconds(2f);

        // Slide-out animation (to the right)
        elapsedTime = 0f;
        Vector2 offScreenPosition = new Vector2(600, startPosition.y); // Adjust the distance to move farther out

        while (elapsedTime < slideInTime)
        {
            elapsedTime += Time.deltaTime;
            rectTransform.anchoredPosition = Vector2.Lerp(startPosition, offScreenPosition, elapsedTime / slideInTime);
            yield return null;
        }

        // Fade out the entire notification
        float fadeDuration = 1f;
        elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1, 0, elapsedTime / fadeDuration);
            yield return null;
        }

        // Update the offset after removing this notification
        currentYOffset -= rectTransform.sizeDelta.y + notificationSpacing;

        // Destroy the notification
        Destroy(notification);
    }
}
