using AideTool.ExtendedEditor.DataTransfer;
using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

#if !EDITOR_DEFAULT
namespace AideTool.ExtendedEditor
{
    [InitializeOnLoad]
    internal static class AideFramework
    {
        private const float UpdateTime = 0.3f;

        internal static event UnityAction NeedsRepaint;
        internal static void OnNeedsRepaint() => NeedsRepaint?.Invoke();
        internal static bool NeedToRepaint { get; private set; }

        internal static float ElapsedTime;

        static AideFramework()
        {
            NeedsRepaint += CallToRepaint;
            EditorApplication.update += Updating;
            EditorApplication.hierarchyWindowItemOnGUI += HandleHierarchy;
        }

        private static void CallToRepaint()
        {
            NeedToRepaint = true;
        }

        private static void Updating()
        {
            if (NeedToRepaint)
            {
                ElapsedTime += Time.deltaTime;

                if (ElapsedTime >= UpdateTime)
                {
                    ElapsedTime -= UpdateTime;
                    NeedToRepaint = false;
                }
            }
        }

        private static void HandleHierarchy(int instanceID, Rect selectionRect)
        {
            GameObject instance = (GameObject)EditorUtility.InstanceIDToObject(instanceID);

            if(instance != null)
            {
                if(instance.TryGetComponent(out InspectorAide aideObject))
                {
                    if(aideObject.Behaviour != InspectorAideBehaviour.Default)
                    {
                        HierarchyItem item = new(instanceID, selectionRect, aideObject);
                        PaintBackground(item);
                        PaintText(item);
                        PaintCollapseToggleIcon(item);
                        PaintPrefabIcon(item);
                        PaintEditPrefabIcon(item);
                    }
                }
            }

        }

        private static void PaintBackground(HierarchyItem item)
        {
            if(item.Aide.Behaviour == InspectorAideBehaviour.Same)
            {
                EditorGUI.DrawRect(item.BackgroundRect, item.Aide.BackgroundColor);
                return;
            }

            if(item.IsSelected)
            {
                EditorGUI.DrawRect(item.BackgroundRect, item.Aide.BackgroundSelectedColor);
                return;
            }

            if(item.IsHovered)
            {
                EditorGUI.DrawRect(item.BackgroundRect, item.Aide.BackgroundHoverColor);
                return;
            }

            EditorGUI.DrawRect(item.BackgroundRect, item.Aide.BackgroundNormalColor);
        }

        private static void PaintText(HierarchyItem item)
        {
            Color32 textColor = Color.white;

            if (item.Aide.Behaviour == InspectorAideBehaviour.Same)
                textColor = item.Aide.TextColor;

            if(item.Aide.Behaviour == InspectorAideBehaviour.Detailed)
            {
                textColor = item.Aide.TextNormalColor;

                if (item.IsHovered)
                    textColor = item.Aide.TextHoverColor;

                if (item.IsSelected)
                    textColor = item.Aide.TextSelectedColor;
            }

            GUIStyle labelStyle = new()
            {
                normal = new GUIStyleState { textColor = textColor },
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft
            };

            EditorGUI.LabelField(item.TextRect, item.Aide.name, labelStyle);
        }

        private static void PaintCollapseToggleIcon(HierarchyItem item)
        {
            if(item.GameObject.transform.childCount > 0)
            {
                Type sceneHierarchyWindowType = typeof(Editor)
                    .Assembly
                    .GetType("UnityEditor.SceneHierarchyWindow");

                PropertyInfo sceneHierarchyWindow = sceneHierarchyWindowType
                    .GetProperty("lastInteractedHierarchyWindow", BindingFlags.Public | BindingFlags.Static);

                int[] expandedIDs = (int[])sceneHierarchyWindowType
                    .GetMethod("GetExpandedIDs", BindingFlags.NonPublic | BindingFlags.Instance)
                    .Invoke(sceneHierarchyWindow.GetValue(null), null);

                string iconID = expandedIDs.Contains(item.InstanceID) ? "IN Foldout on" : "IN foldout";

                GUI.DrawTexture(item.CollapseToggleIconRect, EditorGUIUtility.IconContent(iconID).image);
            }
        }

        private static void PaintPrefabIcon(HierarchyItem item)
        {
            Texture icon = EditorGUIUtility.ObjectContent(EditorUtility.InstanceIDToObject(item.InstanceID), null).image;

            if (EditorExtensions.IsHierarchyFocused() && item.IsSelected)
            {
                if (icon.name == "d_Prefab Icon" || icon.name == "Prefab Icon")
                {
                    icon = EditorGUIUtility.IconContent("d_Prefab On Icon").image;
                }
                if (icon.name == "GameObject Icon")
                {
                    icon = EditorGUIUtility.IconContent("GameObject On Icon").image;
                }
            }

            GUI.DrawTexture(item.PrefabIconRect, icon);
        }

        private static void PaintEditPrefabIcon(HierarchyItem item)
        {
            if (PrefabUtility.GetCorrespondingObjectFromOriginalSource(item.GameObject) != null && PrefabUtility.IsAnyPrefabInstanceRoot(item.GameObject))
            {
                Texture icon = EditorGUIUtility.IconContent("ArrowNavigationRight").image;
                GUI.DrawTexture(item.EditPrefabIconRect, icon);
            }
        }
    }
}
#endif