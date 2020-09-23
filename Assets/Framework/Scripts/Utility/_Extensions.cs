using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
	//Lists
	public static T RandomElement<T>(this List<T> L) => L[Random.Range(0, L.Count)];

	public static List<T> RandomSublist<T>(this List<T> L, int MaxSize = -1)
	{
		List<T> RL = new List<T>();
		int Size = L.Count;
		Size = Random.Range(1, Size);
		Size = (MaxSize > 0) ? Random.Range(1, Mathf.Min(MaxSize, L.Count)) : Random.Range(1, Size);
		while (RL.Count < Size)
		{
			RL.AddUnique(L[Random.Range(0, L.Count)]);
		}
		return RL;
	}

	public static void AddUnique<T>(this List<T> L, T Item) { if (!L.Contains(Item)) { L.Add(Item); } }

	public static List<T> Shuffle<T>(this List<T> L)
	{
		if (L.Count < 2) { return L; }

		for (int i = 0; i < L.Count - 1; i++)
		{
			int RI = Random.Range(i, L.Count);
			T _T = L[i];
			L[i] = L[RI];
			L[RI] = _T;
		}
		return L;
	}

	//Bhaviors
	public static void SetActive(this Behaviour T, bool State) => T.enabled = State;

	//Numbers
	public static bool Between(this float N, float Low, float Hi)
	{
		if (Low > Hi) { var T = Low; Low = Hi; Hi = T; }
		return (Low <= N && N <= Hi);
	}
	public static bool Between(this int N, int Low, int Hi)
	{
		if (Low > Hi) { var T = Low; Low = Hi; Hi = T; }
		return (Low <= N && N <= Hi);
	}
}
