using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class pickupMenuButton : MonoBehaviour, ISelectHandler
{
    /* Current Objective: finish this script to utilize the menu buttons
     * This script should have the capability to modify if this *instance* of the button has been pressed "aka the order has been added" already
     * but still not interfere with the ability to move between them (make the button uninteractable)
    */

    public GameObject currPickupNpc;
    public bool orderAdded;
    private List<Item> currOrder;

    // Start is called before the first frame update
    void Start()
    {
        //GetComponent<Button>().onClick.AddListener(); //add onclick function for adding to the inventory
        if (currPickupNpc != null) 
        { 
            currOrder = currPickupNpc.GetComponent<npcOrderPickup>().orders[findRelatedOrder()];
            Debug.Log("order is allocated!");
        } 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (currPickupNpc != null) { currPickupNpc.GetComponent<npcOrderPickup>().displayDelivery(); }
    }

    int findRelatedOrder()
    {
        if (gameObject.name.Contains("0")) { return 0; }
        else if (gameObject.name.Contains("1")) { return 1; }
        else if (gameObject.name.Contains("2")) { return 2; }
        else if (gameObject.name.Contains("3")) { return 3; }
        else if (gameObject.name.Contains("4")) { return 4; }
        else { return 0; }
    }
}
