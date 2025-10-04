using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AideTool.ExtendedEditor.DataTransfer
{
    internal struct MinAttributeReference
    {
        public float? MinValue { get; set; }
        public bool IsValid { get { return MinValue != null; } }

        public MinAttributeReference(MinAttribute minAttr, RangeAttribute rngAttr)
        {
            MinValue = null;
            List<float> values = new();
            
            if(minAttr != null)
                values.Add(minAttr.min);

            if (rngAttr != null)
                values.Add(rngAttr.min);

            if(values.Count > 0)
                MinValue = values.Min();
        }
    }
}
