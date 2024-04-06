using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Audio;
using Meadow.Foundation.Graphics;
using Meadow.Hardware;
using Meadow.Peripherals.Displays;
using Meadow.Units;
using Simple3dEngine;
using System;
using System.Threading.Tasks;

namespace ProjLab3dTest
{
    // Change F7FeatherV2 to F7FeatherV1 if using Feather V1 Meadow boards
    // Change to F7CoreComputeV2 for Project Lab V3.x
    public class MeadowApp : App<F7CoreComputeV2>
    {
        private IProjectLabHardware projLab;

        MicroGraphics graphics = default!;

        Matrix4x4 projectionMatrix = new();

        Vector3d camera = new(0, 0, 0);
        Vector3d lightDirection = new(0.5f, 0.1f, 0.50f);

        readonly float Width = 320;
        readonly float Height = 240;

        //variable for rotation
        float theta;

        public override Task Initialize()
        {
            Resolver.Log.LogLevel = Meadow.Logging.LogLevel.Trace;

            Resolver.Log.Info("Initialize hardware...");

            //==== instantiate the project lab hardware
            projLab = ProjectLab.Create();

            Resolver.Log.Info($"Running on ProjectLab Hardware {projLab.RevisionString}");

            projLab.RgbLed?.SetColor(Color.Blue);

            graphics = new MicroGraphics(projLab.Display)
            {
                CurrentFont = new Font12x20(),
                Stroke = 1
            };

            // Projection Matrix
            float near = 0.1f;
            float far = 1000.0f;
            float aspectRatio = Height / Width;
            float fov = 1.0f / MathF.Tan(90.0f * 0.5f / 180.0f * MathF.PI);

            projectionMatrix.M[0, 0] = aspectRatio * fov;
            projectionMatrix.M[1, 1] = fov;
            projectionMatrix.M[2, 2] = far / (far - near);
            projectionMatrix.M[3, 2] = (-far * near) / (far - near);
            projectionMatrix.M[2, 3] = 1.0f;
            projectionMatrix.M[3, 3] = 0.0f;

            Resolver.Log.Info("Initialization complete");

            return base.Initialize();
        }

        public override Task Run()
        {
            Resolver.Log.Info("Run...");

            var objectMesh = Shapes.GenerateIcosahedron();

            var color = Color.Cyan;
            var colorFill = Color.DarkCyan;

            // Set up rotation matrices
            var matRotZ = new Matrix4x4();
            var matRotX = new Matrix4x4();

            // Draw Triangles
            var triProjected = new Triangle();
            var triTranslated = new Triangle();
            var triRotatedZ = new Triangle();
            var triRotatedZX = new Triangle();

            while (true)
            {
                graphics.Clear();

                theta += 0.05f; // Adjust the rotation speed as needed

                // Rotation Z
                matRotZ.M[0, 0] = MathF.Cos(theta);
                matRotZ.M[0, 1] = MathF.Sin(theta);
                matRotZ.M[1, 0] = -MathF.Sin(theta);
                matRotZ.M[1, 1] = MathF.Cos(theta);
                matRotZ.M[2, 2] = 1;
                matRotZ.M[3, 3] = 1;

                // Rotation X
                matRotX.M[0, 0] = 1;
                matRotX.M[1, 1] = MathF.Cos(theta * 0.25f);
                matRotX.M[1, 2] = MathF.Sin(theta * 0.25f);
                matRotX.M[2, 1] = -MathF.Sin(theta * 0.25f);
                matRotX.M[2, 2] = MathF.Cos(theta * 0.25f);
                matRotX.M[3, 3] = 1;

                foreach (var tri in objectMesh.Triangles)
                {
                    // Rotate in Z-Axis
                    MatrixOperations.MultiplyMatrixVector(ref tri.Points[0], out triRotatedZ.Points[0], ref matRotZ);
                    MatrixOperations.MultiplyMatrixVector(ref tri.Points[1], out triRotatedZ.Points[1], ref matRotZ);
                    MatrixOperations.MultiplyMatrixVector(ref tri.Points[2], out triRotatedZ.Points[2], ref matRotZ);

                    // Rotate in X-Axis
                    MatrixOperations.MultiplyMatrixVector(ref triRotatedZ.Points[0], out triRotatedZX.Points[0], ref matRotX);
                    MatrixOperations.MultiplyMatrixVector(ref triRotatedZ.Points[1], out triRotatedZX.Points[1], ref matRotX);
                    MatrixOperations.MultiplyMatrixVector(ref triRotatedZ.Points[2], out triRotatedZX.Points[2], ref matRotX);

                    // Offset into the screen
                    triTranslated = triRotatedZX;
                    triTranslated.Points[0].Z = triRotatedZX.Points[0].Z + 5.0f; // Increase the offset to ensure visibility
                    triTranslated.Points[1].Z = triRotatedZX.Points[1].Z + 5.0f; // Increase the offset to ensure visibility
                    triTranslated.Points[2].Z = triRotatedZX.Points[2].Z + 5.0f; // Increase the offset to ensure visibility

                    // Project triangles from 3D --> 2D
                    MatrixOperations.MultiplyMatrixVector(ref triTranslated.Points[0], out triProjected.Points[0], ref projectionMatrix);
                    MatrixOperations.MultiplyMatrixVector(ref triTranslated.Points[1], out triProjected.Points[1], ref projectionMatrix);
                    MatrixOperations.MultiplyMatrixVector(ref triTranslated.Points[2], out triProjected.Points[2], ref projectionMatrix);

                    // Check if triangle is facing towards the camera
                    if (IsTriangleFacingCamera(ref triTranslated, ref camera))
                    {
                        // Scale into view
                        triProjected.Points[0].X += 1.0f;
                        triProjected.Points[0].Y += 1.0f;
                        triProjected.Points[1].X += 1.0f;
                        triProjected.Points[1].Y += 1.0f;
                        triProjected.Points[2].X += 1.0f;
                        triProjected.Points[2].Y += 1.0f;
                        triProjected.Points[0].X *= 0.5f * Width;
                        triProjected.Points[0].Y *= 0.5f * Height;
                        triProjected.Points[1].X *= 0.5f * Width;
                        triProjected.Points[1].Y *= 0.5f * Height;
                        triProjected.Points[2].X *= 0.5f * Width;
                        triProjected.Points[2].Y *= 0.5f * Height;

                        // Calculate light shading
                        float lightIntensity = CalculateLightIntensity(GetNormalForTriangle(ref triTranslated), lightDirection);
                        var colorShaded = colorFill.WithBrightness(lightIntensity);

                        graphics.DrawTriangle(
                            (int)triProjected.Points[0].X, (int)triProjected.Points[0].Y,
                            (int)triProjected.Points[1].X, (int)triProjected.Points[1].Y,
                            (int)triProjected.Points[2].X, (int)triProjected.Points[2].Y,
                            colorShaded, true);

                        graphics.DrawTriangle(
                            (int)triProjected.Points[0].X, (int)triProjected.Points[0].Y,
                            (int)triProjected.Points[1].X, (int)triProjected.Points[1].Y,
                            (int)triProjected.Points[2].X, (int)triProjected.Points[2].Y,
                            color, false);
                    }
                }
                graphics.Show();
            }

            return base.Run();
        }

        public static Vector3d GetNormalForTriangle(ref Triangle triangle)
        {
            // Calculate triangle normal
            Vector3d line1 = triangle.Points[1] - triangle.Points[0];
            Vector3d line2 = triangle.Points[2] - triangle.Points[0];
            return VectorOperations.CrossProduct(line1, line2);
        }

        public static bool IsTriangleFacingCamera(ref Triangle triangle, ref Vector3d cameraPosition)
        {
            // Calculate triangle normal
            var normal = GetNormalForTriangle(ref triangle);

            // Calculate vector from triangle to camera
            Vector3d cameraToTriangle = triangle.Points[0] - cameraPosition;

            // Check the dot product between the normal and the vector to the camera
            float dotProduct = VectorOperations.DotProduct(normal, cameraToTriangle);

            // If the dot product is positive, the triangle is facing towards the camera
            return dotProduct > 0;
        }

        float CalculateLightIntensity(Vector3d normal, Vector3d lightDirection)
        {
            // Ensure both vectors are normalized
            normal = NormalizeVector(ref normal);
            lightDirection = NormalizeVector(ref lightDirection);

            // Calculate dot product
            float dotProduct = VectorOperations.DotProduct(normal, lightDirection);

            // Clamp dot product to ensure it's within [0, 1] range
            dotProduct = MathF.Max(0.0f, MathF.Min(1.0f, dotProduct));

            return dotProduct;
        }

        Vector3d NormalizeVector(ref Vector3d vector)
        {
            float length = MathF.Sqrt(vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z);

            // Avoid division by zero
            if (length != 0)
            {
                vector.X /= length;
                vector.Y /= length;
                vector.Z /= length;
            }

            return vector;
        }
    }
}