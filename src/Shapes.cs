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