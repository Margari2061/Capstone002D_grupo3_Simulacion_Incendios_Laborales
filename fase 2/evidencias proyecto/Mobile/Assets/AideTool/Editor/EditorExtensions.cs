using AideTool.ExtendedEditor.DataTransfer;
using System;
using UnityEditor;
using UnityEngine;

namespace AideTool.ExtendedEditor
{
    internal static class EditorExtensions
    {
        internal static Texture2D MakeTex(this Color color)
        {
            Color32[] pix = new Color32[4];
            for (int i = 0; i < pix.Length; ++i)
                pix[i] = color;

            Texture2D result = new(2, 2);
            result.SetPixels32(pix);
            result.Apply();
            return result;
        }

        internal static Texture2D Colorize(this Texture2D texture, Color color)
        {
            if (texture == null)
            {
                Aide.LogError(texture);
                return color.MakeTex();
            }

            Color32[] pix = texture.GetPixels32();

            int l = pix.Length;
            for(int i = 0; i < l; i++)
                if (pix[i] != Color.black)
                    pix[i] = color;

            Texture2D result = new(texture.width, texture.height);
            result.SetPixels32(pix);
            result.Apply();
            return result;
        }

        internal static void UseVerticalLayout(Action action, GUIStyle style)
        {
            EditorGUILayout.BeginVertical(style);
            action();
            EditorGUILayout.EndVertical();
        }

        internal static void UseButton(SerializedObject obj, MethodProperties method)
        {
            string content = NicifyNames(method.Name);
            if (GUILayout.Button(content, GUILayout.Width(100f), GUILayout.ExpandWidth(true), GUILayout.Height(24f)))
                method.Method.Invoke(obj.targetObject, null);
        }

        internal static bool IsHierarchyFocused() 
        { 
            return EditorWindow.focusedWindow != null && EditorWindow.focusedWindow.titleContent.text == "Hierarchy"; 
        }

        internal static string NicifyNames(string property)
        {
            int underscoreIndex = property.IndexOf('_');


            if (underscoreIndex == -1)
            {
                return ObjectNames.NicifyVariableName(property);
            }

            property = property.Substring(underscoreIndex + 1);
            return ObjectNames.NicifyVariableName(property);
        }

        internal static void PropertyField(FieldProperties field, ref bool initialized)
        {
            if (field.SetNumericProperty)
                SetProperty(field);

            if (field.IsVisible)
            {
                if (field.IsValidator)
                {
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(field.Property, new GUIContent(NicifyNames(field.Display)), true);
                    if (EditorGUI.EndChangeCheck())
                    {
                        initialized = false;
                        AideFramework.OnNeedsRepaint();
                    }
                    EditorGUILayout.Space(2f);
                    return;
                }

                EditorGUILayout.PropertyField(field.Property, new GUIContent(NicifyNames(field.Display)), true);
                EditorGUILayout.Space(2f);
            }
        }

        private static void SetProperty(FieldProperties field)
        {
            switch(field.NumberType)
            {
                case NumericTypes.Int:
                    int intValue = field.Property.intValue;
                    if (intValue < field.MinNumericValue)
                        field.Property.intValue = (int)field.MinNumericValue.Value;
                    if(field.MaxNumericValue != null && intValue > field.MaxNumericValue.Value)
                        field.Property.intValue = (int)field.MaxNumericValue.Value;
                    break;
                case NumericTypes.Float:
                    float floatValue = field.Property.floatValue;
                    if (floatValue < field.MinNumericValue)
                        field.Property.floatValue = field.MinNumericValue.Value;
                    if (field.MaxNumericValue != null && floatValue > field.MaxNumericValue.Value)
                        field.Property.floatValue = field.MaxNumericValue.Value;
                    break;
                case NumericTypes.Double:
                    double doubleValue = field.Property.doubleValue;
                    if (doubleValue < field.MinNumericValue)
                        field.Property.doubleValue = (double)field.MinNumericValue.Value;
                    if (field.MaxNumericValue != null && doubleValue > field.MaxNumericValue.Value)
                        field.Property.doubleValue = (double)field.MaxNumericValue.Value;
                    break;
                case NumericTypes.Long:
                    long longValue = field.Property.longValue;
                    if (longValue < field.MinNumericValue)
                        field.Property.longValue = (long)field.MinNumericValue.Value;
                    if (field.MaxNumericValue != null && longValue > field.MaxNumericValue.Value)
                        field.Property.longValue = (long)field.MaxNumericValue.Value;
                    break;
            }
        }
    }
}
