using System.Collections.Generic;
using System.Linq;

namespace AideTool.ExtendedEditor.DataTransfer
{
    internal class PropertyCache
    {
        internal List<FieldProperties> Fields { get; private set; }
        internal bool Expanded { get; set; }
        internal int InsertionOrder { get; set; }

        private FieldProperties First => Fields.First();
        private FieldProperties Last => Fields.Last();
        internal string FoldoutLabel => First.FoldoutLabel;
        internal int FoldoutOrder => (int)First.Order;

        internal bool IsFold
        {
            get
            {
                int cnt = 0;
                if(First.StartsFoldout)
                {
                    foreach (FieldProperties field in Fields)
                        if (field.IsVisible)
                            cnt++;
                    return cnt > 0;
                }
                return false;
            }
        }

        internal bool InsideFold => First.StartsFoldout && !Last.EndsFoldout;

        internal PropertyCache(FieldProperties field)
        {
            Fields = new() { field };
            Expanded = false;
        }

        internal bool AddField(FieldProperties field) 
        {
            if (field.StartsFoldout)
            {
                Last.EndsFoldout = true;
                return false;
            }

            Fields.Add(field);
            return true;
        }

        internal void MergeFields(PropertyCache cache)
        {
            Last.EndsFoldout = false;

            Fields.AddRange(cache.Fields);

            Last.EndsFoldout = true;
        }

        internal void TurnInvisible(string fieldName)
        {
            foreach (FieldProperties field in Fields)
                if (field.Name == fieldName)
                    field.IsVisible = false;
        }

        internal void Dispose()
        {
            Fields.Clear();
            Fields = null;
        }
    }
}