using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using static Unity.VisualScripting.Member;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI")]
    public GameObject victoryPanel;
    public GameObject gameOverPanel;
    public GameObject playerPrefab;
    public GameObject[] statues;
    public int activeStatueOneAt = 30;
    public int activeStatueTwoAt = 40;

    private bool isGameOver = false;
    public bool gameEnded = false;

    public int enemiesKilled = 0;
    public int victoryCount = 50;
    public TextMeshProUGUI killCounterText;
    public PlayerPresenter playerPresenter;
    public Image healthBar;

    float maxHealth;
    float lerpSpeed;

    public AudioSource combatMusic;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        UpdateCounterText();

        if (victoryPanel != null)
            victoryPanel.SetActive(false);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        Vector3 position = new Vector3(0f, 0.5f, 0f);

        // Instanciar el enemigo
        GameObject newPlayer = Instantiate(playerPrefab, position, Quaternion.identity);
        newPlayer.tag = "Player";

        // player = GameObject.Find("Player(Clone)").GetComponent<PlayerModel>();
        playerPresenter = newPlayer.GetComponent<PlayerPresenter>();

        if (playerPresenter == null)
        {
            Debug.LogError("No se encontró PlayerPresenter en el jugador instanciado!");
            return;
        }

        maxHealth = playerPresenter.GetModel().GetHealth();
    }

    private void FixedUpdate()
    {
        if (gameEnded)
        {
            Debug.Log("game ended");
        }
    }

    private void Update()
    {
        HealthBarFiller();
        healthBarColorChanger();

        lerpSpeed = 3 * Time.deltaTime;
    }

    void HealthBarFiller()
    {
        healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, playerPresenter.GetModel().GetHealth() / maxHealth, lerpSpeed);
    }

    public void EnemyKilled()
    {
        enemiesKilled++;
        UpdateCounterText();

        if (enemiesKilled >= victoryCount)
        {
            Victory();
        }
        if (enemiesKilled >= activeStatueOneAt && !statues[0].activeSelf)
        {
            statues[0].SetActive(true);
        }
        if (enemiesKilled >= activeStatueTwoAt && !statues[1].activeSelf)
        {
            statues[1].SetActive(true);
        }
    }

    void UpdateCounterText()
    {
        if (killCounterText != null)
            killCounterText.text = "Enemigos eliminados: " + enemiesKilled + "/" + victoryCount;
    }

    void healthBarColorChanger()
    {
        Color healthColor = Color.Lerp(Color.red, Color.green, playerPresenter.GetModel().GetHealth() / maxHealth);

        healthBar.color = healthColor;
    }

    void Victory()
    {
        Debug.Log("¡Ganaste!");
        gameEnded = true;
        if (victoryPanel != null)
            victoryPanel.SetActive(true);

        // Detener el spawner
        /*EnemySpawner spawner = FindObjectOfType<EnemySpawner>();
        if (spawner != null)
            spawner.StopSpawning();*/
        Time.timeScale = 0f;
    }


    public void GameOver()
    {
        if (isGameOver) return;
        isGameOver = true;
        // Pausar el tiempo (opcional)
        Time.timeScale = 0f;
        // Mostrar canvas
        gameOverPanel.SetActive(true);
    }

    // Métodos públicos para los botones
    public void Retry()
    {
        Time.timeScale = 1f; // reanudar
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReturnToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }

}

