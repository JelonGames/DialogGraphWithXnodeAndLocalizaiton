using DialogGraphWithXnodeAndLocalization.Nodes;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Localization;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Localization.Tables;
using XNode;
using XNodeEditor;

namespace DialogGraphWithXnodeAndLocalization.Editor.Nodes
{
    [CustomNodeEditor(typeof(AnswerNode))]
    public class AnswerNodeEditor : NodeEditor
    {
        private AnswerNode _node;
        private DialogGraph _graph;

        public override void OnBodyGUI()
        {
            serializedObject.Update();

            _node = (AnswerNode)serializedObject.targetObject;
            _graph = _node.graph as DialogGraph;

            CreatePorts();

            if(_node.Answers.Count == 0)
                EditorGUILayout.HelpBox("Answers list is empty", MessageType.Warning);

            serializedObject.ApplyModifiedProperties();
        }

        #region CreatePorts
        private void CreatePorts()
        {
            NodeEditorGUILayout.PortField(_node.GetPort(nameof(_node.Input)));

            if (_node.Answers.Count == 0)
            {
                NodeEditorGUILayout.PortField(_node.GetPort(nameof(_node.DefaultOutput)));
            }

            NodeEditorGUILayout.DynamicPortList(
                nameof(_node.Answers),
                typeof(DialogRootNode),
                serializedObject,
                NodePort.IO.Output,
                Node.ConnectionType.Override,
                Node.TypeConstraint.Inherited,
                OnCreateReorderableList
            );
        }

        private void OnCreateReorderableList(ReorderableList list)
        {
            int baseElementHeight = 200;

            list.elementHeightCallback = (int index) => 
            { 
                AnswerNode node = serializedObject.targetObject as AnswerNode;
                if (node.Answers[index].HasConditions)
                    return baseElementHeight * 2;
                else
                    return baseElementHeight; 
            };

            list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocuse) =>
            {
                //prepare rects
                Rect firstLineRect = new Rect(rect.x, rect.y, rect.width, (baseElementHeight / 4) - 3);
                Rect secondLineRect = new Rect(rect.x, rect.y + (baseElementHeight / 4), rect.width, ((baseElementHeight / 4) * 2) - 3);
                Rect thirdLineRect = new Rect(rect.x, rect.y + ((baseElementHeight / 4) * 3), rect.width, (baseElementHeight / 4) - 3);
                Rect helpBoxRect = new Rect(rect.x, rect.y + (baseElementHeight / 4), rect.width, 40);

                //prepare list keys with string table localizastion
                List<string> keys = new List<string>();
                StringTableCollection table = LocalizationEditorSettings.GetStringTableCollection(_graph.TableLocalizationReference);
                if (table != null)
                {
                    foreach (var entry in table.SharedData.Entries)
                    {
                        keys.Add(entry.Key);
                    }
                }

                //prepare node and dynamic output port
                AnswerNode node = serializedObject.targetObject as AnswerNode;
                NodePort port = node.GetPort($"{nameof(node.Answers)} {index}");
                port.ValueType = typeof(DialogRootNode);

                // create label element answer list
                EditorGUI.LabelField(
                    new Rect(
                        firstLineRect.position,
                        new Vector2(
                            firstLineRect.width / 2,
                            firstLineRect.height)
                        ),
                    $"{nameof(node.Answers)} {index}"
                );

                // create popup element answer list
                int indexPopup = keys.IndexOf(node.Answers[index].TableEntryReference);
                indexPopup = EditorGUI.Popup(
                    new Rect(
                        firstLineRect.x + (firstLineRect.width / 2),
                        firstLineRect.y + 9,
                        firstLineRect.width / 2,
                        18),
                    indexPopup,
                    keys.ToArray()
                );
                
                if (indexPopup >= 0)
                {
                    node.Answers[index].TableEntryReference = keys[indexPopup];
                    if (!string.IsNullOrEmpty(_graph.SelectedPreviewLocal))
                        MakePreview(
                            GetTranslation(keys[indexPopup]),
                            secondLineRect
                        );
                    else
                        EditorGUI.HelpBox(helpBoxRect, "Select Table Localization Reference", MessageType.Warning);
                }
                else if(keys.Count == 0)
                {
                    node.Answers[index].TableEntryReference = string.Empty;
                    EditorGUI.HelpBox(helpBoxRect, "Select Table Localization Reference", MessageType.Warning);
                }
                else if (string.IsNullOrEmpty(_graph.SelectedPreviewLocal))
                {
                    node.Answers[index].TableEntryReference = string.Empty;
                    EditorGUI.HelpBox(helpBoxRect, "Select Preview Local", MessageType.Warning);
                }
                else
                {
                    node.Answers[index].TableEntryReference = string.Empty;
                    EditorGUI.HelpBox(helpBoxRect, "Select Answer Text", MessageType.Warning);
                }

                //Create has conditons toggle
                EditorGUI.LabelField(
                    new Rect(
                        thirdLineRect.position,
                        new Vector2(
                            thirdLineRect.width / 2,
                            thirdLineRect.height)
                        ),
                    "Has Conditions"
                );

                bool hasConditions = node.Answers[index].HasConditions;
                hasConditions = EditorGUI.Toggle(
                    new Rect(
                        thirdLineRect.x + (thirdLineRect.width / 2),
                        thirdLineRect.y,
                        thirdLineRect.width / 2,
                        thirdLineRect.height),
                    hasConditions
                );

                if(hasConditions != node.Answers[index].HasConditions )
                    node.Answers[index].HasConditions = hasConditions;

                //Create conditions
                var element = serializedObject.FindProperty(nameof(node.Answers)).GetArrayElementAtIndex(index);
                if (hasConditions && element != null)
                {
                    EditorGUI.PropertyField(
                        new Rect(
                            thirdLineRect.x,
                            thirdLineRect.y + (thirdLineRect.height),
                            thirdLineRect.width,
                            thirdLineRect.height / 2),
                        element.FindPropertyRelative("Conditions")
                    );

                    if (node.Answers[index].Conditions.Length == 0)
                        EditorGUI.HelpBox(
                            new Rect(
                                helpBoxRect.x,
                                helpBoxRect.y + ((baseElementHeight / 4) * 6),
                                helpBoxRect.width,
                                helpBoxRect.height
                            ),
                            "You don't set conditions. Default output is pass",
                            MessageType.Info
                        );
                }

                // create output port element list
                if (port != null)
                {
                    Vector2 pos = rect.position + (port.IsOutput ? new Vector2(rect.width + 6, 0) : new Vector2(-36, 0));
                    NodeEditorGUILayout.PortField(pos, port);
                }
            };
        }
        #endregion

        #region Addons to dynamic port
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

        private void MakePreview(string text, Rect rect)
        {
            GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.wordWrap = true;
            labelStyle.padding = new RectOffset(10, 10, 1, 1);
            labelStyle.alignment = TextAnchor.MiddleLeft;

            Texture2D roundedBackground = MakeTex(1, 1, new Color(0.2f, 0.2f, 0.2f, 1f));
            labelStyle.normal.background = roundedBackground;
            labelStyle.border = new RectOffset(10, 10, 10, 10);

            EditorGUI.LabelField(rect, text, labelStyle);
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