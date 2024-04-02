namespace Simple3dEngine;

public static class MatrixOperations
{
    public static void MultiplyMatrixVector(ref Vector3d input, out Vector3d output, ref Matrix4x4 matrix)
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

    public static Vector3d MatrixMultiplyVector(Matrix4x4 m, Vector3d i)
    {
        Vector3d v = new ();
        v.X = i.X * m.M[0, 0] + i.Y * m.M[1, 0] + i.Z * m.M[2, 0] + i.W * m.M[3, 0];
        v.Y = i.X * m.M[0, 1] + i.Y * m.M[1, 1] + i.Z * m.M[2, 1] + i.W * m.M[3, 1];
        v.Z = i.X * m.M[0, 2] + i.Y * m.M[1, 2] + i.Z * m.M[2, 2] + i.W * m.M[3, 2];
        v.W = i.X * m.M[0, 3] + i.Y * m.M[1, 3] + i.Z * m.M[2, 3] + i.W * m.M[3, 3];
        return v;
    }

    public static Matrix4x4 MatrixMakeIdentity()
    {
        Matrix4x4 matrix = new Matrix4x4();
        matrix.M[0, 0] = 1.0f;
        matrix.M[1, 1] = 1.0f;
        matrix.M[2, 2] = 1.0f;
        matrix.M[3, 3] = 1.0f;
        return matrix;
    }

    public static Matrix4x4 MatrixMakeRotationX(float fAngleRad)
    {
        Matrix4x4 matrix = new ();
        matrix.M[0, 0] = 1.0f;
        matrix.M[1, 1] = MathF.Cos(fAngleRad);
        matrix.M[1, 2] = MathF.Sin(fAngleRad);
        matrix.M[2, 1] = -MathF.Sin(fAngleRad);
        matrix.M[2, 2] = MathF.Cos(fAngleRad);
        matrix.M[3, 3] = 1.0f;
        return matrix;
    }

    public static Matrix4x4 MatrixMakeRotationY(float fAngleRad)
    {
        Matrix4x4 matrix = new ();
        matrix.M[0, 0] = MathF.Cos(fAngleRad);
        matrix.M[0, 2] = MathF.Sin(fAngleRad);
        matrix.M[2, 0] = -MathF.Sin(fAngleRad);
        matrix.M[1, 1] = 1.0f;
        matrix.M[2, 2] = MathF.Cos(fAngleRad);
        matrix.M[3, 3] = 1.0f;
        return matrix;
    }

    public static Matrix4x4 MatrixMakeRotationZ(float fAngleRad)
    {
        Matrix4x4 matrix = new ();
        matrix.M[0, 0] = MathF.Cos(fAngleRad);
        matrix.M[0, 1] = MathF.Sin(fAngleRad);
        matrix.M[1, 0] = -MathF.Sin(fAngleRad);
        matrix.M[1, 1] = MathF.Cos(fAngleRad);
        matrix.M[2, 2] = 1.0f;
        matrix.M[3, 3] = 1.0f;
        return matrix;
    }

    public static Matrix4x4 MatrixMakeTranslation(float x, float y, float z)
    {
        Matrix4x4 matrix = new ();
        matrix.M[0, 0] = 1.0f;
        matrix.M[1, 1] = 1.0f;
        matrix.M[2, 2] = 1.0f;
        matrix.M[3, 3] = 1.0f;
        matrix.M[3, 0] = x;
        matrix.M[3, 1] = y;
        matrix.M[3, 2] = z;
        return matrix;
    }

    public static Matrix4x4 MatrixMakeProjection(float fovDegrees, float aspectRatio, float near, float far)
    {
        float fovRad = 1.0f / (MathF.Tan(fovDegrees * 0.5f / 180.0f * MathF.PI));
        Matrix4x4 matrix = new();
        matrix.M[0, 0] = aspectRatio * fovRad;
        matrix.M[1, 1] = fovRad;
        matrix.M[2, 2] = far / (far - near);
        matrix.M[3, 2] = (-far * near) / (far - near);
        matrix.M[2, 3] = 1.0f;
        matrix.M[3, 3] = 0.0f;
        return matrix;
    }

    public static Matrix4x4 MatrixMultiplyMatrix(Matrix4x4 m1, Matrix4x4 m2)
    {
        Matrix4x4 matrix = new ();

        for (int c = 0; c < 4; c++)
        {
            for (int r = 0; r < 4; r++)
            {
                matrix.M[r, c] = m1.M[r, 0] * m2.M[0, c] + m1.M[r, 1] * m2.M[1, c] + m1.M[r, 2] * m2.M[2, c] + m1.M[r, 3] * m2.M[3, c];
            }
        }
        return matrix;
    }

    public static Matrix4x4 MatrixPointAt(Vector3d pos, Vector3d target, Vector3d up)
    {
        // Calculate new forward direction
        Vector3d newForward = VectorOperations.VectorSub(target, pos);
        newForward = VectorOperations.Normalize(newForward);

        // Calculate new Up direction
        Vector3d a = VectorOperations.VectorMul(newForward, VectorOperations.DotProduct(up, newForward));
        Vector3d newUp = VectorOperations.VectorSub(up, a);
        newUp = VectorOperations.Normalize(newUp);

        // New Right direction is easy, it's just a cross product
        Vector3d newRight = VectorOperations.CrossProduct(newUp, newForward);

        // Construct Dimensioning and Translation Matrix
        Matrix4x4 matrix = new ();
        matrix.M[0, 0] = newRight.X; 
        matrix.M[0, 1] = newRight.Y; 
        matrix.M[0, 2] = newRight.Z; 
        matrix.M[0, 3] = 0.0f;
        matrix.M[1, 0] = newUp.X; 
        matrix.M[1, 1] = newUp.Y; 
        matrix.M[1, 2] = newUp.Z; 
        matrix.M[1, 3] = 0.0f;
        matrix.M[2, 0] = newForward.X; 
        matrix.M[2, 1] = newForward.Y; 
        matrix.M[2, 2] = newForward.Z; 
        matrix.M[2, 3] = 0.0f;
        matrix.M[3, 0] = pos.X; 
        matrix.M[3, 1] = pos.Y; 
        matrix.M[3, 2] = pos.Z; 
        matrix.M[3, 3] = 1.0f;
        return matrix;
    }

    public static Matrix4x4 MatrixQuickInverse(Matrix4x4 m) // Only for Rotation/Translation Matrices
    {
        Matrix4x4 matrix = new ();
        matrix.M[0, 0] = m.M[0, 0]; 
        matrix.M[0, 1] = m.M[1, 0]; 
        matrix.M[0, 2] = m.M[2, 0]; 
        matrix.M[0, 3] = 0.0f;
        matrix.M[1, 0] = m.M[0, 1]; 
        matrix.M[1, 1] = m.M[1, 1]; 
        matrix.M[1, 2] = m.M[2, 1]; 
        matrix.M[1, 3] = 0.0f;
        matrix.M[2, 0] = m.M[0, 2]; 
        matrix.M[2, 1] = m.M[1, 2]; 
        matrix.M[2, 2] = m.M[2, 2]; 
        matrix.M[2, 3] = 0.0f;
        matrix.M[3, 0] = -(m.M[3, 0] * matrix.M[0, 0] + m.M[3, 1] * matrix.M[1, 0] + m.M[3, 2] * matrix.M[2, 0]);
        matrix.M[3, 1] = -(m.M[3, 0] * matrix.M[0, 1] + m.M[3, 1] * matrix.M[1, 1] + m.M[3, 2] * matrix.M[2, 1]);
        matrix.M[3, 2] = -(m.M[3, 0] * matrix.M[0, 2] + m.M[3, 1] * matrix.M[1, 2] + m.M[3, 2] * matrix.M[2, 2]);
        matrix.M[3, 3] = 1.0f;
        return matrix;
    }
}