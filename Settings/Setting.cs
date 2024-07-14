using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class Setting<T>
{
    private Slider slider;
    private TMP_Text text;
    private Action<T> updateValueAction;
    private string playerPrefKey;
    private T defaultValue;

    public Setting(Slider slider, TMP_Text text, Action<T> updateValueAction, string playerPrefKey, T defaultValue)
    {
        this.slider = slider;
        this.text = text;
        this.updateValueAction = updateValueAction;
        this.playerPrefKey = playerPrefKey;
        this.defaultValue = defaultValue;
    }

    public void Initialize()
    {
        if (slider != null)
        {
            T value = GetPlayerPrefValue();
            slider.value = Convert.ToSingle(value);
            UpdateText(value);
            updateValueAction(value);

            slider.onValueChanged.AddListener(OnSliderValueChanged);
        }
    }

    public void CleanUp()
    {
        if (slider != null)
        {
            slider.onValueChanged.RemoveListener(OnSliderValueChanged);
        }
    }

    private void OnSliderValueChanged(float value)
    {
        T typedValue = (T)Convert.ChangeType(value, typeof(T));
        UpdateText(typedValue);
        SaveValue(typedValue);
        updateValueAction(typedValue);
    }

    private void UpdateText(T value)
    {
        text.text = value.ToString();
    }

    private void SaveValue(T value)
    {
        if (typeof(T) == typeof(float))
        {
            PlayerPrefs.SetFloat(playerPrefKey, Convert.ToSingle(value));
        }
        // Add more types as needed
        PlayerPrefs.Save();
    }

    private T GetPlayerPrefValue()
    {
        if (typeof(T) == typeof(float))
        {
            return (T)(object)PlayerPrefs.GetFloat(playerPrefKey, Convert.ToSingle(defaultValue));
        }
        // Add more types as needed
        return defaultValue;
    }

    public void ResetToDefault()
    {
        slider.value = Convert.ToSingle(defaultValue);
        UpdateText(defaultValue);
        SaveValue(defaultValue);
        updateValueAction(defaultValue);
    }
}
