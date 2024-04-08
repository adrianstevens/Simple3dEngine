using Meadow;
using Meadow.Devices;
using Meadow.Foundation.Graphics;
using Simple3dEngine;
using System.Threading.Tasks;

namespace ProjLab3dTest;

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
        float fov = 90f;

        projectionMatrix = MatrixOperations.CreateProjectionMatrix(fov, aspectRatio, near, far);

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
        Matrix4x4 matRotZ;
        Matrix4x4 matRotX;

        // Draw Triangles
        var triProjected = new Triangle();
        var triTranslated = new Triangle();
        var triRotatedZ = new Triangle();
        var triRotatedZX = new Triangle();

        float thetaX = 0;
        float thetaZ = 0;

        float translationZ = 4.0f;

        while (true)
        {
            graphics.Clear();

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