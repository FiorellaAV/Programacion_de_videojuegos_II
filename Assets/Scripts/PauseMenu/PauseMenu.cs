using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [Header("Sonido")]
    public AudioMixer audioMixer;
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    [Header("Snapshots de Audio")]
    public AudioMixerSnapshot defaultSnapshot;
    public AudioMixerSnapshot pausedSnapshot;


    [Header("Elementos del Menú")]
    public Button menuButton;
    public Button resumeButton;
    public Button exitButton;
    public GameObject pauseMenuPanel;

    private void Start()
    {
        // Cargar valores guardados o usar 1f como predeterminado
        float masterVol = PlayerPrefs.GetFloat("volume_master", 1f);
        float musicVol = PlayerPrefs.GetFloat("volume_music", 1f);
        float sfxVol = PlayerPrefs.GetFloat("volume_sfx", 1f);

        // Asignar valores a sliders
        masterSlider.value = masterVol;
        musicSlider.value = musicVol;
        sfxSlider.value = sfxVol;

        // Aplicar a mixer
        SetVolume("MasterVolume", masterVol);
        SetVolume("MusicVolume", musicVol);
        SetVolume("SfxVolume", sfxVol);

        // Suscribir eventos
        masterSlider.onValueChanged.AddListener((v) => OnVolumeChanged("MasterVolume", "volume_master", v));
        musicSlider.onValueChanged.AddListener((v) => OnVolumeChanged("MusicVolume", "volume_music", v));
        sfxSlider.onValueChanged.AddListener((v) => OnVolumeChanged("SfxVolume", "volume_sfx", v));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pauseMenuPanel.activeSelf)
            {
                CloseMenu();
            }
            else
            {
                OpenMenu();
            }
        }
    }

    private void OnVolumeChanged(string mixerParam, string playerPrefKey, float value)
    {
        SetVolume(mixerParam, value);
        PlayerPrefs.SetFloat(playerPrefKey, value);
        PlayerPrefs.Save();
    }

    private void SetVolume(string parameter, float value)
    {
        // Convierte valor lineal [0,1] a dB [-80,0]
        float dB = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat(parameter, dB);
    }

    public void OpenMenu()
    {
        if(Time.timeScale == 0f) return; // No abrir si ya está pausado
        Time.timeScale = 0f; // Pausar el juego

        Lowpass(); // Cambiar al snapshot de pausa

        if (IsInGame())
        {
            menuButton.gameObject.SetActive(true);
            exitButton.gameObject.SetActive(false);
        }
        else
        {
            menuButton.gameObject.SetActive(false);
            exitButton.gameObject.SetActive(true);
        }
        pauseMenuPanel.SetActive(true);
    }

    public void CloseMenu()
    {
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f; // Reanudar el juego
        Lowpass(); // Volver al snapshot por defecto
    }

    private void Lowpass()
    {
        if(Time.timeScale == 0)
        {
            pausedSnapshot.TransitionTo(0);
        }
        else
        {
            defaultSnapshot.TransitionTo(0);
        }
    }


    private bool IsInGame()
    {
        return SceneManager.GetActiveScene().name != "Menu";
    }

    public void LoadMainMenu()
    {
        CloseMenu();
        Time.timeScale = 1f; // Asegurarse de reanudar el juego
        SceneManager.LoadScene("Menu");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
