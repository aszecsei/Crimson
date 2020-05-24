using System.Collections.Generic;

namespace Crimson.Input
{
    public class VirtualIntegerAxis : VirtualInput
    {
        public List<VirtualAxis.Node> Nodes;

        public int Value;

        public VirtualIntegerAxis()
        {
            Nodes = new List<VirtualAxis.Node>();
        }

        public VirtualIntegerAxis(params VirtualAxis.Node[] nodes)
        {
            Nodes = new List<VirtualAxis.Node>(nodes);
        }

        public int PreviousValue { get; private set; }

        public override void Update()
        {
            foreach (VirtualAxis.Node node in Nodes) node.Update();

            PreviousValue = Value;
            Value = 0;
            foreach (VirtualAxis.Node node in Nodes)
            {
                var value = node.Value;
                if (value != 0)
                {
                    Value = Mathf.Sign(value);
                    break;
                }
            }
        }

        public static implicit operator int(VirtualIntegerAxis axis)
        {
            return axis.Value;
        }
    }
}