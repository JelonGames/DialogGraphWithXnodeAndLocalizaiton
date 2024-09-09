using UnityEngine;
using UnityEngine.Localization.Settings;
using XNode;

namespace DialogGraphWithXnodeAndLocalization.Nodes
{
    public abstract class DialogRootNode : Node
    {
        [Input(ShowBackingValue.Never)] public DialogRootNode Input;
        [Output(connectionType: ConnectionType.Override)] public DialogRootNode DefaultOutput;

        abstract internal void Execute();

        internal virtual void NextNode(int? index = null)
        {
            NodePort port = GetOutputPort(nameof(DefaultOutput)) as NodePort;
            if (port == null || !port.IsConnected)
            {
                (graph as DialogGraph).currentNode = null;
                return;
            }

            (port.Connection.node as DialogRootNode).Execute();
        }

        public override object GetValue(NodePort port)
        {
            return null;
        }
    }
}