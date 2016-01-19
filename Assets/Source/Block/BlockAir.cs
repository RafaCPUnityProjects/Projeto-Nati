using UnityEngine;
using System.Collections;

public class BlockAir : Block
{
    public BlockAir(string name, bool isTransparent)
        : base(name, isTransparent)
    {

    }
    public override MeshData Draw(Chunk chunk, int x, int y, int z, World world)
    {
        return new MeshData();
    }
}
