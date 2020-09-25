using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinCollector : MonoBehaviour
{
	public CoinType CoinType;
	[System.NonSerialized] public int Score = 0;

	public void TakeCoin(Coin Coin)
	{
		if (CoinType == Coin.CoinType)
		{
			Score += Coin.MatchingScoreWorth;
		}
		else
		{
			Score += Coin.NonMatchingScoreWorth;
		}
		Debug.Log($"{name}'s score =  {Score}");
	}
}
