using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("Canvases")]
    public GameObject mainMenu;
    public GameObject gameHUD;

    [Header("Panels")]
    public GameObject controlsPanel;
    public GameObject settingsPanel;

    bool uiHidden = false;

    public void StartGame()
    {
        mainMenu.SetActive(false);
        gameHUD.SetActive(true);
    }

    public void OpenControls()
    {
        controlsPanel.SetActive(true);
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
    }

    public void CloseControls()
    {
        controlsPanel.SetActive(false);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
    }

    public void ReturnToMenu()
    {
        gameHUD.SetActive(false);
        mainMenu.SetActive(true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            uiHidden = !uiHidden;
            gameHUD.SetActive(!uiHidden);
        }
    }
}
