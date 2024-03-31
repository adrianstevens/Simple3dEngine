using Meadow;
using Meadow.Foundation.Displays;
using Meadow.Foundation.Graphics;
using Simple3dEngine;
using System.Windows.Forms;

namespace WinForms_Sample
{
    public class MeadowApp : App<Meadow.Windows>
    {
        WinFormsDisplay? display;
        MicroGraphics graphics = default!;

        Matrix4x4 matProj = new Matrix4x4();

        Vector3d camera = new Vector3d(0, 0, 0);
        Vector3d lightDirection = new Vector3d(0.0f, 0.0f, -1.0f);

        float Width = 320;
        float Height = 240;

        float fTheta;

        public override Task Initialize()
        {
            display = new WinFormsDisplay((int)Width, (int)Height, displayScale: 1.5f);

            graphics = new MicroGraphics(display)
            {
                CurrentFont = new Font12x20(),
                Stroke = 1
            };

            // Projection Matrix
            float fNear = 0.1f;
            float fFar = 1000.0f;
            float fFov = 90.0f;
            float fAspectRatio = Height / Width;
            float fFovRad = 1.0f / (float)Math.Tan(fFov * 0.5f / 180.0f * 3.14159f);

            matProj.M[0, 0] = fAspectRatio * fFovRad;
            matProj.M[1, 1] = fFovRad;
            matProj.M[2, 2] = fFar / (fFar - fNear);
            matProj.M[3, 2] = (-fFar * fNear) / (fFar - fNear);
            matProj.M[2, 3] = 1.0f;
            matProj.M[3, 3] = 0.0f;

            var meshCube = Shapes.GenerateCube();

            _ = Task.Run(() =>
            {
                while (true)
                {
                    graphics.Clear();

                    // Set up rotation matrices
                    var matRotZ = new Matrix4x4();
                    var matRotX = new Matrix4x4();

                    fTheta += 0.05f; // Adjust the rotation speed as needed

                    // Rotation Z
                    matRotZ.M[0, 0] = (float)Math.Cos(fTheta);
                    matRotZ.M[0, 1] = (float)Math.Sin(fTheta);
                    matRotZ.M[1, 0] = -(float)Math.Sin(fTheta);
                    matRotZ.M[1, 1] = (float)Math.Cos(fTheta);
                    matRotZ.M[2, 2] = 1;
                    matRotZ.M[3, 3] = 1;

                    // Rotation X
                    matRotX.M[0, 0] = 1;
                    matRotX.M[1, 1] = (float)Math.Cos(fTheta * 0.5f);
                    matRotX.M[1, 2] = (float)Math.Sin(fTheta * 0.5f);
                    matRotX.M[2, 1] = -(float)Math.Sin(fTheta * 0.5f);
                    matRotX.M[2, 2] = (float)Math.Cos(fTheta * 0.5f);
                    matRotX.M[3, 3] = 1;

                    // Draw Triangles
                    var triProjected = new Triangle();
                    var triTranslated = new Triangle();
                    var triRotatedZ = new Triangle();
                    var triRotatedZX = new Triangle();

                    foreach (var tri in meshCube.Triangles)
                    {
                        // Rotate in Z-Axis
                        MatrixOperations.MultiplyMatrixVector(ref tri.Points[0], out triRotatedZ.Points[0], matRotZ);
                        MatrixOperations.MultiplyMatrixVector(ref tri.Points[1], out triRotatedZ.Points[1], matRotZ);
                        MatrixOperations.MultiplyMatrixVector(ref tri.Points[2], out triRotatedZ.Points[2], matRotZ);

                        // Rotate in X-Axis
                        MatrixOperations.MultiplyMatrixVector(ref triRotatedZ.Points[0], out triRotatedZX.Points[0], matRotX);
                        MatrixOperations.MultiplyMatrixVector(ref triRotatedZ.Points[1], out triRotatedZX.Points[1], matRotX);
                        MatrixOperations.MultiplyMatrixVector(ref triRotatedZ.Points[2], out triRotatedZX.Points[2], matRotX);

                        // Offset into the screen
                        triTranslated = triRotatedZX;
                        triTranslated.Points[0].Z = triRotatedZX.Points[0].Z + 5.0f; // Increase the offset to ensure visibility
                        triTranslated.Points[1].Z = triRotatedZX.Points[1].Z + 5.0f; // Increase the offset to ensure visibility
                        triTranslated.Points[2].Z = triRotatedZX.Points[2].Z + 5.0f; // Increase the offset to ensure visibility

                        // Project triangles from 3D --> 2D
                        MatrixOperations.MultiplyMatrixVector(ref triTranslated.Points[0], out triProjected.Points[0], matProj);
                        MatrixOperations.MultiplyMatrixVector(ref triTranslated.Points[1], out triProjected.Points[1], matProj);
                        MatrixOperations.MultiplyMatrixVector(ref triTranslated.Points[2], out triProjected.Points[2], matProj);

                        // Check if triangle is facing towards the camera
                        if (IsTriangleFacingCamera(triTranslated, camera))
                        {
                          //  float lightIntensity = CalculateLightIntensity(tri, lightDirection);


                            // Scale into view
                            triProjected.Points[0].X += 1.0f; triProjected.Points[0].Y += 1.0f;
                            triProjected.Points[1].X += 1.0f; triProjected.Points[1].Y += 1.0f;
                            triProjected.Points[2].X += 1.0f; triProjected.Points[2].Y += 1.0f;
                            triProjected.Points[0].X *= 0.5f * Width;
                            triProjected.Points[0].Y *= 0.5f * Height;
                            triProjected.Points[1].X *= 0.5f * Width;
                            triProjected.Points[1].Y *= 0.5f * Height;
                            triProjected.Points[2].X *= 0.5f * Width;
                            triProjected.Points[2].Y *= 0.5f * Height;

                            graphics.DrawTriangle(
                                (int)triProjected.Points[0].X, (int)triProjected.Points[0].Y,
                                (int)triProjected.Points[1].X, (int)triProjected.Points[1].Y,
                                (int)triProjected.Points[2].X, (int)triProjected.Points[2].Y,
                                Color.Red);
                        }
                    }

                    graphics.Show();
                }
            });

            return Task.CompletedTask;
        }

        public override Task Run()
        {
            Application.Run(display!);

            return Task.CompletedTask;
        }

        public static async Task Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            ApplicationConfiguration.Initialize();

            await MeadowOS.Start(args);
        }

        public static bool IsTriangleFacingCamera(Triangle triangle, Vector3d cameraPosition)
        {
            // Calculate triangle normal
            Vector3d line1 = triangle.Points[1] - triangle.Points[0];
            Vector3d line2 = triangle.Points[2] - triangle.Points[0];
            Vector3d normal = VectorOperations.CrossProduct(line1, line2);

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
            normal = NormalizeVector(normal);
            lightDirection = NormalizeVector(lightDirection);

            // Calculate dot product
            float dotProduct = VectorOperations.DotProduct(normal, lightDirection);

            // Clamp dot product to ensure it's within [0, 1] range
            dotProduct = Math.Max(0.0f, Math.Min(1.0f, dotProduct));

            return dotProduct;
        }

        Vector3d NormalizeVector(Vector3d vector)
        {
            float length = (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z);

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
