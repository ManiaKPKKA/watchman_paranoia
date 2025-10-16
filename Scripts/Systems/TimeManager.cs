using System;
using UnityEngine;
using UnityEngine.Events;

public class TimeManager : MonoBehaviour // ← Убедитесь, что есть MonoBehaviour
{
    [Header("Time Settings")]
    public float dayLengthInMinutes = 24f; // ← Изменил на public для доступа из NeedSystem
    [SerializeField] private int currentDay = 1;
    [SerializeField] private float currentHour = 6f;

    [Header("Events")]
    public UnityEvent<int> OnDayChanged;
    public UnityEvent<float> OnHourChanged;
    public UnityEvent<int> OnNewShift;

    public int CurrentDay => currentDay;
    public float CurrentHour => currentHour;

    private bool shiftStarted = false;

    void Update()
    {
        // Вычисляем, насколько нужно ускорить время
        float timeScale = 24f / (dayLengthInMinutes * 60f);
        currentHour += Time.deltaTime * timeScale;

        // Проверяем смену дня
        if (currentHour >= 24f)
        {
            currentHour = 0f;
            currentDay++;
            OnDayChanged?.Invoke(currentDay);
            shiftStarted = false;
        }

        // Проверяем наступление 01:00 для начала "смены"
        if (!shiftStarted && currentHour >= 1f)
        {
            shiftStarted = true;
            OnNewShift?.Invoke(currentDay);
        }

        OnHourChanged?.Invoke(currentHour);
    }

    public string GetFormattedTime()
    {
        int hours = Mathf.FloorToInt(currentHour);
        int minutes = Mathf.FloorToInt((currentHour - hours) * 60f);
        return $"{hours:00}:{minutes:00}";
    }

    // Для отладки в Inspector
    public void SetTime(float hour, int day = 1)
    {
        currentHour = hour;
        currentDay = day;
    }
}