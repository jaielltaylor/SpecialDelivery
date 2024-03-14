using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    private int packageTracker = 6;
    private int foodTracker = 6;
    private int bevTracker = 6;

    public Inventory()
    {
        packageTracker = 6;
        foodTracker = 6;
        bevTracker = 6;
    }
    public void AddDelivery(List<Item> order)
    {
        //GameObject inventory = GameObject.FindGameObjectWithTag("Inventory");
        bool canCarry = false;

        for (int i = 0; i < order.Count; i++)
        {
            switch(order[i].requiredTemp)
            {
                //If the item does not have a required temp
                case 0:
                    canCarry = checkForSize(order[i], packageTracker);
                    packageTracker -= order[i].size;
                    break;
                //If the item requires a cold temp
                case 1:
                    canCarry = checkForSize(order[i], bevTracker);
                    bevTracker -= order[i].size;
                    break;
                //If the item requires a hot temp
                case 2:
                    canCarry = checkForSize(order[i], foodTracker);
                    foodTracker -= order[i].size;
                    break;
            }
        }

        switch(canCarry)
        {
            case false:
                //Function for rejecting order from backpack
                break;
            case true:
                //Function for adding order to backpack
                break;
        }
    }

    public void RemoveDelivery(List<Item> order)
    {
        //GameObject inventory = GameObject.FindGameObjectWithTag("Inventory");
        for (int i = 0; i < order.Count; i++)
        {
            switch (order[i].requiredTemp)
            {
                case 0:
                    packageTracker += order[i].size;
                    break;
                case 1:
                    bevTracker += order[i].size;
                    break;
                case 2:
                    foodTracker += order[i].size;
                    break;
            }
        }

        //Function for removing items from backpack
    }

    private bool checkForSize(Item item, int section)
    {
        switch (item.size)
        {
            default:
            case 1: return spaceForSmall(section);
            case 2: return spaceForMedium(section);
            case 4: return spaceForLarge(section);
        }   
    }

    public bool spaceForSmall(int section) //if false section is Full
    {
        if (section > 0) { return true; }
        else { return false; }
    }

    public bool spaceForLarge(int section)
    {
        if (section >= 4) { return true; }
        else { return false; }
    }

    public bool spaceForMedium(int section)
    {
        if (section >= 2) { return true; }
        else { return false; }
    }
}
