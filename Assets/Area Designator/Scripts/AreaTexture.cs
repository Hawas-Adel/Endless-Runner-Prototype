using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Area))]
public class AreaTexture : MonoBehaviour, IAreaModifier
{
	private Area Area;
	[Header("Terrain finding with raycast")]
	public LayerMask TerrainLayerMask;
	public float RayCastDistance = 1;
	[Header("Texture to apply")]
	public TerrainLayer TerrainLayer;
	public bool Smoothing;

	private List<Terrain> GetTerrains()
	{
		List<Terrain> Terrains = new List<Terrain>();
		for (int i = 0; i < Area.PerimeterPoints.Count; i++)
		{
			RaycastHit hit;
			if (Physics.Raycast(Area.transform.TransformPoint(Area.PerimeterPoints[i]) + (RayCastDistance * Area.transform.up),
				-1 * Area.transform.up, out hit, 2 * RayCastDistance, TerrainLayerMask.value))
			{
				Terrain Terrain = hit.collider.GetComponent<Terrain>();
				if (Terrain != null && !Terrains.Contains(Terrain))
				{
					Terrains.Add(Terrain);
				}
			}
		}
		return Terrains;
	}
	private int GetTerrainLayerIndex(Terrain Terrain)
	{
		var TData = Terrain.terrainData;
		int Index = TData.terrainLayers.ToList().FindIndex(TL => TL == TerrainLayer);
		if (Index == -1)
		{
			var Layers = TData.terrainLayers.ToList();
			Layers.Add(TerrainLayer);
			TData.terrainLayers = Layers.ToArray();
			Index = TData.terrainLayers.Length - 1;
		}
		return Index;
	}

	private void ApplyTexture(Terrain Terrain, int TerrainLayerIndex)
	{
		if (Terrain == null)
		{
			Debug.LogError($"{this.GetType().Name} on {gameObject.name} can't find a Terrain Object in range to work on");
			return;
		}
		var TData = Terrain.terrainData;

		var TerrainBounds = Area.AreaTerrainMapBounds(Terrain, new Vector2(TData.alphamapWidth, TData.alphamapHeight));

		var AlphaMap = TData.GetAlphamaps(0, 0, TData.alphamapWidth, TData.alphamapHeight);

		for (int i = TerrainBounds.Item1.y; i < TerrainBounds.Item2.y; i++)
		{
			for (int j = TerrainBounds.Item1.x; j < TerrainBounds.Item2.x; j++)
			{
				Vector2 TerrainLocalPoint = new Vector2(j, i) / new Vector2(TData.alphamapWidth, TData.alphamapHeight) * new Vector2(TData.size.x, TData.size.z);
				var AreaLocalPoint3 = Area.transform.InverseTransformPoint(Terrain.transform.TransformPoint(new Vector3(TerrainLocalPoint.x, 0, TerrainLocalPoint.y)));
				if (Area.ContainsLocalPoint(AreaLocalPoint3))
				{
					for (int k = 0; k < AlphaMap.GetLength(2); k++)
					{
						if (k == TerrainLayerIndex)
						{ AlphaMap[i, j, k] = 1; }
						else
						{ AlphaMap[i, j, k] = 0; }
					}
				}
			}
		}
		//Smoothing
		if (Smoothing)
		{
			for (int i = TerrainBounds.Item1.y - 5; i < TerrainBounds.Item2.y + 5; i++)
			{
				if (!i.Between(0, TData.alphamapHeight)) { continue; }
				for (int j = TerrainBounds.Item1.x - 5; j < TerrainBounds.Item2.x + 5; j++)
				{
					if (!j.Between(0, TData.alphamapWidth)) { continue; }
					for (int k = 0; k < AlphaMap.GetLength(2); k++)
					{
						//apply a 5*5 Average Mask;
						int count = 0; float sum = 0;
						for (int i2 = i - 2; i2 <= i + 2; i2++)
						{
							if (!i2.Between(0, TData.alphamapHeight)) { continue; }
							for (int j2 = j - 2; j2 <= j + 2; j2++)
							{
								if (!j2.Between(0, TData.alphamapWidth)) { continue; }
								count++;
								sum += AlphaMap[i2, j2, k];
							}
						}
						AlphaMap[i, j, k] = sum / count;
					}

					//normalize Values at pixel (i,j)
					float PixelSum = 0;
					for (int k = 0; k < AlphaMap.GetLength(2); k++)
					{ PixelSum += AlphaMap[i, j, k]; }
					for (int k = 0; k < AlphaMap.GetLength(2); k++)
					{ AlphaMap[i, j, k] /= PixelSum; }
				}
			}
		}

		TData.SetAlphamaps(0, 0, AlphaMap);
	}

	public void ApplyAreaModifier()
	{
		Area = Area ?? GetComponent<Area>();
		var T = GetTerrains();
		T.ForEach(t => ApplyTexture(t, GetTerrainLayerIndex(t)));
	}
}
