using UnityEngine;
using System.Collections;
using System.Threading;
using System;

public class Chunk
{
    public static readonly int chunkWidth = 16;
    public static readonly int chunkHeight = 16;
    public static readonly int chunkDepth = 16;
    private Int2nd location;
    private World world;
    private bool hasGenerated = false;
    private bool hasDrawn = false;
    private Block[,,] _blocks = new Block[chunkWidth, chunkHeight, chunkDepth];
    public delegate void Rundel();
    private Transform chunkTransform;

    public Chunk(Int2nd location, World world)
    {
        this.location = location;
        this.world = world;
        Thread t = new Thread(Generate);
        //t.Priority = System.Threading.ThreadPriority.Highest;
        t.Start();
    }

    public void Generate()
    {
        try
        {
            for (int x = 0; x < chunkWidth; x++)
            {
                for (int y = 0; y < chunkHeight; y++)
                {
                    for (int z = 0; z < chunkDepth; z++)
                    {
                        //_blocks[x, y, z] = Block.stone;
                        System.Random random = new System.Random(x + y + z);
                        int rand = random.Next(10);
                        if (rand > 8)
                        {
                            _blocks[x, y, z] = Block.air;
                        }
                        else if(rand > 6)
                        {
                            _blocks[x, y, z] = Block.stone;
                        }
                        else if (rand > 4)
                        {
                            _blocks[x, y, z] = Block.sand;
                        }
                        else
                        {
                            _blocks[x, y, z] = Block.dirt;
                        }
                    }
                }
            }
            Draw();
        }
        catch (System.Exception e)
        {
            Logger.Log(e.StackTrace);
            Debug.Log(e.StackTrace);
        }

        hasGenerated = true;
    }

    public Block GetBlock(int x, int y, int z)
    {
        return _blocks[x, y, z];
    }

    private MeshData _chunkMesh;

    public void Draw()
    {
        try
        {
            _chunkMesh = new MeshData();
            for (int x = 0; x < chunkWidth; x++)
            {
                for (int y = 0; y < chunkHeight; y++)
                {
                    for (int z = 0; z < chunkDepth; z++)
                    {
                        _chunkMesh.AddMeshData(_blocks[x, y, z].Draw(this, x, y, z, world));
                    }
                }
            }
            WorldManager.GetInstance().RegisterDelegate(new Rundel(_Draw));
            hasDrawn = true;
        }
        catch (System.Exception e)
        {
            Logger.Log(e.StackTrace);
            Debug.Log(e.StackTrace);
        }


    }

    private void _Draw()
    {
        if (chunkTransform == null)
        {
            chunkTransform = Transform.Instantiate(Resources.Load<Transform>("Chunk"), location.ToVector3(), Quaternion.identity) as Transform;
            chunkTransform.name = "Chunk:" + location.ToVector3().ToString();
            chunkTransform.GetComponent<Renderer>().material.mainTexture = TextureAtlas.GetAtlas();
        }
        Mesh mesh = _chunkMesh.ToMesh();
        chunkTransform.GetComponent<MeshCollider>().sharedMesh = mesh;
        chunkTransform.GetComponent<MeshFilter>().sharedMesh = mesh;
    }
}
