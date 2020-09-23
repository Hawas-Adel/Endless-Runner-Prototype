using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AreaFence))]
public class AreaFenceEditor : Editor
{
	private AreaFence TargetFence;
	private void OnEnable() => TargetFence = target as AreaFence;
	public override void OnInspectorGUI()
	{
		EditorGUI.BeginChangeCheck();
		base.OnInspectorGUI();
		if (EditorGUI.EndChangeCheck() || TargetFence.Update)
		{
			TargetFence.Update = false;
			if (TargetFence.FencePieceLength <= 0) { TargetFence.FencePieceLength = 1; return; }
			TargetFence.Area = TargetFence.Area ?? TargetFence.GetComponent<Area>();
			SpawnFence();
		}
	}
	private void SpawnFence()
	{
		Transform GeneratedParent = GetGeneratedParent() ?? CreateGeneratedParent();

		//make sure number of Fence Anchors Syncs with Peremeter Points, Respect Entrance Fence Choice
		var PerimeterPointsCount = (TargetFence.FenceOnEntrance) ? TargetFence.Area.PerimeterPoints.Count : TargetFence.Area.PerimeterPoints.Count - 1;
		for (int i = 0 ; i < Mathf.Max(GeneratedParent.childCount, PerimeterPointsCount) ; i++)
		{
			if (i < GeneratedParent.childCount && i < PerimeterPointsCount)
			{ GeneratedParent.GetChild(i).SetSiblingIndex(i); continue; }
			if (i >= GeneratedParent.childCount)
			{
				Transform _FA = new GameObject($"Fence Anchor ({i})").transform;
				_FA.parent = GeneratedParent;
				_FA.SetSiblingIndex(i);
				continue;
			}
			if (i >= PerimeterPointsCount)
			{ DestroyImmediate(GeneratedParent.GetChild(i).gameObject); continue; }
		}

		//actually Spawn and Position the fence
		for (int i = 0 ; i < GeneratedParent.childCount ; i++)
		{
			//Set Position and Rotation of Fence Anchors (At corresponding point, Looking at next point)
			Transform _FA = GeneratedParent.GetChild(i);
			_FA.localPosition = TargetFence.Area.PerimeterPoints[i];
			int i_n = (i + 1 == TargetFence.Area.PerimeterPoints.Count) ? 0 : i + 1;
			_FA.LookAt(TargetFence.Area.transform.TransformPoint(TargetFence.Area.PerimeterPoints[i_n]));

			//Spawn Fence Blocks
			int SpawnCount = Mathf.FloorToInt(Vector3.Distance(TargetFence.Area.PerimeterPoints[i], TargetFence.Area.PerimeterPoints[i_n]) / TargetFence.FencePieceLength);
			//Sync SpawnCount With Existing blocks
			for (int j = 0 ; j < Mathf.Max(_FA.childCount, SpawnCount) ; j++)
			{
				if (j >= _FA.childCount)
				{
					Transform __FP = (PrefabUtility.InstantiatePrefab(TargetFence.FenceParts.RandomElement()) as GameObject).transform;
					__FP.parent = _FA;
					__FP.localScale = __FP.lossyScale;
				}
				else if (j >= SpawnCount)
				{ DestroyImmediate(_FA.GetChild(j).gameObject); continue; }
				//Position the blocks
				Transform _FP = _FA.GetChild(j);
				_FP.localPosition = j * TargetFence.FencePieceLength * Vector3.forward + TargetFence.PositionOffset;
				_FP.localRotation = Quaternion.Euler(TargetFence.RotationOffset /*+ new Vector3(-_FA.localRotation.eulerAngles.x, 0, 0)*/);
				_FP.GetComponent<Collider>().enabled = TargetFence.UsePeiceColliders;
			}

			//stretch The fence To fill whole edge
			float ScaleFactor = (_FA.childCount == 0) ? 1 :
				Vector3.Distance(TargetFence.Area.PerimeterPoints[i], TargetFence.Area.PerimeterPoints[i_n]) /
				(TargetFence.FencePieceLength * _FA.childCount);
			_FA.localScale = ScaleFactor * Vector3.one;

			//Handelling colliders
			BoxCollider AnchorCollider = _FA.GetComponent<BoxCollider>();
			if (AnchorCollider == null)
			{ AnchorCollider = _FA.gameObject.AddComponent<BoxCollider>(); }

			AnchorCollider.enabled = TargetFence.UseOptimizedCollider;
			AnchorCollider.size = new Vector3(TargetFence.OptimizedColliderThickness, TargetFence.OptimizedColliderHeight,
				TargetFence.FencePieceLength * _FA.childCount);
			AnchorCollider.center = new Vector3(TargetFence.PositionOffset.x, 0.5f * AnchorCollider.size.y, 0.5f * AnchorCollider.size.z);
		}
	}

	private Transform GetGeneratedParent()
	{
		for (int i = 0 ; i < TargetFence.transform.childCount ; i++)
		{
			var Child = TargetFence.transform.GetChild(i);
			if (Child.name == $"Generated Fence ({TargetFence.GUID})")
			{
				return Child;
			}
		}
		return null;
	}
	private Transform CreateGeneratedParent()
	{
		Transform Child = new GameObject($"Generated Fence ({TargetFence.GUID})").transform;
		Child.parent = TargetFence.transform;
		Child.localPosition = Vector3.zero;
		Child.localRotation = Quaternion.identity;
		return Child;
	}
}
