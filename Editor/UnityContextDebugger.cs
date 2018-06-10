using System;
using System.Text;
using Automa.Entities.Systems;
using UnityEditor;
using UnityEngine;

namespace Automa.Entities.Unity.Editor
{
    public class UnityContextDebugger : EditorWindow
    {
        private GUIStyle groupNameStyle;
        private GuiStyles styles;

        [MenuItem("Window/Context Debugger")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            var window = (UnityContextDebugger)EditorWindow.GetWindow(typeof(UnityContextDebugger));
            window.Show(); 
        }

        void OnEnable()
        {
            OnSelectionChange();
            lastUpdate = 0;
            titleContent = new GUIContent("Context Debugger");

            groupNameStyle = new GUIStyle()
            {
                fontStyle = FontStyle.Bold
            };

            styles = Resources.Load<GuiStyles>("Styles");
        }

        private UnityContext unityContext;

        void OnSelectionChange()
        {
            if (Selection.activeObject is GameObject)
            {
                unityContext = ((GameObject) Selection.activeObject).GetComponent<UnityContext>();
                lastUpdate = 0;
            }
        }

        private float lastUpdate;

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
            if (unityContext == null || unityContext.Context == null) return;
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            var systems = unityContext.SystemManager;
            if (systems != null)
            {
                DrawSystems(systems);
            }
            var entities = unityContext.EntityManager;
            if (entities != null)
            {
                DrawGroups(entities);
            }
            EditorGUILayout.EndScrollView();
            lastUpdate = Time.realtimeSinceStartup;
        }

        private void DrawSystems(SystemManager systems)
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.Separator();

            var debug = systems.DebugInfo;
            if (debug == null) return;
            var systemsDebugInfos = debug.Systems;

            if (systemsDebugInfos.Length == 0)
            {
                EditorGUILayout.LabelField("Where is no systems in context");
            }
            else
            {
                for (int i = 0; i < systemsDebugInfos.Length; i++)
                {
                    var type = systemsDebugInfos[i].System.GetType();
                    var time = systemsDebugInfos[i].UpdateTime;

                    EditorGUILayout.BeginHorizontal(GUI.skin.box);
                    EditorGUILayout.PrefixLabel(type.Name, groupNameStyle);
                    EditorGUILayout.LabelField($"{time.Ticks / (float)TimeSpan.TicksPerMillisecond:0.00} ms");
                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.EndVertical();
        }

        private Vector2 scrollPosition;

        private void DrawGroups(EntityManager entities)
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.Separator();

            var debug = entities.DebugInfo;
            if (debug == null) return;
            var groupDebugInfos = debug.Groups;

            if (groupDebugInfos.Length == 0)
            {
                EditorGUILayout.LabelField("Where is no groups in context");
            }
            else
            {
                for (int i = 0; i < groupDebugInfos.Length; i++)
                {
                    var debugInfo = groupDebugInfos[i];
                    var @group = debugInfo.Group;
                    var groupType = @group.GetType();
                    var name = groupType.Name;
                    if (groupType.IsNested && groupType.DeclaringType != null)
                    {
                        name = $"{groupType.DeclaringType.Name}+{name}";
                    }

                    EditorGUILayout.BeginVertical(GUI.skin.box);

                    EditorGUILayout.LabelField($"{name} ({@group.Count})", groupNameStyle);

                    var rect = EditorGUILayout.BeginHorizontal();
                    foreach (var included in debugInfo.IncludeTypes)
                    {
                        var labelRect = GUILayoutUtility.GetRect(new GUIContent(included.Name), styles.includeType);
                        labelRect.x = rect.x;
                        labelRect.y = rect.y;
                        EditorGUI.LabelField(labelRect, included.Name, styles.includeType);
                        rect.x += labelRect.width + 2;
                    }
                    EditorGUILayout.EndHorizontal();

                    rect = EditorGUILayout.BeginHorizontal();
                    foreach (var included in debugInfo.ExcludeTypes)
                    {
                        var labelRect = GUILayoutUtility.GetRect(new GUIContent(included.Name), styles.excludeType);
                        labelRect.x = rect.x;
                        labelRect.y = rect.y;
                        EditorGUI.LabelField(labelRect, included.Name, styles.excludeType);
                        rect.x += labelRect.width + 2;
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.EndVertical();
                }
            }

            EditorGUILayout.EndVertical();
        }
    }
}
