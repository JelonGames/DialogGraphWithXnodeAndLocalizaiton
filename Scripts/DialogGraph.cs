using DialogGraphWithXnodeAndLocalization.Nodes;
using UnityEngine;
using XNode;

namespace DialogGraphWithXnodeAndLocalization
{
    [CreateAssetMenu(fileName = "New Dialog Graph", menuName = "Tools/Dialog/Dialog Graph")]
    public class DialogGraph : NodeGraph
    {
        [HideInInspector]public string TableLocalizationReference;
        [HideInInspector]public string SelectedPreviewLocal;
        
        public DialogRootNode currentNode { get; internal set; }

        public DialogRootNode StartDialog()
        {
            foreach (Node node in nodes)
            {
                if (node.GetType() == typeof(StartDialogNode))
                {
                    currentNode = node as DialogRootNode;
                }
            }
            return GetNextNode();
        }

        public DialogRootNode GetNextNode()
        {
            currentNode.NextNode();
            return currentNode;
        }

        public DialogRootNode GetNextNode(int answerIndex)
        {
            currentNode.NextNode(answerIndex);
            return currentNode;
        }
    }
}