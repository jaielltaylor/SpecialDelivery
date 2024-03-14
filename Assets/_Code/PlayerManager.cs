using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


//This script focuses on the health and stamina management system of the player.
public class PlayerManager : MonoBehaviour
{
    public Controller2D controller;
    public PlayerActions inputs;
    public Image markDisplay;
    public Slider stamina;

    public float maxStamina = 100f;
    public float drainCost = 1f;
    public float stamRecoveryRate = 0.01f;
    public int stamWaitTime = 5;

    public bool? action = false;
    public float moveCost; //Calculate the cost of all the "moves" made during update

    public bool touchingPickup = false;

    //Drain Actions
    public bool canRun = true;
    public bool canWallGrab = true;

    //Cost Actions
    public bool canDodge = true;
    public bool staticDodge = false;
    public bool canTeleport = true;

    //Menu Interactions
    public bool touchingPickupNpc = false;
    public bool touchingDropoffNpc = false;

    public npcOrderPickup currPickupNPC;
    public npcOrderDropoff currDropoffNPC;

    public Backpack backpack;

    //Throw Objects
    public LineRenderer line;
    public GameObject temp;
    public GameObject mark;

    private List<bool> drainActions;

    void Awake()
    {
        line = GetComponent<LineRenderer>();
        line.enabled = false;
        inputs = GetComponent<PlayerActions>();
        backpack = GetComponent<Backpack>();
        controller = GetComponent<Controller2D>();
        markDisplay = GameObject.FindGameObjectWithTag("markDisplay").GetComponent<Image>();
    }

    // Start is called before the first frame update
    void Start()
    {
        stamina.value = maxStamina;

        //drainActions.Add(canRun);
    }

    // Update is called once per frame
    void Update()
    {
        //Can Drain Actions be used?
        if (stamina.value >= drainCost)
        {
            /*
            for (int i = 0; i < drainActions.Count; i++)
            { drainActions[i] = true; }
            */
            canRun = true;
            canWallGrab = true;
        }
        else
        {
            /*
            for (int i = 0; i < drainActions.Count; i++)
            { drainActions[i] = false; }
            */
            canRun = false;
            canWallGrab = false;
        }

        //Can Dodge be used?
        if (stamina.value >= 10f) { canDodge = true; }
        else { canDodge = false; }

        //Can Teleport be used?
        if (mark == null) { inputs.thrown = false; }
        if (stamina.value >= 15f && inputs.thrown) { canTeleport = true; }
        else if (stamina.value < 15f || !inputs.thrown) { canTeleport = false; }

        //Make sure line is displayed properly
        if (inputs.drawingLine) { fixLine(); }

        //if the player is not touching the PickupNPC, the npc's menu, if it is active, is shut off
        //if (!touchingPickupNpc && currPickupNPC != null && currPickupNPC.menu.activeInHierarchy) { currPickupNPC.menu.SetActive(false); }
    }

    void FixedUpdate()
    {
        switch (action)
        {
            case null:
                stamina.value = 0;
                break;
            case false: //no action is being taken
                stamina.value += stamRecoveryRate;
                break;
            case true: //an action is being taken
                stamina.value -= drainCost;
                break;
        }
        /*
        switch (controller.onWall) //comment out if wall climbing and wall walking will not decrease stamina
        {
            case false: break;
            case true:
                stamina.value -= drainCost;
                break;
        }
        */

        switch (inputs.staminaCall)
        {
            case false: break;
            case true:
                stamina.value -= moveCost;
                moveCost = 0;
                break;
        }
    }

    public void displayPickup() 
    { 
        markDisplay.sprite = mark.GetComponent<SpriteRenderer>().sprite; 
        Debug.Log("Picked Up."); 
    }
    public void nullDisplayPickup() 
    { 
        markDisplay.sprite = null;
        Debug.Log("Dropped.");
    }
    public void fixLine()
    {
        //width is the width of the line;
        float width = line.startWidth;
        line.material.mainTextureScale = new Vector2(2 * width, 1f);
        // 1/width is the repetition of the texture per unit (thus you can also do double lines)
    }
}
