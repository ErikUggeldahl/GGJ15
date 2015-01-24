using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using XInputDotNetPure;

namespace MPInput
{
    [Serializable]
    public class MP_InputAction
    {
        public MP_InputAction(string aName)
        {
            ActionName = aName;
        }
        public string ActionName = null;
        public List<MP_ControllerInputDefinition> ControllerInputs = new List<MP_ControllerInputDefinition>();
        public List<MP_KeyboardInputDefinition> KeyboardInputs = new List<MP_KeyboardInputDefinition>();
    }

    [Serializable]
    public class MP_KeyboardInputDefinition
    {
        public MP_KeyboardInputDefinition()
        {
        }
        public MP_KeyboardInputDefinition(KeyCode aKey, bool aAxisInvert = false, bool aRequiresShift = false, bool aRequiresCtrl = false, bool aRequiresAlt = false)
        {
            Key = aKey;
            AxisInvert = aAxisInvert;
            RequiresShift = aRequiresShift;
            RequiresCtrl = aRequiresCtrl;
            RequiresAlt = aRequiresAlt;
        }

        public KeyCode Key;
        public bool AxisInvert;

        public bool RequiresShift;
        public bool RequiresCtrl;
        public bool RequiresAlt;
    }

    [Serializable]
    public class MP_ControllerInputDefinition
    {
        public MP_ControllerInputDefinition()
        {
        }
        public MP_ControllerInputDefinition(MP_eInputXboxAxial aAxial, bool aAxisInvert = false)
        {
            Axial = aAxial;
            AxisInvert = aAxisInvert;
        }

        public MP_eInputXboxAxial Axial;
        public bool AxisInvert;
    }

    public class MP_InputDeviceInfo
    {
        public MP_InputDeviceInfo(MP_eInputType Device, int Index)
        {
            this.Device = Device;
            this.Index = Index;
        }

        public MP_eInputType Device;
        public int Index;
    }

    public enum MP_eInputType
    {
        None = 0,
        Controller,
        Keyboard,
    }

    public enum MP_eInputXboxAxial
    {
        None = 0,
        AButton,
        BButton,
        XButton,
        YButton,
        RightTrigger,
        LeftTrigger,
        LeftBumper,
        RightBumper,
        LeftStickButton,
        RightStickButton,
        LeftStickXAxis,
        LeftStickYAxis,
        RightStickXAxis,
        RightStickYAxis,
        StartButton,
        BackButton,
        DpadUp,
        DpadDown,
        DpadLeft,
        DpadRight
    }

    public class MP_Input : MonoBehaviour
    {
        public static MP_InputConfig Config;

        public const string CONFIG_DIRECTORY = "Assets/MP_Input/Resources/";
        public const string CONFIG_FILENAME = "MP_InputConfig";
        const float CHECK_FOR_CONNECTED_CONTROLLER_INTERVAL = 1.0f;
        const bool DEBUG_INPUT = true;
        const float AXIS_DEADZONE = 0.2f;

        static MP_Input instance = null;
        static MP_Input Instance { get { return instance; } }
        public static MP_Input CreateInstance()
        {
            if (!Application.isPlaying)
                return null;

            if (instance == null)
            {
                instance = new GameObject("MP_Input", typeof(MP_Input)).GetComponent<MP_Input>();
                //instance.hideFlags = HideFlags.HideAndDontSave;
            }
            return instance;
        }



        static bool[] controllersConnected = new bool[4] { false, false, false, false };
        static float lastConnectedCheckTime = 0.1f;
        static int lastConnectededControllerCheck = 0;

        static GamePadState[] currentStates = null;
        static GamePadState[] previousStates = null;

        static float lastStateUpdate = -0.1f;

        static float axisAsButtonDeadzone = 0.5f;

        static MP_InputDeviceInfo workingDevice = new MP_InputDeviceInfo(MP_eInputType.None, 0);

        // Public Interface
        #region publicInterface
        public static bool GetAnyButtonDown(string Action, out MP_InputDeviceInfo Device)
        {
            if (!Application.isPlaying)
            {
                Device = null;
                return false;
            }

            if (currentStates == null)
            {
                CreateInstance();
            }

            workingDevice.Device = MP_eInputType.Controller;
            for (int i = 0; i < currentStates.Length; i++)
            {
                workingDevice.Index = i;
                if (GetButtonDown(Action, workingDevice))
                {
                    Device = new MP_InputDeviceInfo(workingDevice.Device, workingDevice.Index);
                    return true;
                }
            }

            workingDevice.Device = MP_eInputType.Keyboard;
            for (int i = 0; i < 2; i++)
            {
                workingDevice.Index = i;
                if (GetButtonDown(Action, workingDevice))
                {
                    Device = new MP_InputDeviceInfo(workingDevice.Device, workingDevice.Index);
                    return true;
                }
            }

            Device = null;
            return false;
        }

        public static bool GetAnyButtonDown(string Action)
        {
            if (!Application.isPlaying)
                return false;

            MP_InputDeviceInfo discard = new MP_InputDeviceInfo(MP_eInputType.None, 0);
            return GetAnyButtonDown(Action, out discard);
        }

        public static float GetAxis(string Action, MP_InputDeviceInfo Device)
        {
            if (!Application.isPlaying)
                return 0;

            switch (Device.Device)
            {
                case MP_eInputType.Controller: return GetControllerAxis(FindAction(Action), Device.Index);
                case MP_eInputType.Keyboard: return GetKeyboardAxis(FindAction(Action), Device.Index);
                default: return 0;
            }
        }

        public static bool GetButtonDown(string Action, MP_InputDeviceInfo Device)
        {
            if (!Application.isPlaying)
                return false;

            switch (Device.Device)
            {
                case MP_eInputType.Controller: return GetControllerButtonDown(FindAction(Action), Device.Index);
                case MP_eInputType.Keyboard: return GetKeyboardButtonDown(FindAction(Action), Device.Index);
                default: return false;
            }
        }

        public static bool GetButtonUp(string Action, MP_InputDeviceInfo Device)
        {
            if (!Application.isPlaying)
                return false;

            switch (Device.Device)
            {
                case MP_eInputType.Controller: return GetControllerButtonUp(FindAction(Action), Device.Index);
                case MP_eInputType.Keyboard: return GetKeyboardButtonUp(FindAction(Action), Device.Index);
                default: return false;
            }
        }

        public static bool GetButton(string Action, MP_InputDeviceInfo Device)
        {
            if (!Application.isPlaying)
                return false;

            switch (Device.Device)
            {
                case MP_eInputType.Controller: return GetControllerButton(FindAction(Action), Device.Index);
                case MP_eInputType.Keyboard: return GetKeyboardButton(FindAction(Action), Device.Index);
                default: return false;
            }
        }

        public static bool[] GetConnectedControllers()
        {
            return controllersConnected;
        }

        #endregion

        // Internal functions
        #region internalFunctions

        void Update()
        {
            RefreshGamepadSates();

            if (DEBUG_INPUT)
            {
                for (int i = 0; i < Config.InputActions.Count; i++)
                {
                    if (MP_Input.GetAnyButtonDown(Config.InputActions[i].ActionName))
                        Debug.Log(Config.InputActions[i].ActionName);
                }
            }
        }

        void OnGUI()
        {
            if (Application.isEditor && DEBUG_INPUT) // or check the app debug flag
            {
                string debugText = "";

                //debugText += "Keyboard " + 0 + ":\n";
                //for (int ii = 0; ii < Config.InputActions.Count; ii++)
                //{
                //    debugText += Config.InputActions[ii].ActionName + ": " + MP_Input.GetButton("Config.InputActions[i].ActionName", new MP_InputDeviceInfo(MP_eInputType.Keyboard, 0)) + "\n";
                //}
                //debugText += "\n";

                for (int i = 0; i < controllersConnected.Length; i++)
                {
                    if (controllersConnected[i] == true)
                    {
                        debugText += "Controller " + i + ":\n";
                        for (int ii = 0; ii < Config.InputActions.Count; ii++)
                        {
                            debugText += Config.InputActions[ii].ActionName + ": " + MP_Input.GetButton("Config.InputActions[i].ActionName", new MP_InputDeviceInfo(MP_eInputType.Controller, i)) + "\n";
                        }
                    }
                }
                GUI.Label(new Rect(0, 0, Screen.width, Screen.height), debugText);
            }
        }

        void OnEnable()
        {
            gameObject.hideFlags = HideFlags.HideInHierarchy;
            DontDestroyOnLoad(this.gameObject);
            RefreshGamepadSates(true);
            LoadConfig();
        }

        void OnDisable()
        {
            instance = null;
            currentStates = null;
            previousStates = null;
            controllersConnected = null;
            workingDevice = null;
        }

        private static GamePadState GetXinputGamepadState(PlayerIndex index)
        {
            if (instance == null)
                CreateInstance();

            // Make sure we are within range of indexes, otherwise return null
            if (index >= PlayerIndex.One && index <= PlayerIndex.Four)
            {
                return currentStates[(int)index];
            }
            else
            {
                return currentStates[0];
            }
        }

        private static GamePadState GetPreviousXinputGamepadState(PlayerIndex index)
        {
            // Make sure we are within range of indexes, otherwise return null
            if (index >= PlayerIndex.One && index <= PlayerIndex.Four)
            {
                return previousStates[(int)index];
            }
            else
            {
                return previousStates[0];
            }
        }

        void RefreshGamepadSates(bool RefreshNotConnectedStates = false)
        {
            if (instance = this)
            {
                ForceUpdateGamepadStates(RefreshNotConnectedStates);
            }
        }

        static void ForceUpdateGamepadStates(bool RefreshNotConnectedStates)
        {
            if (!Application.isPlaying)
                return;

            if (currentStates == null)
            {
                currentStates = new GamePadState[4];
            }
            if (previousStates == null)
            {
                previousStates = new GamePadState[4];
            }

            if (RefreshNotConnectedStates)
            {
                for (int i = 0; i < 4; i++)
                {
                    controllersConnected[i] = GamePad.GetState((PlayerIndex)i).IsConnected;
                }
            }
            else if (Time.realtimeSinceStartup - lastConnectedCheckTime > CHECK_FOR_CONNECTED_CONTROLLER_INTERVAL)
            {
                controllersConnected[lastConnectededControllerCheck] = GamePad.GetState((PlayerIndex)lastConnectededControllerCheck).IsConnected;
                lastConnectedCheckTime = Time.realtimeSinceStartup;
                if (DEBUG_INPUT == true)
                    Debug.Log("Checked Controller " + lastConnectededControllerCheck + " connected: " + controllersConnected[lastConnectededControllerCheck]);
                lastConnectededControllerCheck = (lastConnectededControllerCheck + 1) % 4;
            }

            for (int i = 0; i < 4; i++)
            {
                if (controllersConnected[i] == true)
                {
                    previousStates[i] = currentStates[i];
                    currentStates[i] = GamePad.GetState((PlayerIndex)i);
                }
            }

            // Log the time that we updated.
            lastStateUpdate = Time.realtimeSinceStartup;
        }

        static float GetControllerAxis(MP_InputAction Action, int Index)
        {
            if (Action == null) return 0;

            float value = 0.0f;

            for (int i = 0; i < Action.ControllerInputs.Count; i++)
            {
                value += AdjustForDeadzone(GetXinputAxis(Action.ControllerInputs[i].Axial, Index, Action.ControllerInputs[i].AxisInvert));
            }
            return Mathf.Clamp(value, -1, 1);
        }

        static float GetKeyboardAxis(MP_InputAction Action, int Index)
        {
            if (Action == null) return 0;

            bool Pos = false;
            bool Neg = false;

            for (int i = 0; i < Action.ControllerInputs.Count; i++)
            {
                if (Input.GetKey(Action.KeyboardInputs[i].Key))
                {
                    if (Action.KeyboardInputs[i].AxisInvert)
                        Neg = true;
                    else Pos = true;
                }
            }
            return (Pos ? 1 : 0) + (Neg ? -1 : 0);
        }

        static float AdjustForDeadzone(float Axis)
        {
            if (Mathf.Abs(Axis) < AXIS_DEADZONE)
                return 0;
            else
            {
                float newAxis = (Mathf.Abs(Axis) - AXIS_DEADZONE) / (1 - AXIS_DEADZONE);
                newAxis = newAxis * Mathf.Sign(Axis);
                return newAxis;
            }
            //return Axis;
        }

        static bool GetControllerButtonDown(MP_InputAction Action, int Index)
        {
            if (Action == null) return false;
            // Return true if any of the buttons bound to that action just came up.
            for (int i = 0; i < Action.ControllerInputs.Count; i++)
            {
                if (GetXinputButton(Action.ControllerInputs[i].Axial, Index, false, Action.ControllerInputs[i].AxisInvert) && !GetXinputButton(Action.ControllerInputs[i].Axial, Index, true, Action.ControllerInputs[i].AxisInvert))
                    return true;
            }
            return false;
        }

        static bool GetKeyboardButtonDown(MP_InputAction Action, int Index)
        {
            if (Action == null) return false;
            for (int i = 0; i < Action.KeyboardInputs.Count; i++)
            {
                if (Input.GetKeyDown(Action.KeyboardInputs[i].Key))
                    return true;
            }
            return false;
        }

        static bool GetControllerButtonUp(MP_InputAction Action, int Index)
        {
            if (Action == null) return false;
            // Return true if any of the buttons bound to that action just came up.
            for (int i = 0; i < Action.ControllerInputs.Count; i++)
            {
                if (GetXinputButton(Action.ControllerInputs[i].Axial, Index, true, Action.ControllerInputs[i].AxisInvert) && !GetXinputButton(Action.ControllerInputs[i].Axial, Index, false, Action.ControllerInputs[i].AxisInvert))
                    return true;
            }
            return false;
        }

        static bool GetKeyboardButtonUp(MP_InputAction Action, int Index)
        {
            if (Action == null) return false;
            for (int i = 0; i < Action.KeyboardInputs.Count; i++)
            {
                if (Input.GetKeyUp(Action.KeyboardInputs[i].Key))
                    return true;
            }
            return false;
        }

        static bool GetControllerButton(MP_InputAction Action, int Index)
        {
            if (Action == null) return false;
            // Return true if any of the buttons bound to that action are down.
            for (int i = 0; i < Action.ControllerInputs.Count; i++)
            {
                if (GetXinputButton(Action.ControllerInputs[i].Axial, Index, false, Action.ControllerInputs[i].AxisInvert))
                    return true;
            }
            return false;
        }

        static bool GetKeyboardButton(MP_InputAction Action, int Index)
        {
            if (Action == null) return false;
            // Return true if any of the buttons bound to that action are down.
            for (int i = 0; i < Action.KeyboardInputs.Count; i++)
            {
                if (Input.GetKey(Action.KeyboardInputs[i].Key))
                    return true;
            }
            return false;
        }

        static bool GetXinputButton(MP_eInputXboxAxial Axial, int Index, bool usePreviousState, bool inverse = false)
        {
            if (controllersConnected != null && controllersConnected[Index])
            {
                GamePadState gamepadState;

                if (usePreviousState)
                    gamepadState = previousStates[Index];
                else
                    gamepadState = currentStates[Index];

                switch (Axial)
                {
                    case MP_eInputXboxAxial.AButton: return (gamepadState.Buttons.A == ButtonState.Pressed);
                    case MP_eInputXboxAxial.BButton: return (gamepadState.Buttons.B == ButtonState.Pressed);
                    case MP_eInputXboxAxial.XButton: return (gamepadState.Buttons.X == ButtonState.Pressed);
                    case MP_eInputXboxAxial.YButton: return (gamepadState.Buttons.Y == ButtonState.Pressed);
                    case MP_eInputXboxAxial.LeftBumper: return (gamepadState.Buttons.LeftShoulder == ButtonState.Pressed);
                    case MP_eInputXboxAxial.RightBumper: return (gamepadState.Buttons.RightShoulder == ButtonState.Pressed);
                    case MP_eInputXboxAxial.LeftStickButton: return (gamepadState.Buttons.LeftStick == ButtonState.Pressed);
                    case MP_eInputXboxAxial.RightStickButton: return (gamepadState.Buttons.RightStick == ButtonState.Pressed);
                    case MP_eInputXboxAxial.StartButton: return (gamepadState.Buttons.Start == ButtonState.Pressed);
                    case MP_eInputXboxAxial.BackButton: return (gamepadState.Buttons.Back == ButtonState.Pressed);
                    case MP_eInputXboxAxial.DpadUp: return (gamepadState.DPad.Up == ButtonState.Pressed);
                    case MP_eInputXboxAxial.DpadDown: return (gamepadState.DPad.Down == ButtonState.Pressed);
                    case MP_eInputXboxAxial.DpadLeft: return (gamepadState.DPad.Left == ButtonState.Pressed);
                    case MP_eInputXboxAxial.DpadRight: return (gamepadState.DPad.Right == ButtonState.Pressed);
                    case MP_eInputXboxAxial.RightTrigger: return (gamepadState.Triggers.Right > axisAsButtonDeadzone);
                    case MP_eInputXboxAxial.LeftTrigger: return (gamepadState.Triggers.Left > axisAsButtonDeadzone);
                    case MP_eInputXboxAxial.LeftStickXAxis: return ((inverse ? -1.0f: 1.0f) * gamepadState.ThumbSticks.Left.X > axisAsButtonDeadzone);
                    case MP_eInputXboxAxial.LeftStickYAxis: return ((inverse ? -1.0f : 1.0f) * gamepadState.ThumbSticks.Left.Y > axisAsButtonDeadzone);
                    case MP_eInputXboxAxial.RightStickXAxis: return ((inverse ? -1.0f : 1.0f) * gamepadState.ThumbSticks.Right.X > axisAsButtonDeadzone);
                    case MP_eInputXboxAxial.RightStickYAxis: return ((inverse ? -1.0f : 1.0f) * gamepadState.ThumbSticks.Right.Y > axisAsButtonDeadzone);
                    default: return false;
                }
            }
            else return false;
        }

        static float GetXinputAxis(MP_eInputXboxAxial Axial, int Index, bool inverse = false)
        {
            if (currentStates == null)
                return 0;

            GamePadState gamepadState = currentStates[Index];

            switch (Axial)
            {
                case MP_eInputXboxAxial.AButton: return (gamepadState.Buttons.A == ButtonState.Pressed) ? (inverse ? -1.0f : 1.0f) : 0.0f;
                case MP_eInputXboxAxial.BButton: return (gamepadState.Buttons.B == ButtonState.Pressed) ? (inverse ? -1.0f : 1.0f) : 0.0f;
                case MP_eInputXboxAxial.XButton: return (gamepadState.Buttons.X == ButtonState.Pressed) ? (inverse ? -1.0f : 1.0f) : 0.0f;
                case MP_eInputXboxAxial.YButton: return (gamepadState.Buttons.Y == ButtonState.Pressed) ? (inverse ? -1.0f : 1.0f) : 0.0f;
                case MP_eInputXboxAxial.LeftBumper: return (gamepadState.Buttons.LeftShoulder == ButtonState.Pressed) ? (inverse ? -1.0f : 1.0f) : 0.0f;
                case MP_eInputXboxAxial.RightBumper: return (gamepadState.Buttons.RightShoulder == ButtonState.Pressed) ? (inverse ? -1.0f : 1.0f) : 0.0f;
                case MP_eInputXboxAxial.LeftStickButton: return (gamepadState.Buttons.LeftStick == ButtonState.Pressed) ? (inverse ? -1.0f : 1.0f) : 0.0f;
                case MP_eInputXboxAxial.RightStickButton: return (gamepadState.Buttons.RightStick == ButtonState.Pressed) ? (inverse ? -1.0f : 1.0f) : 0.0f;
                case MP_eInputXboxAxial.StartButton: return (gamepadState.Buttons.Start == ButtonState.Pressed) ? (inverse ? -1.0f : 1.0f) : 0.0f;
                case MP_eInputXboxAxial.BackButton: return (gamepadState.Buttons.Back == ButtonState.Pressed) ? (inverse ? -1.0f : 1.0f) : 0.0f;
                case MP_eInputXboxAxial.DpadUp: return (gamepadState.DPad.Up == ButtonState.Pressed) ? (inverse ? -1.0f : 1.0f) : 0.0f;
                case MP_eInputXboxAxial.DpadDown: return (gamepadState.DPad.Down == ButtonState.Pressed) ? (inverse ? -1.0f : 1.0f) : 0.0f;
                case MP_eInputXboxAxial.DpadLeft: return (gamepadState.DPad.Left == ButtonState.Pressed) ? (inverse ? -1.0f : 1.0f) : 0.0f;
                case MP_eInputXboxAxial.DpadRight: return (gamepadState.DPad.Right == ButtonState.Pressed) ? (inverse ? -1.0f : 1.0f) : 0.0f;
                case MP_eInputXboxAxial.RightTrigger: return gamepadState.Triggers.Right;
                case MP_eInputXboxAxial.LeftTrigger: return gamepadState.Triggers.Left;
                case MP_eInputXboxAxial.LeftStickXAxis: return (inverse ? -1.0f : 1.0f) * gamepadState.ThumbSticks.Left.X;
                case MP_eInputXboxAxial.LeftStickYAxis: return (inverse ? -1.0f : 1.0f) * gamepadState.ThumbSticks.Left.Y;
                case MP_eInputXboxAxial.RightStickXAxis: return (inverse ? -1.0f : 1.0f) * gamepadState.ThumbSticks.Right.X;
                case MP_eInputXboxAxial.RightStickYAxis: return (inverse ? -1.0f : 1.0f) * gamepadState.ThumbSticks.Right.Y;
                default: return 0;
            }
        }

        static MP_InputAction FindAction(string aActionName)
        {
            if (Config != null)
            {
                if (Config.InputActions.Count == 0)
                    CreateInstance();

                for (int i = 0; i < Config.InputActions.Count; i++)
                {
                    // TODO: Ignore case
                    if (Config.InputActions[i].ActionName.Equals(aActionName))
                        return Config.InputActions[i];
                }
            }
            return null;
        }

        #endregion

        public static void LoadConfig()
        {
            Config = Resources.Load<MP_InputConfig>(CONFIG_FILENAME);
        }
    }
}