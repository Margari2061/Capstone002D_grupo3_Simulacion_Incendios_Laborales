using System;
using UnityEngine;

namespace AideTool
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public sealed class FoldoutAttribute : PropertyAttribute
    {
		public string Display { get; private set; }
		public FoldoutOrder Order { get; private set; }

		public FoldoutAttribute(string display, FoldoutOrder order = FoldoutOrder.Default)
		{
			Display = display;
			Order = order;
		}
	}
}
