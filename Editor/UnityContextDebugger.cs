using System;
using System.Text;
using Automa.Entities;
using Automa.Entities.Systems;
using Automa.Entities.Unity;
using UnityEditor;
using UnityEngine;

namespace Automa.Assets.Plugins.Entities.Unity.Editor
{
    public class UnityContextDebugger : EditorWindow
    {
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
            if (unityContext == null || unityContext.context == null) return;
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
            lastUpdate = Time.realtimeSinceStartup;
        }

        private void DrawSystems(SystemManager systems)
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.Separator();

            var debug = systems.DebugInfo;
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

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel(type.Name);
                    EditorGUILayout.LabelField($"{time.Ticks / (float)TimeSpan.TicksPerMillisecond:0.00} ms");
                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawGroups(EntityManager entities)
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.Separator();

            var debug = entities.DebugInfo;
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

                    EditorGUILayout.LabelField($"{name} ({@group.Count})");
                    StringBuilder sb = new StringBuilder();
                    foreach (var included in debugInfo.IncludeTypes)
                    {
                        sb.Append(included.Name);
                        sb.Append(", ");
                    }
                    if (sb.Length != 0)
                    {
                        EditorGUILayout.LabelField("I: " + sb);
                    }
                    sb.Clear();
                    foreach (var included in debugInfo.ExcludeTypes)
                    {
                        sb.Append(included.Name);
                        sb.Append(", ");
                    }
                    if (sb.Length != 0)
                    {
                        EditorGUILayout.LabelField("E: " + sb);
                    }
                }
            }

            EditorGUILayout.EndVertical();
        }
    }
}
