using Simple3d.Core;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Simple3dEngine;

public class Meadow3dEngine
{
    public Vector3d Camera;

    public Vector3d? Light;

    float Width, Height; //temp

    public List<Object3d> Objects { get; set; }

    private Matrix4x4 projectionMatrix;

    public Meadow3dEngine(float width, float height, float fovDegrees = 90f, float near = 0.1f, float far = 1000.0f)
    {
        Objects = new List<Object3d> { };

        Camera = new Vector3d(0, 0, 0);

        projectionMatrix = MatrixOperations.CreateProjectionMatrix(fovDegrees, height / width, near, far);
    }

    public void AddObject(Object3d obj3d)
    {
        if(Objects.Contains(obj3d)) return;

        Objects.Add(obj3d);
    }

    public void RemoveObject(Object3d obj3d)
    {
        Objects.Remove(obj3d);
    }

    public void ClearObjects()
    {
        Objects.Clear();
    }

    public void Run()
    {
        _ = Task.Run(() =>
        {
            // Set up rotation matrices
            var matRotZ = new Matrix4x4();
            var matRotY = new Matrix4x4();
            var matRotX = new Matrix4x4();

            var triProjected = new Triangle();
            var triTranslated = new Triangle();
            var triRotatedZ = new Triangle();
            var triRotatedYZ = new Triangle();
            var triRotatedXYZ = new Triangle();

            while (true)
            {
                // graphics.Clear();

                foreach (var obj3d in Objects)
                {
                    // Rotation X
                    matRotX = MatrixOperations.CreateXRotationMatrix(obj3d.XRotation);

                    // Rotation X
                    matRotY = MatrixOperations.CreateYRotationMatrix(obj3d.YRotation);

                    // Rotation Z
                    matRotZ = MatrixOperations.CreateZRotationMatrix(obj3d.ZRotation);

                    Triangle tri;

                    for (int i = 0; i < obj3d.Mesh.Triangles.Count; i++)
                    {
                        tri = obj3d.Mesh.Triangles[i];

                        // Rotate in Z-Axis
                        MatrixOperations.MatrixMultiplyTriangle(ref tri, ref triRotatedZ, ref matRotZ);

                        // Rotate in Y-Axis
                        MatrixOperations.MatrixMultiplyTriangle(ref tri, ref triRotatedYZ, ref matRotZ);


                        // Rotate in X-Axis
                        MatrixOperations.MatrixMultiplyTriangle(ref triRotatedYZ, ref triRotatedXYZ, ref matRotX);

                        // Offset into the screen
                        triTranslated = triRotatedXYZ;
                        TriangleOperations.TranslateZ(ref triTranslated, obj3d.XTranslation);

                        // Check if triangle is facing towards the camera
                        if (TriangleOperations.IsFacingCamera(ref triTranslated, ref Camera))
                        {
                            // Project triangles from 3D --> 2D
                            MatrixOperations.MultiplyMatrixVector(ref triTranslated.Points[0], ref triProjected.Points[0], ref projectionMatrix);
                            MatrixOperations.MultiplyMatrixVector(ref triTranslated.Points[1], ref triProjected.Points[1], ref projectionMatrix);
                            MatrixOperations.MultiplyMatrixVector(ref triTranslated.Points[2], ref triProjected.Points[2], ref projectionMatrix);

                            // Scale into view
                            TriangleOperations.TranslateX(ref triProjected, 1.0f);
                            TriangleOperations.TranslateY(ref triProjected, 1.0f);

                            TriangleOperations.ScaleX(ref triProjected, Width * 0.5f);
                            TriangleOperations.ScaleY(ref triProjected, Height * 0.5f);

                            // Calculate light shading
                            if (obj3d.FillTriangles)
                            { 
                                if(Light is not null)
                                {
                                    float lightIntensity = CalculateLightIntensity(TriangleOperations.GetNormal(ref triTranslated), Light.Value);
                                //    var colorShaded = obj3d.ColorFill.WithBrightness(lightIntensity);
                                }
                                else
                                {
                                    //colorShaded = obj3d.ColorFill;
                                }
                                /*
                                graphics.DrawTriangle(
                                (int)triProjected.Points[0].X, (int)triProjected.Points[0].Y,
                                (int)triProjected.Points[1].X, (int)triProjected.Points[1].Y,
                                (int)triProjected.Points[2].X, (int)triProjected.Points[2].Y,
                                colorShaded, true);
                                */
                            }

                            /*
                            if(obj3d.ShowWireFrame)
                            {
                                graphics.DrawTriangle(
                                    (int)triProjected.Points[0].X, (int)triProjected.Points[0].Y,
                                    (int)triProjected.Points[1].X, (int)triProjected.Points[1].Y,
                                    (int)triProjected.Points[2].X, (int)triProjected.Points[2].Y,
                                    obj3d.WireFrameColor, false);
                            } */
                        }
                    }
                }
               // graphics.Show();
            }
        });
    }

    float CalculateLightIntensity(Vector3d normal, Vector3d lightDirection)
    {
        // Ensure both vectors are normalized
        normal = VectorOperations.Normalize(ref normal);
        lightDirection = VectorOperations.Normalize(ref lightDirection);

        // Calculate dot product
        return VectorOperations.DotProduct(ref normal, ref lightDirection);
    }
}
