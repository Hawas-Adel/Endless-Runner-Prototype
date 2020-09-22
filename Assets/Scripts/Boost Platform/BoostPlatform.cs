using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
[RequireComponent(typeof(Collider))]
public class BoostPlatform : MonoBehaviour
{
	[SerializeField] [Min(0)] private float ForceMagnitude = 100;

	private void OnTriggerEnter(Collider other) => other.attachedRigidbody.AddRelativeForce(0, 0, ForceMagnitude, ForceMode.Impulse);
}
