using DialogGraphWithXnodeAndLocalization.Nodes;
using UnityEditor;
using UnityEngine;
using XNode;
using XNodeEditor;

namespace DialogGraphWithXnodeAndLocalization.Editor.Nodes 
{
    [CustomNodeEditor(typeof(StartDialogNode))]
    public class StartDialogNodeEditor : NodeEditor
    {
        private StartDialogNode _node;

        public override void OnBodyGUI()
        {
            serializedObject.Update();

            _node = (StartDialogNode)serializedObject.targetObject;

            NodeEditorGUILayout.PortField(_node.GetPort(nameof(_node.DefaultOutput)));

            serializedObject.ApplyModifiedProperties();
        }

        public override void AddContextMenuItems(GenericMenu menu)
        {
            if (Selection.objects.Length == 1 && Selection.activeObject is Node)
            {
                Node node = Selection.activeObject as Node;
                menu.AddItem(new GUIContent("Move To Top"), on: false, delegate
                {
                    NodeEditorWindow.current.MoveNodeToTop(node);
                });
                menu.AddItem(new GUIContent("Rename"), on: false, NodeEditorWindow.current.RenameSelectedNode);
            }

            if (Selection.objects.Length == 1 && Selection.activeObject is Node)
            {
                Node obj = Selection.activeObject as Node;
                menu.AddCustomContextMenuItems(obj);
            }
        }
    }
}