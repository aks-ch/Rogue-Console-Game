namespace RogueConsoleGame.DataTypes;

public record Vector2(int X, int Y)
{
    public static Vector2 operator +(Vector2 v1, Vector2 v2)
    {
        return new Vector2(v1.X + v2.X, v1.Y + v2.Y);
    }
    
    public static Vector2 operator -(Vector2 v1, Vector2 v2) 
    {
        return new Vector2(v1.X - v2.X, v1.Y - v2.Y);
    }

    public int GetGeometricDistance(Vector2 v2)
    {
        return GetGeometricDistance(this, v2);
    }

    public static int GetGeometricDistance(Vector2 v1, Vector2 v2)
    {
        return Math.Abs(v1.X - v2.X) + Math.Abs(v1.Y - v2.Y);
    }
}