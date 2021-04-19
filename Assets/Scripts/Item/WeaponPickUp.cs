using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponPickUp : Interactable
{
    public WeaponItem weapon;
    public override void Interact(PlayerManager playerManager)
    {
        base.Interact(playerManager);

        // Pick up item and add to player inventory
        PickUpItem(playerManager);
    }

    private void PickUpItem(PlayerManager playerManager)
    {
        PlayerInventory inventory;
        PlayerLocomotion playerLocomotion;
        AnimatorHandler animatorHandler;

        inventory = playerManager.GetComponent<PlayerInventory>();
        playerLocomotion = playerManager.GetComponent<PlayerLocomotion>();
        animatorHandler = playerManager.GetComponentInChildren<AnimatorHandler>();

        playerLocomotion.rigidbody.velocity = Vector3.zero;
        animatorHandler.PlayTargetAnimation(AnimatorHandler.Pick_Up_Item_STATE, true);

        inventory.weaponInventory.Add(weapon);
        playerManager.interactableUI.itemText.text = weapon.itemName;
        playerManager.interactableUI.itemIcon.sprite = weapon.itemIcon;
        playerManager.itemInteractableObject.SetActive(true);
        Destroy(gameObject);
    }
}
