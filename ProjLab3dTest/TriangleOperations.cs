using System;

namespace Simple3dEngine;

public static class TriangleOperations
{
    public static int ClipAgainstPlane(Vector3d plane_p, Vector3d plane_n, Triangle in_tri, out Triangle out_tri1, out Triangle out_tri2)
    {
        // Make sure plane normal is indeed normal
        plane_n = VectorOperations.Normalize(plane_n);

        // Return signed shortest distance from point to plane, plane normal must be normalized
        Func<Vector3d, float> dist = (p) => VectorOperations.DotProduct(plane_n, p - plane_p);

        // Create two temporary storage arrays to classify points either side of plane
        // If distance sign is positive, point lies on "inside" of plane
        Vector3d[] inside_points = new Vector3d[3];
        int nInsidePointCount = 0;
        Vector3d[] outside_points = new Vector3d[3];
        int nOutsidePointCount = 0;

        // Get signed distance of each point in triangle to plane
        float d0 = dist(in_tri.Points[0]);
        float d1 = dist(in_tri.Points[1]);
        float d2 = dist(in_tri.Points[2]);

        if (d0 >= 0) { inside_points[nInsidePointCount++] = in_tri.Points[0]; }
        else { outside_points[nOutsidePointCount++] = in_tri.Points[0]; }
        if (d1 >= 0) { inside_points[nInsidePointCount++] = in_tri.Points[1]; }
        else { outside_points[nOutsidePointCount++] = in_tri.Points[1]; }
        if (d2 >= 0) { inside_points[nInsidePointCount++] = in_tri.Points[2]; }
        else { outside_points[nOutsidePointCount++] = in_tri.Points[2]; }

        // Now classify triangle points, and break the input triangle into 
        // smaller output triangles if required. There are four possible
        // outcomes...

        if (nInsidePointCount == 0)
        {
            // All points lie on the outside of plane, so clip whole triangle
            // It ceases to exist
            out_tri1 = new Triangle();
            out_tri2 = new Triangle();
            return 0; // No returned triangles are valid
        }

        if (nInsidePointCount == 3)
        {
            // All points lie on the inside of plane, so do nothing
            // and allow the triangle to simply pass through
            out_tri1 = in_tri;
            out_tri2 = new Triangle();
            return 1; // Just the one returned original triangle is valid
        }

        if (nInsidePointCount == 1 && nOutsidePointCount == 2)
        {
            // Triangle should be clipped. As two points lie outside
            // the plane, the triangle simply becomes a smaller triangle

            // Copy appearance info to new triangle
            out_tri1 = new Triangle(in_tri.Points[0], in_tri.Points[1], in_tri.Points[2]);

            // The inside point is valid, so keep that...
            out_tri1.Points[0] = inside_points[0];

            // but the two new points are at the locations where the 
            // original sides of the triangle (lines) intersect with the plane
            out_tri1.Points[1] = VectorOperations.VectorIntersectPlane(plane_p, plane_n, inside_points[0], outside_points[0]);
            out_tri1.Points[2] = VectorOperations.VectorIntersectPlane(plane_p, plane_n, inside_points[0], outside_points[1]);

            out_tri2 = new Triangle();
            return 1; // Return the newly formed single triangle
        }

        if (nInsidePointCount == 2 && nOutsidePointCount == 1)
        {
            // Triangle should be clipped. As two points lie inside the plane,
            // the clipped triangle becomes a "quad". Fortunately, we can
            // represent a quad with two new triangles

            // Copy appearance info to new triangles
            out_tri1 = new Triangle(in_tri.Points[0], in_tri.Points[1], in_tri.Points[2]);
            out_tri2 = new Triangle(in_tri.Points[0], in_tri.Points[1], in_tri.Points[2]);

            // The first triangle consists of the two inside points and a new
            // point determined by the location where one side of the triangle
            // intersects with the plane
            out_tri1.Points[0] = inside_points[0];
            out_tri1.Points[1] = inside_points[1];
            out_tri1.Points[2] = VectorOperations.VectorIntersectPlane(plane_p, plane_n, inside_points[0], outside_points[0]);

            // The second triangle is composed of one of he inside points, a
            // new point determined by the intersection of the other side of the 
            // triangle and the plane, and the newly created point above
            out_tri2.Points[0] = inside_points[1];
            out_tri2.Points[1] = out_tri1.Points[2];
            out_tri2.Points[2] = VectorOperations.VectorIntersectPlane(plane_p, plane_n, inside_points[1], outside_points[0]);

            return 2; // Return two newly formed triangles which form a quad
        }

        // This shouldn't happen, but if it does, simply return no valid triangles
        out_tri1 = new Triangle();
        out_tri2 = new Triangle();
        return 0;
    }
}