using System;
using System.Collections.Generic;
using Automa.Entities.Debugging;
using Automa.Entities.Systems;
using UnityEditor;
using UnityEngine;

namespace Automa.Entities.Unity.Editor
{
    public class UnityContextDebugger : EditorWindow
    {
        private static readonly List<bool> systemsGroupsExpanded = new List<bool>();
        private GUIStyle expanderStyle;
        private GUIStyle groupNameStyle;
        private bool groupsToggled;

        private float lastUpdate;

        private Vector2 scrollPosition;
        private GuiStyles styles;

        private static bool systemsToggled = true;

        private UnityContext unityContext;
        private GUIStyle wrapStyle;

        [MenuItem("Window/Context Debugger")]
        private static void Init()
        {
            // Get existing open window or if none, make a new one:
            var window = (UnityContextDebugger) GetWindow(typeof(UnityContextDebugger));
            window.Show();
        }

        private void OnEnable()
        {
            OnSelectionChange();
            lastUpdate = 0;
            titleContent = new GUIContent("Context Debugger");

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

        private void OnSelectionChange()
        {
            if (Selection.activeObject is GameObject)
            {
                unityContext = ((GameObject) Selection.activeObject).GetComponent<UnityContext>();
                lastUpdate = 0;
            }
        }

        private void Update()
        {
            if (Time.realtimeSinceStartup > lastUpdate + 0.5f)
            {
                if (unityContext == null) OnSelectionChange();
                Repaint();
            }
        }

        private void OnGUI()
        {
            if (expanderStyle == null)
                expanderStyle = new GUIStyle(GUI.skin.box);
            if (unityContext == null || unityContext.Context == null) return;
            scrollPosition =
                EditorGUILayout.BeginScrollView(scrollPosition, false, false, GUILayout.Width(Screen.width));
            var contextDebug = unityContext.Context is IDebuggableContext d ? d.DebugInfo : null;
            var systems = unityContext.SystemManager;
            if (systems != null)
            {
                EditorGUILayout.BeginVertical(expanderStyle);
                systemsToggled = EditorGUILayout.Foldout(systemsToggled, contextDebug != null
                    ? $"Systems ({contextDebug.UpdateTime.Ticks / (float) TimeSpan.TicksPerMillisecond:0.00} ms)"
                    : "Systems");
                if (systemsToggled)
                {
                    DrawSystems(systems);
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
            lastUpdate = Time.realtimeSinceStartup;
        }

        private void DrawSystems(SystemManager systems)
        {
            var debug = systems.DebugInfo;
            if (debug == null) return;
            var systemsDebugInfos = debug.Systems;

            if (systemsDebugInfos.Length == 0)
            {
                EditorGUILayout.LabelField("Where is no systems in context");
            }
            else
            {
                var width = EditorGUIUtility.currentViewWidth - 60;
                if (systemsGroupsExpanded.Count < systemsDebugInfos.Length)
                {
                    var count = systemsDebugInfos.Length - systemsGroupsExpanded.Count;
                    for (var i = 0; i < count; i++)
                    {
                        systemsGroupsExpanded.Add(false);
                    }
                }
                for (var i = 0; i < systemsDebugInfos.Length; i++)
                {
                    var systemsDebugInfo = systemsDebugInfos[i];
                    var system = systemsDebugInfo.System;
                    var type = system.GetType();
                    var time = systemsDebugInfo.UpdateTime;
                    var groups = systemsDebugInfo.Groups;

                    EditorGUILayout.BeginVertical(GUI.skin.box);
                    EditorGUILayout.BeginHorizontal();
                    if (groups == null || groups.Length == 0)
                    {
                        EditorGUILayout.LabelField("", GUILayout.Width(16));
                    }
                    else
                    {
                        systemsGroupsExpanded[i] = GUILayout.Toggle(systemsGroupsExpanded[i], "", EditorStyles.foldout,
                            GUILayout.Width(16));
                    }
                    system.IsEnabled = GUILayout.Toggle(system.IsEnabled, "", GUILayout.Width(16));
                    EditorGUILayout.LabelField(type.Name, GUILayout.Width(width - 82));
                    EditorGUILayout.LabelField($"{time.Ticks / (float) TimeSpan.TicksPerMillisecond:0.00} ms",
                        GUILayout.Width(50));
                    EditorGUILayout.EndHorizontal();
                    if (groups != null && groups.Length > 0 && systemsGroupsExpanded[i])
                    {
                        foreach (var group in groups)
                        {
                            DrawGroup(group);
                        }
                    }
                    EditorGUILayout.EndVertical();
                }
            }
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
            EditorGUILayout.LabelField(group.Count.ToString(), groupNameStyle, GUILayout.Width(30));
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