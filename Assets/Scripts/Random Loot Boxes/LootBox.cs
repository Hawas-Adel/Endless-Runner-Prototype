using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class LootBox : MonoBehaviour
{
	[SerializeField] private List<abst_LootBoxItem> PotentialPickups = default;

	private void OnTriggerEnter(Collider other)
	{
		var PLBI = other.GetComponentInParent<PlayerLootBoxItem>();
		if (PLBI)
		{
			PickUpRandomItem(PLBI);
			gameObject.SetActive(false);
		}
	}

	private void PickUpRandomItem(PlayerLootBoxItem Player)
	{
		var item = PotentialPickups.RandomElement();
		Player.CurrentItem = item;
	}
}

public abstract class abst_LootBoxItem : ScriptableObject
{
	public virtual void UseItem(PlayerLootBoxItem User) => Debug.Log($"{User.name} used {name}");
}
