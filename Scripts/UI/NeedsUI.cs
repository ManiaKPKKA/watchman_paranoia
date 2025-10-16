// Scripts/UI/NeedsUI.cs
using UnityEngine;
using TMPro;

public class NeedsUI : MonoBehaviour
{
    [Header("Time UI")]
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI dayText;

    [Header("Need Bars")]
    public UIBar hungerBar;
    public UIBar thirstBar;
    public UIBar warmthBar;
    public UIBar sleepBar;
    public UIBar sanityBar;

    private TimeManager timeManager;
    private NeedSystem needSystem;

    void Start()
    {
        timeManager = FindFirstObjectByType<TimeManager>();
        needSystem = FindFirstObjectByType<NeedSystem>();

        if (timeManager != null)
        {
            timeManager.OnHourChanged.AddListener(UpdateTimeUI);
            timeManager.OnDayChanged.AddListener(UpdateDayUI);
        }
    }

    void Update()
    {
        if (needSystem != null)
        {
            UpdateNeedBars();
        }
    }

    private void UpdateTimeUI(float hour)
    {
        if (timeText != null && timeManager != null)
            timeText.text = timeManager.GetFormattedTime();
    }

    private void UpdateDayUI(int day)
    {
        if (dayText != null)
            dayText.text = $"День: {day}/30";
    }

    private void UpdateNeedBars()
    {
        if (hungerBar != null) hungerBar.SetValue(needSystem.hunger.currentValue / 100f);
        if (thirstBar != null) thirstBar.SetValue(needSystem.thirst.currentValue / 100f);
        if (warmthBar != null) warmthBar.SetValue(needSystem.warmth.currentValue / 100f);
        if (sleepBar != null) sleepBar.SetValue(needSystem.sleep.currentValue / 100f);
        if (sanityBar != null) sanityBar.SetValue(needSystem.sanity.currentValue / 100f);
    }
}

// Вспомогательный класс для UI полосок (добавьте в тот же файл)
[System.Serializable]
public class UIBar
{
    public RectTransform fillRect;

    public void SetValue(float fillAmount)
    {
        if (fillRect != null)
        {
            fillRect.anchorMax = new Vector2(fillAmount, 1f);
        }
    }
}