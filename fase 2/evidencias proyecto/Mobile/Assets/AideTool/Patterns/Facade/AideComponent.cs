using System;
using UnityEngine;

namespace AideTool.Patterns
{
    public class AideComponent<T> : IAideCore where T : Component
    {
        private readonly T m_component;
        public T Component => m_component;

        private GameObject m_gameObject;
        public GameObject GameObject
        {
            get
            {
                if (m_gameObject == null)
                    m_gameObject = Component.gameObject;
                return m_gameObject;
            }
        }

        private Transform m_transform;
        public Transform Transform
        {
            get
            {
                if (m_transform == null)
                    m_transform = Component.transform;
                return m_transform;
            }
        }

        private string m_name;
        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(m_name))
                    m_name = Transform.name;
                return m_name;
            }
            set
            {
                m_name = value;
                Transform.name = value;
            }
        }

        public AideComponent(T component)
        {
            if (component == null)
                throw new NullReferenceException("Component is null");
            m_component = component;
        }
    }
}
