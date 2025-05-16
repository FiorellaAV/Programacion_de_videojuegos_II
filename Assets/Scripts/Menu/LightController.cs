using UnityEngine;
using System.Collections;

public class LightController : MonoBehaviour
{
    public Light targetLight;
    public float transitionDuration = 1f;

    void Start()
    {
        targetLight = GetComponent<Light>();
    }

    public void TurnOffLight()
    {
        StartCoroutine(TransitionLight(targetLight.intensity, 0f));
    }

    public void TurnOnLight()
    {
        StartCoroutine(TransitionLight(targetLight.intensity, 0.06f));
    }

    private IEnumerator TransitionLight(float from, float to)
    {
        float timer = 0f;

        while (timer < transitionDuration)
        {
            timer += Time.deltaTime;
            targetLight.intensity = Mathf.Lerp(from, to, timer / transitionDuration);
            yield return null;
        }

        targetLight.intensity = to;
    }
}

