using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public Animator introAnimator;
    public Animator menuAnimator;

    public LightController r_eye;
    // Start is called before the first frame update
    void Start()
    {
        HideCursor();
        introAnimator.enabled = true;
        StartCoroutine(WaitIntroFinish(7));
    }

    void AnimateMenu()
    {
        menuAnimator.enabled = true;
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
}
