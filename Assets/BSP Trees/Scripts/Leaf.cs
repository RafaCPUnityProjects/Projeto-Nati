using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Leaf : MonoBehaviour
{
    public static string seed = "0";
    private const int MIN_LEAF_SIZE = 6;

    public int y, x, width, height; // the position and size of this Leaf

    public Leaf leftChild; // the Leaf's left child Leaf
    public Leaf rightChild; // the Leaf's right child Leaf
    public Room room; // the room that is inside this Leaf
    public List<Vector2> halls; // hallways to connect this Leaf to other Leafs

    System.Random random = new System.Random(seed.GetHashCode());
    List<Leaf> debug_leafs;

    void Awake()
    {

    }

    void Start()
    {
        Initialize();
    }

    public Leaf(int X, int Y, int Width, int Height)
    {
        // initialize our leaf
        x = X;
        y = Y;
        width = Width;
        height = Height;
    }

    public bool Split()
    {
        // begin splitting the leaf into two children
        if (leftChild != null || rightChild != null)
            return false; // we're already split! Abort!

        // determine direction of split
        // if the width is >25% larger than height, we split vertically
        // if the height is >25% larger than the width, we split horizontally
        // otherwise we split randomly

        bool splitH = random.NextDouble() > .5f;
        if (width > height && width / height >= 1.25)
            splitH = false;
        else if (height > width && height / width >= 1.25)
            splitH = true;

        int max = (splitH ? height : width) - MIN_LEAF_SIZE; // determine the maximum height or width
        if (max <= MIN_LEAF_SIZE)
            return false; // the area is too small to split any more...

        int split = random.Next(MIN_LEAF_SIZE, max); // determine where we're going to split

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
    }

    void Initialize()
    {
        const int MAX_LEAF_SIZE = 20;


        List<Leaf> _leafs = new List<Leaf>();

        //Leaf l; // helper Leaf

        // first, create a Leaf to be the 'root' of all Leafs.
        Leaf root = new Leaf(0, 0, width, height);
        _leafs.Add(root);

        bool did_split = true;
        // we loop through every Leaf in our Vector over and over again, until no more Leafs can be split.
        while (did_split)
        {
            did_split = false;
            foreach (var leaf in _leafs)

            {
                if (leaf.leftChild == null && leaf.rightChild == null) // if this Leaf is not already split...
                {
                    // if this Leaf is too big, or 75% chance...
                    if (leaf.width > MAX_LEAF_SIZE || leaf.height > MAX_LEAF_SIZE || random.NextDouble() > 0.25)
                    {
                        if (leaf.Split()) // split the Leaf!
                        {
                            // if we did split, push the child leafs to the Vector so we can loop into them next
                            _leafs.Add(leaf.leftChild);
                            _leafs.Add(leaf.rightChild);
                            did_split = true;
                        }
                    }
                }
            }
        }
        debug_leafs = new List<Leaf>();
        debug_leafs = _leafs;
    }


    public class Room
    {
        float worldPosX, worldPosY;
        float width, height;

        public Room()
        {
            this.worldPosX = 0;
            this.worldPosY = 0;
            this.width = 0;
            this.height = 0;
        }

        public Room(float worldPosX, float worldPosY, float width, float height)
        {
            this.worldPosX = worldPosX;
            this.worldPosY = worldPosY;
            this.width = width;
            this.height = height;
        }

        public Vector3 Center
        {
            get
            {
                return new Vector3(worldPosX, 0, worldPosY);
            }
        }

        public Vector3 Size
        {
            get
            {
                return new Vector3(width, 1, height);
            }
        }
    }

    void OnDrawGizmos()
    {
        if (debug_leafs != null)
        {
            foreach (var leaf in debug_leafs)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawCube(leaf.room.Center, leaf.room.Size);
            }
        }
    }
}