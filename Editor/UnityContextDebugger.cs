using System;
using System.Collections;
using System.Collections.Generic;
using Automa.Entities.Debugging;
using Automa.Entities.Systems.Debugging;
using UnityEditor;
using UnityEngine;

namespace Automa.Entities.Unity.Editor
{
    [CustomEditor(typeof(UnityContext), true)]
    class UnityContextDebugger : UnityEditor.Editor
    {
        public static float UpdateInterval = 0.1f;
        private static readonly Dictionary<string, bool> systemsGroupsExpanded = new Dictionary<string, bool>();
        private GUIStyle expanderStyle;
        private GUIStyle groupNameStyle;

        private Vector2 scrollPosition;
        private GuiStyles styles;

        private static bool systemsToggled = true;

        private UnityContext unityContext;
        private GUIStyle wrapStyle;
        private Coroutine updateCycleCoroutine;

        void OnEnable()
        {
            unityContext = serializedObject.targetObject as UnityContext;
            if (unityContext != null)
            {
                updateCycleCoroutine = unityContext.StartCoroutine(UpdateCicle());
            }
            groupNameStyle = new GUIStyle
            {
                //fontStyle = FontStyle.Bold
                wordWrap = true
            };
            wrapStyle = new GUIStyle
            {
                wordWrap = true
            };

            styles = Resources.Load<GuiStyles>("Styles");
        }

        IEnumerator UpdateCicle()
        {
            while (true)
            {
                yield return new WaitForSecondsRealtime(UpdateInterval);
                Repaint();
            }
        }

        void OnDisable()
        {
            if (unityContext != null && updateCycleCoroutine != null)
            {
                unityContext.StopCoroutine(updateCycleCoroutine);
            }
            updateCycleCoroutine = null;
            unityContext = null;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (expanderStyle == null)
            {
                expanderStyle = new GUIStyle(GUI.skin.box);
            }
            if (unityContext == null || unityContext.Context == null) return;
            scrollPosition =
                EditorGUILayout.BeginScrollView(scrollPosition, false, false, GUILayout.Width(Screen.width));
            var contextDebug = unityContext.Context is IDebuggableContext d ? d.DebugInfo : null;
            var systems = unityContext.SystemManager;
            if (systems != null)
            {
                EditorGUILayout.BeginVertical(expanderStyle);
                systemsToggled = EditorGUILayout.Foldout(systemsToggled, contextDebug != null
                    ? $"Systems ({contextDebug.UpdateTime.Ticks / (float)TimeSpan.TicksPerMillisecond:0.00} ms)"
                    : "Systems");
                if (systemsToggled && systems.DebugInfo != null)
                {
                    DrawSystems(systems.DebugInfo.Systems, EditorGUIUtility.currentViewWidth, true);
                }
                EditorGUILayout.EndVertical();
            }
            //            var entities = unityContext.EntityManager;
            //            if (entities != null)
            //            {
            //                EditorGUILayout.BeginVertical(expanderStyle);
            //                groupsToggled = EditorGUILayout.Foldout(groupsToggled, "Groups");
            //                
            //                EditorGUILayout.EndVertical();
            //            }
            EditorGUILayout.EndScrollView();
        }


        private void DrawSystems(SystemDebugInfo[] systemsDebugInfos, float areaWidth, bool isEnabled)
        {
            if (systemsDebugInfos.Length == 0)
            {
                EditorGUILayout.LabelField("Where is no systems in context");
                return;
            }
            var width = areaWidth - 60;
            for (var i = 0; i < systemsDebugInfos.Length; i++)
            {
                var systemsDebugInfo = systemsDebugInfos[i];
                if (systemsDebugInfo is SystemGroupDebugInfo systemGroupDebug)
                {
                    var systemGroup = systemGroupDebug.SystemGroup;
                    var type = systemGroup.GetType();
                    var time = systemsDebugInfo.UpdateTime;

                    var backgroundColor = GUI.backgroundColor;
                    GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f);

                    EditorGUI.indentLevel = 0;
                    EditorGUILayout.BeginVertical(GUI.skin.box);
                    EditorGUILayout.BeginHorizontal();

                    var expanded = SetExpanded(type, GUILayout.Toggle(IsExpanded(type), "", EditorStyles.foldout,
                            GUILayout.Width(16)));

                    systemGroup.IsEnabled = GUILayout.Toggle(systemGroup.IsEnabled, "", GUILayout.Width(16));
                    EditorGUILayout.LabelField(type.Name, GUILayout.Width(width - 82));
                    var currentIsEnabled = systemGroup.IsEnabled && isEnabled;
                    if (currentIsEnabled)
                    {
                        EditorGUILayout.LabelField($"{time.Ticks / (float)TimeSpan.TicksPerMillisecond:0.00} ms",
                            GUILayout.Width(50));
                    }
                    EditorGUILayout.EndHorizontal();

                    if (expanded)
                    {
                        DrawSystems(systemGroupDebug.Systems, width, currentIsEnabled);
                    }
                    EditorGUILayout.EndVertical();
                    GUI.backgroundColor = backgroundColor;
                }
                else
                {
                    var system = systemsDebugInfo.System;
                    var type = system.GetType();
                    var time = systemsDebugInfo.UpdateTime;
                    var groups = systemsDebugInfo.Groups;

                    var backgroundColor = GUI.backgroundColor;
                    GUI.backgroundColor = new Color(0.9f, 0.9f, 0.9f);
                    EditorGUILayout.BeginVertical(GUI.skin.box);
                    EditorGUILayout.BeginHorizontal();
                    var expanded = false;
                    if (groups == null || groups.Length == 0)
                    {
                        EditorGUILayout.LabelField("", GUILayout.Width(16));
                    }
                    else
                    {
                        expanded = SetExpanded(type, GUILayout.Toggle(IsExpanded(type), "", EditorStyles.foldout,
                            GUILayout.Width(16)));
                    }
                    system.IsEnabled = GUILayout.Toggle(system.IsEnabled, "", GUILayout.Width(16));
                    EditorGUILayout.LabelField(type.Name, GUILayout.Width(width - 82));
                    if (system.IsEnabled && isEnabled)
                    {
                        EditorGUILayout.LabelField($"{time.Ticks / (float)TimeSpan.TicksPerMillisecond:0.00} ms",
                            GUILayout.Width(50));
                    }
                    EditorGUILayout.EndHorizontal();
                    if (expanded)
                    {
                        foreach (var group in groups)
                        {
                            DrawGroup(group);
                        }
                    }
                    EditorGUILayout.EndVertical();
                    GUI.backgroundColor = backgroundColor;
                }
            }
        }

        private bool SetExpanded(Type type, bool value)
        {
            var typeName = type.FullName;
            systemsGroupsExpanded[typeName ?? ""] = value;
            return value;
        }

        private bool IsExpanded(Type type)
        {
            var typeName = type.FullName;
            if (systemsGroupsExpanded.TryGetValue(typeName ?? "", out var result))
            {
                return result;
            }
            return false;
        }

        private void DrawGroup(GroupDebugInfo debugInfo)
        {
            var group = debugInfo.Group;
            var groupType = group.GetType();
            var name = groupType.Name;
            if (groupType.IsNested && groupType.DeclaringType != null)
            {
                name = $"{groupType.DeclaringType.Name}+{name}";
            }

            EditorGUILayout.BeginVertical(GUI.skin.box);
            var width = Screen.width - 60;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(name, groupNameStyle, GUILayout.Width(width - 30));
            EditorGUILayout.LabelField(group.Count.ToString(), GUILayout.Width(30));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            var usedSpace = 0.0f;
            var availableSpace = EditorGUIUtility.currentViewWidth - 110;
            var firstInRow = true;
            foreach (var included in debugInfo.IncludeTypes)
            {
                var size = styles.includeType.CalcSize(new GUIContent(included.Name));
                usedSpace += size.x;
                if (usedSpace > availableSpace || firstInRow)
                {
                    firstInRow = true;
                    usedSpace = 0;
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                }
                EditorGUILayout.LabelField(included.Name, styles.includeType, GUILayout.Width(size.x));
                firstInRow = false;
            }
            foreach (var excluded in debugInfo.ExcludeTypes)
            {
                var size = styles.excludeType.CalcSize(new GUIContent(excluded.Name));
                usedSpace += size.x;
                if (usedSpace > availableSpace || firstInRow)
                {
                    firstInRow = true;
                    usedSpace = 0;
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                }
                EditorGUILayout.LabelField(excluded.Name, styles.excludeType, GUILayout.Width(size.x));
                firstInRow = false;
            }
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.EndVertical();
        }
    }
}
