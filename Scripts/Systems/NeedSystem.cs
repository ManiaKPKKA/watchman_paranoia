// Scripts/Player/NeedSystem.cs
using System;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class Need
{
    public string name;
    [Range(0, 100)] public float currentValue = 100f;
    public float drainRatePerHour = 5f;
    public float criticalThreshold = 20f;

    public UnityEvent<float> OnValueChanged;
    public UnityEvent OnBecomeCritical;
    public UnityEvent OnBecomeNormal;

    private bool isCritical = false;

    public void UpdateValue(float drainMultiplier)
    {
        float oldValue = currentValue;
        currentValue -= drainRatePerHour * Time.deltaTime * drainMultiplier;
        currentValue = Mathf.Clamp(currentValue, 0, 100f);

        if (Mathf.Abs(oldValue - currentValue) > 0.1f)
        {
            OnValueChanged?.Invoke(currentValue);
        }

        bool nowCritical = currentValue <= criticalThreshold;
        if (nowCritical && !isCritical)
        {
            isCritical = true;
            OnBecomeCritical?.Invoke();
        }
        else if (!nowCritical && isCritical)
        {
            isCritical = false;
            OnBecomeNormal?.Invoke();
        }
    }

    public void Restore(float amount)
    {
        currentValue = Mathf.Clamp(currentValue + amount, 0, 100f);
        OnValueChanged?.Invoke(currentValue);
    }
}

public class NeedSystem : MonoBehaviour
{
    [Header("Player Needs")]
    public Need hunger = new Need { name = "�����", drainRatePerHour = 4f };
    public Need thirst = new Need { name = "�����", drainRatePerHour = 6f };
    public Need warmth = new Need { name = "�����", drainRatePerHour = 3f };
    public Need sleep = new Need { name = "���", drainRatePerHour = 2f };
    public Need sanity = new Need { name = "��������", drainRatePerHour = 1f };

    [Header("Settings")]
    public float needUpdateInterval = 5f;

    private TimeManager timeManager;
    private PlayerMovement playerMovement;

    void Start()
    {
        timeManager = FindFirstObjectByType<TimeManager>();
        playerMovement = GetComponent<PlayerMovement>();

        // ������������� �� ����������� �������
        hunger.OnBecomeCritical.AddListener(() => OnNeedCritical("�����"));
        thirst.OnBecomeCritical.AddListener(() => OnNeedCritical("�����"));
        sleep.OnBecomeCritical.AddListener(() => OnNeedCritical("���"));
        sanity.OnBecomeCritical.AddListener(() => OnNeedCritical("��������"));
    }

    void Update()
    {
        if (timeManager == null) return;

        // ��������� ����������� � ������ ��������� �������
        float timeMultiplier = 24f / (timeManager.dayLengthInMinutes * 60f);

        hunger.UpdateValue(timeMultiplier);
        thirst.UpdateValue(timeMultiplier);
        warmth.UpdateValue(timeMultiplier);
        sleep.UpdateValue(timeMultiplier);
        sanity.UpdateValue(timeMultiplier);
    }

    private void OnNeedCritical(string needName)
    {
        NotificationManager.Instance?.ShowNotification($"��������: {needName} �� ����������� ������!");

        // ������� ��� ����������� ������ ������������
        switch (needName)
        {
            case "���":
                // ���������� ��� ������� ���������
                if (playerMovement != null)
                {
                    playerMovement.walkSpeed = 3f;
                    playerMovement.runSpeed = 5f;
                }
                break;
            case "��������":
                // ���������� ������� ������� (������� �����)
                break;
        }
    }

    // ��������� ������ ��� ��������������
    public void Eat(float nutrition)
    {
        hunger.Restore(nutrition);
        thirst.Restore(nutrition * 0.5f);
    }

    public void Drink(float hydration)
    {
        thirst.Restore(hydration);
    }

    public void Sleep(float rest)
    {
        sleep.Restore(rest);
        sanity.Restore(rest * 0.3f);

        // ��������������� �������� ����� ���
        if (playerMovement != null)
        {
            playerMovement.walkSpeed = 5f;
            playerMovement.runSpeed = 8f;
        }
    }
}