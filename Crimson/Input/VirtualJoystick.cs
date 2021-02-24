using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Crimson.Input
{
    public class VirtualJoystick : VirtualInput
    {
        public List<Node> Nodes;
        public bool Normalized;
        public float? SnapSlices;

        public VirtualJoystick(bool normalized)
        {
            Nodes = new List<Node>();
            Normalized = normalized;
        }

        public VirtualJoystick(bool normalized, params Node[] nodes)
        {
            Nodes = new List<Node>(nodes);
            Normalized = normalized;
        }

        public Vector2 Value { get; private set; }
        public Vector2 PreviousValue { get; private set; }

        public override void Update()
        {
            foreach (Node node in Nodes) node.Update();

            PreviousValue = Value;
            Value = Vector2.Zero;
            foreach (Node node in Nodes)
            {
                var value = node.Value;
                if (value != Vector2.Zero)
                {
                    if (Normalized)
                    {
                        if (SnapSlices.HasValue)
                            value = value.SnappedNormal(SnapSlices.Value);
                        else
                            value.Normalize();
                    }
                    else if (SnapSlices.HasValue)
                    {
                        value = value.Snapped(SnapSlices.Value);
                    }

                    Value = value;
                    break;
                }
            }
        }

        public static implicit operator Vector2(VirtualJoystick joystick)
        {
            return joystick.Value;
        }

        public abstract class Node : VirtualInputNode
        {
            public abstract Vector2 Value { get; }
        }

        public class PadLeftStick : Node
        {
            public float Deadzone;
            public int GamepadIndex;

            public PadLeftStick(int gamepadIndex, float deadzone)
            {
                GamepadIndex = gamepadIndex;
                Deadzone = deadzone;
            }

            public override Vector2 Value => CInput.GamePads[GamepadIndex].GetLeftStick(Deadzone);
        }

        public class PadRightStick : Node
        {
            public float Deadzone;
            public int GamepadIndex;

            public PadRightStick(int gamepadIndex, float deadzone)
            {
                GamepadIndex = gamepadIndex;
                Deadzone = deadzone;
            }

            public override Vector2 Value => CInput.GamePads[GamepadIndex].GetRightStick(Deadzone);
        }

        public class PadDpad : Node
        {
            public int GamepadIndex;

            public PadDpad(int gamepadIndex)
            {
                GamepadIndex = gamepadIndex;
            }

            public override Vector2 Value
            {
                get
                {
                    var value = Vector2.Zero;

                    if (CInput.GamePads[GamepadIndex].DPadRightCheck)
                        value.X = 1f;
                    else if (CInput.GamePads[GamepadIndex].DPadLeftCheck) value.X = -1f;

                    if (CInput.GamePads[GamepadIndex].DPadDownCheck)
                        value.Y = 1f;
                    else if (CInput.GamePads[GamepadIndex].DPadUpCheck) value.Y = -1f;

                    return value;
                }
            }
        }

        public class KeyboardKeys : Node
        {
            public Keys Down;
            public Keys Left;
            public OverlapBehaviors OverlapBehavior;
            public Keys Right;

            private bool turnedX;
            private bool turnedY;
            public Keys Up;
            private Vector2 value;

            public KeyboardKeys(OverlapBehaviors overlapBehavior, Keys left, Keys right, Keys up, Keys down)
            {
                OverlapBehavior = overlapBehavior;
                Left = left;
                Right = right;
                Up = up;
                Down = down;
            }

            public override Vector2 Value => value;

            public override void Update()
            {
                //X Axis
                if (CInput.Keyboard.Check(Left))
                {
                    if (CInput.Keyboard.Check(Right))
                    {
                        switch (OverlapBehavior)
                        {
                            default:
                            case OverlapBehaviors.CancelOut:
                                value.X = 0;
                                break;

                            case OverlapBehaviors.TakeNewer:
                                if (!turnedX)
                                {
                                    value.X *= -1;
                                    turnedX = true;
                                }

                                break;

                            case OverlapBehaviors.TakeOlder:
                                //X stays the same
                                break;
                        }
                    }
                    else
                    {
                        turnedX = false;
                        value.X = -1;
                    }
                }
                else if (CInput.Keyboard.Check(Right))
                {
                    turnedX = false;
                    value.X = 1;
                }
                else
                {
                    turnedX = false;
                    value.X = 0;
                }

                //Y Axis
                if (CInput.Keyboard.Check(Up))
                {
                    if (CInput.Keyboard.Check(Down))
                    {
                        switch (OverlapBehavior)
                        {
                            default:
                            case OverlapBehaviors.CancelOut:
                                value.Y = 0;
                                break;

                            case OverlapBehaviors.TakeNewer:
                                if (!turnedY)
                                {
                                    value.Y *= -1;
                                    turnedY = true;
                                }

                                break;

                            case OverlapBehaviors.TakeOlder:
                                //Y stays the same
                                break;
                        }
                    }
                    else
                    {
                        turnedY = false;
                        value.Y = -1;
                    }
                }
                else if (CInput.Keyboard.Check(Down))
                {
                    turnedY = false;
                    value.Y = 1;
                }
                else
                {
                    turnedY = false;
                    value.Y = 0;
                }
            }
        }
    }
}