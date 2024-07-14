using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuManager : MonoBehaviour
{
    [SerializeField] private DefaultSettings defaultSettings;

    [Header("FOV")]
    [SerializeField] private Slider fovSlider;
    [SerializeField] private TMP_Text fovText;
    [SerializeField] private Camera playerCamera;

    [Header("Mouse Sensitivity")]
    [SerializeField] private Slider msSlider;
    [SerializeField] private TMP_Text msText;
    [SerializeField] private PlayerLook look;

    private FOVSetting fovSetting;
    private MouseSensitivitySetting mouseSensitivitySetting;

    private void Awake()
    {
        if (playerCamera == null) Debug.Log("Camera is null");
        fovSetting = new FOVSetting(fovSlider, fovText, playerCamera, defaultSettings.defaultFOV);

        if (look == null) Debug.Log("Player look is null");
        mouseSensitivitySetting = new MouseSensitivitySetting(msSlider, msText, look, defaultSettings.defaultMouseSensitivity);
    }

    private void OnEnable()
    {
        fovSetting.Initialize();
        mouseSensitivitySetting.Initialize();
    }

    private void OnDisable()
    {
        fovSetting.CleanUp();
        mouseSensitivitySetting.CleanUp();
    }

    public void ResetFOVSliderToDefault()
    {
        fovSetting.ResetToDefault();
    }
    
    public void ResetMsSliderToDefault()
    {
        mouseSensitivitySetting.ResetToDefault();
    }

    // Add similar methods for other settings if needed
}