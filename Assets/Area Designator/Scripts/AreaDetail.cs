using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Area))]
public class AreaDetail : MonoBehaviour, IAreaModifier
{
	private Area Area;
	[Header("Terrain finding with raycast")]
	public LayerMask TerrainLayerMask;
	public float RayCastDistance = 1;
	[Header("Details to apply")]
	public List<Texture2D> DetailTextures;
	[Range(0f, 1f)] public float DetailDensity;
	public int DetailMult;

	private List<Terrain> GetTerrains()
	{
		List<Terrain> Terrains = new List<Terrain>();
		for (int i = 0; i < Area.PerimeterPoints.Count; i++)
		{
			RaycastHit hit;
			if (Physics.Raycast(Area.transform.TransformPoint(Area.PerimeterPoints[i]) + RayCastDistance * Area.transform.up,
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

	private List<int> GetDetailLayerIndeces(Terrain Terrain)
	{
		List<int> Result = new List<int>();
		var TData = Terrain.terrainData;
		for (int i = 0; i < DetailTextures.Count; i++)
		{
			int Layerindex = -1;
			Layerindex = TData.detailPrototypes.ToList().FindIndex(DP => DP.prototypeTexture == DetailTextures[i]);
			if (Layerindex == -1)
			{
				var DPs = TData.detailPrototypes.ToList();
				DPs.Add(new DetailPrototype()
				{
					prototypeTexture = DetailTextures[i],
				});
				TData.detailPrototypes = DPs.ToArray();
				Layerindex = TData.detailPrototypes.Length - 1;
			}
			Result.Add(Layerindex);
		}
		return Result;
	}

	public void ApplyDetails(Terrain Terrain)
	{
		if (Terrain == null)
		{
			Debug.LogError($"{this.GetType().Name} on {gameObject.name} can't find a Terrain Object in range to work on");
			return;
		}
		var TData = Terrain.terrainData;

		var TerrainBounds = Area.AreaTerrainMapBounds(Terrain, new Vector2(TData.detailWidth, TData.detailHeight));

		List<int> DetailLayerIndeces = GetDetailLayerIndeces(Terrain);
		if (DetailLayerIndeces.Count == 0) { return; }

		List<int[,]> DetailMaps = new List<int[,]>();
		for (int i = 0; i < DetailLayerIndeces.Count; i++)
		{
			DetailMaps.Add(TData.GetDetailLayer(0, 0, TData.detailWidth, TData.detailHeight, DetailLayerIndeces[i]));
		}

		// For each pixel in the detail map...
		for (int i = TerrainBounds.Item1.y; i < TerrainBounds.Item2.y; i++)
		{
			for (int j = TerrainBounds.Item1.x; j < TerrainBounds.Item2.x; j++)
			{
				Vector2 TerrainLocalPoint = new Vector2(j, i) / new Vector2(TData.detailWidth, TData.detailHeight) * new Vector2(TData.size.x, TData.size.z);
				var AreaLocalPoint3 = Area.transform.InverseTransformPoint(Terrain.transform.TransformPoint(new Vector3(TerrainLocalPoint.x, 0, TerrainLocalPoint.y)));
				if (Area.ContainsLocalPoint(AreaLocalPoint3))
				{
					if (Random.Range(0f, 1f) < DetailDensity)
					{
						int _DetailMult = DetailMult;
						for (int k = 0; k < DetailMaps.Count; k++)
						{
							if (_DetailMult <= 0) { break; }
							int _LocalDetailMult = (k != DetailMaps.Count - 1) ? Random.Range(0, _DetailMult) : _DetailMult;
							DetailMaps[k][i, j] = _LocalDetailMult;
							_DetailMult -= _LocalDetailMult;
						}
					}
					else
					{
						for (int k = 0; k < DetailMaps.Count; k++)
						{
							DetailMaps[k][i, j] = 0;
						}
					}
				}
			}

			// Assign the modified map back.
			for (int k = 0; k < DetailMaps.Count; k++)
			{
				TData.SetDetailLayer(0, 0, k, DetailMaps[k]);
			}
		}
	}

	public void ApplyAreaModifier()
	{
		Area = Area ?? GetComponent<Area>();
		GetTerrains().ForEach(T => ApplyDetails(T));
	}
}
