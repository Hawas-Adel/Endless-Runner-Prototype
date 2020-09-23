using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Area))]
public class AreaFence : MonoBehaviour, IAreaAutoUpdate
{
	[HideInInspector]public Area Area;
	[SerializeField] public List<GameObject> FenceParts = default;
	[SerializeField] public bool FenceOnEntrance = default;
	[SerializeField] public float FencePieceLength = 1;
	[Header("Offsets")]
	[SerializeField] public Vector3 PositionOffset = default;
	[SerializeField] public Vector3 RotationOffset = default;
	[Header("Colliders")]
	[SerializeField] public bool UsePeiceColliders = true;
	[SerializeField] public bool UseOptimizedCollider = false;
	[SerializeField] public float OptimizedColliderThickness = 1;
	[SerializeField] public float OptimizedColliderHeight = 1;

	[SerializeField] [HideInInspector] public string _GUID;
	public string GUID => _GUID = (string.IsNullOrEmpty(_GUID)) ? System.Guid.NewGuid().ToString() : _GUID;

	[System.NonSerialized] [HideInInspector] public bool Update = false;

	public void AreaUpdate()
	{
		Update = true;
	}

	[ContextMenu("Flip Area Orientation")]
	public void FlipAreaOrientation()
	{
		GetComponent<Area>().PerimeterPoints.Reverse();
		Update = true;
	}
}
