namespace DialogGraphWithXnodeAndLocalization.Nodes
{
    [NodeTint("#7a0800")]
    [NodeWidth(300)]
    [CreateNodeMenu("Nodes/End Node")]
    public class EndDialogNode : DialogRootNode
    {
        public bool HasEvent;
        public SerializableEvent[] EndEvents;

        internal override void Execute()
        {
            if (HasEvent)
            {
                for (int i = 0; i < EndEvents.Length; i++)
                {
                    EndEvents[i].Invoke();
                }
            }

            (graph as DialogGraph).currentNode = this;
        }

        internal override void NextNode(int? index = null) 
        {
            (graph as DialogGraph).currentNode = null;
        }
    }
}