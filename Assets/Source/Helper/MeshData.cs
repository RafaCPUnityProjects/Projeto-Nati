using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeshData
{
    private List<Vector3> vertices = new List<Vector3>();
    private List<int> tris = new List<int>();
    private List<Vector2> uvs = new List<Vector2>();

    public MeshData()
    {

    }

    public MeshData(List<Vector3> vertices, List<int> tris, List<Vector2> uvs)
    {
        this.vertices = vertices;
        this.tris = tris;
        this.uvs = uvs;
    }

    public void AddPos(Vector3 v)
    {
        for (int i = 0; i < vertices.Count; i++)
        {
            vertices[i] += v;
        }
    }

    public void AddMeshData(MeshData toAdd)
    {
        if(toAdd.vertices.Count < 1)
        {
            return;
        }

        if(vertices.Count < 1)
        {
            vertices = toAdd.vertices;
            tris = toAdd.tris;
            uvs = toAdd.uvs;
            return;
        }
        int count = vertices.Count;
        vertices.AddRange(toAdd.vertices);
        foreach(int i in toAdd.tris)
        {
            tris.Add(i + count);
        }
        
        uvs.AddRange(toAdd.uvs);
    }

    public Mesh ToMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = tris.ToArray();
        mesh.uv = uvs.ToArray();

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.Optimize();

        return mesh;
    }
}
