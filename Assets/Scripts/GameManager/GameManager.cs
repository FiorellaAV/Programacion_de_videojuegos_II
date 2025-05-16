using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI")]
    public GameObject victoryPanel;
    public GameObject gameOverCanvas; // Asignar en Inspector
    public GameObject playerPrefab;

    private bool isGameOver = false;
    public bool gameEnded = false;

    public int enemiesKilled = 0;
    public int victoryCount = 50;
    public TextMeshProUGUI killCounterText;
    public PlayerModel player;
    public Image healthBar;

    float maxHealth;
    float lerpSpeed;

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

        if (gameOverCanvas != null)
            gameOverCanvas.SetActive(false);

        Vector3 position = new Vector3(0f, 0.5f, 0f);

        // Instanciar el enemigo
        GameObject newPlayer = Instantiate(playerPrefab, position, Quaternion.identity);
        newPlayer.tag = "Player";
        
        maxHealth = player.getHealth();
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
        healthBarFiller();

        lerpSpeed = 3 * Time.deltaTime;
    }

    void healthBarFiller()
    {
        healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, player.getHealth() / maxHealth, lerpSpeed);
    }

    public void EnemyKilled()
    {
        enemiesKilled++;
        UpdateCounterText();

        if (enemiesKilled >= victoryCount)
        {
            Victory();
        }
    }

    void UpdateCounterText()
    {
        if (killCounterText != null)
            killCounterText.text = "Enemigos eliminados: " + enemiesKilled + "/" + victoryCount;
    }

    void Victory()
    {
        Debug.Log("¡Ganaste!");
        gameEnded = true;
        if (victoryPanel != null)
            victoryPanel.SetActive(true);

        // Detener el spawner
        EnemySpawner spawner = FindObjectOfType<EnemySpawner>();
        if (spawner != null)
            spawner.StopSpawning();
    }


    public void GameOver()
    {
        if (isGameOver) return;
        isGameOver = true;
        // Pausar el tiempo (opcional)
        Time.timeScale = 0f;
        // Mostrar canvas
        gameOverCanvas.SetActive(true);
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
        SceneManager.LoadScene("MainMenu"); // o el nombre de tu escena
    }

}

