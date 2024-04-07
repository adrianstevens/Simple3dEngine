using Simple3dEngine;

namespace Simple3d.Core;

public struct Object3d
{
    public Mesh Mesh { get; set; }

    public float XRotation { get; set; }
    public float YRotation { get; set; }
    public float ZRotation { get; set; }

    public bool ShowWireFrame { get; set; }

    public bool FillTriangles { get; set; }

    //public Matrix4x4? XRotation { get; set; }
    //public Matrix4x4? YRotation { get; set; }
    //public Matrix4x4? ZRotation { get; set; }
}