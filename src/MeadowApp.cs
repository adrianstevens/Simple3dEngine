using Meadow;
using Meadow.Foundation.Displays;
using Meadow.Foundation.Graphics;
using Simple3dEngine;
using System.Windows.Forms;

namespace WinForms_Sample;

//<!=SNIP=>

public class MeadowApp : App<Meadow.Windows>
{
    WinFormsDisplay? display;
    MicroGraphics graphics = default!;

    Matrix4x4 matProj = new Matrix4x4();

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
        float fAspectRatio = 240 / 320f;
        float fFovRad = 1.0f / (float)Math.Tan(fFov * 0.5f / 180.0f * 3.14159f);

        matProj.M[0,0] = fAspectRatio * fFovRad;
        matProj.M[1,1] = fFovRad;
        matProj.M[2,2] = fFar / (fFar - fNear);
        matProj.M[3,2] = (-fFar * fNear) / (fFar - fNear);
        matProj.M[2,3] = 1.0f;
        matProj.M[3,3] = 0.0f;

        var meshCube = Shapes.GenerateCube();

        _ = Task.Run(() =>
        {
            graphics.Clear();

            // Set up rotation matrices
            var matRotZ = new Matrix4x4();
            var matRotX = new Matrix4x4();

            fTheta += 1.0f * 1; //fElapsedTime;

            // Rotation Z
            matRotZ.M[0,0] = (float)Math.Cos(fTheta);
            matRotZ.M[0,1] = (float)Math.Sin(fTheta);
            matRotZ.M[1,0] = -(float)Math.Sin(fTheta);
            matRotZ.M[1,1] = (float)Math.Cos(fTheta);
            matRotZ.M[2,2] = 1;
            matRotZ.M[3,3] = 1;

            // Rotation X
            matRotX.M[0,0] = 1;
            matRotX.M[1,1] = (float)Math.Cos(fTheta * 0.5f);
            matRotX.M[1,2] = (float)Math.Sin(fTheta * 0.5f);
            matRotX.M[2,1] = -(float)Math.Sin(fTheta * 0.5f);
            matRotX.M[2,2] = (float)Math.Cos(fTheta * 0.5f);
            matRotX.M[3,3] = 1;

            // Draw Triangles
            var triProjected = new Triangle();
            var triTranslated = new Triangle();
            var triRotatedZ = new Triangle();
            var triRotatedZX = new Triangle();

            foreach (var tri in  meshCube.Triangles)
            {
                // Rotate in Z-Axis
                MatrixOperations.MultiplyMatrixVector(ref tri.p[0], out triRotatedZ.p[0], matRotZ);
                MatrixOperations.MultiplyMatrixVector(ref tri.p[1], out triRotatedZ.p[1], matRotZ);
                MatrixOperations.MultiplyMatrixVector(ref tri.p[2], out triRotatedZ.p[2], matRotZ);

                // Rotate in X-Axis
                MatrixOperations.MultiplyMatrixVector(ref triRotatedZ.p[0], out triRotatedZX.p[0], matRotX);
                MatrixOperations.MultiplyMatrixVector(ref triRotatedZ.p[1], out triRotatedZX.p[1], matRotX);
                MatrixOperations.MultiplyMatrixVector(ref triRotatedZ.p[2], out triRotatedZX.p[2], matRotX);

                // Offset into the screen
                triTranslated = triRotatedZX;
                triTranslated.p[0].Z = triRotatedZX.p[0].Z + 3.0f;
                triTranslated.p[1].Z = triRotatedZX.p[1].Z + 3.0f;
                triTranslated.p[2].Z = triRotatedZX.p[2].Z + 3.0f;

                // Project triangles from 3D --> 2D
                MatrixOperations.MultiplyMatrixVector(ref triTranslated.p[0], out triProjected.p[0], matProj);
                MatrixOperations.MultiplyMatrixVector(ref triTranslated.p[1], out triProjected.p[1], matProj);
                MatrixOperations.MultiplyMatrixVector(ref triTranslated.p[2], out triProjected.p[2], matProj);

                // Scale into view
                triProjected.p[0].X += 1.0f; triProjected.p[0].Y += 1.0f;
                triProjected.p[1].X += 1.0f; triProjected.p[1].Y += 1.0f;
                triProjected.p[2].X += 1.0f; triProjected.p[2].Y += 1.0f;
                triProjected.p[0].X *= 0.5f * Width;
                triProjected.p[0].Y *= 0.5f * Height;
                triProjected.p[1].X *= 0.5f * Width;
                triProjected.p[1].Y *= 0.5f * Height;
                triProjected.p[2].X *= 0.5f * Width;
                triProjected.p[2].Y *= 0.5f * Height;

                graphics.DrawTriangle((int)triProjected.p[0].X, (int)triProjected.p[0].Y,
                    (int)triProjected.p[1].X, (int)triProjected.p[1].Y,
                    (int)triProjected.p[2].X, (int)triProjected.p[2].Y,
                    Color.Red, false);

            }

            graphics.Show();
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
}

//<!=SNOP=>