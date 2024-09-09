using System;
using System.Collections.Generic;
using XNode;

namespace DialogGraphWithXnodeAndLocalization.Nodes
{
    [NodeTint("#002e11")]
    [NodeWidth(450)]
    [CreateNodeMenu("Nodes/Answer Node")]
    public class AnswerNode : DialogRootNode
    {
        [Output(connectionType = ConnectionType.Override, dynamicPortList = true)] public List<Answer> Answers = new List<Answer>();

        internal override void Execute()
        {
            (graph as DialogGraph).currentNode = this;
        }
        
        internal override void NextNode(int? index = null)
        {
            if(index == null)
            {
                (graph as DialogGraph).currentNode = null;
                return;
            }

            NodePort port = null;
            if (Answers.Count > index)
                port = GetOutputPort($"{nameof(Answers)} {index}");

            if(!port.IsConnected || port == null)
            {
                (graph as DialogGraph).currentNode = null;
                return;
            }

            (port.Connection.node as DialogRootNode).Execute();
        }
    }

    [Serializable]
    public class Answer
    {
        public string TableEntryReference;
        public bool HasConditions;
        public Condition[] Conditions;

        public bool IsAvaliable
        {
            get
            {
                return CheckConditions();
            }
        }

        private bool CheckConditions()
        {
            bool success = true;

            if (Conditions.Length > 0)
                for (int i = 0; i < Conditions.Length; i++)
                {
                    if (!Conditions[i].Invoke())
                    {
                        success = false;
                        break;
                    }
                }

            return success;
        }
    }
}