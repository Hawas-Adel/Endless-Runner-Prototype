using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
	#region Singlton
	public static GameplayManager Instance { get; private set; }
	private void OnEnable()
	{
		if (Instance)
		{
			Destroy(this);
		}
		else
		{
			Instance = this;
		}
	}
	private void OnDisable()
	{
		if (Instance == this)
		{
			Instance = null;
		}
	}
	#endregion

	public void Win() => Debug.Log("Win");

	public void Lose() => Debug.Log("Lose");
}
