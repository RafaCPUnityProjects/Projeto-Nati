using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Leaf
{
    public int y, x, width, height; // the position and size of this Leaf

    public Leaf leftChild; // the Leaf's left child Leaf
    public Leaf rightChild; // the Leaf's right child Leaf
    public Room room; // the room that is inside this Leaf
    public List<Room> halls; // hallways to connect this Leaf to other Leafs

    public int MIN_LEAF_SIZE
    {
        get
        {
            return LeafTutorial.MIN_LEAF_SIZE;
        }
    }

    public Leaf(int X, int Y, int Width, int Height)
    {
        // initialize our leaf
        x = X;
        y = Y;
        width = Width;
        height = Height;

        room = new Room(x, y, width * .8f, height * .8f);
        //Debug.Log("x=" + x + " y=" + y + " width=" + width + " height=" + height);
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

        bool splitH = LeafTutorial.random.NextDouble() > .5f;
        if (width > height && width / height >= 1.25)
            splitH = false;
        else if (height > width && height / width >= 1.25)
            splitH = true;

        int max = (splitH ? height : width) - MIN_LEAF_SIZE; // determine the maximum height or width
        if (max <= MIN_LEAF_SIZE)
            return false; // the area is too small to split any more...

        int split = LeafTutorial.random.Next(MIN_LEAF_SIZE, max); // determine where we're going to split

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

    Room GetRoom()
    {
        if (room != null)
        {
            return room;
        }
        else
        {
            Room lRoom = new Room();
            Room rRoom = new Room();
            Debug.Log(lRoom == null);
            if (leftChild != null)
            {
                lRoom = leftChild.GetRoom();
            }
            if (rightChild != null)
            {
                rRoom = leftChild.GetRoom();
            }
            if (lRoom == null && rRoom == null)
            {
                Debug.Log("both rooms null");
                return null;
            }
            else if (rRoom == null)
            {
                Debug.Log("rRoom null");
                return lRoom;
            }
            else if (lRoom == null)
            {
                Debug.Log("lRoom null");
                return rRoom;
            }
            else if (LeafTutorial.random.NextDouble() > 0.5f)
            {
                return lRoom;
            }
            else
            {
                return rRoom;
            }
        }
    }


    public class Room
    {
        float worldPosX, worldPosY;
        float width, height;

        public bool isARoom;

        public Room()
        {
            this.worldPosX = 0;
            this.worldPosY = 0;
            this.width = 0;
            this.height = 0;
        }

        public Room(float worldPosX, float worldPosY, float width, float height, bool isARoom = true)
        {
            this.worldPosX = worldPosX;
            this.worldPosY = worldPosY;
            this.width = width;
            this.height = height;
            this.isARoom = isARoom;
        }

        public float left
        {
            get
            {
                return worldPosX - width / 2;
            }
        }
        public float right
        {
            get
            {
                return worldPosX + width / 2;
            }
        }
        public float top
        {
            get
            {
                return worldPosY + height / 2;
            }
        }
        public float bottom
        {
            get
            {
                return worldPosY - height / 2;
            }
        }

        public Vector3 Center
        {
            get
            {
                return new Vector3((width + worldPosX) / 2f, 0, (height + worldPosY) / 2f);
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

    public void CreateRooms()
    {
        // this function generates all the rooms and hallways for this leaf and all it's children.
        if (leftChild != null || rightChild != null)
        {
            // this leaf has been split, so go into the children leafs
            if (leftChild != null)
            {
                leftChild.CreateRooms();
            }
            if (rightChild != null)
            {
                rightChild.CreateRooms();
            }

            // if there are both left and right children in this leaf, create a hallway between them
            if (leftChild != null && rightChild != null)
            {

                CreateHall(leftChild.GetRoom(), rightChild.GetRoom());
            }

        }
        else
        {
            // this leaf is the ready to make a room
            Vector2 roomSize;
            Vector2 roomPos;
            // the room can be between 3 x 3 tiles to the size of the leaf - 2.
            roomSize = new Vector2(LeafTutorial.random.Next(3, width - 2), LeafTutorial.random.Next(3, height - 2));
            // place the room within the leaf don't put it right against the side of the leaf (that would merge rooms together)
            roomPos = new Vector2(LeafTutorial.random.Next(1, (int)(width - roomSize.x - 1)), LeafTutorial.random.Next(1, (int)(height - roomSize.y - 1)));
            room = new Room(x + roomPos.x, y + roomPos.y, roomSize.x, roomSize.y);
        }
    }

    public void CreateHall(Room l, Room r)
    {
        // now we connect these 2 rooms together with hallways.
        // this looks pretty complicated, but it's just trying to figure out which  point is where and then either draw a straight line, or a pair of lines to make a right-angle to connect them.
        // you could do some extra logic to make your halls more bendy, or do some more advanced things if you wanted.

        halls = new List<Room>();

        Vector2 point1 = new Vector2(LeafTutorial.random.Next((int)(l.left + 1), (int)(l.right - 2)), LeafTutorial.random.Next((int)(l.bottom - 2), (int)(l.top + 1)));
        Vector2 point2 = new Vector2(LeafTutorial.random.Next((int)(r.left + 1), (int)(r.right - 2)), LeafTutorial.random.Next((int)(r.bottom - 2), (int)(r.top + 1)));

        float w = point2.x - point1.x;
        float h = point2.y - point1.y;

        if (w < 0)
        {
            if (h < 0)
            {
                if (LeafTutorial.random.NextDouble() < 0.5)
                {
                    halls.Add(new Room(point2.x, point1.y, Math.Abs(w), 1,false));
                    halls.Add(new Room(point2.x, point2.y, 1, Math.Abs(h), false));
                }
                else
                {
                    halls.Add(new Room(point2.x, point2.y, Math.Abs(w), 1, false));
                    halls.Add(new Room(point1.x, point2.y, 1, Math.Abs(h), false));
                }
            }
            else if (h > 0)
            {

                if (LeafTutorial.random.NextDouble() < 0.5)
                {
                    halls.Add(new Room(point2.x, point1.y, Math.Abs(w), 1, false));
                    halls.Add(new Room(point2.x, point1.y, 1, Math.Abs(h), false));
                }
                else
                {
                    halls.Add(new Room(point2.x, point2.y, Math.Abs(w), 1, false));
                    halls.Add(new Room(point1.x, point1.y, 1, Math.Abs(h), false));
                }
            }
            else // if (h == 0)
            {
                halls.Add(new Room(point2.x, point2.y, Math.Abs(w), 1, false));
            }
        }
        else if (w > 0)
        {
            if (h < 0)
            {
                if (LeafTutorial.random.NextDouble() < 0.5)
                {
                    halls.Add(new Room(point1.x, point2.y, Math.Abs(w), 1, false));
                    halls.Add(new Room(point1.x, point2.y, 1, Math.Abs(h), false));
                }
                else
                {
                    halls.Add(new Room(point1.x, point1.y, Math.Abs(w), 1, false));
                    halls.Add(new Room(point2.x, point2.y, 1, Math.Abs(h), false));
                }
            }
            else if (h > 0)
            {
                if (LeafTutorial.random.NextDouble() < 0.5)
                {
                    halls.Add(new Room(point1.x, point1.y, Math.Abs(w), 1, false));
                    halls.Add(new Room(point2.x, point1.y, 1, Math.Abs(h), false));
                }
                else
                {
                    halls.Add(new Room(point1.x, point2.y, Math.Abs(w), 1, false));
                    halls.Add(new Room(point1.x, point1.y, 1, Math.Abs(h), false));
                }
            }
            else // if (h == 0)
            {
                halls.Add(new Room(point1.x, point1.y, Math.Abs(w), 1, false));
            }
        }
        else // if (w == 0)
        {
            if (h < 0)
            {
                halls.Add(new Room(point2.x, point2.y, 1, Math.Abs(h), false));
            }
            else if (h > 0)
            {
                halls.Add(new Room(point1.x, point1.y, 1, Math.Abs(h), false));
            }
        }

    }
}