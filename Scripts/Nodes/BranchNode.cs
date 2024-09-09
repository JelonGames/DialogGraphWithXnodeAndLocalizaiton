using XNode;

namespace DialogGraphWithXnodeAndLocalization.Nodes
{
    [NodeTint("#9c6a00")]
    [NodeWidth(300)]
    [CreateNodeMenu("Nodes/Branch Node")]
    public class BranchNode : DialogRootNode
    {
        public Condition[] Conditions;
        [Output(connectionType = ConnectionType.Override)] public DialogRootNode Pass;
        [Output(connectionType = ConnectionType.Override)] public DialogRootNode Fail;

        internal override void Execute()
        {
            bool success = true;

            if(Conditions.Length > 0)
                for (int i = 0; i < Conditions.Length; i++)
                {
                    if (!Conditions[i].Invoke())
                    {
                        success = false;
                        break;
                    }
                }

            //Trigger next nodes
            NodePort port;
            if (success) port = GetOutputPort(nameof(Pass));
            else port = GetOutputPort(nameof(Fail));


            if (port == null || !port.IsConnected)
            {
                (graph as DialogGraph).currentNode = null;
                return;
            }
            for (int i = 0; i < port.ConnectionCount; i++)
            {
                NodePort connection = port.GetConnection(i);
                (connection.node as DialogRootNode).Execute();
            }
        }

        internal override void NextNode(int? index = null) { }
    }
}