using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Area))]
public class AreaTrees : MonoBehaviour, IAreaModifier
{
	private Area Area;
	[Header("Terrain finding with raycast")]
	public LayerMask TerrainLayerMask;
	public float RayCastDistance = 1;
	[Header("Trees to apply")]
	public List<GameObject> TreeObjecs;
	[Range(0f, 1f)] public float TreeDensity;
	public float InterPointDistance = 1;

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

	private List<int> GetTreeLayerIndeces(Terrain Terrain)
	{
		List<int> Result = new List<int>();
		var TData = Terrain.terrainData;
		for (int i = 0; i < TreeObjecs.Count; i++)
		{
			int Layerindex = -1;
			Layerindex = TData.treePrototypes.ToList().FindIndex(DP => DP.prefab == TreeObjecs[i]);
			if (Layerindex == -1)
			{
				var DPs = TData.treePrototypes.ToList();
				DPs.Add(new TreePrototype()
				{
					prefab = TreeObjecs[i],
				});
				TData.treePrototypes = DPs.ToArray();
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
		List<int> TreeLayerIndeces = GetTreeLayerIndeces(Terrain);
		if (TreeLayerIndeces.Count == 0) { return; }

		//Remove trees in TreeObjecs from area first
		var Trees = TData.treeInstances.ToList();
		for (int i = 0; i < Trees.Count; i++)
		{
			if (TreeLayerIndeces.Contains(Trees[i].prototypeIndex) && Area.ContainsLocalPoint(Area.transform.InverseTransformPoint(Terrain.transform.TransformPoint(Vector3.Scale(Trees[i].position, TData.size)))))
			{ Trees.RemoveAt(i); i--; }
		}

		// set new Trees
		Area.AreaPoints(InterPointDistance) //get a grid of points inside this area
			.Where(_ => Random.value < TreeDensity).ToList() //Randomly cull some points
			.ConvertAll(p => Vector3.Scale(Terrain.transform.InverseTransformPoint(Area.transform.TransformPoint(p)), new Vector3(1 / TData.size.x, 1, 1 / TData.size.z))) // convert to normalized Terrain coords
			.ForEach(p => Trees.Add(new TreeInstance() //place a random tree instance at each point
			{
				position = p,
				prototypeIndex = TreeLayerIndeces.RandomElement(),
				color = new Color32(216, 216, 216, 255),
				heightScale = 1,
				widthScale = 1,
				rotation = Random.Range(0, 2 * Mathf.PI)
			}));

		TData.SetTreeInstances(Trees.ToArray(), true);
	}

	public void ApplyAreaModifier()
	{
		Area = Area ?? GetComponent<Area>();
		GetTerrains().ForEach(T => ApplyDetails(T));
	}
}
