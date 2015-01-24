using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace MPInput
{

    public class MP_InputEditor : EditorWindow
    {

        private List<bool> actionFoldouts = new List<bool>();
        private Vector2 scrollPos = new Vector2();

        private GUIStyle foldoutStyle;

        [MenuItem("Input/Config")]
        public static void ShowWindow()
        {
            MP_Input.LoadConfig();
            EditorWindow.GetWindow(typeof(MP_InputEditor), false, "MP_Input");
        }

        void OnGUI()
        {
            if (MP_Input.Config == null)
                MP_Input.LoadConfig();

            if (MP_Input.Config == null)
            {
                EditorGUILayout.LabelField("Could not find input config file");
                if (GUILayout.Button("Create Config"))
                    CreateConfig();
                return;
            }
            
            if (foldoutStyle == null)
            {
                foldoutStyle = new GUIStyle(EditorStyles.foldout);
                foldoutStyle.fixedWidth = 1;
            }

            EditorGUI.BeginChangeCheck();
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            // Draw Actions
            for (int i = 0; i < MP_Input.Config.InputActions.Count; i++)
            {
                if (actionFoldouts.Count <= i)
                    actionFoldouts.Add(false);

                EditorGUILayout.Separator();
                EditorGUILayout.BeginHorizontal();
                actionFoldouts[i] = EditorGUILayout.Foldout(actionFoldouts[i], "", foldoutStyle);
                MP_Input.Config.InputActions[i].ActionName = EditorGUILayout.TextField(MP_Input.Config.InputActions[i].ActionName);
                if (GUILayout.Button("Remove",GUILayout.Width(100)))
                {
                    MP_Input.Config.InputActions.RemoveAt(i);
                    break;
                }

                EditorGUILayout.EndHorizontal();

                if (actionFoldouts[i])
                    DrawActionConfig(MP_Input.Config.InputActions[i]);

                GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });
            }

            if (GUILayout.Button("New Action"))
                MP_Input.Config.InputActions.Add(new MP_InputAction("Empty"));
            if (GUILayout.Button("Save Config"))
                SaveConfig();

            EditorGUILayout.EndScrollView();

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(MP_Input.Config);
            }
        }

        public static void SaveConfig()
        {
            AssetDatabase.SaveAssets();
        }

        void CreateConfig()
        {
            AssetDatabase.CreateAsset(new MP_InputConfig(), MP_Input.CONFIG_DIRECTORY + MP_Input.CONFIG_FILENAME + ".asset");
        }

        void IndentLine(int ammount)
        {
            EditorGUILayout.LabelField("", GUILayout.Width(20 * ammount));
        }

        void DrawActionConfig(MP_InputAction Action)
        {
            // Controller Inputs
            EditorGUILayout.Separator();
            EditorGUILayout.BeginHorizontal();
            IndentLine(1);

            EditorGUILayout.LabelField("XInput:", GUILayout.Width(100));
            if (GUILayout.Button("Add"))
                Action.ControllerInputs.Add(new MP_ControllerInputDefinition());

            EditorGUILayout.EndHorizontal();


            for (int i = 0; i < Action.ControllerInputs.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                IndentLine(2);
                if (GUILayout.Button("-", GUILayout.Width(18)))
                {
                    Action.ControllerInputs.RemoveAt(i);
                }
                else
                {
                    MP_ControllerInputDefinition Def = Action.ControllerInputs[i];

                    Def.Axial = (MP_eInputXboxAxial)EditorGUILayout.EnumPopup(Def.Axial, GUILayout.Width(130));
                    Def.AxisInvert = EditorGUILayout.ToggleLeft("Inverse", Def.AxisInvert, GUILayout.Width(67));

                    Action.ControllerInputs[i] = Def;
                }
                EditorGUILayout.EndHorizontal();
            }

            // Keyboard Inputs
            EditorGUILayout.Separator();
            EditorGUILayout.BeginHorizontal();
            IndentLine(1);

            EditorGUILayout.LabelField("Key:", GUILayout.Width(100));
            if (GUILayout.Button("Add"))
                Action.KeyboardInputs.Add(new MP_KeyboardInputDefinition());

            EditorGUILayout.EndHorizontal();
            for (int i = 0; i < Action.KeyboardInputs.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                IndentLine(2);
                if (GUILayout.Button("-", GUILayout.Width(18)))
                {
                    Action.KeyboardInputs.RemoveAt(i);
                }
                else
                {
                    MP_KeyboardInputDefinition Def = Action.KeyboardInputs[i];

                    Def.Key = (KeyCode)EditorGUILayout.EnumPopup(Def.Key, GUILayout.Width(130));
                    Def.AxisInvert = EditorGUILayout.ToggleLeft("Inverse", Def.AxisInvert, GUILayout.Width(67));
                    Def.RequiresAlt = EditorGUILayout.ToggleLeft("Alt", Def.RequiresAlt, GUILayout.Width(38));
                    Def.RequiresCtrl = EditorGUILayout.ToggleLeft("Ctrl", Def.RequiresCtrl, GUILayout.Width(40));
                    Def.RequiresShift = EditorGUILayout.ToggleLeft("Shift", Def.RequiresShift, GUILayout.Width(45));

                    Action.KeyboardInputs[i] = Def;
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.Separator();
        }
    }
}