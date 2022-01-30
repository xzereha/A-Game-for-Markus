using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance
    {
        get
        {
            s_Instance ??= FindObjectOfType<GameManager>();
            s_Instance ??= new GameObject("GameManager").AddComponent<GameManager>();
            return s_Instance;
        }
    }

    private static GameManager s_Instance;

    [SerializeField] private SceneReference m_MainMenu;
    [SerializeField] private SceneReference m_GameMenu;
    [SerializeField] private SceneReference[] m_GameLevels;
    private Scene m_GameMenuScene;
    private Scene m_ActiveLevel;
    private string m_SceneToLoad;
    private int m_LevelIndex;

    public static void LoadNextLevel()
    {
        Instance.m_LevelIndex++;
        string sceneToLoad = Instance.m_LevelIndex < Instance.m_GameLevels.Length? Instance.m_GameLevels[Instance.m_LevelIndex].ScenePath : Instance.m_MainMenu.ScenePath;
        Instance.LoadSceneInternal(sceneToLoad);
    }

    public static void StartGame()
    {
        Instance.m_LevelIndex = 0;
        Instance.LoadSceneInternal(Instance.m_GameLevels[Instance.m_LevelIndex].ScenePath);
        MessageHandler.TriggerEvent("GameStarted");
    }

    public static void LoadMainMenu()
    {
        MessageHandler.TriggerEvent("GameStopped");
        Instance.LoadSceneInternal(Instance.m_MainMenu.ScenePath);
    }

    public static void LoadScene(string path)
    {
        Instance.LoadSceneInternal(path);
    }

    private void LoadSceneInternal(string path)
    {
        if(m_SceneToLoad != null)
        {
            Debug.LogError($"Already working on loading scene {m_SceneToLoad}");
            return;
        }
        m_SceneToLoad = path;
        Debug.Log($"Loading scene {path}");
        if(m_ActiveLevel.isLoaded)
        {
            SceneManager.UnloadSceneAsync(m_ActiveLevel);
        }
        AsyncOperation async = SceneManager.LoadSceneAsync(m_SceneToLoad, LoadSceneMode.Additive);
        async.completed += OnSceneLoad;
    }

    private void Start() 
    {
        if(SceneManager.GetActiveScene().buildIndex == 0)
        {
            MessageHandler.StartListen("GameOver", OnGameOver);
            AsyncOperation async = SceneManager.LoadSceneAsync(m_GameMenu, LoadSceneMode.Additive);
            async.completed += LoadGameMenu;
            LoadSceneInternal(m_MainMenu.ScenePath);
        }
    }

    private void OnGameOver()
    {
        LoadMainMenu();
    }

    private void OnSceneLoad(AsyncOperation task)
    {
        task.completed -= OnSceneLoad;
        m_ActiveLevel = SceneManager.GetSceneByPath(m_SceneToLoad);
        Debug.Log($"Scene load complete {m_ActiveLevel.name}");
        m_SceneToLoad = null;
        SceneManager.SetActiveScene(m_ActiveLevel);
    }

    private void LoadGameMenu(AsyncOperation task)
    {
        task.completed -= LoadGameMenu;
        m_GameMenuScene = SceneManager.GetSceneByPath(m_GameMenu);
    }
}
