using DialogGraphWithXnodeAndLocalization.Nodes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEditor;
using UnityEditor.Localization;
using UnityEngine;
using UnityEngine.Localization.Tables;
using XNodeEditor;

namespace DialogGraphWithXnodeAndLocalization.Editor
{
    [CustomNodeGraphEditor(typeof(DialogGraph))]
    public class DialogGraphEditor : NodeGraphEditor
    {
        private DialogGraph _graph;

        private GUIStyle _centralizedStyle;

        private int _maxWidthVariableElement = 200;
        private int _minHeightVariableElement = 50;

        private int _maxWidthContenerElement = 210;
        private int _minHeightContenerElement = 60;

        public override void OnGUI()
        {
            serializedObject.Update();
            base.OnGUI();
            _graph = (DialogGraph)target;

            // Prepare centralizedStyle
            _centralizedStyle = new GUIStyle(GUI.skin.label);
            _centralizedStyle.alignment = TextAnchor.MiddleCenter;
            
            // Make header in window
            GUIStyle verticalStyle = new GUIStyle();
            verticalStyle.normal.background = Texture2D.grayTexture;
            GUILayout.BeginHorizontal(verticalStyle, GUILayout.ExpandWidth(true), GUILayout.MinHeight(_minHeightContenerElement));

            // Make Variable
            PopupSelectLocalizationTabel();
            PopupSelectionPreviewLocal();

            // End header in window
            GUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        }

        public override bool CanRemove(XNode.Node node)
        {
            switch (node)
            {
                case StartDialogNode:
                    return false;
                default:
                    return base.CanRemove(node);
            }
        }

        #region Variables
        private void PopupSelectLocalizationTabel()
        {
            List<string> tablesName = new List<string>();

            //Get tables name
            ReadOnlyCollection<StringTableCollection> tables = LocalizationEditorSettings.GetStringTableCollections();
            if (tables != null && tables.Count > 0)
            {
                foreach (StringTableCollection table in tables)
                {
                    if(table.Group == "Dialogue")
                        tablesName.Add(table.TableCollectionNameReference);
                }
            }

            // make view
            GUILayout.BeginVertical(_centralizedStyle, GUILayout.MaxWidth(_maxWidthContenerElement), GUILayout.MinHeight(_minHeightVariableElement));

            int index = tablesName.IndexOf(_graph.TableLocalizationReference);
            EditorGUILayout.LabelField("Table Localization Reference");
            index = EditorGUILayout.Popup(index, tablesName.ToArray(), GUILayout.MaxWidth(_maxWidthVariableElement));

            if (index >= 0)
            {
                _graph.TableLocalizationReference = tablesName[index];
            }
            else
            {
                _graph.TableLocalizationReference = string.Empty;
                EditorGUILayout.HelpBox("Select localization string table", MessageType.Warning);
            }

            GUILayout.EndVertical();
        }

        private void PopupSelectionPreviewLocal()
        {
            List<string> localeCodes = new List<string>();

            // Get avaliable locales
            StringTableCollection table = LocalizationEditorSettings.GetStringTableCollection(_graph.TableLocalizationReference);
            if(table != null)
            {
                ReadOnlyCollection<LazyLoadReference<LocalizationTable>> locales = table.Tables;

                foreach (var locale in locales)
                {
                    if (!locale.isSet)
                        continue;

                    localeCodes.Add(locale.asset.LocaleIdentifier.Code);
                }
            }

            // make view
            GUILayout.BeginVertical(_centralizedStyle, GUILayout.MaxWidth(_maxWidthContenerElement), GUILayout.MinHeight(_minHeightVariableElement));

            int index = localeCodes.IndexOf(_graph.SelectedPreviewLocal);
            EditorGUILayout.LabelField("Selected Preview Local");
            index = EditorGUILayout.Popup(index, localeCodes.ToArray(), GUILayout.MaxWidth(_maxWidthVariableElement));

            if (index >= 0)
            {
                _graph.SelectedPreviewLocal = localeCodes[index];
            }
            else if(localeCodes.Count == 0)
            {
                _graph.SelectedPreviewLocal = string.Empty;
                EditorGUILayout.HelpBox("Select Table Localization Reference", MessageType.Warning);
            }
            else
            {
                _graph.SelectedPreviewLocal = string.Empty;
                EditorGUILayout.HelpBox("Select locale for preview window", MessageType.Warning);
            }

            GUILayout.EndVertical();
        }
        #endregion

        public override void OnCreate()
        {
            DialogGraph graph = target as DialogGraph;

            bool startNodeExist = false;
            bool endNodeExist = false;
            foreach(XNode.Node item in graph.nodes)
            {
                if (startNodeExist && endNodeExist) break;

                if(item.GetType() == typeof(StartDialogNode))
                {
                    startNodeExist = true;
                    continue;
                }

                if(item.GetType() == typeof(EndDialogNode))
                {
                    endNodeExist = true;
                    continue;
                }
            }

            if (!startNodeExist)
                CreateNode(
                    typeof(StartDialogNode),
                    new Vector2(0, 0)
                );

            if (!endNodeExist)
                CreateNode(
                    typeof(EndDialogNode),
                    new Vector2(800, 0)
                );
        }

        public override string GetNodeMenuName(Type type)
        {
            if ((typeof(DialogRootNode).IsAssignableFrom(type) || typeof(XNode.NodeGroups.NodeGroup).IsAssignableFrom(type))
                && !typeof(StartDialogNode).IsAssignableFrom(type))
                return base.GetNodeMenuName(type);
            else
                return null;
        }
    }
}