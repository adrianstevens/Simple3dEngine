namespace Simple3dEngine
{
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

        public static float DotProduct(Vector3d v1, Vector3d v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z;
        }

        public static float VectorLength(Vector3d v)
        {
            return (float)Math.Sqrt(DotProduct(v, v));
        }

        public static Vector3d Normalize(Vector3d v)
        {
            float l = VectorLength(v);
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

        public static Vector3d VectorIntersectPlane(Vector3d plane_p, Vector3d plane_n, Vector3d lineStart, Vector3d lineEnd)
        {
            plane_n = Normalize(plane_n);
            float plane_d = -DotProduct(plane_n, plane_p);
            float ad = DotProduct(lineStart, plane_n);
            float bd = DotProduct(lineEnd, plane_n);
            float t = (-plane_d - ad) / (bd - ad);
            Vector3d lineStartToEnd = VectorSub(lineEnd, lineStart);
            Vector3d lineToIntersect = VectorMul(lineStartToEnd, t);
            return VectorAdd(lineStart, lineToIntersect);
        }
    }
}