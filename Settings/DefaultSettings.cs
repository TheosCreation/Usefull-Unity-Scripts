using UnityEngine;

[CreateAssetMenu(fileName = "DefaultSettings", menuName = "Settings/DefaultSettings")]
public class DefaultSettings : ScriptableObject
{
    public float defaultFOV = 105f;
    public float defaultMouseSensitivity = 1f;
}