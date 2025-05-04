using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameplayManager : MonoBehaviour
{
    #region START
    public static GameplayManager Instance;

    private bool hasGameFinished;

    private void Awake()
    {
        Instance = this;
        hasGameFinished = false;

        score = 0;
        scoreText.text = score.ToString();

        scoreToSpawn = 1;
        totalTime = 10f;
        currentTime = totalTime;
        StartCoroutine(CountTime());

        SpawnScore();

        GameManager.Instance.IsInitialized = true;
    }
    #endregion

    #region SCORE

    [SerializeField] private TMP_Text scoreText;
    private int score;
    private int scoreToSpawn;
    private int remainingScore;

    [SerializeField] private GameObject scorePrefab;
    [SerializeField] private float spawnX, spawnY;

    private void SpawnScore()
    {
        remainingScore = scoreToSpawn;

        for (int i = 0; i < remainingScore; i++)
        {
            Vector3 spawnPos = new Vector3(Random.Range(-spawnX, spawnX), Random.Range(-spawnY, spawnY), 0);

            RaycastHit2D hit = Physics2D.CircleCast(spawnPos, 1f, Vector2.zero);

            bool canSpawn = hit;

            while (canSpawn)
            {
                spawnPos = new Vector3(Random.Range(-spawnX, spawnX), Random.Range(-spawnY, spawnY), 0);
                hit = Physics2D.CircleCast(spawnPos, 1f, Vector2.zero);
                canSpawn = hit;
            }

            Instantiate(scorePrefab, spawnPos, Quaternion.identity);
        }
    }
    public void UpdateScore()
    {
        score++;
        scoreText.text = score.ToString();

        remainingScore--;

        if(remainingScore == 0)
        {
            scoreToSpawn++;

            totalTime = scoreToSpawn * 5f;
            currentTime = totalTime;

            SpawnScore();

        }
    }

    [SerializeField] private Transform timerImage;

    private float totalTime;
    private float currentTime;

    private IEnumerator CountTime()
    {
        while(!hasGameFinished)
        {
            currentTime -= Time.deltaTime;
            Vector3 temp = timerImage.localScale;
            temp.x = currentTime / totalTime;
            timerImage.localScale = temp;

            if(currentTime <= 0f)
            {
                GameEnded();
            }

            yield return null;
        }
    }

    #endregion

    #region GAME_OVER

    public UnityAction GameEnd;

    [SerializeField] private AudioClip loseClip;

    private void GameEnded()
    {
        hasGameFinished = true;
        GameEnd?.Invoke();
        GameManager.Instance.CurrentScore = score;
        SoundManager.Instance.StopAllSounds();
        SoundManager.Instance.PlaySound(loseClip);

        StartCoroutine(GameOver());
    }


    private IEnumerator GameOver()
    {
        yield return new WaitForSeconds(4f);
        GameManager.Instance.GoToMainMenu();
    }

    #endregion

    #region UI

    public GameObject pausePanel;
    public GameObject pButton;

    public void PauseGame()
    {
        pausePanel.SetActive(true);
        pButton.SetActive(false);
        Time.timeScale = 0f;
    } 

    public void ResumeGame()
    {
        pausePanel.SetActive(false);
        pButton.SetActive(true);
        Time.timeScale = 1f;
    }

    public void MainMenu()
    {
        SoundManager.Instance.StopAllSounds();
        SceneManager.LoadScene("MainMenu");
    }
    #endregion
}
