using UnityEditor;
using UnityEngine;

namespace AideTool.ExtendedEditor
{
    internal static class EditorStyle
    {
        internal static GUIStyle Box;
        internal static GUIStyle BoxChild;
        internal static GUIStyle Foldout;
        internal static GUIStyle Button;
        internal static GUIStyle Text;

        static EditorStyle()
        {
            bool pro = EditorGUIUtility.isProSkin;

            Color c_on = pro ? Color.white : new Color(51f / 255f, 102f / 255f, 204f / 255f, 1);

            Button = new(EditorStyles.miniButton)
            {
                font = Font.CreateDynamicFontFromOSFont(new[] { "Calibri", "Arial" }, 17)
            };

            Text = new(EditorStyles.label)
            {
                richText = true,
                contentOffset = new Vector2(0, 5),
                font = Font.CreateDynamicFontFromOSFont(new[] { "Calibri", "Arial" }, 14)
            };

            Foldout = new(EditorStyles.foldout)
            {
                overflow = new RectOffset(-10, 0, 3, 0),
                padding = new RectOffset(25, 0, 0, 0)
            };

            Foldout.active.textColor = c_on;
            Foldout.onActive.textColor = c_on;
            Foldout.focused.textColor = c_on;
            Foldout.onFocused.textColor = c_on;
            Foldout.hover.textColor = c_on;
            Foldout.onHover.textColor = c_on;

            Box = new(GUI.skin.box)
            {
                padding = new RectOffset(4, 4, 4, 4)
            };

            Box.normal.background = new Color(0f, 0f, 0f, 0.08f).MakeTex();

            BoxChild = new(GUI.skin.box);
            BoxChild.normal.background = new Color(0f, 0f, 0f, 0f).MakeTex();
            BoxChild.active.textColor = c_on;
            BoxChild.onActive.textColor = c_on;

            BoxChild.focused.textColor = c_on;
            BoxChild.onFocused.textColor = c_on;

            EditorStyles.foldout.active.textColor = c_on;
            EditorStyles.foldout.onActive.textColor = c_on;
            EditorStyles.foldout.focused.textColor = c_on;
            EditorStyles.foldout.onFocused.textColor = c_on;
            EditorStyles.foldout.hover.textColor = c_on;
            EditorStyles.foldout.onHover.textColor = c_on;
        }

        
    }
}
