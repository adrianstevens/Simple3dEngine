namespace Simple3dEngine
{
    public static class MatrixOperations
    {
        public static void MultiplyMatrixVector(ref Vector3d input, out Vector3d output, Matrix4x4 matrix)
        {
            output = new Vector3d
            {
                X = input.X * matrix.M[0, 0] + input.Y * matrix.M[1, 0] + input.Z * matrix.M[2, 0] + matrix.M[3, 0],
                Y = input.X * matrix.M[0, 1] + input.Y * matrix.M[1, 1] + input.Z * matrix.M[2, 1] + matrix.M[3, 1],
                Z = input.X * matrix.M[0, 2] + input.Y * matrix.M[1, 2] + input.Z * matrix.M[2, 2] + matrix.M[3, 2]
            };

            float w = input.X * matrix.M[0, 3] + input.Y * matrix.M[1, 3] + input.Z * matrix.M[2, 3] + matrix.M[3, 3];

            if (w != 0.0f)
            {
                output.X /= w;
                output.Y /= w;
                output.Z /= w;
            }
        }
    }
}
