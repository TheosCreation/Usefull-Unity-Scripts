using UnityEngine;

public class PlayerPrefsReset : MonoBehaviour
{
    [ContextMenu("Reset PlayerPrefs")]
    private void ResetPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("PlayerPrefs have been reset.");
    }
}