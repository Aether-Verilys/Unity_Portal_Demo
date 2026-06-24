using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
using StarterAssets;

public class Player : MonoBehaviour
{
    private Animator _animator;
    private StarterAssetsInputs _starterAssetsInputs;
    private EraController _eraController;
    private bool _hasAnimator;
    private int _animIDCall;
    private bool _isCallActive;
    private bool _hasEnteredCallState;
    private int _stateBeforeCallHash;

    private void Start()
    {
        _hasAnimator = TryGetComponent(out _animator);
        TryGetComponent(out _starterAssetsInputs);

        if (_eraController == null)
        {
            _eraController = FindFirstObjectByType<EraController>();
        }

        AssignAnimationIDs();
    }

    private void Update()
    {
        _hasAnimator = TryGetComponent(out _animator);

        if (!_hasAnimator)
        {
            return;
        }

        if (!_isCallActive && IsCallPressed())
        {
            StartCall();
        }

        if (_isCallActive)
        {
            UpdateCallState();
        }
    }

    private void AssignAnimationIDs()
    {
        _animIDCall = Animator.StringToHash("Call");
    }

    private void StartCall()
    {
        _stateBeforeCallHash = _animator.GetCurrentAnimatorStateInfo(0).shortNameHash;
        _hasEnteredCallState = false;
        _isCallActive = true;
        SetInputBlocked(true);
        _animator.SetBool(_animIDCall, true);
        _eraController?.OnClickSwitchEra();
    }

    private void UpdateCallState()
    {
        AnimatorStateInfo currentState = _animator.GetCurrentAnimatorStateInfo(0);

        if (!_hasEnteredCallState)
        {
            if (_animator.IsInTransition(0))
            {
                return;
            }

            if (currentState.shortNameHash != _stateBeforeCallHash)
            {
                _hasEnteredCallState = true;
            }

            return;
        }

        if (_animator.IsInTransition(0))
        {
            return;
        }

        if (currentState.shortNameHash == _stateBeforeCallHash || currentState.normalizedTime >= 1f)
        {
            FinishCall();
        }
    }

    private void FinishCall()
    {
        _animator.SetBool(_animIDCall, false);
        _isCallActive = false;
        _hasEnteredCallState = false;
        SetInputBlocked(false);
    }

    private void SetInputBlocked(bool isBlocked)
    {
        if (_starterAssetsInputs == null)
        {
            return;
        }

        _starterAssetsInputs.SetInputEnabled(!isBlocked);
    }

    private bool IsCallPressed()
    {
#if ENABLE_INPUT_SYSTEM
        return Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame;
#else
        return Input.GetKeyDown(KeyCode.E);
#endif
    }
}
