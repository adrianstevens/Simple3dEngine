namespace Simple3dEngine;

public struct Triangle
{
    public Vector3d[] p;

    public Triangle(Vector3d p1, Vector3d p2, Vector3d p3)
    {
        p = new Vector3d[3];
        p[0] = p1;
        p[1] = p2;
        p[2] = p3;
    }

    public Triangle()
    {
        p = new Vector3d[3];
        p[0] = new Vector3d();
        p[1] = new Vector3d();
        p[2] = new Vector3d();
    }
}