using AideTool.ExtendedEditor.DataTransfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

#if !EDITOR_DEFAULT
namespace AideTool.ExtendedEditor
{
    [CustomEditor(typeof(MonoBehaviour), true, isFallback = true), CanEditMultipleObjects]
    public class AideEditor : Editor
    {
        private readonly List<PropertyCache> m_cache = new();
        private readonly List<MethodProperties> m_methods = new();
        private bool m_initialized;

        protected virtual void OnEnable() 
        {
            m_initialized = false; 
        }

        protected virtual void OnDisable()
        {
            if (target != null)
                foreach (PropertyCache c in m_cache)
                    c.Dispose();
        }

        public override bool RequiresConstantRepaint()
        {
            return AideFramework.NeedToRepaint;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            Setup();

            if (m_cache.Count == 0 && m_methods.Count == 0)
            {
                DrawDefaultInspector();
                return;
            }

            Header();
            Body();

            serializedObject.ApplyModifiedProperties();
        }

#region Setup
        private void Setup()
        {
            if (!m_initialized)
            {
                m_cache.Clear();
                m_methods.Clear();

                SerializedProperty scriptProperty = serializedObject.FindProperty("m_Script");

                if (scriptProperty != null)
                {
                    FieldProperties scriptProperties = new(scriptProperty)
                    {
                        IsVisible = true,
                        IsValidator = false,
                        FoldoutLabel = string.Empty,
                        EndsFoldout = true,
                        Order = FoldoutOrder.Invisible,
                        Display = null,
                        MinNumericValue = null,
                        MaxNumericValue = null,
                        NumberType = NumericTypes.NotNumber
                    };
                    AddPropertyToCache(scriptProperties);
                }

                List<FieldInfo> fields = GetFields();

                foreach (FieldInfo field in fields)
                {
                    bool includeField = DefineIncludeField(field);
                    bool isValidator = DefineValidatorField(field);
                    string foldoutLabel = DefineFoldoutStart(field, out FoldoutOrder order);
                    bool endsFoldout = DefineFoldoutEnd(field);
                    string display = DefineDisplay(field);
                    float? minValue = DefineMinValue(field, out NumericTypes numType);
                    float? maxValue = DefineMaxValue(field, numType);

                    SerializedProperty property = serializedObject.FindProperty(field.Name);

                    if(property != null)
                    {
                        FieldProperties fieldProperties = new(property)
                        {
                            IsVisible = includeField,
                            IsValidator = isValidator,
                            FoldoutLabel = foldoutLabel,
                            EndsFoldout = endsFoldout,
                            Order = order,
                            Display = display,
                            MinNumericValue = minValue,
                            MaxNumericValue = maxValue,
                            NumberType = numType
                        };

                        AddPropertyToCache(fieldProperties);
                    }
                    
                }

                MergeProperties();
                SortProperties();

                List<MethodProperties> methods = GetMethods(out List<FieldVisibilityMethod> visList);

                foreach (FieldVisibilityMethod method in visList)
                    foreach (string fieldName in method.FieldNames)
                        foreach (PropertyCache cache in m_cache)
                        {
                            bool visible = (bool)method.Method?.Invoke(serializedObject.targetObject, null);
                            if(!visible)
                                cache.TurnInvisible(fieldName);
                        }

                m_methods.AddRange(methods);

                m_initialized = true;
            }
        }

        private void AddPropertyToCache(FieldProperties field)
        {
            if (m_cache.Count == 0 || !m_cache.Last().InsideFold)
            {
                PropertyCache cache = new(field);
                cache.InsertionOrder = m_cache.Count;
                m_cache.Add(cache);
                return;
            }

            bool result = m_cache
                .Last()
                .AddField(field);

            if(!result)
            {
                PropertyCache cache = new(field);
                cache.InsertionOrder = m_cache.Count;
                m_cache.Add(cache);
            }
        }
#endregion

#region Draw
        private void Header()
        {
            EditorGUILayout.Space();
            ComponentHeaderAttribute componentHeader = target
                .GetType()
                .GetCustomAttribute<ComponentHeaderAttribute>();

            if (componentHeader != null && !componentHeader.UsesDefaultHeader)
                return;

            SerializedProperty property = m_cache
                .First()
                .Fields
                .First()
                .Property;

            using (new EditorGUI.DisabledScope("m_Script" == property.propertyPath))
            {
                EditorGUILayout.PropertyField(property, true);
                EditorGUILayout.Space();
            }
        }

        private void Body()
        {
            int cacheLength = m_cache.Count;
            for (int i = 1; i < cacheLength; i++)
            {
                PropertyCache cache = m_cache[i];

                if(cache.IsFold)
                {
                    EditorExtensions.UseVerticalLayout(() => Foldout(m_cache[i]), EditorStyle.Box);
                    EditorGUI.indentLevel = 0;
                    continue;
                }

                EditorExtensions.PropertyField(cache.Fields.First(), ref m_initialized);
            }

            EditorGUILayout.Space();

            int methodsLen = m_methods.Count;
            for (int i = 0; i < methodsLen; i+=2)
            {
                EditorGUILayout.BeginHorizontal();

                EditorExtensions.UseButton(serializedObject, m_methods[i]);

                if(i+1 < methodsLen)
                    EditorExtensions.UseButton(serializedObject, m_methods[i+1]);

                EditorGUILayout.EndHorizontal();
            }
        }

        private void Foldout(PropertyCache cache)
        {
            EditorGUI.indentLevel = 1;
            cache.Expanded = EditorGUILayout.Foldout(cache.Expanded, EditorExtensions.NicifyNames(cache.FoldoutLabel), true,
                    EditorStyle.Foldout);

            if (cache.Expanded)
            {
                EditorGUI.indentLevel = 2;

                EditorExtensions.UseVerticalLayout(() =>
                {
                    for (int i = 0; i < cache.Fields.Count; i++)
                    {
                        Child(cache, i);
                        
                    }
                }, EditorStyle.BoxChild);
            }
        }

        private void Child(PropertyCache cache, int i)
        {
            FieldProperties field = cache.Fields[i];

            EditorExtensions.PropertyField(field, ref m_initialized);
        }
#endregion

#region Field Properties
        private bool DefineIncludeField(FieldInfo field)
        {
            HideInInspector hideattr = field.GetCustomAttribute<HideInInspector>();
            if (hideattr != null)
                return false;

            return true;
        }

        private bool DefineValidatorField(FieldInfo field)
        {
            InspectorValidatorAttribute attribute = field.GetCustomAttribute<InspectorValidatorAttribute>();
            if (attribute == null)
                return false;
            return true;
        }

        private string DefineFoldoutStart(FieldInfo field, out FoldoutOrder order)
        {
            FoldoutAttribute attribute = field.GetCustomAttribute<FoldoutAttribute>();
            if (attribute == null)
            {
                order = FoldoutOrder.Default;
                return string.Empty;
            }
            order = attribute.Order;
            return attribute.Display;
        }

        private bool DefineFoldoutEnd(FieldInfo field)
        {
            EndFoldoutAttribute attribute = field.GetCustomAttribute<EndFoldoutAttribute>();
            if (attribute == null)
                return false;
            return true;
        }

        private string DefineDisplay(FieldInfo field)
        {
            InspectorNameAttribute attribute = field.GetCustomAttribute<InspectorNameAttribute>();
            if (attribute != null)
                return attribute.displayName;
            return field.Name;
        }

        private float? DefineMinValue(FieldInfo field, out NumericTypes numTypes)
        {
            MinAttribute minAttribute = field.GetCustomAttribute<MinAttribute>();
            RangeAttribute rangeAttribute = field.GetCustomAttribute<RangeAttribute>();

            MinAttributeReference attr = new(minAttribute, rangeAttribute);

            if(attr.IsValid)
            {
                if(field.FieldType == typeof(int))
                {
                    numTypes = NumericTypes.Int;
                    return attr.MinValue;
                }

                if(field.FieldType == typeof(float))
                {
                    numTypes = NumericTypes.Float;
                    return attr.MinValue;
                }
                
                if(field.FieldType == typeof(double))
                {
                    numTypes = NumericTypes.Double;
                    return attr.MinValue;
                }
                
                if(field.FieldType == typeof(long))
                {
                    numTypes = NumericTypes.Long;
                    return attr.MinValue;
                }
            }

            numTypes = NumericTypes.NotNumber;
            return null;
        }
        
        private float? DefineMaxValue(FieldInfo field, NumericTypes numTypes)
        {
            if (numTypes == NumericTypes.NotNumber)
                return null;

            RangeAttribute attribute = field.GetCustomAttribute<RangeAttribute>();

            if (attribute != null)
                return attribute.max;

            return null;
        }

        private void MergeProperties()
        {
            List<PropertyCache> toRemove = new();

            for (int i = 0; i < m_cache.Count - 1; i++)
            {
                if (string.IsNullOrWhiteSpace(m_cache[i].FoldoutLabel))
                    continue;

                for (int e = i+1; e < m_cache.Count; e++)
                {
                    if (m_cache[i].FoldoutLabel == m_cache[e].FoldoutLabel)
                    {
                        m_cache[i].MergeFields(m_cache[e]);
                        toRemove.Add(m_cache[e]);
                    }
                }
            }

            foreach(PropertyCache cache in toRemove)
                m_cache.Remove(cache);
        }

        private void SortProperties()
        {
            m_cache.Sort((a, b) => {
                int result = a.FoldoutOrder - b.FoldoutOrder;
                return result;
            });
        }
#endregion

#region Reflection
        private static List<Type> GetTypeTree(Type type, int depth = 8)
        {
            List<Type> types = new();
            int i = 0;
            while (type != typeof(MonoBehaviour) && i < depth)
            {
                types.Add(type);
                type = type.BaseType;
                i++;
            }

            List<Type> result = new();

            for (int e = types.Count - 1; e >= 0; e--)
                result.Add(types[e]);

            return result;
        }

        private List<FieldInfo> GetFields()
        {
            List<FieldInfo> result = new();
            List<Type> typeTree = GetTypeTree(target.GetType());

            foreach (Type type in typeTree)
            {
                FieldInfo[] fields = type
                    .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                foreach(FieldInfo field in fields)
                    if(!field.Name.Contains("k__BackingField") && !result.Contains(field))
                        result.Add(field);
            }

            return result;
        }

        private List<MethodProperties> GetMethods(out List<FieldVisibilityMethod> visibilityMethods)
        {
            List<MethodProperties> result = new();
            visibilityMethods = new();
            List<Type> typeTree = GetTypeTree(target.GetType());

            foreach (Type type in typeTree)
            {
                MethodInfo[] methods = type
                    .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                foreach(MethodInfo method in methods)
                {
                    ContextMenu contextAttribute = method.GetCustomAttribute<ContextMenu>();
                    if(contextAttribute != null)
                    {
                        MethodProperties contextProperty = new(method, contextAttribute.menuItem);
                        result.Add(contextProperty);
                    }

                    HideFieldAttribute hideAttribute = method.GetCustomAttribute<HideFieldAttribute>();
                    if(hideAttribute != null)
                    {
                        FieldVisibilityMethod visibilityProperty = new(method, hideAttribute.FieldNames);
                        visibilityMethods.Add(visibilityProperty);
                    }
                }
            }

            return result;
        }
#endregion

    }
}
#endif