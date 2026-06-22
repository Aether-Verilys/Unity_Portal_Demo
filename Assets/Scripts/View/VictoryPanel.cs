using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VictoryPanel : MonoBehaviour
{
    public GameObject root;
    public TMP_Text resultText;
    public Button toLobbyBtn;
    private void Awake()
    {
        if (root != null)
        {
            root.SetActive(false);
        }
        if (toLobbyBtn != null)
        {
            toLobbyBtn.onClick.AddListener(BackToLobby);
        }
    }
    
    private void BackToLobby()
    {
        SceneManager.LoadScene("Start");
    }
    
    public void ShowVictory(int totalAsset, float elapsedTime)
    {
        if (root != null)
        {
            root.SetActive(true);
        }

        if (resultText != null)
        {
            var span = TimeSpan.FromSeconds(elapsedTime);
            string timeStr = span.Hours > 0 
                ? $"{(int)span.TotalHours}:{span.Minutes:D2}:{span.Seconds:D2}" 
                : $"{span.Minutes:D2}:{span.Seconds:D2}";

            resultText.text = $"You have become the richest person in the world!\nFinal Assets: {totalAsset} Gold\nClear Time: {timeStr}";
        }
    }

    public void LockGameplayInput()
    {
    }
}
