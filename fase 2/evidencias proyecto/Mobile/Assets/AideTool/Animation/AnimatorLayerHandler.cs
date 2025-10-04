using AideTool.Patterns;
using System.Collections.Generic;
using UnityEngine;

namespace AideTool.Animation
{
    public class AnimatorLayerHandler
    {
        private readonly Dictionary<Animator, int> m_indexes = new();

        private float m_weight;
        public float Weight
        {
            get => m_weight;
            set
            {
                value = value.Clamp();
                m_weight = value;
                foreach (KeyValuePair<Animator, int> layer in m_indexes)
                    layer.Key.SetLayerWeight(layer.Value, value);
            }
        }

        private readonly AideBehaviour m_context;

        public AnimatorLayerHandler(Animator[] animators, AideBehaviour context, string layerName, float startWeight = 0f)
        {
            foreach(Animator animator in animators)
            {
                int index = animator.GetLayerIndex(layerName);
                if(index > 0)
                    m_indexes.Add(animator, index);
            }

            m_context = context;

            Weight = startWeight;
        }

        public void Enable() => Weight = 1f;

        public void Disable() => Weight = 0f;
    }
}
