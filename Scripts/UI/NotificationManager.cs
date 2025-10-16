using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager Instance; // �������� ��� ������� �������

    [Header("��������� UI")]
    public TextMeshProUGUI notificationText; // ����� ��� �����������
    public float displayTime = 3f; // ����� ������ �����������

    private Coroutine currentNotification;

    void Awake()
    {
        // ������ ��������
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
        // �������� ����� ��� ������
        if (notificationText != null)
            notificationText.gameObject.SetActive(false);
    }

    // ��������� ����� ��� ������ �����������
    public void ShowNotification(string message)
    {
        // ���� ��� ���� �������� ����������� - ������������� ���
        if (currentNotification != null)
        {
            StopCoroutine(currentNotification);
        }

        // ��������� ����� �����������
        currentNotification = StartCoroutine(ShowNotificationRoutine(message));
    }

    private IEnumerator ShowNotificationRoutine(string message)
    {
        // ���������� ����� � ������������� ���������
        if (notificationText != null)
        {
            notificationText.text = message;
            notificationText.gameObject.SetActive(true);
        }

        // ���� ��������� �����
        yield return new WaitForSeconds(displayTime);

        // �������� �����
        if (notificationText != null)
            notificationText.gameObject.SetActive(false);

        currentNotification = null;
    }
}