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
}