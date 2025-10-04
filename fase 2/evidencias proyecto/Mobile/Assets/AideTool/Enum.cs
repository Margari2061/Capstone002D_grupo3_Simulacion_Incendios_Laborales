namespace AideTool
{
    public enum Direction
    {
        None,
        Left,
        Right,
        Up,
        Down
    }

    public enum ButtonState
    {
        Free,
        Pressed,
        Holding,
        Held,
        Released
    }

    public enum RequestResultStatus
    {
        Progressing,
        Failed,
        Ok,
        Other
    }

    public enum InspectorAideBehaviour
    {
        Default, Same, Detailed
    }

    public enum FoldoutOrder
    {
        Invisible, Default, First, Last
    }

}
