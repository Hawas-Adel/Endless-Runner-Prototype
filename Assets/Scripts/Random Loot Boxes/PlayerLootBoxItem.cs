using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLootBoxItem : MonoBehaviour
{
	[System.NonSerialized] public abst_LootBoxItem CurrentItem;

	public void OnUseItem(InputAction.CallbackContext CTX)
	{
		if (CTX.performed && CurrentItem)
		{
			CurrentItem.UseItem(this);
			CurrentItem = null;
		}
	}
}
