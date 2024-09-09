using DialogGraphWithXnodeAndLocalization.Nodes;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Localization;
using UnityEngine;
using UnityEngine.Localization.Tables;
using XNodeEditor;

namespace DialogGraphWithXnodeAndLocalization.Editor.Nodes
{
    [CustomNodeEditor(typeof(DialogueNode))]
    public class DialogueNodeEditor : NodeEditor
    {
        private DialogueNode _node;
        private DialogGraph _graph;

        public override void OnBodyGUI()
        {
            serializedObject.Update();

            _node = target as DialogueNode;
            _graph = _node.graph as DialogGraph;

            CreatePorts();
            CreateVariables();

            serializedObject.ApplyModifiedProperties();
        }

        private void CreatePorts()
        {
            NodeEditorGUILayout.PortField(_node.GetPort(nameof(_node.Input)));
            NodeEditorGUILayout.PortField(_node.GetPort(nameof(_node.DefaultOutput)));
        }

        private void CreateVariables()
        {
            CreatePopupDialogLocalizationKeyField();
            CreateInterlocutorImageField();
        }

        #region Method generete variable fields
        private void CreatePopupDialogLocalizationKeyField()
        {
            List<string> keys = new List<string>();

            // Get avaliable keys in string tabel
            StringTableCollection table = LocalizationEditorSettings.GetStringTableCollection(_graph.TableLocalizationReference);
            if(table != null)
            {
                foreach(var entry in table.SharedData.Entries)
                {
                    keys.Add(entry.Key);
                }
            }

            // make view
            GUILayout.BeginVertical();

            EditorGUILayout.LabelField(nameof(_node.DialogText));
            int index = keys.IndexOf(_node.DialogText);
            index = EditorGUILayout.Popup(index, keys.ToArray());

            if (index >= 0)
            {
                _node.DialogText = keys[index];
                if(!string.IsNullOrEmpty(_graph.SelectedPreviewLocal))
                    MakePreview(GetTranslation(keys[index]));
                else
                    EditorGUILayout.HelpBox("Select Table Localization Reference", MessageType.Warning);
            }
            else if(keys.Count == 0)
            {
                _node.DialogText = string.Empty;
                EditorGUILayout.HelpBox("Select Table Localization Reference", MessageType.Warning);
            }
            else if (string.IsNullOrEmpty(_graph.SelectedPreviewLocal))
            {
                _node.DialogText = string.Empty;
                EditorGUILayout.HelpBox("Select Preview Local", MessageType.Warning);
            }
            else
            {
                _node.DialogText = string.Empty;
                EditorGUILayout.HelpBox("Select Dialog Text", MessageType.Warning);
            }

            GUILayout.EndVertical();
        }

        private void CreateInterlocutorImageField()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Is Interlocutor Image?");
            bool value = _node.IsInterlocutorImage;
            value = EditorGUILayout.Toggle("", value);
            _node.IsInterlocutorImage = value;
            EditorGUILayout.EndHorizontal();

            if (value)
            {
                EditorGUILayout.LabelField("Interlocutor Image");
                _node.InterlocurotImage = EditorGUILayout.ObjectField("", _node.InterlocurotImage, typeof(Sprite), false) as Sprite;
            }
            else
            {
                _node.InterlocurotImage = null;
            }
        }
        #endregion

        #region Addons to variable fields
        private string GetTranslation(string key)
        {
            var table = LocalizationEditorSettings.GetStringTableCollection((_node.graph as DialogGraph).TableLocalizationReference).GetTable(_graph.SelectedPreviewLocal) as StringTable;
            if (table != null)
            {
                var entry = table.GetEntry(key);
                if (entry != null)
                {
                    return entry.GetLocalizedString();
                }
            }
            return "No translation found";
        }

        private void MakePreview(string text)
        {
            GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.wordWrap = true;
            labelStyle.padding = new RectOffset(10, 10, 1, 1);
            labelStyle.alignment = TextAnchor.MiddleLeft;

            Texture2D roundedBackground = MakeTex(1, 1, new Color(0.2f, 0.2f, 0.2f, 1f));
            labelStyle.normal.background = roundedBackground;
            labelStyle.border = new RectOffset(10, 10, 10, 10);

            float availableWidth = 200;
            float minHeight = labelStyle.CalcHeight(new GUIContent(text), availableWidth) - labelStyle.padding.left - labelStyle.padding.right - 10;


            EditorGUILayout.LabelField(text, labelStyle, GUILayout.MinHeight(minHeight));
        }

        private Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; i++)
            {
                pix[i] = col;
            }
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }
        #endregion
    }
}