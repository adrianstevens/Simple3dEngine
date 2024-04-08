using System;
using System.Collections.Generic;

namespace Simple3dEngine;

public static class Shapes
{
    public static Mesh GenerateCube()
    {
        Mesh meshCube = new()
        {
            Triangles = new List<Triangle>
            {
                // SOUTH
                new Triangle(new Vector3d(0.0f, 0.0f, 0.0f), new Vector3d(0.0f, 1.0f, 0.0f), new Vector3d(1.0f, 1.0f, 0.0f)),
                new Triangle(new Vector3d(0.0f, 0.0f, 0.0f), new Vector3d(1.0f, 1.0f, 0.0f), new Vector3d(1.0f, 0.0f, 0.0f)),

                // EAST
                new Triangle(new Vector3d(1.0f, 0.0f, 0.0f), new Vector3d(1.0f, 1.0f, 0.0f), new Vector3d(1.0f, 1.0f, 1.0f)),
                new Triangle(new Vector3d(1.0f, 0.0f, 0.0f), new Vector3d(1.0f, 1.0f, 1.0f), new Vector3d(1.0f, 0.0f, 1.0f)),

                // NORTH
                new Triangle(new Vector3d(1.0f, 0.0f, 1.0f), new Vector3d(1.0f, 1.0f, 1.0f), new Vector3d(0.0f, 1.0f, 1.0f)),
                new Triangle(new Vector3d(1.0f, 0.0f, 1.0f), new Vector3d(0.0f, 1.0f, 1.0f), new Vector3d(0.0f, 0.0f, 1.0f)),

                // WEST
                new Triangle(new Vector3d(0.0f, 0.0f, 1.0f), new Vector3d(0.0f, 1.0f, 1.0f), new Vector3d(0.0f, 1.0f, 0.0f)),
                new Triangle(new Vector3d(0.0f, 0.0f, 1.0f), new Vector3d(0.0f, 1.0f, 0.0f), new Vector3d(0.0f, 0.0f, 0.0f)),

                // TOP
                new Triangle(new Vector3d(0.0f, 1.0f, 0.0f), new Vector3d(0.0f, 1.0f, 1.0f), new Vector3d(1.0f, 1.0f, 1.0f)),
                new Triangle(new Vector3d(0.0f, 1.0f, 0.0f), new Vector3d(1.0f, 1.0f, 1.0f), new Vector3d(1.0f, 1.0f, 0.0f)),

                // BOTTOM
                new Triangle(new Vector3d(1.0f, 0.0f, 1.0f), new Vector3d(0.0f, 0.0f, 1.0f), new Vector3d(0.0f, 0.0f, 0.0f)),
                new Triangle(new Vector3d(1.0f, 0.0f, 1.0f), new Vector3d(0.0f, 0.0f, 0.0f), new Vector3d(1.0f, 0.0f, 0.0f)),
            }
        };

        return meshCube;
    }

    public static Mesh GenerateDodecahedron()
    {
        float phi = 1.618034f;

        Vector3d[] vertices = new Vector3d[]
        {
            new Vector3d(1, 1, 1),
            new Vector3d(1, 1, -1),
            new Vector3d(1, -1, 1),
            new Vector3d(-1, 1, 1),

            new Vector3d(-1, -1, -1),
            new Vector3d(-1, -1, 1),
            new Vector3d(-1, 1, -1),
            new Vector3d(1, -1, -1),

            new Vector3d(0, phi, 1 / phi),
            new Vector3d(0, phi, -1 / phi),
            new Vector3d(0, -phi, 1 / phi),
            new Vector3d(0, -phi, -1 / phi),

            new Vector3d(1 / phi, 0, phi),
            new Vector3d(1 / phi, 0, -phi),
            new Vector3d(-1 / phi, 0, phi),
            new Vector3d(-1 / phi, 0, -phi),


            new Vector3d(phi, 1 / phi, 0),
            new Vector3d(phi, -1 / phi, 0),
            new Vector3d(-phi, 1 / phi, 0),
            new Vector3d(-phi, -1 / phi, 0),
        };

        int[] tris = new int[]
        {
            0, 16, 8,
            8, 16, 1,
            8, 1, 9,

            9, 1, 13,
            9, 13, 15,
            9, 15, 6,

            8, 9, 6,
            8, 6, 18,
            8, 18, 3,

            8, 3, 14,
            8, 14, 12,
            8, 12, 0,

            0, 12, 2,
            0, 2, 17,
            0, 17, 16,

            1, 16, 17,
            1, 17, 7,
            1, 7, 13,

            13, 7, 11,
            13, 11, 4,
            13, 4, 15,

            6, 15, 4,
            6, 4, 19,
            6, 19, 18,

            18, 19, 5,
            18, 5, 14,
            18, 14, 3,

            14, 5, 10,
            14, 10, 2,
            14, 2, 12,

            2, 10, 11,
            2, 11, 7,
            2, 7, 17,

            4, 11, 10,
            4, 10, 5,
            4, 5, 19
        };

        Mesh dodecahedron = new Mesh();

        dodecahedron.Triangles = new List<Triangle>();

        for (int i = 0; i < tris.Length; i += 3)
        {
            dodecahedron.Triangles.Add(new Triangle(vertices[tris[i]], vertices[tris[i + 1]], vertices[tris[i + 2]]));
        }

        return dodecahedron;
    }

    public static Mesh GenerateOctahedron()
    {
        List<Vector3d> vertices = new()
        {
            new (1, 0, 0), // 0
            new (-1, 0, 0), // 1
            new (0, 1, 0), // 2
            new (0, -1, 0), // 3
            new (0, 0, 1), // 4 (Top vertex)
            new (0, 0, -1) // 5 (Bottom vertex)
        };

        Mesh octahedron = new Mesh
        {
            Triangles = new List<Triangle>
            {
                // Upper half
                new (vertices[4], vertices[0], vertices[2]),
                new (vertices[4], vertices[2], vertices[1]),
                new (vertices[4], vertices[1], vertices[3]),
                new (vertices[4], vertices[3], vertices[0]),

                // Lower half
                new (vertices[5], vertices[2], vertices[0]),
                new (vertices[5], vertices[1], vertices[2]),
                new (vertices[5], vertices[3], vertices[1]),
                new (vertices[5], vertices[0], vertices[3]),
            }
        };

        return octahedron;
    }

    public static Mesh GenerateTetrahedron()
    {
        // Edge length of the tetrahedron
        float edge = 1.0f;
        float sqrt2 = MathF.Sqrt(2.0f);

        Mesh tetrahedron = new()
        {
            Triangles = new List<Triangle>
            {
                // Base
                new Triangle(new Vector3d(0, 0, 0), new Vector3d(edge / 2, edge * sqrt2 / 2, 0), new Vector3d(edge, 0, 0)),
                // Sides
                new Triangle(new Vector3d(0, 0, 0), new Vector3d(edge / 2, edge * sqrt2 / 6, edge * sqrt2 * 2 / 3), new Vector3d(edge / 2, edge * sqrt2 / 2, 0)),
                new Triangle(new Vector3d(edge, 0, 0), new Vector3d(edge / 2, edge * sqrt2 / 2, 0), new Vector3d(edge / 2, edge * sqrt2 / 6, edge * sqrt2 * 2 / 3)),
                new Triangle(new Vector3d(0, 0, 0), new Vector3d(edge, 0, 0), new Vector3d(edge / 2, edge * sqrt2 / 6, edge * sqrt2 * 2 / 3)),
            }
        };

        return tetrahedron;
    }

    public static Mesh GenerateIcosahedron()
    {
        float t = (1.0f + MathF.Sqrt(5.0f)) / 2.0f; // golden ratio
        float s = 1.0f;

        List<Vector3d> vertices = new()
        {
            new Vector3d(-s, t, 0), new Vector3d(s, t, 0), new Vector3d(-s, -t, 0), new Vector3d(s, -t, 0),
            new Vector3d(0, -s, t), new Vector3d(0, s, t), new Vector3d(0, -s, -t), new Vector3d(0, s, -t),
            new Vector3d(t, 0, -s), new Vector3d(t, 0, s), new Vector3d(-t, 0, -s), new Vector3d(-t, 0, s)
        };

        Mesh icosahedron = new()
        {
            Triangles = new List<Triangle>
            {
                // 5 faces around point 0
                new Triangle(vertices[0], vertices[11], vertices[5]),
                new Triangle(vertices[0], vertices[5], vertices[1]),
                new Triangle(vertices[0], vertices[1], vertices[7]),
                new Triangle(vertices[0], vertices[7], vertices[10]),
                new Triangle(vertices[0], vertices[10], vertices[11]),

                // 5 adjacent faces
                new Triangle(vertices[1], vertices[5], vertices[9]),
                new Triangle(vertices[5], vertices[11], vertices[4]),
                new Triangle(vertices[11], vertices[10], vertices[2]),
                new Triangle(vertices[10], vertices[7], vertices[6]),
                new Triangle(vertices[7], vertices[1], vertices[8]),

                // 5 faces around point 3
                new Triangle(vertices[3], vertices[9], vertices[4]),
                new Triangle(vertices[3], vertices[4], vertices[2]),
                new Triangle(vertices[3], vertices[2], vertices[6]),
                new Triangle(vertices[3], vertices[6], vertices[8]),
                new Triangle(vertices[3], vertices[8], vertices[9]),

                // 5 adjacent faces
                new Triangle(vertices[4], vertices[9], vertices[5]),
                new Triangle(vertices[2], vertices[4], vertices[11]),
                new Triangle(vertices[6], vertices[2], vertices[10]),
                new Triangle(vertices[8], vertices[6], vertices[7]),
                new Triangle(vertices[9], vertices[8], vertices[1])
            }
        };

        return icosahedron;
    }
}