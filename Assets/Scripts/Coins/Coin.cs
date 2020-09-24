using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Coin : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		var collector = other.GetComponentInParent<CoinCollector>();
		if (collector)
		{
			collector.CoinsCoint++;
			Debug.Log($"{collector.name} has {collector.CoinsCoint} Coins");
			gameObject.SetActive(false);
		}
	}
}
