using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PerlinNoise;

namespace PerlinNoise
{
    public class MapDisplay : MonoBehaviour
    {
        public Renderer textureRenderer;
		public MeshFilter meshFilter;
		public MeshRenderer meshRenderer;

        public void DrawTexture(Texture2D texture)
        {
            textureRenderer.sharedMaterial.mainTexture = texture;
            textureRenderer.transform.localScale = new Vector3(texture.width, 1, texture.height);
        }

		public void DrawMesh(MeshData meshData, Texture2D texture2D)
		{
			meshFilter.sharedMesh = meshData.CreateMesh();
			meshRenderer.sharedMaterial.mainTexture = texture2D;
		}
	}
}
