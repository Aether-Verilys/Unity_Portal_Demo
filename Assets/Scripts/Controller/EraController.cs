using System;
using System.Collections;
using UnityEngine;
using AmazingAssets.DynamicRadialMasks;

public class EraController : MonoBehaviour
{
    public TradePanel tradePanel;
    public DRMGameObject drmGameObject;
    public float transitionDuration = 1.0f;

    private readonly EraStateModel _eraState = new EraStateModel();
    private Coroutine _transitionCoroutine;
    private bool _isFirstChange = true;

    public event Action<EraType> OnEraChanged;

    public EraType CurrentEra => _eraState.CurrentEra;

    private void Awake()
    {
        if (tradePanel != null)
        {
            tradePanel.OnClickSwitchEra += OnClickSwitchEra;
        }

        _eraState.OnEraChanged += HandleEraChanged;
        HandleEraChanged(_eraState.CurrentEra);
    }

    private void OnDestroy()
    {
        if (tradePanel != null)
        {
            tradePanel.OnClickSwitchEra -= OnClickSwitchEra;
        }

        _eraState.OnEraChanged -= HandleEraChanged;
    }

    public void OnClickSwitchEra()
    {
        _eraState.SwitchEra();
    }

    private void HandleEraChanged(EraType era)
    {
        UpdateVisuals(era);

        OnEraChanged?.Invoke(era);
    }

    private void UpdateVisuals(EraType era)
    {
        if (drmGameObject == null) return;

        float targetRadius = (era == EraType.Tech) ? 400f : 0f;

        if (_isFirstChange)
        {
            drmGameObject.radius = targetRadius;
            _isFirstChange = false;
            return;
        }

        if (_transitionCoroutine != null) StopCoroutine(_transitionCoroutine);
        _transitionCoroutine = StartCoroutine(TransitionRadius(targetRadius));
    }

    private IEnumerator TransitionRadius(float target)
    {
        float startRadius = drmGameObject.radius;
        float elapsed = 0;
        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            drmGameObject.radius = Mathf.Lerp(startRadius, target, elapsed / transitionDuration);
            yield return null;
        }
        drmGameObject.radius = target;
        _transitionCoroutine = null;
    }
}
