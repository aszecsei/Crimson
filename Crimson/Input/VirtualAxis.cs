using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace Crimson.Input
{
    public class VirtualAxis : VirtualInput
    {
        public List<Node> Nodes;

        public VirtualAxis()
        {
            Nodes = new List<Node>();
        }

        public VirtualAxis(params Node[] nodes)
        {
            Nodes = new List<Node>(nodes);
        }

        public float Value { get; private set; }
        public float PreviousValue { get; private set; }

        public override void Update()
        {
            foreach (Node node in Nodes) node.Update();

            PreviousValue = Value;
            Value = 0;
            foreach (Node node in Nodes)
            {
                var value = node.Value;
                if (value != 0)
                {
                    Value = value;
                    break;
                }
            }
        }

        public static implicit operator float(VirtualAxis axis)
        {
            return axis.Value;
        }

        public abstract class Node : VirtualInputNode
        {
            public abstract float Value { get; }
        }

        public class PadLeftStickX : Node
        {
            public float Deadzone;
            public int GamepadIndex;

            public PadLeftStickX(int gamepadIndex, float deadzone)
            {
                GamepadIndex = gamepadIndex;
                Deadzone = deadzone;
            }

            public override float Value =>
                Mathf.SignThreshold(CInput.gamePadData[GamepadIndex].GetLeftStick().X, Deadzone);
        }

        public class PadLeftStickY : Node
        {
            public float Deadzone;
            public int GamepadIndex;

            public PadLeftStickY(int gamepadIndex, float deadzone)
            {
                GamepadIndex = gamepadIndex;
                Deadzone = deadzone;
            }

            public override float Value =>
                Mathf.SignThreshold(CInput.gamePadData[GamepadIndex].GetLeftStick().Y, Deadzone);
        }

        public class PadRightStickX : Node
        {
            public float Deadzone;
            public int GamepadIndex;

            public PadRightStickX(int gamepadIndex, float deadzone)
            {
                GamepadIndex = gamepadIndex;
                Deadzone = deadzone;
            }

            public override float Value =>
                Mathf.SignThreshold(CInput.gamePadData[GamepadIndex].GetRightStick().X, Deadzone);
        }

        public class PadRightStickY : Node
        {
            public float Deadzone;
            public int GamepadIndex;

            public PadRightStickY(int gamepadIndex, float deadzone)
            {
                GamepadIndex = gamepadIndex;
                Deadzone = deadzone;
            }

            public override float Value =>
                Mathf.SignThreshold(CInput.gamePadData[GamepadIndex].GetRightStick().Y, Deadzone);
        }

        public class PadDpadLeftRight : Node
        {
            public int GamepadIndex;

            public PadDpadLeftRight(int gamepadIndex)
            {
                GamepadIndex = gamepadIndex;
            }

            public override float Value
            {
                get
                {
                    if (CInput.gamePadData[GamepadIndex].DPadRightCheck) return 1f;

                    if (CInput.gamePadData[GamepadIndex].DPadLeftCheck) return -1f;

                    return 0;
                }
            }
        }

        public class PadDpadUpDown : Node
        {
            public int GamepadIndex;

            public PadDpadUpDown(int gamepadIndex)
            {
                GamepadIndex = gamepadIndex;
            }

            public override float Value
            {
                get
                {
                    if (CInput.gamePadData[GamepadIndex].DPadDownCheck) return 1f;

                    if (CInput.gamePadData[GamepadIndex].DPadUpCheck) return -1f;

                    return 0;
                }
            }
        }

        public class KeyboardKeys : Node
        {
            public Keys Negative;
            public OverlapBehaviors OverlapBehavior;
            public Keys Positive;
            private bool turned;

            private float value;

            public KeyboardKeys(OverlapBehaviors overlapBehavior, Keys negative, Keys positive)
            {
                OverlapBehavior = overlapBehavior;
                Negative = negative;
                Positive = positive;
            }

            public override float Value => value;

            public override void Update()
            {
                if (CInput.keyboardData.Check(Positive))
                {
                    if (CInput.keyboardData.Check(Negative))
                    {
                        switch (OverlapBehavior)
                        {
                            default:
                            case OverlapBehaviors.CancelOut:
                                value = 0;
                                break;

                            case OverlapBehaviors.TakeNewer:
                                if (!turned)
                                {
                                    value *= -1;
                                    turned = true;
                                }

                                break;

                            case OverlapBehaviors.TakeOlder:
                                //value stays the same
                                break;
                        }
                    }
                    else
                    {
                        turned = false;
                        value = 1;
                    }
                }
                else if (CInput.keyboardData.Check(Negative))
                {
                    turned = false;
                    value = -1;
                }
                else
                {
                    turned = false;
                    value = 0;
                }
            }
        }
    }
}