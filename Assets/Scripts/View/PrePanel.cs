using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PrePanel : MonoBehaviour
{
    public Button startBtn;
    public Button settingsBtn;
    public Button exitBtn;
    public Toggle configToggle;
    public ApiConfigPanel apiConfigPanel;
    public ApiConfigService apiConfigService;

    private void Awake()
    {
        if (apiConfigPanel != null)
        {
            apiConfigPanel.OnClickBack += HandleBackFromApiPanel;
            apiConfigPanel.OnConfigChanged += HandleConfigChanged;
        }
    }

    private void OnDestroy()
    {
        if (apiConfigPanel != null)
        {
            apiConfigPanel.OnClickBack -= HandleBackFromApiPanel;
            apiConfigPanel.OnConfigChanged -= HandleConfigChanged;
        }
    }

    private void Start()
    {
        // Use singleton instance if not assigned via Inspector
        if (apiConfigService == null)
        {
            apiConfigService = ApiConfigService.Instance;
        }

        if (apiConfigPanel != null)
        {
            apiConfigPanel.SetVisible(false);
        }
    }

    private void OnEnable()
    {
        startBtn.onClick.AddListener(HandleStartGame);
        settingsBtn.onClick.AddListener(OpenApiPanel);
        exitBtn.onClick.AddListener(HandleExit);
        
        Refresh(apiConfigService.IsReady());
    }

    private void OnDisable()
    {
        startBtn.onClick.RemoveListener(HandleStartGame);
        settingsBtn.onClick.RemoveListener(OpenApiPanel);
        exitBtn.onClick.RemoveListener(HandleExit);
    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }

    public void Refresh(bool isConfigured)
    {
        if (configToggle != null)
        {
            configToggle.isOn = isConfigured;
        }

        if (startBtn != null)
        {
            startBtn.interactable = isConfigured;
        }
        
        Debug.Log($"[PrePanel] Refreshing. isConfigured: {isConfigured}");
    }

    private void HandleStartGame()
    {
        if (apiConfigService.IsReady())
        {
            SceneManager.LoadScene("Main");
        }
    }

    private void OpenApiPanel()
    {
        SetActive(false);
        apiConfigPanel.SetVisible(true);
        apiConfigPanel.Prefill(apiConfigService.Load());
    }

    private void HandleBackFromApiPanel()
    {
        apiConfigPanel.SetVisible(false);
        SetActive(true);
        Refresh(apiConfigService.IsReady());
    }

    private void HandleConfigChanged()
    {
        Refresh(apiConfigService.IsReady());
    }

    private void HandleExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
