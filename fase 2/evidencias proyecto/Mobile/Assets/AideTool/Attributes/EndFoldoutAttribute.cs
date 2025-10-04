using System;
using UnityEngine;

namespace AideTool
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public sealed class EndFoldoutAttribute : PropertyAttribute
    {
		public EndFoldoutAttribute() { }
	}
}
