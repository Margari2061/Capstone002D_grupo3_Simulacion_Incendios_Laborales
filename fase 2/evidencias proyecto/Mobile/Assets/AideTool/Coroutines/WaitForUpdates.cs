using UnityEngine;

namespace AideTool
{
    public sealed class WaitForUpdates : CustomYieldInstruction
    {
        private readonly int m_totalCount;
        private int m_count;

        public override bool keepWaiting
        {
            get
            {
                if(m_count < m_totalCount)
                {
                    m_count++;
                    return true;
                }
                return false;
            }
        }

        public WaitForUpdates(int count)
        {
            m_count = 0;
            m_totalCount = count;
        }
    }
}