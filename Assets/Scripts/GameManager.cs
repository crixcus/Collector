using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Init();
            return;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private string highScoreKey = "HighScore";

    public int HighScore
    {
        get => PlayerPrefs.GetInt(highScoreKey, 0);
        set => PlayerPrefs.SetInt(highScoreKey, value);
    }

    public int CurrentScore { get; set; }
    public bool IsInitialized { get; set; }

    private void Init()
    {
        IsInitialized = false;
        CurrentScore = 0;
    }

    private string MainMenu = "MainMenu";
    private string Gameplay = "Gameplay";

    public void GoToMainMenu()
    {
        StartCoroutine(LoadSceneWithDelay(MainMenu));
    }

    public void GoToGameplay()
    {
        StartCoroutine(LoadSceneWithDelay(Gameplay));
    }

    public IEnumerator LoadSceneWithDelay(string sceneName)
    {
        yield return new WaitForSeconds(0.1f);
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }
}
