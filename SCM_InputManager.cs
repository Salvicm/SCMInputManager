using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

/// <Requirements>
/// IMPORTANT: YOU NEED TO DOWNLOAD XInputDotNetPure at: https://github.com/speps/XInputDotNet/releases
/// </Requirements>
public class SCM_InputManager : MonoBehaviour
{
    [SerializeField] private float vibrationStrength = 0.5f;
    [SerializeField] private bool OverloadControls = false;
    private bool vibrationLock = false;
   
    public enum controlOptions { CONTROLLER, KEYBOARD }
    public enum controllerButtons { X_BTN, Y_BTN, A_BTN, B_BTN, LEFT_T, LEFT_B, RIGHT_T, RIGHT_B, LEFT_BTN, RIGHT_BTN, LEFT_STICK, RIGHT_STICK, START, SELECT, U_PAD, D_PAD, L_PAD, R_PAD }
    public controlOptions controller = controlOptions.CONTROLLER;
    List<controllerButtons> lastClicks = new List<controllerButtons>();

    public bool inputInactive = false;
    #region controller
    /// <summary>
    /// Buttons on the controller
    /// </summary>
    public controllerButtons PrimaryAction_A = controllerButtons.A_BTN;
    public controllerButtons PrimaryAction_B = controllerButtons.B_BTN;
    public controllerButtons PrimaryAction_X = controllerButtons.X_BTN;
    public controllerButtons PrimaryAction_Y = controllerButtons.Y_BTN;

    public controllerButtons SecondaryAction_A = controllerButtons.RIGHT_T;
    public controllerButtons SecondaryAction_B = controllerButtons.LEFT_T;
    public controllerButtons SecondaryAction_X = controllerButtons.LEFT_B;
    public controllerButtons SecondaryAction_Y = controllerButtons.RIGHT_B;

    public controllerButtons TertiaryAction_A = controllerButtons.U_PAD;
    public controllerButtons TertiaryAction_B = controllerButtons.D_PAD;
    public controllerButtons TertiaryAction_X = controllerButtons.R_PAD;
    public controllerButtons TertiaryAction_Y = controllerButtons.L_PAD;

    public controllerButtons Action_Pause = controllerButtons.START;
    public controllerButtons Action_Select = controllerButtons.SELECT;

    public controllerButtons PrimaryStick = controllerButtons.LEFT_STICK;
    public controllerButtons PrimaryStick_Click = controllerButtons.LEFT_BTN;
    public controllerButtons SecondaryStick = controllerButtons.RIGHT_STICK;
    public controllerButtons SecondaryStick_Click = controllerButtons.RIGHT_BTN;
    #endregion

    #region keyboard

    public Vector4 movement = Vector4.zero;
    public int movementXKeyboard = 0;
    public int movementYKeyboard = 0;

    public string KPrimaryAction_A = "mouse 0";
    public string KPrimaryAction_B = "mouse 1";
    public string KPrimaryAction_X = "q";
    public string KPrimaryAction_Y = "e";

    public string KSecondaryAction_A = "z";
    public string KSecondaryAction_B = "x";
    public string KSecondaryAction_X = "c";
    public string KSecondaryAction_Y = "v";

    public string KTertiaryAction_A = "f";
    public string KTertiaryAction_B = "g";
    public string KTertiaryAction_X = "h";
    public string KTertiaryAction_Y = "i";

    public string KMovement_U = "w";
    public string KMovement_D = "s";
    public string KMovement_R = "d";
    public string KMovement_L = "a";

    public string KAction_Pause = "p";
    public string KAction_Select = "o";


    #endregion

    private bool PlayerIndexSet = false;
    public PlayerIndex PlayerIndex;

    public GamePadState State;
    public GamePadState PrevState;
    public float LeftAngle;
    public float RightAngle;
    public float LastLeftAngle;
    public float LastRightAngle;
    private bool isVibrating = false;
    // Use IsMoving to block character control if needed
    public bool IsMoving { get; private set; } = false; 
    /// <summary>
    /// Check which stick is being used and load inputs with it
    /// </summary>
    void Start()
    {
        if (!PlayerPrefs.HasKey("Stick"))
        {
            PlayerPrefs.SetInt("Stick", 0);
        }
        if (!OverloadControls) LoadInputs();
    }


    private void FixedUpdate()
    {
        // Find a PlayerIndex, for a single player game
        // Will find the first controller that is connected and use it
        // This checks for the first controller connected
        if (!PlayerIndexSet || !PrevState.IsConnected)
        {
            for (int i = 0; i < 4; ++i)
            {
                PlayerIndex index = (PlayerIndex)i;
                GamePadState state = GamePad.GetState(index);
                if (state.IsConnected)
                {
                    controller = controlOptions.CONTROLLER;
                    PlayerIndex = index;
                    PlayerIndexSet = true;
                }
            }
        }

        if (controller == controlOptions.CONTROLLER)
        {
            if (isVibrating)
            {
                GamePad.SetVibration(PlayerIndex, vibrationStrength, vibrationStrength);
            }
            else
            {
                GamePad.SetVibration(PlayerIndex, 0, 0);
            }
        }

    }


    private void Update()
    {


        // Save previous State to make comparisons
        PrevState = State;
        State = GamePad.GetState(PlayerIndex);
        LastLeftAngle = LeftAngle;
        LastRightAngle = RightAngle;
        LeftAngle = GetAngle("left");
        RightAngle = GetAngle("right");

        switch (controller)
        {
            case controlOptions.CONTROLLER:
                if (Cursor.visible == true)
                    Cursor.visible = false;
              


                CheckKeyboardStroke();
                break;
            case controlOptions.KEYBOARD:
                if (Cursor.visible == false)
                    Cursor.visible = true;
                movement = Vector4.zero;
                movement.w = Input.GetKey(KMovement_U) ? 1 : 0;
                movement.x = Input.GetKey(KMovement_D) ? 1 : 0;
                movement.y = Input.GetKey(KMovement_R) ? 1 : 0;
                movement.z = Input.GetKey(KMovement_L) ? 1 : 0;
                movementYKeyboard = (int)movement.w - (int)movement.x;
                movementXKeyboard = (int)movement.y - (int)movement.z;
                IsMoving = movement != Vector4.zero ? true : false;

                CheckControllerStroke();
                break;
            default:
                break;
        }

    }

    #region One Click
    // Main buttons
    public bool GetPrimaryAction_A()
    {
        if (inputInactive) return false;
        switch (controller)
        {
            case controlOptions.CONTROLLER:
                return getBtnStateClicked(PrimaryAction_A);
            case controlOptions.KEYBOARD:
                return Input.GetKeyDown(KPrimaryAction_A);
            default:
                return false;
        }
    }
    public bool GetPrimaryAction_B()
    {
        if (inputInactive) return false;

        switch (controller)
        {
            case controlOptions.CONTROLLER:
                return getBtnStateClicked(PrimaryAction_B);
            case controlOptions.KEYBOARD:
                return Input.GetKeyDown(KPrimaryAction_B);
            default:
                return false;
        }
    }
    public bool GetPrimaryAction_X()
    {
        if (inputInactive) return false;
        switch (controller)
        {
            case controlOptions.CONTROLLER:
                return getBtnStateClicked(PrimaryAction_X);
            case controlOptions.KEYBOARD:
                return Input.GetKeyDown(KPrimaryAction_X);
            default:
                return false;
        }
    }
    public bool GetPrimaryAction_Y()
    {
        if (inputInactive) return false;

        switch (controller)
        {
            case controlOptions.CONTROLLER:
                return getBtnStateClicked(PrimaryAction_Y);
            case controlOptions.KEYBOARD:
                return Input.GetKeyDown(KPrimaryAction_Y);
            default:
                return false;
        }
    }

    // Triggers
    public bool GetSecondaryAction_A()
    {
        if (inputInactive) return false;
        switch (controller)
        {
            case controlOptions.CONTROLLER:
                return getBtnStateClicked(SecondaryAction_A);
            case controlOptions.KEYBOARD:
                return Input.GetKeyDown(KSecondaryAction_A);
            default:
                return false;
        }
    }
    public bool GetSecondaryAction_B()
    {
        if (inputInactive) return false;

        switch (controller)
        {
            case controlOptions.CONTROLLER:
                return getBtnStateClicked(SecondaryAction_B);
            case controlOptions.KEYBOARD:
                return Input.GetKeyDown(KSecondaryAction_B);
            default:
                return false;
        }
    }
    public bool GetSecondaryAction_X()
    {
        if (inputInactive) return false;
        switch (controller)
        {
            case controlOptions.CONTROLLER:
                return getBtnStateClicked(SecondaryAction_X);
            case controlOptions.KEYBOARD:
                return Input.GetKeyDown(KSecondaryAction_X);
            default:
                return false;
        }
    }
    public bool GetSecondaryAction_Y()
    {
        if (inputInactive) return false;

        switch (controller)
        {
            case controlOptions.CONTROLLER:
                return getBtnStateClicked(SecondaryAction_Y);
            case controlOptions.KEYBOARD:
                return Input.GetKeyDown(KSecondaryAction_Y);
            default:
                return false;
        }
    }

    // Tertiary actions(D-pad)
    public bool GetTertiaryAction_A()
    {
        if (inputInactive) return false;
        switch (controller)
        {
            case controlOptions.CONTROLLER:
                return getBtnStateClicked(TertiaryAction_A);
            case controlOptions.KEYBOARD:
                return Input.GetKeyDown(KTertiaryAction_A);
            default:
                return false;
        }
    }
    public bool GetTertiaryAction_B()
    {
        if (inputInactive) return false;

        switch (controller)
        {
            case controlOptions.CONTROLLER:
                return getBtnStateClicked(TertiaryAction_B);
            case controlOptions.KEYBOARD:
                return Input.GetKeyDown(KTertiaryAction_B);
            default:
                return false;
        }
    }
    public bool GetTertiaryAction_X()
    {
        if (inputInactive) return false;
        switch (controller)
        {
            case controlOptions.CONTROLLER:
                return getBtnStateClicked(TertiaryAction_X);
            case controlOptions.KEYBOARD:
                return Input.GetKeyDown(KTertiaryAction_X);
            default:
                return false;
        }
    }
    public bool GetTertiaryAction_Y()
    {
        if (inputInactive) return false;

        switch (controller)
        {
            case controlOptions.CONTROLLER:
                return getBtnStateClicked(TertiaryAction_Y);
            case controlOptions.KEYBOARD:
                return Input.GetKeyDown(KTertiaryAction_Y);
            default:
                return false;
        }
    }


    // Other Buttons
    public bool GetPauseAction()
    {
        if (inputInactive) return false;

        switch (controller)
        {
            case controlOptions.CONTROLLER:
                return getBtnStateClicked(Action_Pause);
            case controlOptions.KEYBOARD:
                return Input.GetKeyDown(KAction_Pause);
            default:
                return false;
        }
    }
    public bool GetSelectAction()
    {
        if (inputInactive) return false;

        switch (controller)
        {
            case controlOptions.CONTROLLER:
                return getBtnStateClicked(Action_Select);
            case controlOptions.KEYBOARD:
                return Input.GetKeyDown(KAction_Select);
            default:
                return false;
        }
    }
    public bool GetPrimaryStick()
    {
        if (inputInactive) return false;

        return getBtnStateClicked(PrimaryStick_Click);
    }
    public bool GetSecondaryStick()
    {
        if (inputInactive) return false;

        return getBtnStateClicked(SecondaryStick_Click);
    }

    #endregion

    #region Continuous press
    // Main buttons
    public bool GetPrimaryAction_A_Continuous()
    {
        if (inputInactive) return false;
        switch (controller)
        {
            case controlOptions.CONTROLLER:
                return getBtnStatePressed(PrimaryAction_A);
            case controlOptions.KEYBOARD:
                return Input.GetKey(KPrimaryAction_A);
            default:
                return false;
        }
    }
    public bool GetPrimaryAction_B_Continuous()
    {
        if (inputInactive) return false;

        switch (controller)
        {
            case controlOptions.CONTROLLER:
                return getBtnStatePressed(PrimaryAction_B);
            case controlOptions.KEYBOARD:
                return Input.GetKey(KPrimaryAction_B);
            default:
                return false;
        }
    }
    public bool GetPrimaryAction_X_Continuous()
    {
        if (inputInactive) return false;
        switch (controller)
        {
            case controlOptions.CONTROLLER:
                return getBtnStatePressed(PrimaryAction_X);
            case controlOptions.KEYBOARD:
                return Input.GetKey(KPrimaryAction_X);
            default:
                return false;
        }
    }
    public bool GetPrimaryAction_Y_Continuous()
    {
        if (inputInactive) return false;

        switch (controller)
        {
            case controlOptions.CONTROLLER:
                return getBtnStatePressed(PrimaryAction_Y);
            case controlOptions.KEYBOARD:
                return Input.GetKey(KPrimaryAction_Y);
            default:
                return false;
        }
    }

    // Triggers
    public bool GetSecondaryAction_A_Continuous()
    {
        if (inputInactive) return false;
        switch (controller)
        {
            case controlOptions.CONTROLLER:
                return getBtnStatePressed(SecondaryAction_A);
            case controlOptions.KEYBOARD:
                return Input.GetKey(KSecondaryAction_A);
            default:
                return false;
        }
    }
    public bool GetSecondaryAction_B_Continuous()
    {
        if (inputInactive) return false;

        switch (controller)
        {
            case controlOptions.CONTROLLER:
                return getBtnStatePressed(SecondaryAction_B);
            case controlOptions.KEYBOARD:
                return Input.GetKey(KSecondaryAction_B);
            default:
                return false;
        }
    }
    public bool GetSecondaryAction_X_Continuous()
    {
        if (inputInactive) return false;
        switch (controller)
        {
            case controlOptions.CONTROLLER:
                return getBtnStatePressed(SecondaryAction_X);
            case controlOptions.KEYBOARD:
                return Input.GetKey(KSecondaryAction_X);
            default:
                return false;
        }
    }
    public bool GetSecondaryAction_Y_Continuous()
    {
        if (inputInactive) return false;

        switch (controller)
        {
            case controlOptions.CONTROLLER:
                return getBtnStatePressed(SecondaryAction_Y);
            case controlOptions.KEYBOARD:
                return Input.GetKey(KSecondaryAction_Y);
            default:
                return false;
        }
    }

    // Tertiary actions(D-pad)
    public bool GetTertiaryAction_A_Continuous()
    {
        if (inputInactive) return false;
        switch (controller)
        {
            case controlOptions.CONTROLLER:
                return getBtnStatePressed(TertiaryAction_A);
            case controlOptions.KEYBOARD:
                return Input.GetKey(KTertiaryAction_A);
            default:
                return false;
        }
    }
    public bool GetTertiaryAction_B_Continuous()
    {
        if (inputInactive) return false;

        switch (controller)
        {
            case controlOptions.CONTROLLER:
                return getBtnStatePressed(TertiaryAction_B);
            case controlOptions.KEYBOARD:
                return Input.GetKey(KTertiaryAction_B);
            default:
                return false;
        }
    }
    public bool GetTertiaryAction_X_Continuous()
    {
        if (inputInactive) return false;
        switch (controller)
        {
            case controlOptions.CONTROLLER:
                return getBtnStatePressed(TertiaryAction_X);
            case controlOptions.KEYBOARD:
                return Input.GetKey(KTertiaryAction_X);
            default:
                return false;
        }
    }
    public bool GetTertiaryAction_Y_Continuous()
    {
        if (inputInactive) return false;

        switch (controller)
        {
            case controlOptions.CONTROLLER:
                return getBtnStatePressed(TertiaryAction_Y);
            case controlOptions.KEYBOARD:
                return Input.GetKey(KTertiaryAction_Y);
            default:
                return false;
        }
    }


    // Other Buttons
    public bool GetPauseAction_Continuous()
    {
        if (inputInactive) return false;

        switch (controller)
        {
            case controlOptions.CONTROLLER:
                return getBtnStatePressed(Action_Pause);
            case controlOptions.KEYBOARD:
                return Input.GetKey(KAction_Pause);
            default:
                return false;
        }
    }
    public bool GetSelectAction_Continuous()
    {
        if (inputInactive) return false;
        switch (controller)
        {
            case controlOptions.CONTROLLER:
                return getBtnStatePressed(Action_Select);
            case controlOptions.KEYBOARD:
                return Input.GetKey(KAction_Select);
            default:
                return false;
        }
    }

    /// <summary>
    /// Primary stick is left stick, Secondary is right
    /// </summary>
    /// <returns></returns>
    public bool GetPrimaryStick_Continuous()
    {
        if (inputInactive) return false;

        return getBtnStatePressed(PrimaryStick_Click);
    }
    public bool GetSecondaryStick_Continuous()
    {
        if (inputInactive) return false;

        return getBtnStatePressed(SecondaryStick_Click);
    }
    #endregion

    #region Analog
    public float GetRightTrigger()
    {
        return State.Triggers.Right;
    }
    public float GetLeftTrigger()
    {
        return State.Triggers.Left;
    }
    #endregion

    #region Movement

    public float getMovementX()
    {
        switch (controller)
        {
            case controlOptions.CONTROLLER:
                return PrimaryStick == controllerButtons.LEFT_STICK ? State.ThumbSticks.Left.X : State.ThumbSticks.Right.X;

            case controlOptions.KEYBOARD:
                return movementXKeyboard;
            default:
                return 0;
        }
    }
    public float getMovementY()
    {
        switch (controller)
        {
            case controlOptions.CONTROLLER:
                return PrimaryStick == controllerButtons.LEFT_STICK ? State.ThumbSticks.Left.Y : State.ThumbSticks.Right.Y;

            case controlOptions.KEYBOARD:
                return movementYKeyboard;
            default:
                return 0;
        }
    }

    public float GetPrimaryMovX()
    {
        switch (controller)
        {
            case controlOptions.CONTROLLER:
                return State.ThumbSticks.Left.X;
            default:
                return 0;
        }
    }
    public float GetPrimaryMovY()
    {
        switch (controller)
        {
            case controlOptions.CONTROLLER:
                return State.ThumbSticks.Left.Y;
            default:
                return 0;
        }
    }
    public float GetSecondaryMovX()
    {
        switch (controller)
        {
            case controlOptions.CONTROLLER:
                return State.ThumbSticks.Right.X;


            default:
                return 0;
        }
    }
    public float GetSecondaryMovY()
    {
        switch (controller)
        {
            case controlOptions.CONTROLLER:
                return State.ThumbSticks.Right.Y;
            default:
                return 0;
        }
    }

    public float GetPrimaryStickAngle()
    {
        return LeftAngle;
    }
    public float GetSecondaryStickAngle()
    {
        return RightAngle;
    }
    public Vector2 GetPrimaryStickState()
    {
        return new Vector2(State.ThumbSticks.Left.X, State.ThumbSticks.Left.Y);
    }
    public Vector2 GetSecondaryStickState()
    {
        return new Vector2(State.ThumbSticks.Right.X, State.ThumbSticks.Right.Y);
    }
    public Vector2 GetDpadState()
    {
        Vector4 movAux;
        switch (controller)
        {
            case controlOptions.CONTROLLER:
                movAux.w = getBtnStatePressed(controllerButtons.U_PAD) ? 1 : 0;
                movAux.x = getBtnStatePressed(controllerButtons.D_PAD) ? 1 : 0;
                movAux.y = getBtnStatePressed(controllerButtons.R_PAD) ? 1 : 0;
                movAux.z = getBtnStatePressed(controllerButtons.L_PAD) ? 1 : 0;
                break;
            case controlOptions.KEYBOARD:

            default:
                movAux.w = movement.w;
                movAux.x = movement.x;
                movAux.y = movement.y;
                movAux.z = movement.z;
                break;
        }
        Vector2 newMovAux;
        newMovAux.y = (int)movAux.w - (int)movAux.x;
        newMovAux.x = (int)movAux.y - (int)movAux.z;
        return newMovAux;
    }
    #endregion

    /// <summary>
    /// Modify the strength of the vibration
    /// </summary>
    /// <param name="strength">0-1</param>
    public void setVibrationStrength(float strength)
    {
        vibrationStrength = Mathf.Clamp(strength, 0, 1);
    }
    /// <summary>
    /// Set a vibration with a timer
    /// </summary>
    /// <param name="time">Time in int</param>
    public void setVibrationTimer(float time)
    {
        StartCoroutine("VibrationWait", time);
    }
    /// <summary>
    /// Force Activate or deactivate vibration
    /// </summary>
    /// <param name="_a"></param>
    public void setVibration(bool _a)
    {
        isVibrating = _a;
    }

   
    private IEnumerator VibrationWait(float time)
    {
        if (!vibrationLock)
        {
            Debug.Log(time);

            vibrationLock = true;
            isVibrating = true;
            yield return new WaitForSecondsRealtime(time);
            isVibrating = false;
            vibrationLock = false;
        }
        else
            yield return null;
    }
    #region Status Checker
    /// <summary>
    /// Get info of the button on the frame
    /// </summary>
    public bool getBtnStatePressed(controllerButtons input)
    {
        switch (input)
        {
            case controllerButtons.X_BTN:
                return State.Buttons.X == ButtonState.Pressed;
            case controllerButtons.Y_BTN:
                return State.Buttons.Y == ButtonState.Pressed;
            case controllerButtons.A_BTN:
                return State.Buttons.A == ButtonState.Pressed;
            case controllerButtons.B_BTN:
                return State.Buttons.B == ButtonState.Pressed;
            case controllerButtons.LEFT_T:
                return (State.Triggers.Left >= 0.75f) ? true : false;
            case controllerButtons.LEFT_B:
                return State.Buttons.LeftShoulder == ButtonState.Pressed;
            case controllerButtons.RIGHT_T:
                return (State.Triggers.Right >= 0.75f) ? true : false;
            case controllerButtons.RIGHT_B:
                return State.Buttons.RightShoulder == ButtonState.Pressed;
            case controllerButtons.LEFT_BTN:
                return State.Buttons.LeftStick == ButtonState.Pressed;
            case controllerButtons.RIGHT_BTN:
                return State.Buttons.RightStick == ButtonState.Pressed;
            case controllerButtons.START:
                return State.Buttons.Start == ButtonState.Pressed;
            case controllerButtons.SELECT:
                return State.Buttons.Back == ButtonState.Pressed;
            case controllerButtons.U_PAD:
                return State.DPad.Up == ButtonState.Pressed;
            case controllerButtons.D_PAD:
                return State.DPad.Down == ButtonState.Pressed;
            case controllerButtons.L_PAD:
                return State.DPad.Left == ButtonState.Pressed;
            case controllerButtons.R_PAD:
                return State.DPad.Right == ButtonState.Pressed;


            default:
                return false;
        }
    }
    /// <summary>
    /// Chech if the button has been pressed this frame
    /// </summary>
    public bool getBtnStateClicked(controllerButtons input)
    {
        switch (input)
        {
            case controllerButtons.X_BTN:
                return (State.Buttons.X == ButtonState.Pressed && PrevState.Buttons.X == ButtonState.Released);
            case controllerButtons.Y_BTN:
                return (State.Buttons.Y == ButtonState.Pressed && PrevState.Buttons.Y == ButtonState.Released);
            case controllerButtons.A_BTN:
                return (State.Buttons.A == ButtonState.Pressed && PrevState.Buttons.A == ButtonState.Released);
            case controllerButtons.B_BTN:
                return (State.Buttons.B == ButtonState.Pressed && PrevState.Buttons.B == ButtonState.Released);
            case controllerButtons.LEFT_T:
                return (State.Triggers.Left >= 0.75f && PrevState.Triggers.Left < 0.75f);
            case controllerButtons.LEFT_B:
                return (State.Buttons.LeftShoulder == ButtonState.Pressed && PrevState.Buttons.LeftShoulder == ButtonState.Released);
            case controllerButtons.RIGHT_T:
                return (State.Triggers.Right >= 0.75f && PrevState.Triggers.Right < 0.75f);
            case controllerButtons.RIGHT_B:
                return (State.Buttons.RightShoulder == ButtonState.Pressed && PrevState.Buttons.RightShoulder == ButtonState.Released);
            case controllerButtons.LEFT_BTN:
                return (State.Buttons.LeftStick == ButtonState.Pressed && PrevState.Buttons.LeftStick == ButtonState.Released);
            case controllerButtons.RIGHT_BTN:
                return (State.Buttons.RightStick == ButtonState.Pressed && PrevState.Buttons.RightStick == ButtonState.Released);
            case controllerButtons.START:
                return (State.Buttons.Start == ButtonState.Pressed && PrevState.Buttons.Start == ButtonState.Released);
            case controllerButtons.SELECT:
                return (State.Buttons.Back == ButtonState.Pressed && PrevState.Buttons.Back == ButtonState.Released);
            case controllerButtons.U_PAD:
                return (State.DPad.Up == ButtonState.Pressed && PrevState.DPad.Up == ButtonState.Released);
            case controllerButtons.D_PAD:
                return (State.DPad.Down == ButtonState.Pressed && PrevState.DPad.Down == ButtonState.Released);
            case controllerButtons.L_PAD:
                return (State.DPad.Left == ButtonState.Pressed && PrevState.DPad.Left == ButtonState.Released);
            case controllerButtons.R_PAD:
                return (State.DPad.Right == ButtonState.Pressed && PrevState.DPad.Right == ButtonState.Released);

            default:
                return false;
        }
    }
    
    /// <summary>
    /// Check if any key has been pressed(There must be a better way)
    /// </summary>
    private void CheckKeyboardStroke()
    {
        if (inputInactive) return;

        if (Input.GetKeyDown(KeyCode.A)
                        || Input.GetKeyDown(KeyCode.S)
                        || Input.GetKeyDown(KeyCode.D)
                        || Input.GetKeyDown(KeyCode.W)
                        || Input.GetMouseButton(0)
                        || Input.GetMouseButton(1)
                        || Input.GetMouseButton(2)
                        || Input.GetKeyDown(KeyCode.UpArrow)
                        || Input.GetKeyDown(KeyCode.DownArrow)
                        || Input.GetKeyDown(KeyCode.RightArrow)
                        || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            controller = controlOptions.KEYBOARD;
        }
    }
    /// <summary>
    /// Check if any key on the gamepad has been pressed
    /// </summary>
    private void CheckControllerStroke()
    {
        if (inputInactive) return;

        if (getBtnStateClicked(PrimaryAction_A) || getBtnStateClicked(PrimaryAction_B) ||
            getBtnStateClicked(PrimaryAction_X) || getBtnStateClicked(PrimaryAction_Y) ||
            getBtnStateClicked(SecondaryAction_A) || getBtnStateClicked(SecondaryAction_B) ||
            getBtnStateClicked(SecondaryAction_X) || getBtnStateClicked(SecondaryAction_Y) ||
            getBtnStateClicked(TertiaryAction_A) || getBtnStateClicked(TertiaryAction_B) ||
            getBtnStateClicked(TertiaryAction_X) || getBtnStateClicked(TertiaryAction_Y) 
                || getBtnStateClicked(Action_Pause) || getBtnStateClicked(Action_Select)
                || State.ThumbSticks.Right.X != 0 || State.ThumbSticks.Left.X != 0
                || State.ThumbSticks.Right.Y != 0 || State.ThumbSticks.Left.Y != 0)
        {
            controller = controlOptions.CONTROLLER;
        }
    }
    #endregion

    /// <summary>
    /// Get the current angle of the joysticks
    /// </summary>
    /// <param name="side"></param>
    /// <returns></returns>
    private float GetAngle(string side)
    {
        float angleReturn = 0;
        float x = 0;
        float y = 0;
        switch (side)
        {
            case "left":
                x = State.ThumbSticks.Left.X;
                y = State.ThumbSticks.Left.Y;
                if (x == 0 && y == 0) return LastLeftAngle;
                angleReturn = LastLeftAngle;
                break;
            case "right":
                x = State.ThumbSticks.Right.X;
                y = State.ThumbSticks.Right.Y;
                if (x == 0 && y == 0) return LastRightAngle;
                angleReturn = LastRightAngle;
                break;
        }

        if (x == 0 && y == 0)
            return angleReturn;
        else if (x == 0 && y != 0)
            if (y > 0)
                return 90.0f;
            else
                return -90.0f;
        else if (x != 0 && y == 0)
            if (x > 0)
                return 0f;
            else
                return 180f;
        else
            return Mathf.Atan2(y, x) * Mathf.Rad2Deg;
    }

   

    /// <summary>
    /// This allows for remapping the controls
    /// </summary>
    #region setters

    #region controller
    #region primaries
    public void SetPrimaryA(controllerButtons button)
    {
        PrimaryAction_A = button;
        PlayerPrefs.SetInt("PrimaryAction_A", (int)PrimaryAction_A);
    }
    public void SetPrimaryB(controllerButtons button)
    {
        PrimaryAction_B = button;
        PlayerPrefs.SetInt("PrimaryAction_B", (int)PrimaryAction_B);
    }
    public void SetPrimaryX(controllerButtons button)
    {
        PrimaryAction_X = button;
        PlayerPrefs.SetInt("PrimaryAction_X", (int)PrimaryAction_X);
    }
    public void SetPrimaryY(controllerButtons button)
    {
        PrimaryAction_Y = button;
        PlayerPrefs.SetInt("PrimaryAction_Y", (int)PrimaryAction_Y);
    }
    #endregion
    #region secondaries
    public void SetSecondaryA(controllerButtons button)
    {
        SecondaryAction_A = button;
        PlayerPrefs.SetInt("SecondaryAction_A", (int)SecondaryAction_A);
    }
    public void SetSecondaryB(controllerButtons button)
    {
        SecondaryAction_B = button;
        PlayerPrefs.SetInt("SecondaryAction_B", (int)SecondaryAction_B);
    }
    public void SetSecondaryX(controllerButtons button)
    {
        SecondaryAction_X = button;
        PlayerPrefs.SetInt("SecondaryAction_X", (int)SecondaryAction_X);
    }
    public void SetSecondaryY(controllerButtons button)
    {
        SecondaryAction_Y = button;
        PlayerPrefs.SetInt("SecondaryAction_Y", (int)SecondaryAction_Y);
    }
    #endregion
    #region tertiaries
    public void SetTertiaryA(controllerButtons button)
    {
        TertiaryAction_A = button;
        PlayerPrefs.SetInt("TertiaryAction_A", (int)TertiaryAction_A);
    }
    public void SetTertiaryB(controllerButtons button)
    {
        TertiaryAction_B = button;
        PlayerPrefs.SetInt("TertiaryAction_B", (int)TertiaryAction_B);
    }
    public void SetTertiaryX(controllerButtons button)
    {
        TertiaryAction_X = button;
        PlayerPrefs.SetInt("TertiaryAction_X", (int)TertiaryAction_X);
    }
    public void SetTertiaryY(controllerButtons button)
    {
        TertiaryAction_Y = button;
        PlayerPrefs.SetInt("TertiaryAction_Y", (int)TertiaryAction_Y);
    }
    #endregion
    #region extras
    public void SetPause(controllerButtons button)
    {
        Action_Pause = button;
        PlayerPrefs.SetInt("Action_Pause", (int)Action_Pause);
    }
    public void SetSelect(controllerButtons button)
    {
        Action_Select = button;
        PlayerPrefs.SetInt("Action_Select", (int)Action_Select);
    }
    #endregion
    #endregion
    #region keyboard
    #region primaries
    public void SetPrimaryA(string button)
    {
        KPrimaryAction_A = button;
        PlayerPrefs.SetString("KPrimaryAction_A", KPrimaryAction_A);
    }
    public void SetPrimaryB(string button)
    {
        KPrimaryAction_B = button;
        PlayerPrefs.SetString("KPrimaryAction_B", KPrimaryAction_B);
    }
    public void SetPrimaryX(string button)
    {
        KPrimaryAction_X = button;
        PlayerPrefs.SetString("KPrimaryAction_X", KPrimaryAction_X);
    }
    public void SetPrimaryY(string button)
    {
        KPrimaryAction_Y = button;
        PlayerPrefs.SetString("KPrimaryAction_Y", KPrimaryAction_Y);
    }
    #endregion
    #region secondaries
    public void SetSecondaryA(string button)
    {
        KSecondaryAction_A = button;
        PlayerPrefs.SetString("KSecondaryAction_A", KSecondaryAction_A);
    }
    public void SetSecondaryB(string button)
    {
        KSecondaryAction_B = button;
        PlayerPrefs.SetString("KSecondaryAction_B", KSecondaryAction_B);
    }
    public void SetSecondaryX(string button)
    {
        KSecondaryAction_X = button;
        PlayerPrefs.SetString("KSecondaryAction_X", KSecondaryAction_X);
    }
    public void SetSecondaryY(string button)
    {
        KSecondaryAction_Y = button;
        PlayerPrefs.SetString("KSecondaryAction_Y", KSecondaryAction_Y);
    }
    #endregion
    #region tertiaries
    public void SetTertiaryA(string button)
    {
        KTertiaryAction_A = button;
        PlayerPrefs.SetString("KTertiaryAction_A", KTertiaryAction_A);
    }
    public void SetTertiaryB(string button)
    {
        KTertiaryAction_B = button;
        PlayerPrefs.SetString("KTertiaryAction_B", KTertiaryAction_B);
    }
    public void SetTertiaryX(string button)
    {
        KTertiaryAction_X = button;
        PlayerPrefs.SetString("KTertiaryAction_X", KTertiaryAction_X);
    }
    public void SetTertiaryY(string button)
    {
        KTertiaryAction_Y = button;
        PlayerPrefs.SetString("KTertiaryAction_Y", KTertiaryAction_Y);
    }
    #endregion
    #region extras
    public void SetPause(string button)
    {
        KAction_Pause = button;
        PlayerPrefs.SetString("KAction_Pause", KAction_Pause);
    }
    public void SetSelect(string button)
    {
        KAction_Select = button;
        PlayerPrefs.SetString("KAction_Select", KAction_Select);
    }

    public void SetUp(string button){
        KMovement_U = button;
        PlayerPrefs.SetString("KMovement_U", KMovement_U);
    }
    public void SetDown(string button)
    {
        KMovement_D = button;
        PlayerPrefs.SetString("KMovement_D", KMovement_D);
    }
    public void SetLeft(string button)
    {
        KMovement_L = button;
        PlayerPrefs.SetString("KMovement_L", KMovement_L);
    }
    public void SetRight(string button)
    {
        KMovement_R = button;
        PlayerPrefs.SetString("KMovement_R", KMovement_R);
    }
    #endregion
    #endregion

    #endregion

    public controllerButtons[] getLastTenInputs()
    {
        return null;
    }

    /// <summary>
    /// Checks if the buttons have been saved on the playerprefs to check if it's been modified, this allows for re-mapping the controls
    /// </summary>
    private void LoadInputs()
    {
        #region keyboard
        // Primaries
        if (!PlayerPrefs.HasKey("KPrimaryAction_A"))
        {
            PlayerPrefs.SetString("KPrimaryAction_A", KPrimaryAction_A);
        }
        else
        {
            KPrimaryAction_A = PlayerPrefs.GetString("KPrimaryAction_A");
        }

        if (!PlayerPrefs.HasKey("KPrimaryAction_B"))
        {
            PlayerPrefs.SetString("KPrimaryAction_B", KPrimaryAction_B);
        }
        else
        {
            KPrimaryAction_B = PlayerPrefs.GetString("KPrimaryAction_B");
        }

        if (!PlayerPrefs.HasKey("KPrimaryAction_X"))
        {
            PlayerPrefs.SetString("KPrimaryAction_X", KPrimaryAction_X);
        }
        else
        {
            KPrimaryAction_X = PlayerPrefs.GetString("KPrimaryAction_X");
        }

        if (!PlayerPrefs.HasKey("KPrimaryAction_Y"))
        {
            PlayerPrefs.SetString("KPrimaryAction_Y", KPrimaryAction_Y);
        }
        else
        {
            KPrimaryAction_Y = PlayerPrefs.GetString("KPrimaryAction_Y");
        }
        // Secondaries
        if (!PlayerPrefs.HasKey("KSecondaryAction_A"))
        {
            PlayerPrefs.SetString("KSecondaryAction_A", KSecondaryAction_A);
        }
        else
        {
            KSecondaryAction_A = PlayerPrefs.GetString("KSecondaryAction_A");
        }

        if (!PlayerPrefs.HasKey("KSecondaryAction_B"))
        {
            PlayerPrefs.SetString("KSecondaryAction_B", KSecondaryAction_B);
        }
        else
        {
            KSecondaryAction_B = PlayerPrefs.GetString("KSecondaryAction_B");
        }

        if (!PlayerPrefs.HasKey("KSecondaryAction_X"))
        {
            PlayerPrefs.SetString("KSecondaryAction_X", KSecondaryAction_X);
        }
        else
        {
            KSecondaryAction_Y = PlayerPrefs.GetString("KSecondaryAction_X");
        }

        if (!PlayerPrefs.HasKey("KSecondaryAction_Y"))
        {
            PlayerPrefs.SetString("KSecondaryAction_Y", KSecondaryAction_Y);
        }
        else
        {
            KSecondaryAction_Y = PlayerPrefs.GetString("KSecondaryAction_Y");
        }
        // Tertiaries
        if (!PlayerPrefs.HasKey("KTertiaryAction_A"))
        {
            PlayerPrefs.SetString("KTertiaryAction_A", KTertiaryAction_A);
        }
        else
        {
            KTertiaryAction_A = PlayerPrefs.GetString("KTertiaryAction_A");
        }

        if (!PlayerPrefs.HasKey("KTertiaryAction_B"))
        {
            PlayerPrefs.SetString("KTertiaryAction_B", KTertiaryAction_B);
        }
        else
        {
            KTertiaryAction_B = PlayerPrefs.GetString("KTertiaryAction_B");
        }

        if (!PlayerPrefs.HasKey("KTertiaryAction_X"))
        {
            PlayerPrefs.SetString("KTertiaryAction_X", KTertiaryAction_X);
        }
        else
        {
            KTertiaryAction_X = PlayerPrefs.GetString("KTertiaryAction_X");
        }

        if (!PlayerPrefs.HasKey("KTertiaryAction_Y"))
        {
            PlayerPrefs.SetString("KTertiaryAction_Y", KTertiaryAction_Y);
        }
        else
        {
            KTertiaryAction_Y = PlayerPrefs.GetString("KTertiaryAction_Y");
        }
        // Extras
        if (!PlayerPrefs.HasKey("KAction_Pause"))
        {
            PlayerPrefs.SetString("KAction_Pause", KAction_Pause);
        }
        else
        {
            KAction_Pause = PlayerPrefs.GetString("KAction_Pause");
        }

        if (!PlayerPrefs.HasKey("KAction_Select"))
        {
            PlayerPrefs.SetString("KAction_Select", KAction_Select);
        }
        else
        {
            KAction_Select = PlayerPrefs.GetString("KTertiaryAction_Y");
        }
        // Movement
        if (!PlayerPrefs.HasKey("KMovement_U"))
        {
            PlayerPrefs.SetString("KMovement_U", KMovement_U);
        }
        else
        {
            KAction_Select = PlayerPrefs.GetString("KMovement_U");
        }

        if (!PlayerPrefs.HasKey("KMovement_D"))
        {
            PlayerPrefs.SetString("KMovement_D", KMovement_D);
        }
        else
        {
            KMovement_D = PlayerPrefs.GetString("KMovement_D");
        }

        if (!PlayerPrefs.HasKey("KMovement_R"))
        {
            PlayerPrefs.SetString("KMovement_R", KMovement_R);
        }
        else
        {
            KMovement_R = PlayerPrefs.GetString("KMovement_R");
        }

        if (!PlayerPrefs.HasKey("KMovement_L"))
        {
            PlayerPrefs.SetString("KMovement_L", KMovement_L);
        }
        else
        {
            KMovement_L = PlayerPrefs.GetString("KMovement_L");
        }
        #endregion
        #region controller

        if (!PlayerPrefs.HasKey("PrimaryAction_A"))
        {
            PlayerPrefs.SetInt("PrimaryAction_A", (int)PrimaryAction_A);
        }
        else
        {
            PrimaryAction_A = (controllerButtons)PlayerPrefs.GetInt("PrimaryAction_A");
        }

        if (!PlayerPrefs.HasKey("PrimaryAction_B"))
        {
            PlayerPrefs.SetInt("PrimaryAction_B", (int)PrimaryAction_B);
        }
        else
        {
            PrimaryAction_B = (controllerButtons)PlayerPrefs.GetInt("PrimaryAction_B");
        }

        if (!PlayerPrefs.HasKey("PrimaryAction_X"))
        {
            PlayerPrefs.SetInt("PrimaryAction_X", (int)PrimaryAction_X);
        }
        else
        {
            PrimaryAction_X = (controllerButtons)PlayerPrefs.GetInt("PrimaryAction_X");
        }

        if (!PlayerPrefs.HasKey("PrimaryAction_Y"))
        {
            PlayerPrefs.SetInt("PrimaryAction_Y", (int)PrimaryAction_Y);
        }
        else
        {
            PrimaryAction_Y = (controllerButtons)PlayerPrefs.GetInt("PrimaryAction_Y");
        }
        // Secondaries

        if (!PlayerPrefs.HasKey("SecondaryAction_A"))
        {
            PlayerPrefs.SetInt("SecondaryAction_A", (int)SecondaryAction_A);
        }
        else
        {
            SecondaryAction_A = (controllerButtons)PlayerPrefs.GetInt("SecondaryAction_A");
        }

        if (!PlayerPrefs.HasKey("SecondaryAction_B"))
        {
            PlayerPrefs.SetInt("SecondaryAction_B", (int)SecondaryAction_B);
        }
        else
        {
            SecondaryAction_B = (controllerButtons)PlayerPrefs.GetInt("SecondaryAction_B");
        }

        if (!PlayerPrefs.HasKey("SecondaryAction_X"))
        {
            PlayerPrefs.SetInt("SecondaryAction_X", (int)SecondaryAction_X);
        }
        else
        {
            SecondaryAction_X = (controllerButtons)PlayerPrefs.GetInt("SecondaryAction_X");
        }

        if (!PlayerPrefs.HasKey("SecondaryAction_Y"))
        {
            PlayerPrefs.SetInt("SecondaryAction_Y", (int)SecondaryAction_Y);
        }
        else
        {
            SecondaryAction_Y = (controllerButtons)PlayerPrefs.GetInt("SecondaryAction_Y");
        }
        // Tertiaries

        if (!PlayerPrefs.HasKey("TertiaryAction_A"))
        {
            PlayerPrefs.SetInt("TertiaryAction_A", (int)TertiaryAction_A);
        }
        else
        {
            TertiaryAction_A = (controllerButtons)PlayerPrefs.GetInt("TertiaryAction_A");
        }

        if (!PlayerPrefs.HasKey("TertiaryAction_B"))
        {
            PlayerPrefs.SetInt("TertiaryAction_B", (int)TertiaryAction_B);
        }
        else
        {
            TertiaryAction_B = (controllerButtons)PlayerPrefs.GetInt("TertiaryAction_B");
        }

        if (!PlayerPrefs.HasKey("TertiaryAction_X"))
        {
            PlayerPrefs.SetInt("TertiaryAction_X", (int)TertiaryAction_X);
        }
        else
        {
            TertiaryAction_X = (controllerButtons)PlayerPrefs.GetInt("TertiaryAction_X");
        }
        if (!PlayerPrefs.HasKey("TertiaryAction_Y"))
        {
            PlayerPrefs.SetInt("TertiaryAction_Y", (int)TertiaryAction_Y);
        }
        else
        {
            TertiaryAction_Y = (controllerButtons)PlayerPrefs.GetInt("TertiaryAction_Y");
        }
        //extras

        if (!PlayerPrefs.HasKey("Action_Pause"))
        {
            PlayerPrefs.SetInt("Action_Pause", (int)Action_Pause);
        }
        else
        {
            Action_Pause = (controllerButtons)PlayerPrefs.GetInt("Action_Pause");
        }

        if (!PlayerPrefs.HasKey("Action_Select"))
        {
            PlayerPrefs.SetInt("Action_Select", (int)Action_Select);
        }
        else
        {
            Action_Select = (controllerButtons)PlayerPrefs.GetInt("Action_Select");
        }

        #endregion

    }
}
