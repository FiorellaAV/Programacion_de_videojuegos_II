using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int enemiesKilled = 0;
    public int victoryCount = 50;
    public TextMeshProUGUI killCounterText;
    public GameObject victoryPanel;
    public bool gameEnded = false;
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

        maxHealth = player.getHealth();
    }

    void Start()
    {
        
        UpdateCounterText();
        if (victoryPanel != null)
            victoryPanel.SetActive(false);
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
}

