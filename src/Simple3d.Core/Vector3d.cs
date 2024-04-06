namespace Simple3dEngine;

public struct Vector3d
{
    public float X, Y, Z, W;

    public Vector3d(float x, float y, float z, float w = 1.0f)
    {
        X = x; Y = y; Z = z; W = w;
    }

    public static Vector3d operator +(Vector3d v1, Vector3d v2)
    {
        return new Vector3d(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z, v1.W + v2.W);
    }

    public static Vector3d operator -(Vector3d v1, Vector3d v2)
    {
        return new Vector3d(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z, v1.W - v2.W);
    }
}