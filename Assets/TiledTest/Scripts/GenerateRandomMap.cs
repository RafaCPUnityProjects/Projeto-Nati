using UnityEngine;
using System.Collections;
using System;

public class GenerateRandomMap : MonoBehaviour
{
    public string seed;
    public bool useRandomSeed = false;

    public GameObject[] room3x3;
    public GameObject[] room5x5;
    public GameObject[] room7x7;
    public GameObject[] room10x10;

    System.Random random;

    void Start()
    {
        InitializeSeed();
        GenerateMap();
    }

    void InitializeSeed()
    {
        if(useRandomSeed)
        {
            seed += System.DateTime.Now.ToString();
            print("Random seed = " + seed);
        }
        random = new System.Random(seed.GetHashCode());
    }

    private void GenerateMap()
    {
        
    }

}
