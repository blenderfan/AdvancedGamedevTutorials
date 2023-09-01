using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{

    public delegate void StartTouchEvent(Vector2 position, float time);
    public delegate void EndTouchEvent(Vector2 position, float time);

    public event StartTouchEvent OnStartTouch;
    public event EndTouchEvent OnEndTouch;

    #region Public Variables

    #endregion

    AdvancedGamedevMinimaxControls controls;

    #region Private Variables



    #endregion

    private static InputManager instance = null;

    public static InputManager Instance => instance;

    private void Awake()
    {

        instance = this;

        this.controls = new AdvancedGamedevMinimaxControls();
        this.controls.Enable();

        this.EnablePlayerControlScheme();
    }

    private void EnablePlayerControlScheme()
    {
        this.controls.Player.Enable();
        this.controls.Player.TouchPress.started += (ctx) => StartTouch(ctx);
        this.controls.Player.TouchPress.canceled += (ctx) => EndTouch(ctx);
    }

    private void StartTouch(InputAction.CallbackContext context)
    {
        if(this.OnStartTouch != null)
        {
            this.OnStartTouch(this.controls.Player.TouchPosition.ReadValue<Vector2>(), (float)context.startTime);
        }
    }

    private void EndTouch(InputAction.CallbackContext context)
    {
        if(this.OnEndTouch != null)
        {
            this.OnEndTouch(this.controls.Player.TouchPosition.ReadValue<Vector2>(), (float)context.time);
        }
    }

    public Vector2 GetTouchPosition()
    {
        return this.controls.Player.TouchPosition.ReadValue<Vector2>();
    }
}
