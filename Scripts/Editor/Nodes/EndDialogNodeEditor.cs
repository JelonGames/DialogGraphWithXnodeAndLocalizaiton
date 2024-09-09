using DialogGraphWithXnodeAndLocalization.Nodes;
using UnityEditor;
using UnityEngine;
using XNode;
using XNodeEditor;

namespace DialogGraphWithXnodeAndLocalization.Editor.Nodes
{
    [CustomNodeEditor(typeof(EndDialogNode))]
    public class EndDialogNodeEditor : NodeEditor
    {
        private EndDialogNode _node;

        public override void OnBodyGUI()
        {
            serializedObject.Update();

            _node = (EndDialogNode)serializedObject.targetObject;

            CreatePort();
            CreateFields();

            serializedObject.ApplyModifiedProperties();
        }

        private void CreatePort()
        {
            NodeEditorGUILayout.PortField(_node.GetPort(nameof(_node.Input)));
        }

        private void CreateFields()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Has Events");
            bool value = _node.HasEvent;
            value = EditorGUILayout.Toggle("", value);
            _node.HasEvent = value;
            EditorGUILayout.EndHorizontal();

            if (value)
                NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(_node.EndEvents)));
        }
    }
}