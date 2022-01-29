using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMenu : MonoBehaviour
{
    [SerializeField] private GameObject m_UI;
    private bool m_MenuOpen;

    private void Start() 
    {
        MessageHandler.StartListen("PauseGame", OpenMenu);
    }

    private void OnDestroy() 
    {
        MessageHandler.StopListening("PauseGame", OpenMenu);
    }

    public void OpenMenu()
    {
        Time.timeScale = 0;
        m_UI.SetActive(true);
    }

    public void CloseMenu()
    {
        m_UI.SetActive(false);
        Time.timeScale = 1;
    }

    public void ExitGame()
    {
        GameManager.LoadMainMenu();
        CloseMenu();
    }
}
