using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AideTool.Animation
{
    public abstract class AnimatorParameterHandler<T>
    {
        protected readonly Animator[] m_animators;
        protected readonly List<int> Hashes = new();
        protected T Value { get; set; }

        protected AnimatorParameterHandler(Animator[] animators, params string[] parameters)
        {
            m_animators = animators;

            IEnumerable<int> hashes = parameters
                .Select(p => Animator.StringToHash(p));

            Hashes.AddRange(hashes);
        }

        protected abstract void SetAction(Animator animator, T value);

        public virtual void Set(T value)
        {
            Value = value;

            foreach (Animator anim in m_animators)
                SetAction(anim, value);
        }

        public virtual T Get() => Value;

        public virtual void Trigger()
        {
            Debug.LogWarning("Only trigger parameters can be triggered");
        }

        public virtual void Reset()
        {
            Debug.LogWarning("Only trigger parameters can be reset");
        }
    }
}
