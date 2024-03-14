using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryPickup : MonoBehaviour
{
    public Inventory inventory;
    public Backpack bp;

    private PlayerManager manager;
    // Start is called before the first frame update
    void Awake()
    {
        inventory = new Inventory();
        bp = GetComponent<Backpack>();
        manager = GetComponent<PlayerManager>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("deliveryPickup"))
        {
            manager.touchingPickupNpc = true;
            manager.currPickupNPC = other.gameObject.GetComponent<npcOrderPickup>();
            
        }
        
        if (other.CompareTag("deliveryDropoff"))
        {
            manager.touchingDropoffNpc = true;
            manager.currDropoffNPC = other.gameObject.GetComponent<npcOrderDropoff>();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("deliveryPickup"))
        {
            manager.touchingPickupNpc = false;

            if (manager.currPickupNPC != null && manager.currPickupNPC.menu.activeInHierarchy) { manager.currPickupNPC.menu.SetActive(false); }
            manager.inputs.openPickup = false;
            manager.currPickupNPC = null;
        }

        if (other.CompareTag("deliveryDropoff"))
        {
            manager.touchingDropoffNpc = false;
            
            if (manager.currDropoffNPC != null && manager.currDropoffNPC.menu.activeInHierarchy) { manager.currDropoffNPC.menu.SetActive(false); }
            manager.inputs.openDropoff = false;
            manager.currDropoffNPC = null;
        }
    }
}
