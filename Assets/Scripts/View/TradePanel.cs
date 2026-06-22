using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TradePanel : MonoBehaviour
{
    public TMP_InputField itemInput;
    public Button submitButton;
    public Button switchEraButton;

    public Toggle saleToggle;

    public GameObject itemPrefab;

    public GameObject buyBox;
    public GameObject sellBox;
    
    [Header("Dialog Area")]
    public GameObject dialogRoot;
    public Button sureButton;
    public Button closeButton;
    public TMP_Text itemText;
    public TMP_Text priceText;
    public TMP_Text dialogText;


    public RectTransform contentTrans;
    public event Action OnClickSwitchEra;
    public event Action<string> OnSubmitItem;
    public event Action OnClickBuy;
    public event Action OnClickRetry;
    public event Action<bool> OnSaleToggleChanged;

    private void Awake()
    {
        if (submitButton != null)
        {
            submitButton.onClick.AddListener(Submit);
        }

        if (switchEraButton != null)
        {
            switchEraButton.onClick.AddListener(() => OnClickSwitchEra?.Invoke());
        }

        if (sureButton != null)
        {
            sureButton.onClick.AddListener(() => OnClickBuy?.Invoke());
        }

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(() => OnClickRetry?.Invoke());
        }

        if (saleToggle != null)
        {
            saleToggle.onValueChanged.AddListener(OnSaleToggleValueChanged);
            // Default to "Buy" mode (isOn = false)
            saleToggle.isOn = false;
            OnSaleToggleValueChanged(saleToggle.isOn);
        }

        HideDecision();
    }

    private void OnSaleToggleValueChanged(bool isSelling)
    {
        if (sellBox != null) sellBox.SetActive(isSelling);
        if (buyBox != null) buyBox.SetActive(!isSelling);
        OnSaleToggleChanged?.Invoke(isSelling);
    }

    private void OnDestroy()
    {
        if (submitButton != null)
        {
            submitButton.onClick.RemoveListener(Submit);
        }

        if (switchEraButton != null)
        {
            switchEraButton.onClick.RemoveAllListeners();
        }

        if (sureButton != null)
        {
            sureButton.onClick.RemoveAllListeners();
        }

        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
        }
    }

    public string GetInputItemName()
    {
        return itemInput == null ? string.Empty : itemInput.text.Trim();
    }

    public List<string> GetSelectedItemNames()
    {
        List<string> selected = new List<string>();
        if (contentTrans == null) return selected;
        foreach (Transform child in contentTrans)
        {
            TradeItem item = child.GetComponent<TradeItem>();
            if (item != null && item.toggle != null && item.toggle.isOn)
            {
                string text = item.itemName.text.Trim();
                if (!string.IsNullOrEmpty(text) && text != "Button")
                {
                    selected.Add(text);
                }
            }
        }
        return selected;
    }

    public void ShowQuote(QuoteResult result)
    {
        if (result == null)
        {
            if (itemText != null) itemText.text = string.Empty;
            if (priceText != null) priceText.text = string.Empty;
            if (dialogText != null) dialogText.text = string.Empty;
            return;
        }

        if (itemText != null) itemText.text = result.itemName;
        if (priceText != null) priceText.text = $"G {result.price}";
        if (dialogText != null) dialogText.text = result.reason;
    }

    public void UpdateDialogContent(string item, string price, string dialog)
    {
        if (itemText != null) itemText.text = item;
        if (priceText != null) priceText.text = price;
        if (dialogText != null) dialogText.text = dialog;
    }

    public void ShowDecision(QuoteResult quote)
    {
        if (dialogRoot != null)
        {
            dialogRoot.SetActive(true);
        }
    }

    public void SetBuyButtonInteractable(bool canBuy)
    {
        if (sureButton != null)
        {
            sureButton.interactable = canBuy;
        }
    }

    public void HideDecision()
    {
        if (dialogRoot != null)
        {
            dialogRoot.SetActive(false);
        }
    }

    public void SetButtonsInteractable(bool interactable)
    {
        if (sureButton != null)
        {
            sureButton.interactable = interactable;
        }

        if (closeButton != null)
        {
            closeButton.interactable = interactable;
        }
    }

    public void ClearInput()
    {
        if (itemInput != null)
        {
            itemInput.text = string.Empty;
        }
    }

    public void SetInteractable(bool interactable)
    {
        if (itemInput != null)
        {
            itemInput.interactable = interactable;
        }

        if (submitButton != null)
        {
            submitButton.interactable = interactable;
        }

        SetButtonsInteractable(interactable);
    }

    public void SetSubmitButtonInteractable(bool interactable)
    {
        if (submitButton != null)
        {
            submitButton.interactable = interactable;
        }
    }

    private void Submit()
    {
        if (saleToggle != null && saleToggle.isOn)
        {
            var selected = GetSelectedItemNames();
            if (selected.Count > 0)
            {
                OnSubmitItem?.Invoke(string.Join(", ", selected));
            }
            else
            {
                OnSubmitItem?.Invoke(string.Empty);
            }
        }
        else
        {
            OnSubmitItem?.Invoke(GetInputItemName());
        }
    }

    public void AddTradeItem(string itemName)
    {
        if (itemPrefab == null || contentTrans == null) return;
        
        GameObject go = Instantiate(itemPrefab, contentTrans);
        TradeItem tradeItem = go.GetComponent<TradeItem>();
        if (tradeItem != null)
        {
            tradeItem.itemName.text = itemName;
            if (tradeItem.toggle != null)
            {
                tradeItem.toggle.isOn = false;
                // Add listener to enforce single selection
                tradeItem.toggle.onValueChanged.AddListener((isOn) => 
                {
                    if (isOn)
                    {
                        OnItemToggleSelected(tradeItem);
                    }
                });
            }
        }
    }

    private void OnItemToggleSelected(TradeItem selectedItem)
    {
        if (contentTrans == null) return;
        
        foreach (Transform child in contentTrans)
        {
            TradeItem item = child.GetComponent<TradeItem>();
            if (item != null && item != selectedItem && item.toggle != null)
            {
                // Temporarily disable listener to avoid recursive calls if we were listening for off too
                // but since we only care about "if (isOn)", this is safe.
                item.toggle.isOn = false;
            }
        }
    }

    public void RemoveSelectedItems()
    {
        if (contentTrans == null) return;
        List<GameObject> toRemove = new List<GameObject>();
        foreach (Transform child in contentTrans)
        {
            TradeItem item = child.GetComponent<TradeItem>();
            if (item != null && item.toggle != null && item.toggle.isOn)
            {
                toRemove.Add(child.gameObject);
            }
        }
        foreach (var go in toRemove)
        {
            Destroy(go);
        }
    }
}
