using XNode;

namespace DialogGraphWithXnodeAndLocalization.Nodes
{
    [NodeTint("#26005c")]
    [NodeWidth(300)]
    [CreateNodeMenu("Nodes/Event Node")]
    public class EventNode : DialogRootNode
    {
        public SerializableEvent[] Events;

        internal override void Execute()
        {
            for (int i = 0; i < Events.Length; i++)
            {
                Events[i].Invoke();
            }

            NextNode();
        }
    }
}