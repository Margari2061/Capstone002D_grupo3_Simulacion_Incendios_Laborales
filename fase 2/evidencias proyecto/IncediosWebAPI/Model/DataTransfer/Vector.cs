namespace IncediosWebAPI.Model.DataTransfer;

public struct Vector
{
    public float X { get; set; }
    public float Y { get; set; }

    public Vector()
    {
        X = 0;
        Y = 0;
    }

    public Vector(float x, float y)
    {
        X = x; 
        Y = y; 
    }
}
