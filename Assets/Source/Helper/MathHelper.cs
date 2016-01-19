using UnityEngine;
using System.Collections;

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

}
