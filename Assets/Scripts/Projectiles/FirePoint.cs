using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FirePoint : MonoBehaviour
{
	[SerializeField] private Projectile ProjectilePrefab = default;
	[SerializeField] [Min(0)] private float ProjectileLifeTime = 4;
	[SerializeField] [Min(0)] private Vector3 ProjectileRelativeLaunchForce = new Vector3(0, 0, 1000);

	public void Fire()
	{
		var Projectile = Instantiate(ProjectilePrefab.gameObject, transform.position, transform.rotation).GetComponent<Rigidbody>();
		Destroy(Projectile.gameObject, ProjectileLifeTime);
		Projectile.AddRelativeForce(ProjectileRelativeLaunchForce);
	}

	public void OnPlayerFire(InputAction.CallbackContext CTX)
	{
		if (CTX.performed)
		{
			Fire();
		}
	}
}
