using Meadow;
using Meadow.Foundation.Displays;
using Meadow.Foundation.Graphics;
using Simple3dEngine;
using System.Windows.Forms;

namespace WinForms_Sample;

public class MeadowApp : App<Meadow.Windows>
{
    WinFormsDisplay? display;
    MicroGraphics graphics = default!;

    Matrix4x4 projectionMatrix = new();

    Vector3d camera = new(0, 0, 0);
    Vector3d lightDirection = new(0.5f, 0.0f, -1.0f);

    readonly float Width = 1280;
    readonly float Height = 960;

    public override Task Initialize()
    {
        display = new WinFormsDisplay((int)Width, (int)Height, displayScale: 1.0f);

        graphics = new MicroGraphics(display)
        {
            CurrentFont = new Font12x20(),
            Stroke = 1
        };

        // Projection Matrix
        float near = 0.1f;
        float far = 1000.0f;
        float aspectRatio = Height / Width;
        float fov = 90f;

        projectionMatrix = MatrixOperations.CreateProjectionMatrix(fov, aspectRatio, near, far);

        return Task.CompletedTask;
    }

    public override Task Run()
    {
        _ = Task.Run(() =>
        {
            var objectMesh = Shapes.GenerateDodecahedron();

            var color = Color.Cyan;
            var colorFill = Color.DarkCyan;

            // Set up rotation matrices
            Matrix4x4 matRotZ;
            Matrix4x4 matRotX;

            // Draw Triangles
            var triProjected = new Triangle();
            var triTranslated = new Triangle();
            var triRotatedZ = new Triangle();
            var triRotatedZX = new Triangle();

            float thetaX = 0;
            float thetaZ = 0;

            float translationZ = 5.0f;

            bool lightZUp = true;


            while (true)
            {
                graphics.Clear();

                color = color.WithHue(color.Hue + 0.00001);
                colorFill = color.WithHue(colorFill.Hue + 0.00001);

                if (lightZUp)
                {
                    if (lightDirection.Z >= 1.0f)
                    {
                        lightZUp = false;
                    }
                    else
                    {
                        lightDirection.Z += 0.01f;
                    }
                }
                else
                {
                    if (lightDirection.Z <= 0.0f)
                    {
                        lightZUp = true;
                    }
                    else
                    {
                        lightDirection.Z -= 0.01f;
                    }
                }

                thetaX += 0.01f;
                thetaZ += 0.05f;

                // Rotation X
                matRotX = MatrixOperations.CreateXRotationMatrix(thetaX);

                // Rotation Z
                matRotZ = MatrixOperations.CreateZRotationMatrix(thetaZ);

                Triangle tri;

                for (int i = 0; i < objectMesh.Triangles.Count; i++)
                {
                    tri = objectMesh.Triangles[i];

                    // Rotate in Z-Axis
                    MatrixOperations.MatrixMultiplyTriangle(ref tri, ref triRotatedZ, ref matRotZ);

                    // Rotate in X-Axis
                    MatrixOperations.MatrixMultiplyTriangle(ref triRotatedZ, ref triRotatedZX, ref matRotX);

                    // Offset into the screen
                    triTranslated = triRotatedZX;
                    TriangleOperations.TranslateZ(ref triTranslated, translationZ);

                    // Check if triangle is facing towards the camera
                    if (TriangleOperations.IsFacingCamera(ref triTranslated, ref camera))
                    {
                        // Project triangles from 3D --> 2D
                        MatrixOperations.MatrixMultiplyTriangle(ref triTranslated, ref triProjected, ref projectionMatrix);

                        // Scale into viewspace
                        TriangleOperations.TranslateX(ref triProjected, 1.0f);
                        TriangleOperations.TranslateY(ref triProjected, 1.0f);
                        TriangleOperations.ScaleX(ref triProjected, Width * 0.5f);
                        TriangleOperations.ScaleY(ref triProjected, Height * 0.5f);

                        // Calculate light shading
                        float lightIntensity = CalculateLightIntensity(TriangleOperations.GetNormal(ref triTranslated), lightDirection);
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
                graphics.ShowUnsafe();
            }
        });

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

    float CalculateLightIntensity(Vector3d normal, Vector3d lightDirection)
    {
        // Ensure both vectors are normalized
        normal = VectorOperations.Normalize(ref normal);
        lightDirection = VectorOperations.Normalize(ref lightDirection);

        // Calculate dot product
        return VectorOperations.DotProduct(ref normal, ref lightDirection);
    }
}