using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlockRegistry
{
    private static List<Block> _blocks = new List<Block>();

    public static int RegisterBlock(Block b)
    {
        _blocks.Add(b);
        return _blocks.Count - 1;
    }
    public static void PrintAllBlocks()
    {
        Logger.Log("Registered Blocks:");
        foreach (Block b in _blocks)
        {
            Logger.Log(string.Format("ID:{0},Name:{1}", b.GetBlockID(), b.GetBlockName()));
        }
    }
}
