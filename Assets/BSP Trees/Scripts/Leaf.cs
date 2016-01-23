using UnityEngine;
using System.Collections;

public class Leaf : MonoBehaviour
{

    /*

    private const int MIN_LEAF_SIZE = 6;

    public int y, x, width, height; // the position and size of this Leaf

    public Leaf leftChild; // the Leaf's left child Leaf
    public Leaf rightChild; // the Leaf's right child Leaf
    public Rectangle room; // the room that is inside this Leaf
    public Vector2[] halls; // hallways to connect this Leaf to other Leafs

    public Leaf(int X, int Y, int Width, int Height)
    {
        // initialize our leaf
        x = X;
        y = Y;
        width = Width;
        height = Height;
    }

    public bool split()
    {
        // begin splitting the leaf into two children
        if (leftChild != null || rightChild != null)
            return false; // we're already split! Abort!

        // determine direction of split
        // if the width is >25% larger than height, we split vertically
        // if the height is >25% larger than the width, we split horizontally
        // otherwise we split randomly
        bool splitH = FlxG.random() > 0.5;
        if (width > height && width / height >= 1.25)
            splitH = false;
        else if (height > width && height / width >= 1.25)
            splitH = true;

        int max = (splitH ? height : width) - MIN_LEAF_SIZE; // determine the maximum height or width
        if (max <= MIN_LEAF_SIZE)
            return false; // the area is too small to split any more...

        int split = Registry.randomNumber(MIN_LEAF_SIZE, max); // determine where we're going to split

        // create our left and right children based on the direction of the split
        if (splitH)
        {
            leftChild = new Leaf(x, y, width, split);
            rightChild = new Leaf(x, y + split, width, height - split);
        }
        else
        {
            leftChild = new Leaf(x, y, split, height);
            rightChild = new Leaf(x + split, y, width - split, height);
        }
        return true; // split successful!
    }*/
}
