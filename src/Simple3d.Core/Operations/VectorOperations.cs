using System;

namespace Simple3dEngine;

public static class VectorOperations
{
    public static Vector3d VectorAdd(Vector3d v1, Vector3d v2)
    {
        return new Vector3d(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z, 1.0f);
    }

    public static Vector3d VectorSub(Vector3d v1, Vector3d v2)
    {
        return new Vector3d(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z, 1.0f);
    }

    public static Vector3d VectorMul(Vector3d v1, float k)
    {
        return new Vector3d(v1.X * k, v1.Y * k, v1.Z * k, 1.0f);
    }

    public static Vector3d VectorDiv(Vector3d v1, float k)
    {
        return new Vector3d(v1.X / k, v1.Y / k, v1.Z / k, 1.0f);
    }

    public static float DotProduct(ref Vector3d v1, ref Vector3d v2)
    {
        return v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z;
    }

    public static float VectorLength(ref Vector3d v)
    {
        return MathF.Sqrt(DotProduct(ref v, ref v));
    }

    public static Vector3d Normalize(ref Vector3d v)
    {
        float l = VectorLength(ref v);
        return new Vector3d(v.X / l, v.Y / l, v.Z / l, 1.0f);
    }

    public static Vector3d CrossProduct(Vector3d v1, Vector3d v2)
    {
        return new Vector3d(
            v1.Y * v2.Z - v1.Z * v2.Y,
            v1.Z * v2.X - v1.X * v2.Z,
            v1.X * v2.Y - v1.Y * v2.X,
            1.0f
        );
    }

    public static Vector3d VectorIntersectPlane(Vector3d planeP, Vector3d planeN, Vector3d lineStart, Vector3d lineEnd)
    {
        planeN = Normalize(ref planeN);
        float plane_d = -DotProduct(ref planeN, ref planeP);
        float ad = DotProduct(ref lineStart, ref planeN);
        float bd = DotProduct(ref lineEnd, ref planeN);
        float t = (-plane_d - ad) / (bd - ad);
        Vector3d lineStartToEnd = VectorSub(lineEnd, lineStart);
        Vector3d lineToIntersect = VectorMul(lineStartToEnd, t);
        return VectorAdd(lineStart, lineToIntersect);
    }
}