using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Int2nd
{
    public int x;
    public int z;

    public Int2nd(int x, int z)
    {
        this.x = x;
        this.z = z;
    }
    public Vector3 ToVector3()
    {
        return new Vector3(x, 0, z);
    }
    public Vector2 ToVector2()
    {
        return new Vector2(x, z);
    }
}

public class MathHelper
{
    public static MeshData DrawBlock(Chunk chunk, int x, int y, int z, World world, List<Vector2> uvs)
    {
        MeshData temp = new MeshData();
        if (y < 1 || chunk.GetBlock(x, y - 1, z).IsTransparent())
        {
            temp.AddMeshData(new MeshData(
                new List<Vector3>()
                {
                    new Vector3(0,0,0),
                    new Vector3(0,0,1),
                    new Vector3(1,0,0),
                    new Vector3(1,0,1)
                },
                new List<int>() { 0, 2, 1, 3, 1, 2 },
                new List<Vector2>(uvs)));
        }
        if (y > Chunk.chunkHeight - 2 || chunk.GetBlock(x, y + 1, z).IsTransparent())
        {
            temp.AddMeshData(new MeshData(
                new List<Vector3>()
                {
                    new Vector3(0,1,0),
                    new Vector3(0,1,1),
                    new Vector3(1,1,0),
                    new Vector3(1,1,1)
                },
                new List<int>() { 0, 1, 2, 3, 2, 1 },
                new List<Vector2>(uvs)));
        }
        if (x < 1 || chunk.GetBlock(x - 1, y, z).IsTransparent())
        {
            temp.AddMeshData(new MeshData(
                new List<Vector3>()
                {
                    new Vector3(0,0,1),
                    new Vector3(0,1,1),
                    new Vector3(0,0,0),
                    new Vector3(0,1,0)
                },
                new List<int>() { 0, 1, 2, 3, 2, 1 },
                new List<Vector2>(uvs)));
        }
        if (x > Chunk.chunkWidth - 2 || chunk.GetBlock(x + 1, y, z).IsTransparent())
        {
            temp.AddMeshData(new MeshData(
                new List<Vector3>()
                {
                    new Vector3(1,0,1),
                    new Vector3(1,1,1),
                    new Vector3(1,0,0),
                    new Vector3(1,1,0)
                },
                new List<int>() { 0, 2, 1, 3, 1, 2 },
                new List<Vector2>(uvs)));
        }
        if (z < 1 || chunk.GetBlock(x, y, z - 1).IsTransparent())
        {
            temp.AddMeshData(new MeshData(
                new List<Vector3>()
                {
                    new Vector3(1,0,0),
                    new Vector3(1,1,0),
                    new Vector3(0,0,0),
                    new Vector3(0,1,0)
                },
                new List<int>() { 0, 2, 1, 3, 1, 2 },
                new List<Vector2>(uvs)));
        }
        if (z > Chunk.chunkWidth - 2 || chunk.GetBlock(x, y, z + 1).IsTransparent())
        {
            temp.AddMeshData(new MeshData(
                new List<Vector3>()
                {
                    new Vector3(1,0,1),
                    new Vector3(1,1,1),
                    new Vector3(0,0,1),
                    new Vector3(0,1,1)
                },
                new List<int>() { 0, 1, 2, 3, 2, 1 },
                new List<Vector2>(uvs)));
        }
        temp.AddPos(new Vector3(x - .5f, y - .5f, z - .5f));
        return temp;
    }
}
