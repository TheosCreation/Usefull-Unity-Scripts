using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FOVSetting
{
    private Setting<float> setting;

    public FOVSetting(Slider slider, TMP_Text text, Camera camera, float defaultFOV)
    {
        setting = new Setting<float>(
            slider,
            text,
            value => UpdateCameraFOV(camera, value),
            "FOV",
            defaultFOV
        );
    }

    private void UpdateCameraFOV(Camera camera, float value)
    {
        if (camera != null)
        {
            camera.fieldOfView = value;
        }
    }
    public void ResetToDefault()
    {
        setting.ResetToDefault();
    }

    public void Initialize()
    {
        setting.Initialize();
    }
    
    public void CleanUp()
    {
        setting.CleanUp();
    }
}