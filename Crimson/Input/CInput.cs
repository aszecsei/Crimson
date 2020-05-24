#region Using Statements

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

#endregion

namespace Crimson.Input
{
    // Mercilessly cribbed from Matt Thorson's Monocle
    public static class CInput
    {
        internal static List<VirtualInput> VirtualInputs;

        /// <summary>
        ///     True if input should be enabled, false otherwise.
        /// </summary>
        public static bool isEnabled = true;

        /// <summary>
        ///     Represents the input information from the currently attached keyboard.
        /// </summary>
        public static KeyboardData keyboardData { get; private set; }

        /// <summary>
        ///     Represents the input information from the currently attached mouse.
        /// </summary>
        public static MouseData mouseData { get; private set; }

        /// <summary>
        ///     Represents the input information from all currently attached controllers.
        /// </summary>
        public static GamePadData[] gamePadData { get; private set; }

        /// <summary>
        ///     False if input should be enabled, true otherwise.
        /// </summary>
        public static bool isDisabled
        {
            get => !isEnabled;
            set => isEnabled = !value;
        }

        /// <summary>
        ///     Initialize all input systems.
        /// </summary>
        public static void Initialize()
        {
            // Init devices
            keyboardData = new KeyboardData();
            mouseData = new MouseData();
            gamePadData = new GamePadData[4];
            for (var i = 0; i < 4; i++) gamePadData[i] = new GamePadData((PlayerIndex) i);

            VirtualInputs = new List<VirtualInput>();
        }

        /// <summary>
        ///     Shuts down all input systems.
        /// </summary>
        public static void Shutdown()
        {
            foreach (var gamepad in gamePadData) gamepad.StopRumble();
        }

        /// <summary>
        ///     Updates all input systems.
        /// </summary>
        public static void Update()
        {
            keyboardData.Update();
            mouseData.Update();
            for (var i = 0; i < 4; i++) gamePadData[i].Update();

            UpdateVirtualInputs();
        }

        /// <summary>
        ///     Updates all input system with empty states.
        /// </summary>
        public static void UpdateNull()
        {
            keyboardData.UpdateNull();
            mouseData.UpdateNull();
            for (var i = 0; i < 4; i++) gamePadData[i].UpdateNull();

            UpdateVirtualInputs();
        }

        private static void UpdateVirtualInputs()
        {
            foreach (var virtualInput in VirtualInputs) virtualInput.Update();
        }

        #region Keyboard

        public class KeyboardData
        {
            public KeyboardState CurrentState;
            public KeyboardState PreviousState;

            public KeyboardData()
            {
                PreviousState = new KeyboardState();
                CurrentState = new KeyboardState();
            }

            public void Update()
            {
                PreviousState = CurrentState;
                CurrentState = Keyboard.GetState();
            }

            public void UpdateNull()
            {
                PreviousState = CurrentState;
                CurrentState = new KeyboardState();
            }

            #region Axis

            public int AxisCheck(Keys negative, Keys positive, int both = 0)
            {
                if (Check(negative))
                {
                    if (Check(positive)) return both;

                    return -1;
                }

                if (Check(positive)) return 1;

                return 0;
            }

            #endregion

            #region Basic Checks

            public bool Check(Keys key)
            {
                if (isDisabled) return false;

                return CurrentState.IsKeyDown(key);
            }

            public bool Pressed(Keys key)
            {
                if (isDisabled) return false;

                return CurrentState.IsKeyDown(key) && PreviousState.IsKeyUp(key);
            }

            public bool Pressed(params Keys[] keys)
            {
                if (isDisabled) return false;

                foreach (var key in keys)
                    if (Pressed(key))
                        return true;

                return false;
            }

            public bool Released(Keys key)
            {
                if (isDisabled) return false;

                return CurrentState.IsKeyUp(key) && PreviousState.IsKeyDown(key);
            }

            #endregion
        }

        #endregion

        #region Mouse

        public class MouseData
        {
            public MouseState CurrentState;
            public MouseState PreviousState;

            public MouseData()
            {
                PreviousState = new MouseState();
                CurrentState = new MouseState();
            }

            public void Update()
            {
                PreviousState = CurrentState;
                CurrentState = Mouse.GetState();
            }

            public void UpdateNull()
            {
                PreviousState = CurrentState;
                CurrentState = new MouseState();
            }

            #region Buttons

            public bool CheckLeftButton => CurrentState.LeftButton == ButtonState.Pressed;

            public bool CheckRightButton => CurrentState.RightButton == ButtonState.Pressed;

            public bool CheckMiddleButton => CurrentState.MiddleButton == ButtonState.Pressed;

            public bool PressedLeftButton => CurrentState.LeftButton == ButtonState.Pressed &&
                                             PreviousState.LeftButton == ButtonState.Released;

            public bool PressedRightButton => CurrentState.RightButton == ButtonState.Pressed &&
                                              PreviousState.RightButton == ButtonState.Released;

            public bool PressedMiddleButton => CurrentState.MiddleButton == ButtonState.Pressed &&
                                               PreviousState.MiddleButton == ButtonState.Released;

            public bool ReleasedLeftButton => CurrentState.LeftButton == ButtonState.Released &&
                                              PreviousState.LeftButton == ButtonState.Pressed;

            public bool ReleasedRightButton => CurrentState.RightButton == ButtonState.Released &&
                                               PreviousState.RightButton == ButtonState.Pressed;

            public bool ReleasedMiddleButton => CurrentState.MiddleButton == ButtonState.Released &&
                                                PreviousState.MiddleButton == ButtonState.Pressed;

            #endregion

            #region Wheel

            public int Wheel => CurrentState.ScrollWheelValue;

            public int WheelDelta => CurrentState.ScrollWheelValue - PreviousState.ScrollWheelValue;

            #endregion

            #region Position

            public bool WasMoved => CurrentState.X != PreviousState.X || CurrentState.Y != PreviousState.Y;

            public float X
            {
                get => Position.X;
                set => Position = new Vector2(value, Position.Y);
            }

            public float Y
            {
                get => Position.Y;
                set => Position = new Vector2(Position.X, value);
            }

            public Vector2 Position
            {
                get => Vector2.Transform(
                    new Vector2(CurrentState.X - Engine.Viewport.X, CurrentState.Y - Engine.Viewport.Y),
                    Matrix.Invert(Engine.ScreenMatrix));
                set
                {
                    var vector = Vector2.Transform(value, Engine.ScreenMatrix);
                    Mouse.SetPosition(Mathf.RoundToInt(vector.X), Mathf.RoundToInt(vector.Y));
                }
            }

            public Vector2 RawPosition =>
                new Vector2(CurrentState.X - Engine.Viewport.X, CurrentState.Y - Engine.Viewport.Y);

            #endregion
        }

        #endregion

        #region GamePads

        public class GamePadData
        {
            private float _rumbleStrength;
            private float _rumbleTime;
            public bool Attached;
            public GamePadState CurrentState;
            public GamePadState PreviousState;

            public GamePadData(PlayerIndex playerIndex)
            {
                PlayerIndex = playerIndex;
            }

            public PlayerIndex PlayerIndex { get; }

            public void Update()
            {
                PreviousState = CurrentState;
                CurrentState = GamePad.GetState(PlayerIndex);
                Attached = CurrentState.IsConnected;

                if (_rumbleTime > 0)
                {
                    _rumbleTime -= Time.DeltaTime;
                    if (_rumbleTime <= 0) GamePad.SetVibration(PlayerIndex, 0, 0);
                }
            }

            public void UpdateNull()
            {
                PreviousState = CurrentState;
                CurrentState = new GamePadState();
                Attached = GamePad.GetState(PlayerIndex).IsConnected;

                if (_rumbleTime > 0) _rumbleTime -= Time.DeltaTime;

                GamePad.SetVibration(PlayerIndex, 0, 0);
            }

            public void Rumble(float strength, float time)
            {
                if (_rumbleTime <= 0 || strength > _rumbleStrength ||
                    strength == _rumbleStrength && time > _rumbleTime)
                {
                    GamePad.SetVibration(PlayerIndex, strength, strength);
                    _rumbleStrength = strength;
                    _rumbleTime = time;
                }
            }

            public void StopRumble()
            {
                GamePad.SetVibration(PlayerIndex, 0, 0);
                _rumbleTime = 0;
            }

            #region Buttons

            public bool Check(Buttons button)
            {
                if (isDisabled) return false;

                return CurrentState.IsButtonDown(button);
            }

            public bool Pressed(Buttons button)
            {
                if (isDisabled) return false;

                return CurrentState.IsButtonDown(button) && PreviousState.IsButtonUp(button);
            }

            public bool Released(Buttons button)
            {
                if (isDisabled) return false;

                return CurrentState.IsButtonUp(button) && PreviousState.IsButtonDown(button);
            }

            #endregion

            #region Sticks

            public Vector2 GetLeftStick()
            {
                var ret = CurrentState.ThumbSticks.Left;
                ret.Y = -ret.Y;
                return ret;
            }

            public Vector2 GetLeftStick(float deadzone)
            {
                var ret = CurrentState.ThumbSticks.Left;
                if (ret.LengthSquared() < deadzone * deadzone)
                    ret = Vector2.Zero;
                else
                    ret.Y = -ret.Y;

                return ret;
            }

            public Vector2 GetRightStick()
            {
                var ret = CurrentState.ThumbSticks.Right;
                ret.Y = -ret.Y;
                return ret;
            }

            public Vector2 GetRightStick(float deadzone)
            {
                var ret = CurrentState.ThumbSticks.Right;
                if (ret.LengthSquared() < deadzone * deadzone)
                    ret = Vector2.Zero;
                else
                    ret.Y = -ret.Y;

                return ret;
            }

            #region Left Stick Directions

            public bool LeftStickLeftCheck(float deadzone)
            {
                return CurrentState.ThumbSticks.Left.X <= -deadzone;
            }

            public bool LeftStickLeftPressed(float deadzone)
            {
                return CurrentState.ThumbSticks.Left.X <= -deadzone && PreviousState.ThumbSticks.Left.X > -deadzone;
            }

            public bool LeftStickLeftReleased(float deadzone)
            {
                return CurrentState.ThumbSticks.Left.X > -deadzone && PreviousState.ThumbSticks.Left.X <= -deadzone;
            }

            public bool LeftStickRightCheck(float deadzone)
            {
                return CurrentState.ThumbSticks.Left.X >= deadzone;
            }

            public bool LeftStickRightPressed(float deadzone)
            {
                return CurrentState.ThumbSticks.Left.X >= deadzone && PreviousState.ThumbSticks.Left.X < deadzone;
            }

            public bool LeftStickRightReleased(float deadzone)
            {
                return CurrentState.ThumbSticks.Left.X < deadzone && PreviousState.ThumbSticks.Left.X >= deadzone;
            }

            public bool LeftStickDownCheck(float deadzone)
            {
                return CurrentState.ThumbSticks.Left.Y <= -deadzone;
            }

            public bool LeftStickDownPressed(float deadzone)
            {
                return CurrentState.ThumbSticks.Left.Y <= -deadzone && PreviousState.ThumbSticks.Left.Y > -deadzone;
            }

            public bool LeftStickDownReleased(float deadzone)
            {
                return CurrentState.ThumbSticks.Left.Y > -deadzone && PreviousState.ThumbSticks.Left.Y <= -deadzone;
            }

            public bool LeftStickUpCheck(float deadzone)
            {
                return CurrentState.ThumbSticks.Left.Y >= deadzone;
            }

            public bool LeftStickUpPressed(float deadzone)
            {
                return CurrentState.ThumbSticks.Left.Y >= deadzone && PreviousState.ThumbSticks.Left.Y < deadzone;
            }

            public bool LeftStickUpReleased(float deadzone)
            {
                return CurrentState.ThumbSticks.Left.Y < deadzone && PreviousState.ThumbSticks.Left.Y >= deadzone;
            }

            public float LeftStickHorizontal(float deadzone)
            {
                var h = CurrentState.ThumbSticks.Left.X;
                if (Mathf.Abs(h) < deadzone) return 0;

                return h;
            }

            public float LeftStickVertical(float deadzone)
            {
                var v = CurrentState.ThumbSticks.Left.Y;
                if (Mathf.Abs(v) < deadzone) return 0;

                return -v;
            }

            #endregion

            #region Right Stick Directions

            public bool RightStickLeftCheck(float deadzone)
            {
                return CurrentState.ThumbSticks.Right.X <= -deadzone;
            }

            public bool RightStickLeftPressed(float deadzone)
            {
                return CurrentState.ThumbSticks.Right.X <= -deadzone && PreviousState.ThumbSticks.Right.X > -deadzone;
            }

            public bool RightStickLeftReleased(float deadzone)
            {
                return CurrentState.ThumbSticks.Right.X > -deadzone && PreviousState.ThumbSticks.Right.X <= -deadzone;
            }

            public bool RightStickRightCheck(float deadzone)
            {
                return CurrentState.ThumbSticks.Right.X >= deadzone;
            }

            public bool RightStickRightPressed(float deadzone)
            {
                return CurrentState.ThumbSticks.Right.X >= deadzone && PreviousState.ThumbSticks.Right.X < deadzone;
            }

            public bool RightStickRightReleased(float deadzone)
            {
                return CurrentState.ThumbSticks.Right.X < deadzone && PreviousState.ThumbSticks.Right.X >= deadzone;
            }

            public bool RightStickUpCheck(float deadzone)
            {
                return CurrentState.ThumbSticks.Right.Y <= -deadzone;
            }

            public bool RightStickUpPressed(float deadzone)
            {
                return CurrentState.ThumbSticks.Right.Y <= -deadzone && PreviousState.ThumbSticks.Right.Y > -deadzone;
            }

            public bool RightStickUpReleased(float deadzone)
            {
                return CurrentState.ThumbSticks.Right.Y > -deadzone && PreviousState.ThumbSticks.Right.Y <= -deadzone;
            }

            public bool RightStickDownCheck(float deadzone)
            {
                return CurrentState.ThumbSticks.Right.Y >= deadzone;
            }

            public bool RightStickDownPressed(float deadzone)
            {
                return CurrentState.ThumbSticks.Right.Y >= deadzone && PreviousState.ThumbSticks.Right.Y < deadzone;
            }

            public bool RightStickDownReleased(float deadzone)
            {
                return CurrentState.ThumbSticks.Right.Y < deadzone && PreviousState.ThumbSticks.Right.Y >= deadzone;
            }

            public float RightStickHorizontal(float deadzone)
            {
                var h = CurrentState.ThumbSticks.Right.X;
                if (Mathf.Abs(h) < deadzone) return 0;

                return h;
            }

            public float RightStickVertical(float deadzone)
            {
                var v = CurrentState.ThumbSticks.Right.Y;
                if (Mathf.Abs(v) < deadzone) return 0;

                return -v;
            }

            #endregion

            #endregion

            #region DPad

            public int DPadHorizontal => CurrentState.DPad.Right == ButtonState.Pressed ? 1 :
                CurrentState.DPad.Left == ButtonState.Pressed ? -1 : 0;

            public int DPadVertical => CurrentState.DPad.Up == ButtonState.Pressed ? 1 :
                CurrentState.DPad.Down == ButtonState.Pressed ? -1 : 0;

            public Vector2 DPad => new Vector2(DPadHorizontal, DPadVertical);

            public bool DPadLeftCheck => CurrentState.DPad.Left == ButtonState.Pressed;

            public bool DPadLeftPressed => CurrentState.DPad.Left == ButtonState.Pressed &&
                                           PreviousState.DPad.Left == ButtonState.Released;

            public bool DPadLeftReleased => CurrentState.DPad.Left == ButtonState.Released &&
                                            PreviousState.DPad.Left == ButtonState.Pressed;

            public bool DPadRightCheck => CurrentState.DPad.Right == ButtonState.Pressed;

            public bool DPadRightPressed => CurrentState.DPad.Right == ButtonState.Pressed &&
                                            PreviousState.DPad.Right == ButtonState.Released;

            public bool DPadRightReleased => CurrentState.DPad.Right == ButtonState.Released &&
                                             PreviousState.DPad.Right == ButtonState.Pressed;

            public bool DPadUpCheck => CurrentState.DPad.Up == ButtonState.Pressed;

            public bool DPadUpPressed => CurrentState.DPad.Up == ButtonState.Pressed &&
                                         PreviousState.DPad.Up == ButtonState.Released;

            public bool DPadUpReleased => CurrentState.DPad.Up == ButtonState.Released &&
                                          PreviousState.DPad.Up == ButtonState.Pressed;

            public bool DPadDownCheck => CurrentState.DPad.Down == ButtonState.Pressed;

            public bool DPadDownPressed => CurrentState.DPad.Down == ButtonState.Pressed &&
                                           PreviousState.DPad.Down == ButtonState.Released;

            public bool DPadDownReleased => CurrentState.DPad.Down == ButtonState.Released &&
                                            PreviousState.DPad.Down == ButtonState.Pressed;

            #endregion

            #region Triggers

            public float LeftTrigger(float deadzone)
            {
                var t = CurrentState.Triggers.Left;
                if (t < deadzone) return 0;

                return t;
            }

            public bool LeftTriggerCheck(float threshold)
            {
                if (isDisabled) return false;

                return CurrentState.Triggers.Left >= threshold;
            }

            public bool LeftTriggerPressed(float threshold)
            {
                if (isDisabled) return false;

                return CurrentState.Triggers.Left >= threshold && PreviousState.Triggers.Left < threshold;
            }

            public bool LeftTriggerReleased(float threshold)
            {
                if (isDisabled) return false;

                return CurrentState.Triggers.Left < threshold && PreviousState.Triggers.Left >= threshold;
            }

            public float RightTrigger(float deadzone)
            {
                var t = CurrentState.Triggers.Right;
                if (t < deadzone) return 0;

                return t;
            }

            public bool RightTriggerCheck(float threshold)
            {
                if (isDisabled) return false;

                return CurrentState.Triggers.Right >= threshold;
            }

            public bool RightTriggerPressed(float threshold)
            {
                if (isDisabled) return false;

                return CurrentState.Triggers.Right >= threshold && PreviousState.Triggers.Right < threshold;
            }

            public bool RightTriggerReleased(float threshold)
            {
                if (isDisabled) return false;

                return CurrentState.Triggers.Right < threshold && PreviousState.Triggers.Right >= threshold;
            }

            #endregion
        }

        #endregion

        #region Helpers

        public static void RumbleFirst(float strength, float time)
        {
            gamePadData[0].Rumble(strength, time);
        }

        public static int Axis(bool negative, bool positive, int bothValue = 0)
        {
            if (negative)
            {
                if (positive) return bothValue;

                return -1;
            }

            if (positive) return 1;

            return 0;
        }

        public static int Axis(float axisValue, float deadzone)
        {
            if (Mathf.Abs(axisValue) >= deadzone) return Mathf.Sign(axisValue);

            return 0;
        }

        public static int Axis(bool negative, bool positive, int bothValue, float axisValue, float deadzone)
        {
            var ret = Axis(axisValue, deadzone);
            if (ret == 0) ret = Axis(negative, positive, bothValue);

            return ret;
        }

        #endregion
    }
}