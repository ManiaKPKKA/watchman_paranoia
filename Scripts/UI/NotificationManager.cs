using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager Instance; // Синглтон для легкого доступа

    [Header("Настройки UI")]
    public TextMeshProUGUI notificationText; // Текст для уведомлений
    public float displayTime = 3f; // Время показа уведомления

    private Coroutine currentNotification;

    void Awake()
    {
        // Делаем синглтон
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Скрываем текст при старте
        if (notificationText != null)
            notificationText.gameObject.SetActive(false);
    }

    // Публичный метод для показа уведомлений
    public void ShowNotification(string message)
    {
        // Если уже есть активное уведомление - останавливаем его
        if (currentNotification != null)
        {
            StopCoroutine(currentNotification);
        }

        // Запускаем новое уведомление
        currentNotification = StartCoroutine(ShowNotificationRoutine(message));
    }

    private IEnumerator ShowNotificationRoutine(string message)
    {
        // Показываем текст и устанавливаем сообщение
        if (notificationText != null)
        {
            notificationText.text = message;
            notificationText.gameObject.SetActive(true);
        }

        // Ждем указанное время
        yield return new WaitForSeconds(displayTime);

        // Скрываем текст
        if (notificationText != null)
            notificationText.gameObject.SetActive(false);

        currentNotification = null;
    }
}