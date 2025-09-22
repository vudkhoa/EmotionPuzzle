using CustomUtils;
using System;
using UnityEngine;

public class GameInput : SingletonMono<GameInput>
{
    private GameInputAction gameInputAction;

    public event EventHandler OnMoveUp;
    public event EventHandler OnMoveDown;
    public event EventHandler OnMoveLeft;
    public event EventHandler OnMoveRight;
    public event EventHandler OnFunction;
    public event EventHandler OnPause;
    public event EventHandler OnTutorial;
    public event EventHandler OnCloseUI;
    public event EventHandler OnOneButton;
    public event EventHandler OnTWoButton;
    public event EventHandler OnThreeButton;
    public event EventHandler OnFourButton;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
        gameInputAction = new GameInputAction();
    }

    private void OnEnable()
    {
        gameInputAction.Enable();
        gameInputAction.Player.MoveUp.performed += MoveUp_performed;
        gameInputAction.Player.MoveDown.performed += MoveDown_performed;
        gameInputAction.Player.MoveLeft.performed += MoveLeft_performed;
        gameInputAction.Player.MoveRight.performed += MoveRight_performed;
        gameInputAction.Player.Function.performed += Function_performed;
        gameInputAction.Player.Pause.performed += Pause_performed;
        gameInputAction.UI.Tutorial.performed += Tutorial_performed;
        gameInputAction.UI.Close.performed += CloseUI_performed;
        gameInputAction.UI.Element1.performed += Element1_performed;
        gameInputAction.UI.Element2.performed += Element2_performed;
        gameInputAction.UI.Element3.performed += Element3_performed;
        gameInputAction.UI.Element4.performed += Element4_performed;
    }

    private void OnDisable()
    {
        gameInputAction.Player.MoveUp.performed -= MoveUp_performed;
        gameInputAction.Player.MoveDown.performed -= MoveDown_performed;
        gameInputAction.Player.MoveLeft.performed -= MoveLeft_performed;
        gameInputAction.Player.MoveRight.performed -= MoveRight_performed;
        gameInputAction.Player.Function.performed -= Function_performed;
        gameInputAction.Player.Pause.performed -= Pause_performed;
        gameInputAction.UI.Tutorial.performed -= Tutorial_performed;
        gameInputAction.UI.Close.performed -= CloseUI_performed;
        gameInputAction.UI.Element1.performed -= Element1_performed;
        gameInputAction.UI.Element2.performed -= Element2_performed;
        gameInputAction.UI.Element3.performed -= Element3_performed;
        gameInputAction.UI.Element4.performed -= Element4_performed;
        gameInputAction.Disable();
    }

    private void Element4_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnFourButton?.Invoke(this, EventArgs.Empty);
    }

    private void Element3_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnThreeButton?.Invoke(this, EventArgs.Empty);
    }

    private void Element2_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnTWoButton?.Invoke(this, EventArgs.Empty);
    }

    private void Element1_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnOneButton?.Invoke(this, EventArgs.Empty);
    }

    private void CloseUI_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnCloseUI?.Invoke(this, EventArgs.Empty);
    }

    private void Tutorial_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnTutorial?.Invoke(this, EventArgs.Empty);
    }

    private void Pause_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnPause?.Invoke(this, EventArgs.Empty);
    }
    private void Function_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnFunction?.Invoke(this, EventArgs.Empty);
    }

    private void MoveRight_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnMoveRight?.Invoke(this, EventArgs.Empty);
    }

    private void MoveLeft_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnMoveLeft?.Invoke(this, EventArgs.Empty);
    }

    private void MoveDown_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnMoveDown?.Invoke(this, EventArgs.Empty);
    }

    private void MoveUp_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnMoveUp?.Invoke(this, EventArgs.Empty);
    }    
}
