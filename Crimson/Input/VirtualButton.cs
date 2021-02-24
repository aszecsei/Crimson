using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace Crimson.Input
{
    public class VirtualButton : VirtualInput
    {
        private float bufferCounter;
        public float BufferTime;
        private bool canRepeat;
        private bool consumed;

        private float firstRepeatTime;
        private float multiRepeatTime;
        public List<Node> Nodes;
        private float repeatCounter;

        public VirtualButton(float bufferTime)
        {
            Nodes = new List<Node>();
            BufferTime = bufferTime;
        }

        public VirtualButton()
            : this(0)
        {
        }

        public VirtualButton(float bufferTime, params Node[] nodes)
        {
            Nodes = new List<Node>(nodes);
            BufferTime = bufferTime;
        }

        public VirtualButton(params Node[] nodes)
            : this(0, nodes)
        {
        }

        public bool Repeating { get; private set; }

        public bool Check
        {
            get
            {
                if (CInput.isDisabled) return false;

                foreach (Node node in Nodes)
                    if (node.Check)
                        return true;

                return false;
            }
        }

        public bool Pressed
        {
            get
            {
                if (CInput.isDisabled) return false;

                if (consumed) return false;

                if (bufferCounter > 0 || Repeating) return true;

                foreach (Node node in Nodes)
                    if (node.Pressed)
                        return true;

                return false;
            }
        }

        public bool Released
        {
            get
            {
                if (CInput.isDisabled) return false;

                foreach (Node node in Nodes)
                    if (node.Released)
                        return true;

                return false;
            }
        }

        public void SetRepeat(float repeatTime)
        {
            SetRepeat(repeatTime, repeatTime);
        }

        public void SetRepeat(float firstRepeatTime, float multiRepeatTime)
        {
            this.firstRepeatTime = firstRepeatTime;
            this.multiRepeatTime = multiRepeatTime;
            canRepeat = this.firstRepeatTime > 0;
            if (!canRepeat) Repeating = false;
        }

        public override void Update()
        {
            consumed = false;
            bufferCounter -= Time.DeltaTime;

            var check = false;
            foreach (Node node in Nodes)
            {
                node.Update();
                if (node.Pressed)
                {
                    bufferCounter = BufferTime;
                    check = true;
                }
                else if (node.Check)
                {
                    check = true;
                }
            }

            if (!check)
            {
                Repeating = false;
                repeatCounter = 0;
                bufferCounter = 0;
            }
            else if (canRepeat)
            {
                Repeating = false;
                if (repeatCounter == 0)
                {
                    repeatCounter = firstRepeatTime;
                }
                else
                {
                    repeatCounter -= Time.DeltaTime;
                    if (repeatCounter <= 0)
                    {
                        Repeating = true;
                        repeatCounter = multiRepeatTime;
                    }
                }
            }
        }

        /// <summary>
        ///     Ends the press buffer for this button
        /// </summary>
        public void ConsumeBuffer()
        {
            bufferCounter = 0;
        }

        /// <summary>
        ///     This button will not register a press for the rest of the current frame, but otherwise continues to function
        ///     normally. If the player continues to hold the button, next frame will not count as a Press. Also ends the
        ///     Press buffer for this button
        /// </summary>
        public void ConsumePress()
        {
            bufferCounter = 0;
            consumed = true;
        }

        public static implicit operator bool(VirtualButton button)
        {
            return button.Check;
        }

        public abstract class Node : VirtualInputNode
        {
            public abstract bool Check { get; }
            public abstract bool Pressed { get; }
            public abstract bool Released { get; }
        }

        public class KeyboardKey : Node
        {
            public Keys Key;

            public KeyboardKey(Keys key)
            {
                Key = key;
            }

            public override bool Check => CInput.Keyboard.Check(Key);

            public override bool Pressed => CInput.Keyboard.Pressed(Key);

            public override bool Released => CInput.Keyboard.Released(Key);
        }

        public class PadButton : Node
        {
            public Buttons Button;
            public int GamepadIndex;

            public PadButton(int gamepadIndex, Buttons button)
            {
                GamepadIndex = gamepadIndex;
                Button = button;
            }

            public override bool Check => CInput.GamePads[GamepadIndex].Check(Button);

            public override bool Pressed => CInput.GamePads[GamepadIndex].Pressed(Button);

            public override bool Released => CInput.GamePads[GamepadIndex].Released(Button);
        }

        #region Pad Left Stick

        public class PadLeftStickRight : Node
        {
            public float Deadzone;
            public int GamepadIndex;

            public PadLeftStickRight(int gamepadindex, float deadzone)
            {
                GamepadIndex = gamepadindex;
                Deadzone = deadzone;
            }

            public override bool Check => CInput.GamePads[GamepadIndex].LeftStickRightCheck(Deadzone);

            public override bool Pressed => CInput.GamePads[GamepadIndex].LeftStickRightPressed(Deadzone);

            public override bool Released => CInput.GamePads[GamepadIndex].LeftStickRightReleased(Deadzone);
        }

        public class PadLeftStickLeft : Node
        {
            public float Deadzone;
            public int GamepadIndex;

            public PadLeftStickLeft(int gamepadindex, float deadzone)
            {
                GamepadIndex = gamepadindex;
                Deadzone = deadzone;
            }

            public override bool Check => CInput.GamePads[GamepadIndex].LeftStickLeftCheck(Deadzone);

            public override bool Pressed => CInput.GamePads[GamepadIndex].LeftStickLeftPressed(Deadzone);

            public override bool Released => CInput.GamePads[GamepadIndex].LeftStickLeftReleased(Deadzone);
        }

        public class PadLeftStickUp : Node
        {
            public float Deadzone;
            public int GamepadIndex;

            public PadLeftStickUp(int gamepadindex, float deadzone)
            {
                GamepadIndex = gamepadindex;
                Deadzone = deadzone;
            }

            public override bool Check => CInput.GamePads[GamepadIndex].LeftStickUpCheck(Deadzone);

            public override bool Pressed => CInput.GamePads[GamepadIndex].LeftStickUpPressed(Deadzone);

            public override bool Released => CInput.GamePads[GamepadIndex].LeftStickUpReleased(Deadzone);
        }

        public class PadLeftStickDown : Node
        {
            public float Deadzone;
            public int GamepadIndex;

            public PadLeftStickDown(int gamepadindex, float deadzone)
            {
                GamepadIndex = gamepadindex;
                Deadzone = deadzone;
            }

            public override bool Check => CInput.GamePads[GamepadIndex].LeftStickDownCheck(Deadzone);

            public override bool Pressed => CInput.GamePads[GamepadIndex].LeftStickDownPressed(Deadzone);

            public override bool Released => CInput.GamePads[GamepadIndex].LeftStickDownReleased(Deadzone);
        }

        #endregion

        #region Pad Right Stick

        public class PadRightStickRight : Node
        {
            public float Deadzone;
            public int GamepadIndex;

            public PadRightStickRight(int gamepadindex, float deadzone)
            {
                GamepadIndex = gamepadindex;
                Deadzone = deadzone;
            }

            public override bool Check => CInput.GamePads[GamepadIndex].RightStickRightCheck(Deadzone);

            public override bool Pressed => CInput.GamePads[GamepadIndex].RightStickRightPressed(Deadzone);

            public override bool Released => CInput.GamePads[GamepadIndex].RightStickRightReleased(Deadzone);
        }

        public class PadRightStickLeft : Node
        {
            public float Deadzone;
            public int GamepadIndex;

            public PadRightStickLeft(int gamepadindex, float deadzone)
            {
                GamepadIndex = gamepadindex;
                Deadzone = deadzone;
            }

            public override bool Check => CInput.GamePads[GamepadIndex].RightStickLeftCheck(Deadzone);

            public override bool Pressed => CInput.GamePads[GamepadIndex].RightStickLeftPressed(Deadzone);

            public override bool Released => CInput.GamePads[GamepadIndex].RightStickLeftReleased(Deadzone);
        }

        public class PadRightStickUp : Node
        {
            public float Deadzone;
            public int GamepadIndex;

            public PadRightStickUp(int gamepadindex, float deadzone)
            {
                GamepadIndex = gamepadindex;
                Deadzone = deadzone;
            }

            public override bool Check => CInput.GamePads[GamepadIndex].RightStickUpCheck(Deadzone);

            public override bool Pressed => CInput.GamePads[GamepadIndex].RightStickUpPressed(Deadzone);

            public override bool Released => CInput.GamePads[GamepadIndex].RightStickUpReleased(Deadzone);
        }

        public class PadRightStickDown : Node
        {
            public float Deadzone;
            public int GamepadIndex;

            public PadRightStickDown(int gamepadindex, float deadzone)
            {
                GamepadIndex = gamepadindex;
                Deadzone = deadzone;
            }

            public override bool Check => CInput.GamePads[GamepadIndex].RightStickDownCheck(Deadzone);

            public override bool Pressed => CInput.GamePads[GamepadIndex].RightStickDownPressed(Deadzone);

            public override bool Released => CInput.GamePads[GamepadIndex].RightStickDownReleased(Deadzone);
        }

        #endregion

        #region Pad Triggers

        public class PadLeftTrigger : Node
        {
            public int GamepadIndex;
            public float Threshold;

            public PadLeftTrigger(int gamepadIndex, float threshold)
            {
                GamepadIndex = gamepadIndex;
                Threshold = threshold;
            }

            public override bool Check => CInput.GamePads[GamepadIndex].LeftTriggerCheck(Threshold);

            public override bool Pressed => CInput.GamePads[GamepadIndex].LeftTriggerPressed(Threshold);

            public override bool Released => CInput.GamePads[GamepadIndex].LeftTriggerReleased(Threshold);
        }

        public class PadRightTrigger : Node
        {
            public int GamepadIndex;
            public float Threshold;

            public PadRightTrigger(int gamepadIndex, float threshold)
            {
                GamepadIndex = gamepadIndex;
                Threshold = threshold;
            }

            public override bool Check => CInput.GamePads[GamepadIndex].RightTriggerCheck(Threshold);

            public override bool Pressed => CInput.GamePads[GamepadIndex].RightTriggerPressed(Threshold);

            public override bool Released => CInput.GamePads[GamepadIndex].RightTriggerReleased(Threshold);
        }

        #endregion

        #region Pad DPad

        public class PadDPadRight : Node
        {
            public int GamepadIndex;

            public PadDPadRight(int gamepadIndex)
            {
                GamepadIndex = gamepadIndex;
            }

            public override bool Check => CInput.GamePads[GamepadIndex].DPadRightCheck;

            public override bool Pressed => CInput.GamePads[GamepadIndex].DPadRightPressed;

            public override bool Released => CInput.GamePads[GamepadIndex].DPadRightReleased;
        }

        public class PadDPadLeft : Node
        {
            public int GamepadIndex;

            public PadDPadLeft(int gamepadIndex)
            {
                GamepadIndex = gamepadIndex;
            }

            public override bool Check => CInput.GamePads[GamepadIndex].DPadLeftCheck;

            public override bool Pressed => CInput.GamePads[GamepadIndex].DPadLeftPressed;

            public override bool Released => CInput.GamePads[GamepadIndex].DPadLeftReleased;
        }

        public class PadDPadUp : Node
        {
            public int GamepadIndex;

            public PadDPadUp(int gamepadIndex)
            {
                GamepadIndex = gamepadIndex;
            }

            public override bool Check => CInput.GamePads[GamepadIndex].DPadUpCheck;

            public override bool Pressed => CInput.GamePads[GamepadIndex].DPadUpPressed;

            public override bool Released => CInput.GamePads[GamepadIndex].DPadUpReleased;
        }

        public class PadDPadDown : Node
        {
            public int GamepadIndex;

            public PadDPadDown(int gamepadIndex)
            {
                GamepadIndex = gamepadIndex;
            }

            public override bool Check => CInput.GamePads[GamepadIndex].DPadDownCheck;

            public override bool Pressed => CInput.GamePads[GamepadIndex].DPadDownPressed;

            public override bool Released => CInput.GamePads[GamepadIndex].DPadDownReleased;
        }

        #endregion

        #region Mouse

        public class MouseLeftButton : Node
        {
            public override bool Check => CInput.Mouse.CheckLeftButton;

            public override bool Pressed => CInput.Mouse.PressedLeftButton;

            public override bool Released => CInput.Mouse.ReleasedLeftButton;
        }

        public class MouseRightButton : Node
        {
            public override bool Check => CInput.Mouse.CheckRightButton;

            public override bool Pressed => CInput.Mouse.PressedRightButton;

            public override bool Released => CInput.Mouse.ReleasedRightButton;
        }

        public class MouseMiddleButton : Node
        {
            public override bool Check => CInput.Mouse.CheckMiddleButton;

            public override bool Pressed => CInput.Mouse.PressedMiddleButton;

            public override bool Released => CInput.Mouse.ReleasedMiddleButton;
        }

        #endregion

        #region Other Virtual Inputs

        public class VirtualAxisTrigger : Node
        {
            public enum Modes
            {
                LargerThan,
                LessThan,
                Equals
            }

            private readonly VirtualAxis axis;

            public ThresholdModes Mode;
            public float Threshold;

            public VirtualAxisTrigger(VirtualAxis axis, ThresholdModes mode, float threshold)
            {
                this.axis = axis;
                Mode = mode;
                Threshold = threshold;
            }

            public override bool Check
            {
                get
                {
                    if (Mode == ThresholdModes.LargerThan) return axis.Value >= Threshold;

                    if (Mode == ThresholdModes.LessThan) return axis.Value <= Threshold;

                    return axis.Value == Threshold;
                }
            }

            public override bool Pressed
            {
                get
                {
                    if (Mode == ThresholdModes.LargerThan)
                        return axis.Value >= Threshold && axis.PreviousValue < Threshold;

                    if (Mode == ThresholdModes.LessThan)
                        return axis.Value <= Threshold && axis.PreviousValue > Threshold;

                    return axis.Value == Threshold && axis.PreviousValue != Threshold;
                }
            }

            public override bool Released
            {
                get
                {
                    if (Mode == ThresholdModes.LargerThan)
                        return axis.Value < Threshold && axis.PreviousValue >= Threshold;

                    if (Mode == ThresholdModes.LessThan)
                        return axis.Value > Threshold && axis.PreviousValue <= Threshold;

                    return axis.Value != Threshold && axis.PreviousValue == Threshold;
                }
            }
        }

        public class VirtualIntegerAxisTrigger : Node
        {
            public enum Modes
            {
                LargerThan,
                LessThan,
                Equals
            }

            private readonly VirtualIntegerAxis axis;

            public ThresholdModes Mode;
            public int Threshold;

            public VirtualIntegerAxisTrigger(VirtualIntegerAxis axis, ThresholdModes mode, int threshold)
            {
                this.axis = axis;
                Mode = mode;
                Threshold = threshold;
            }

            public override bool Check
            {
                get
                {
                    if (Mode == ThresholdModes.LargerThan) return axis.Value >= Threshold;

                    if (Mode == ThresholdModes.LessThan) return axis.Value <= Threshold;

                    return axis.Value == Threshold;
                }
            }

            public override bool Pressed
            {
                get
                {
                    if (Mode == ThresholdModes.LargerThan)
                        return axis.Value >= Threshold && axis.PreviousValue < Threshold;

                    if (Mode == ThresholdModes.LessThan)
                        return axis.Value <= Threshold && axis.PreviousValue > Threshold;

                    return axis.Value == Threshold && axis.PreviousValue != Threshold;
                }
            }

            public override bool Released
            {
                get
                {
                    if (Mode == ThresholdModes.LargerThan)
                        return axis.Value < Threshold && axis.PreviousValue >= Threshold;

                    if (Mode == ThresholdModes.LessThan)
                        return axis.Value > Threshold && axis.PreviousValue <= Threshold;

                    return axis.Value != Threshold && axis.PreviousValue == Threshold;
                }
            }
        }

        public class VirtualJoystickXTrigger : Node
        {
            public enum Modes
            {
                LargerThan,
                LessThan,
                Equals
            }

            private readonly VirtualJoystick joystick;

            public ThresholdModes Mode;
            public float Threshold;

            public VirtualJoystickXTrigger(VirtualJoystick joystick, ThresholdModes mode, float threshold)
            {
                this.joystick = joystick;
                Mode = mode;
                Threshold = threshold;
            }

            public override bool Check
            {
                get
                {
                    if (Mode == ThresholdModes.LargerThan) return joystick.Value.X >= Threshold;

                    if (Mode == ThresholdModes.LessThan) return joystick.Value.X <= Threshold;

                    return joystick.Value.X == Threshold;
                }
            }

            public override bool Pressed
            {
                get
                {
                    if (Mode == ThresholdModes.LargerThan)
                        return joystick.Value.X >= Threshold && joystick.PreviousValue.X < Threshold;

                    if (Mode == ThresholdModes.LessThan)
                        return joystick.Value.X <= Threshold && joystick.PreviousValue.X > Threshold;

                    return joystick.Value.X == Threshold && joystick.PreviousValue.X != Threshold;
                }
            }

            public override bool Released
            {
                get
                {
                    if (Mode == ThresholdModes.LargerThan)
                        return joystick.Value.X < Threshold && joystick.PreviousValue.X >= Threshold;

                    if (Mode == ThresholdModes.LessThan)
                        return joystick.Value.X > Threshold && joystick.PreviousValue.X <= Threshold;

                    return joystick.Value.X != Threshold && joystick.PreviousValue.X == Threshold;
                }
            }
        }

        public class VirtualJoystickYTrigger : Node
        {
            private readonly VirtualJoystick joystick;
            public ThresholdModes Mode;
            public float Threshold;

            public VirtualJoystickYTrigger(VirtualJoystick joystick, ThresholdModes mode, float threshold)
            {
                this.joystick = joystick;
                Mode = mode;
                Threshold = threshold;
            }

            public override bool Check
            {
                get
                {
                    if (Mode == ThresholdModes.LargerThan) return joystick.Value.X >= Threshold;

                    if (Mode == ThresholdModes.LessThan) return joystick.Value.X <= Threshold;

                    return joystick.Value.X == Threshold;
                }
            }

            public override bool Pressed
            {
                get
                {
                    if (Mode == ThresholdModes.LargerThan)
                        return joystick.Value.X >= Threshold && joystick.PreviousValue.X < Threshold;

                    if (Mode == ThresholdModes.LessThan)
                        return joystick.Value.X <= Threshold && joystick.PreviousValue.X > Threshold;

                    return joystick.Value.X == Threshold && joystick.PreviousValue.X != Threshold;
                }
            }

            public override bool Released
            {
                get
                {
                    if (Mode == ThresholdModes.LargerThan)
                        return joystick.Value.X < Threshold && joystick.PreviousValue.X >= Threshold;

                    if (Mode == ThresholdModes.LessThan)
                        return joystick.Value.X > Threshold && joystick.PreviousValue.X <= Threshold;

                    return joystick.Value.X != Threshold && joystick.PreviousValue.X == Threshold;
                }
            }
        }

        #endregion
    }
}