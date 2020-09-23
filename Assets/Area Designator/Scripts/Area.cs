using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class Area : MonoBehaviour
{
	public List<Vector3> PerimeterPoints = new List<Vector3>() { Vector3.zero, Vector3.zero, Vector3.zero };

	public void ApplyAllModifiers()
	{
		GetComponents<IAreaModifier>().ToList().ForEach(A => A.ApplyAreaModifier());
	}
	public bool ContainsLocalPoint(Vector3 P)
	{
		//return true;
		int IntersectingEdgesCount = 0;
		for (int i = 0; i < PerimeterPoints.Count; i++)
		{
			int i_n = (i + 1 == PerimeterPoints.Count) ? 0 : i + 1;
			//check if point is on any edge
			if (LocalPointOnEdge(P, PerimeterPoints[i], PerimeterPoints[i_n]))
			{ return true; }

			//Extend Point to Infinity
			float Inf = PerimeterPoints.Max(p => p.x) + 1;
			Vector3 Ext_P = new Vector3(Inf, P.y, P.z);
			//check if extention of point intersects with odd No. of edges
			if (LinesIntersect(P, Ext_P, PerimeterPoints[i], PerimeterPoints[i_n]))
			{ IntersectingEdgesCount++; }
		}
		if (IntersectingEdgesCount % 2 == 1)
		{
			return true;
		}
		return false;
	}

	private bool LocalPointOnEdge(Vector3 Point, Vector3 L1, Vector3 L2)
	{
		//Debug.Log($"P={Point}-:-L1={L1}-:-L2={L2}");
		Point.y = 0; L1.y = 0; L2.y = 0;
		float D_P_L1 = Vector3.Distance(Point, L1),
			D_P_L2 = Vector3.Distance(Point, L2),
			D_L1_L2 = Vector3.Distance(L1, L2);
		float Variance = 0.0001f;
		return (D_P_L1 + D_P_L2).Between(D_L1_L2 - Variance, D_L1_L2 + Variance);
		//return (D_P_L1 + D_P_L2 == D_L1_L2);
	}
	private bool LinesIntersect(Vector3 p1, Vector3 q1, Vector3 p2, Vector3 q2)
	{
		p1.y = 0; p2.y = 0; q1.y = 0; q2.y = 0;
		float p1q1p2 = Vector3.SignedAngle(q1 - p1, p2 - q1, Vector3.up),
			p1q1q2 = Vector3.SignedAngle(q1 - p1, q2 - q1, Vector3.up),
			p2q2p1 = Vector3.SignedAngle(q2 - p2, p1 - q2, Vector3.up),
			p2q2q1 = Vector3.SignedAngle(q2 - p2, q1 - q2, Vector3.up);
		if (Mathf.Sign(p1q1p2) != Mathf.Sign(p1q1q2) &&
			Mathf.Sign(p2q2p1) != Mathf.Sign(p2q2q1))
		{ return true; }
		return false;
	}


	public (Vector2Int, Vector2Int) AreaTerrainMapBounds(Terrain T, Vector2 TMapSize)
	{
		var WorldPoints = PerimeterPoints.ConvertAll(P => transform.TransformPoint(P));
		Vector3 MinAreaPoint = new Vector3(WorldPoints.Min(P => P.x), 0, WorldPoints.Min(P => P.z));
		Vector3 MaxAreaPoint = new Vector3(WorldPoints.Max(P => P.x), 0, WorldPoints.Max(P => P.z));

		//{MinAreaLocal,MaxAreaLocal}=>To World Point=>To Local For Terain=>/Size=>*TMapSize
		var MinTerrainLocal3 = T.transform.InverseTransformPoint(MinAreaPoint);
		var MaxTerrainLocal3 = T.transform.InverseTransformPoint(MaxAreaPoint);
		Vector2 MinTerrainLocal = new Vector2(MinTerrainLocal3.x, MinTerrainLocal3.z) / new Vector2(T.terrainData.size.x, T.terrainData.size.z) * TMapSize;
		Vector2 MaxTerrainLocal = new Vector2(MaxTerrainLocal3.x, MaxTerrainLocal3.z) / new Vector2(T.terrainData.size.x, T.terrainData.size.z) * TMapSize;

		// Make sure Values are not out of bounds
		Vector2Int MinTerrainLocalint = new Vector2Int(Mathf.RoundToInt(Mathf.Max(0, MinTerrainLocal.x)), Mathf.RoundToInt(Mathf.Max(0, MinTerrainLocal.y)));
		Vector2Int MaxTerrainLocalint = new Vector2Int(Mathf.RoundToInt(Mathf.Min(TMapSize.x, MaxTerrainLocal.x)), Mathf.RoundToInt(Mathf.Min(TMapSize.y, MaxTerrainLocal.y)));

		return (MinTerrainLocalint, MaxTerrainLocalint);
	}

	public List<Vector3> AreaPoints(float InterPointDistance)
	{
		if (InterPointDistance <= 0) { InterPointDistance = 0.01f; }
		List<Vector3> Result = new List<Vector3>();

		Vector3 MinAreaPoint = new Vector3(PerimeterPoints.Min(P => P.x), 0, PerimeterPoints.Min(P => P.z));
		Vector3 MaxAreaPoint = new Vector3(PerimeterPoints.Max(P => P.x), 0, PerimeterPoints.Max(P => P.z));

		for (float i = MinAreaPoint.x; i <= MaxAreaPoint.x + 0.9 * InterPointDistance; i += InterPointDistance)
		{
			for (float j = MinAreaPoint.z; j <= MaxAreaPoint.z + 0.9 * InterPointDistance; j += InterPointDistance)
			{
				var P = new Vector3(i, 0, j);
				if (ContainsLocalPoint(P))
				{ Result.Add(P); }
			}
		}
		return Result;
	}
}

public interface IAreaModifier
{
	void ApplyAreaModifier();
}
public interface IAreaAutoUpdate
{
	void AreaUpdate();
}
