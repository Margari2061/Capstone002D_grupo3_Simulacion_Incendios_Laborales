using UnityEngine;

namespace AideTool
{
    public sealed class WaitForFrames : CustomYieldInstruction
    {
        private readonly float m_start;
        private readonly float m_totalTime;

        public override bool keepWaiting
        {
            get
            {
                return Time.time - m_start < m_totalTime;
            }
        }

        public WaitForFrames(int frames)
        {
            m_start = Time.time;
            float frameTime = 1f / 24f;
            m_totalTime = frames * frameTime;
        }
        
        public WaitForFrames(int frames, int framesPerSecond)
        {
            m_start = Time.time;
            float frameTime = 1f / (float)framesPerSecond;
            m_totalTime = frames * frameTime;
        }
    }
}
