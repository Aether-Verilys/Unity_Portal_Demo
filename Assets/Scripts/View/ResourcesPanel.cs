using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ResourcesPanel : MonoBehaviour
{
    public TMP_Text goldText;
    public TMP_Text targetGoldText;
    public TMP_Text heldItemText;

    [Header("Tier Toast")]
    public GameObject tierToastRoot;
    public TMP_Text tierToastText;
    float toastDuration = 10f;

    private Coroutine _hideCoroutine;
    
    public void RefreshGold(int gold, int targetGold)
    {
        if (goldText != null)
        {
            // 使用 N0 格式化数字，增加千分位分隔符供更好的阅读体验
            goldText.text = gold.ToString("N0");
        }

        if (targetGoldText != null)
        {
            targetGoldText.text = targetGold.ToString("N0");
        }
    }

    public void RefreshHeldItem(string itemName)
    {
        if (heldItemText != null)
        {
            heldItemText.text = string.IsNullOrEmpty(itemName) ? "Held: None" : $"Held: {itemName}";
        }
    }

    public void ShowTierMilestone(string tierName, string text)
    {
        if (tierToastRoot != null)
        {
            tierToastRoot.SetActive(true);
        }

        if (tierToastText != null)
        {
            tierToastText.text = $"{tierName}\n{text}";
        }

        // 5秒后自动关闭
        if (_hideCoroutine != null)
        {
            StopCoroutine(_hideCoroutine);
        }
        _hideCoroutine = StartCoroutine(AutoHideToast());
    }

    private System.Collections.IEnumerator AutoHideToast()
    {
        yield return new WaitForSeconds(toastDuration);
        HideTierToast();
        _hideCoroutine = null;
    }

    public void HideTierToast()
    {
        if (tierToastRoot != null)
        {
            tierToastRoot.SetActive(false);
        }
    }
}
