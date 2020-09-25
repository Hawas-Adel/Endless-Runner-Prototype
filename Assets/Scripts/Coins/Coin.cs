using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Coin : MonoBehaviour
{
	public CoinType CoinType;
	public int MatchingScoreWorth;
	public int NonMatchingScoreWorth;


	private void OnTriggerEnter(Collider other)
	{
		var collector = other.GetComponentInParent<CoinCollector>();
		if (collector)
		{
			collector.TakeCoin(this);
			gameObject.SetActive(false);
		}
	}
}

public enum CoinType { Type1, Type2, Type3, Type4 }
