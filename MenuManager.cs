using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject menuPage;
    public GameObject settingsPage;

    private void Start()
    {
        menuPage.SetActive(true);
        settingsPage.SetActive(false);
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void Quit()
    {
        Debug.Log("Quiting");
        Application.Quit();
    }

    public void OpenMainMenuPage()
    {
        menuPage.SetActive(true);
        settingsPage.SetActive(false);
    }
    public void OpenSettingsPage()
    {
        settingsPage.SetActive(true);
        menuPage.SetActive(false);
    }
}