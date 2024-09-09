using UnityEngine;

namespace DialogGraphWithXnodeAndLocalization.Nodes
{
    [NodeTint("#002e11")]
    [NodeWidth(300)]
    [CreateNodeMenu("Nodes/Chat Node")]
    public class DialogueNode : DialogRootNode
    {
        public string DialogText;

        public bool IsInterlocutorImage;
        public Sprite InterlocurotImage;

        internal override void Execute()
        {
            (graph as DialogGraph).currentNode = this;
        }
    }
}