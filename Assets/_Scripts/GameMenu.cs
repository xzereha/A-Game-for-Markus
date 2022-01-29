using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMenu : MonoBehaviour
{
    [SerializeField] private GameObject m_UI;
    [SerializeField] private TMPro.TMP_Text m_DeathCounter;
    private bool m_MenuOpen;
    private int m_Deaths;
    private void Start() 
    {
        MessageHandler.StartListen("PauseGame", OpenMenu);
        MessageHandler.StartListen("PlayerDied", OnPlayerDeath);
        MessageHandler.StartListen("GameStarted", OnGameStart);
        MessageHandler.StartListen("GameStarted", OnGameStop);
    }

    private void OnDestroy() 
    {
        MessageHandler.StopListening("GameStopped", OnGameStop);
        MessageHandler.StopListening("GameStarted", OnGameStart);
        MessageHandler.StopListening("PlayerDied", OnPlayerDeath);
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

    private void OnGameStart()
    {
        m_DeathCounter.gameObject.SetActive(true);
        m_Deaths = 0;
        m_DeathCounter.text = "Deaths : " + m_Deaths;
    }

    private void OnGameStop()
    {
        m_DeathCounter.gameObject.SetActive(false);
    }

    private void OnPlayerDeath()
    {
        m_Deaths++;
        m_DeathCounter.text = "Deaths : " + m_Deaths;
    }
}
