using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PausePanel : MonoBehaviour
{
    public Button backToLobbyBtn;
    void Start()
    {
        backToLobbyBtn.onClick.RemoveAllListeners();
        backToLobbyBtn.onClick.AddListener(OnBackToLobby);
    }

    private void OnBackToLobby()
    {
        SceneManager.LoadScene("Start");
    }

}
