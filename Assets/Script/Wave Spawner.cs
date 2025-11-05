using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class WaveSpawner : MonoBehaviour
{
    [Header("UI Texts")]
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;

    [Header("Lose Panel Texts")]
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI finalHighScoreText;

    int waveCount = 1;
    int score = 0;
    int highScore = 0;

    [Header("UI Panels")]
    public GameObject losePanel;

    [Header("Wave Settings")]
    public float spawnRate = 1.0f;
    public int startingEnemyCount = 3;
    private int enemyCount;

    [Header("Spawner Points")]
    public Transform leftSpawner, rightSpawner;

    [Header("Enemy Reference")]
    public GameObject Enemy;

    [Header("Pause Button")]
    public GameObject PauseButton;

    private bool waveInProgress = false;
    private List<GameObject> aliveEnemies = new List<GameObject>();

    void Start()
    {
        Time.timeScale = 1f;
        losePanel.SetActive(false);

        enemyCount = startingEnemyCount;

        // Load high score from PlayerPrefs
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        UpdateUI();

        StartCoroutine(StartWave());
    }

    void Update()
    {
        waveText.text = "Wave: " + waveCount;

        
        if (!waveInProgress && aliveEnemies.Count == 0)
        {
            StartCoroutine(StartWave());
        }
    }

    IEnumerator StartWave()
    {
        waveInProgress = true;

        // Spawn all enemies
        for (int i = 0; i < enemyCount; i++)
        {
            Transform spawnPoint = Random.value < 0.5f ? leftSpawner : rightSpawner;
            GameObject enemy = Instantiate(Enemy, spawnPoint.position, Quaternion.identity);
            aliveEnemies.Add(enemy);

            Enemy enemyScript = enemy.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                enemyScript.onDeath += () =>
                {
                    aliveEnemies.Remove(enemy);
                    AddScore(10); // add 10 points per enemy killed
                };
            }

            yield return new WaitForSeconds(spawnRate);
        }

        // Wait until all enemies are dead
        yield return new WaitUntil(() => aliveEnemies.Count == 0);

        // Prepare next wave
        enemyCount *= 2;
        waveCount++;
        waveInProgress = false;
    }

    void AddScore(int amount)
    {
        score += amount;
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
        }
        UpdateUI();
    }

    void UpdateUI()
    {
        scoreText.text = "Score: " + score;
        highScoreText.text = "High Score: " + highScore;
    }

    public void PlayerDied()
    {
        Time.timeScale = 0f;
        losePanel.SetActive(true);
        PauseButton.SetActive(false);

        // Update lose panel texts
        if (finalScoreText != null)
            finalScoreText.text = "Final Score: " + score;
        if (finalHighScoreText != null)
            finalHighScoreText.text = "High Score: " + highScore;
    }

    public void Retry()
    {
        PauseButton.SetActive(true);
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
