using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DamageIndicator : MonoBehaviour
{
    public Image Image;
    public float FlashSpeed;

    private Coroutine _coroutine;

    public void Flash()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
        }

        Image.enabled = true;
        Image.color = Color.red;
        _coroutine = StartCoroutine(FadeAway());
    }

    private IEnumerator FadeAway()
    {
        float startAlpha = 0.3f;
        float a = startAlpha;

        while (a > 0.0f)
        {
            a -= (startAlpha / FlashSpeed) * Time.deltaTime;
            Image.color = new Color(1.0f, 0.0f, 0.0f, a);
            yield return null;
        }

        Image.enabled = false;
    }
}