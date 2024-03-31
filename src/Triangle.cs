namespace Simple3dEngine;

public struct Triangle
{
    public Vector3d[] Points;

    public Triangle(Vector3d p1, Vector3d p2, Vector3d p3)
    {
        Points = new Vector3d[3];
        Points[0] = p1;
        Points[1] = p2;
        Points[2] = p3;
    }

    public Triangle()
    {
        Points = new Vector3d[3];
        Points[0] = new Vector3d();
        Points[1] = new Vector3d();
        Points[2] = new Vector3d();
    }
}