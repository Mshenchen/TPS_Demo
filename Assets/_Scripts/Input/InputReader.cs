using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "InputReader")]
public class InputReader : ScriptableObject,GameInput.IGameplayActions,GameInput.IUIActions
{
    private GameInput _gameInput;
    public event Action<Vector2> MoveEvent;
    public event Action<Vector2> LookEvent;
    public event Action JumpEvent;
    public event Action JumpCancelEvent;
    public event Action<bool> SprintEvent;
    public event Action<bool> AimEvent;
    public event Action<bool> ShootEvent;
    public event Action<bool> EquipEvent;
    public event Action<bool> SwitchWeaponEvent;
    public event Action ResumeEvent;
    public event Action PauseEvent;
    
    private void OnEnable()
    {
        if (_gameInput == null)
        {
            _gameInput = new GameInput();
            _gameInput.Gameplay.SetCallbacks(this);
            _gameInput.UI.SetCallbacks(this);
            SetGameplay();
        }
    }

    public void SetGameplay()
    {
        _gameInput.Gameplay.Enable();
        _gameInput.UI.Disable();
    }
    public void SetUI()
    {
        _gameInput.Gameplay.Disable();
        _gameInput.UI.Enable();
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        MoveEvent?.Invoke(context.ReadValue<Vector2>());
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            JumpEvent?.Invoke();
        }

        if (context.phase == InputActionPhase.Canceled)
        {
            JumpCancelEvent?.Invoke();
        }
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            PauseEvent?.Invoke();
            SetUI();
        }
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        LookEvent?.Invoke(context.ReadValue<Vector2>());
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Performed:
                SprintEvent?.Invoke(true);
                break;
            case InputActionPhase.Canceled:
                SprintEvent?.Invoke(false);
                break;
        }
        
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Performed:
                AimEvent?.Invoke(true);
                break;
            case InputActionPhase.Canceled:
                AimEvent?.Invoke(false);
                break;
        }
    }
    public void OnShoot(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Performed:
                ShootEvent?.Invoke(true);
                break;
            case InputActionPhase.Canceled:
                ShootEvent?.Invoke(false);
                break;
        }
    }

    public void OnResume(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            ResumeEvent?.Invoke();
            SetGameplay();
        }
    }

    public void OnEquip(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Performed:
                EquipEvent?.Invoke(true);
                break;
            
        }
    }

    public void OnSwitchWeapon(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Performed:
                SwitchWeaponEvent?.Invoke(true);
                break;

        }
    }
}
