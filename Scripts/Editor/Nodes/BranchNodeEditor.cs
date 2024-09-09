using DialogGraphWithXnodeAndLocalization.Nodes;
using UnityEditor;
using XNodeEditor;

namespace DialogGraphWithXnodeAndLocalization.Editor.Nodes
{
    [CustomNodeEditor(typeof(BranchNode))]
    public class BranchNodeEditor : NodeEditor
    {
        private BranchNode _node;

        public override void OnBodyGUI()
        {
            serializedObject.Update();

            _node = (BranchNode)serializedObject.targetObject;

            CreatePorts();
            CreateFields();

            serializedObject.ApplyModifiedProperties();
        }

        private void CreatePorts()
        {
            NodeEditorGUILayout.PortField(_node.GetPort(nameof(_node.Input)));
            NodeEditorGUILayout.PortField(_node.GetPort(nameof(_node.Pass)));
            NodeEditorGUILayout.PortField(_node.GetPort(nameof(_node.Fail)));
        }

        private void CreateFields()
        {
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(_node.Conditions)));
        }
    }
}