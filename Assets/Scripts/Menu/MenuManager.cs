using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public Animator introAnimator;
    public Animator menuAnimator;
    public Animator brickUpAnimator;

    public LightController r_eye;

    public GameObject brickUp, brickMid, brickDown;
    public GameObject buttonPlay, buttonCredits, buttonExit;


    [SerializeField] bool clicked = false;
    // Start is called before the first frame update
    void Start()
    {
        HideCursor();
        introAnimator.enabled = true;
        StartCoroutine(WaitIntroFinish(8));
    }

    void AnimateMenu()
    {
        menuAnimator.SetTrigger("MenuIntro");
    }

    IEnumerator WaitIntroFinish(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        AnimateMenu();
        SeeCursor();
    }

    void HideCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void SeeCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void ClickBrickUp()
    {
        brickUp.GetComponent<AudioSource>().Play();
        if (!clicked)
        {
            clicked = true;
            brickUpAnimator.SetTrigger("Click");
            r_eye.TurnOnLight();
            StartCoroutine(Play());
        }
        else
        {
            clicked = false;
            brickUpAnimator.SetTrigger("ClickOff");
            r_eye.TurnOffLight();
        }
        
        
    }

    public void Credits()
    {
        brickMid.GetComponent<AudioSource>().Play();
    }

    IEnumerator Play()
    {
        buttonExit.GetComponent<Button>().interactable = false;
        buttonCredits.GetComponent<Button>().interactable = false;
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("MainRoom");
    }

    public void OnApplicationQuit()
    {
        Application.Quit();
    }
}
