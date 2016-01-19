using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Block
{
    public static readonly Block air = new BlockAir("Air", true);
    public static readonly Block stone = new Block("Stone", "stone");
    public static readonly Block dirt = new Block("Dirt", "dirt");
    public static readonly Block sand = new Block("Sand", "sand");
    public static readonly List<Vector2> basicUv = new List<Vector2>()
    {
        new Vector2(0,0),new Vector2(0,1),new Vector2(1,0),new Vector2(1,1)
    };

    private string blockName;
    private int blockID;
    private bool isTransparent = false;
    private Vector2[] uvMap;

    public Block(string blockName)
    {
        this.blockName = blockName;
        blockID = BlockRegistry.RegisterBlock(this);
    }

    public Block(string blockName, bool isTransparent) : this(blockName)
    {
        this.isTransparent = isTransparent;
    }

    public Block(string blockName, string textureName) : this(blockName)
    {
        uvMap = TextureAtlas.GetCoordinate(textureName).ToUvMap();
    }

    public string GetBlockName()
    {
        return blockName;
    }

    public int GetBlockID()
    {
        return blockID;
    }

    public bool IsTransparent()
    {
        return isTransparent;
    }

    public virtual MeshData Draw(Chunk chunk, int x, int y, int z, World world)
    {
        if(uvMap != null)
        {
            return MathHelper.DrawBlock(chunk, x, y, z, world, new List<Vector2>(uvMap));
        }
        else
        {
            return new MeshData();
        }
    }
}
