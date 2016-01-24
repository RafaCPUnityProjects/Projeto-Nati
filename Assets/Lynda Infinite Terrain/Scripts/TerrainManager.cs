using UnityEngine;
using System.Collections;

public class TerrainManager : MonoBehaviour
{
    public Sprite[] sprites;
    public int horizontalTiles = 25;
    public int verticalTiles = 25;
    public int key = 1;

    public Sprite SelectedRandomSprite(int x, int y)
    {
        return sprites[RandomHelper.Range(x, y, key, sprites.Length)];
    }

    void Start()
    {
        var offset = new Vector3(0 - horizontalTiles / 2, 0 - verticalTiles / 2, 0);
        for (int x = 0; x < horizontalTiles; x++)
        {
            for (int y = 0; y < verticalTiles; y++)
            {
                var tile = new GameObject();
                tile.transform.position = new Vector3(x, y, 0) + offset;
                var spriteRenderer = tile.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = SelectedRandomSprite(x, y);
                tile.name = "Terrain " + tile.transform.position;
                tile.transform.parent = transform;
            }
        }
    }
}
