namespace AideTool.Patterns
{
    public interface IObserver
    {
        public void OnEnableObserver();
        public void StartObserver();
        public void UpdateObserver();
        public void FixedUpdateObserver();
        public void LateUpdateObserver();
        public void OnDisableObserver();
    }
}
