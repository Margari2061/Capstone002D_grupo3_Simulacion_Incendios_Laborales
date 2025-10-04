namespace AideTool.Input.Legacy
{
    public sealed class LegacyButton : IButton
    {
        public LegacyButton(float holdTime = 0f) : base(holdTime) { }

		public void SetValues(bool value) => HandleState(value);

        public void SetValues(string buttonName) => HandleState(UnityEngine.Input.GetButton(buttonName));
    }
}