using UnityEditor;

namespace AideTool.ExtendedEditor.DataTransfer
{
    internal sealed class FieldProperties
    {
        internal SerializedProperty Property { get; set; }
        internal string Name { get { return Property.name; } }
        internal bool IsValidator { get; set; }
        internal bool IsVisible { get; set; }
        internal string FoldoutLabel { get; set; }
        internal FoldoutOrder Order { get; set; }
        internal NumericTypes NumberType { get; set; }
        internal float? MinNumericValue { get; set; }
        internal float? MaxNumericValue { get; set; }
        internal bool EndsFoldout { get; set; }
        
        internal bool StartsFoldout
        {
            get
            {
                return (!string.IsNullOrWhiteSpace(FoldoutLabel));
            }
        }
        internal bool SetNumericProperty
        {
            get
            {
                if(NumberType != NumericTypes.NotNumber && MinNumericValue != null)
                    return true;
                return false;
            }
        }

        private string m_display;
        public string Display
        {
            get
            {
                if (string.IsNullOrWhiteSpace(m_display))
                    return Name;
                return m_display;
            }
            set { m_display = value; }
        }

        public FieldProperties(SerializedProperty property)
        {
            Property = property;
        }


    }
}
