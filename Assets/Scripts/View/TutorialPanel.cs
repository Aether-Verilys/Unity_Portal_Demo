using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialPanel : MonoBehaviour
{
    public Image bg;
    public Button startBtn;
    
    public event Action OnClickStart;
    
    float fadeDuration = 5.0f;
    
    void Start()
    {
        if (startBtn != null)
        {
            startBtn.onClick.AddListener(() => {
                OnClickStart?.Invoke();
                gameObject.SetActive(false);
            });
        }

        if (bg != null)
        {
            StartCoroutine(FadeBackgroundAlpha());
        }
    }

    private IEnumerator FadeBackgroundAlpha()
    {
        float elapsed = 0f;
        Color color = bg.color;
        float startAlpha = 1f; // 255
        float endAlpha = 0.8f;  

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / fadeDuration);
            color.a = newAlpha;
            bg.color = color;
            yield return null;
        }

        color.a = endAlpha;
        bg.color = color;
    }
}
