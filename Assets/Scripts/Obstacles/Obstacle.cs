﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[SelectionBase]
[RequireComponent(typeof(Collider))]
public class Obstacle : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == 8)//player
		{
			GameplayManager.Instance.Lose();
		}
	}
}
