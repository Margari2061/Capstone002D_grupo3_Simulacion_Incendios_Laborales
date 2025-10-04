using UnityEditor;
using UnityEngine;

namespace AideTool.ExtendedEditor.DataTransfer
{
    internal struct HierarchyItem
    {
        internal int InstanceID { get; private set; }
        internal bool IsSelected { get; private set; }
        internal bool IsHovered { get; private set; }
        internal GameObject GameObject { get; private set; }
        internal InspectorAide Aide { get; private set; }
        internal Rect BackgroundRect { get; private set; }
        internal Rect TextRect { get; private set; }
        internal Rect CollapseToggleIconRect { get; private set; }
        internal Rect PrefabIconRect { get; private set; }
        internal Rect EditPrefabIconRect { get; private set; }

        internal HierarchyItem(int instanceID, Rect selectionRect, InspectorAide prettyObject)
        {
            InstanceID = instanceID;
            IsSelected = Selection.Contains(instanceID);
            GameObject = prettyObject.gameObject;
            Aide = prettyObject;

            float xPos = selectionRect.position.x + 60f - 28f - selectionRect.xMin;
            float yPos = selectionRect.position.y;
            float xSize = selectionRect.size.x + selectionRect.xMin + 28f - 60 + 16f;
            float ySize = selectionRect.size.y;
            BackgroundRect = new Rect(xPos, yPos, xSize, ySize);

            xPos = selectionRect.position.x + 18f;
            yPos = selectionRect.position.y;
            xSize = selectionRect.size.x - 18f;
            ySize = selectionRect.size.y;
            TextRect = new Rect(xPos, yPos, xSize, ySize);

            xPos = selectionRect.position.x - 14f;
            yPos = selectionRect.position.y + 1f;
            xSize = 13f;
            ySize = 13f;
            CollapseToggleIconRect = new Rect(xPos, yPos, xSize, ySize);

            xPos = selectionRect.position.x;
            yPos = selectionRect.position.y;
            xSize = 16f;
            ySize = 16f;
            PrefabIconRect = new Rect(xPos, yPos, xSize, ySize);

            xPos = BackgroundRect.xMax - 16f;
            yPos = selectionRect.yMin;
            xSize = 16f;
            ySize = 16f;
            EditPrefabIconRect = new Rect(xPos, yPos, xSize, ySize);

            IsHovered = BackgroundRect.Contains(Event.current.mousePosition);
        }
    }
}
