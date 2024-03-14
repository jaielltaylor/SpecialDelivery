using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class PlayerActions : MonoBehaviour
{
    public Controller2D controller;
    public PlayerManager manager;

    public float walkSpeed = 30f;
    public float throwPower = 100f;

    private float horizontalMove = 0f;
    private float verticalMove = 0f;
    public float horizDirection = 0f;
    public bool staminaCall = false;

    public bool drawingLine;
    public Vector3 throwDir;
    private Vector3 mousePos;
    private Transform throwOrigin;
    private string originDir;

    [Header("Command Calls")]
    public bool run = false;
    public bool jump = false;
    public bool crouch = false;
    public bool roll = false;
    public bool tp = false;
    public bool thrown = false;
    public bool openPickup = false;
    public bool openDropoff = false;
    public bool openBackpack = false;

    // Update is called once per frame
    void Update()
    {

        horizontalMove = Input.GetAxisRaw("Horizontal") * walkSpeed;
        verticalMove = Input.GetAxisRaw("Vertical") * walkSpeed;
        
        if(Input.GetButtonDown("Pickup") && manager.touchingPickup)
        {
            manager.temp.GetComponent<pickupToThrow>().pickupDestroy(); //Destroy the pickup and set the mark
            manager.displayPickup(); //Display the Pickup in the UI
        }


        switch(openPickup)
        {
            default:
            case true:
                if (Input.GetButtonDown("Interact") && manager.touchingPickupNpc)
                {
                    openPickup = false;
                    manager.currPickupNPC.menu.SetActive(false);
                }
                break;
            case false:
                if (Input.GetButtonDown("Interact") && manager.touchingPickupNpc)
                {
                    openPickup = true;
                    manager.currPickupNPC.menu.SetActive(true);
                    manager.currPickupNPC.SetActiveMenu();
                }
                break;
        }

        switch (openDropoff)
        {
            default:
            case true:
                if (Input.GetButtonDown("Interact") && manager.touchingDropoffNpc)
                {
                    openPickup = false;
                    manager.currDropoffNPC.menu.SetActive(false);
                }
                break;
            case false:
                if (Input.GetButtonDown("Interact") && manager.touchingDropoffNpc)
                {
                    openPickup = true;
                    manager.currDropoffNPC.menu.SetActive(true);
                    manager.currDropoffNPC.SetActiveMenu();
                }
                break;
        }

        switch (openBackpack)
        {
            default:
            case true:
                if (Input.GetButtonDown("Inventory"))
                {
                    openBackpack = false;
                    manager.backpack.menu.SetActive(false);
                }
                break;
            case false:
                if (Input.GetButtonDown("Inventory"))
                {
                    openBackpack = true;
                    manager.backpack.menu.SetActive(true);
                    manager.backpack.SetActiveMenu();
                }
                break;
        }


        if (Input.GetButtonDown("Run") && !Input.GetButtonDown("Crouch") && manager.canRun)
        {
            manager.action = run = true;
        }
        else if (Input.GetButtonUp("Run") || Input.GetButtonDown("Crouch")) 
        { 
            manager.action = run = false;
        }
        else if (!manager.canRun) 
        {
            run = false;
            manager.action = null;
            StartCoroutine(tiredState());
        }
        
        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
        }

        if (Input.GetButtonDown("Roll") && manager.canDodge)
        {
            horizDirection = Input.GetAxis("Horizontal");
            if (horizDirection != 0) 
            { 
                manager.staticDodge = false;
                manager.moveCost += 10f;
            }
            else 
            { 
                manager.staticDodge = true;
                manager.moveCost += 5f;
            }
            staminaCall = true;
            roll = true;
        }

        if (Input.GetButtonDown("Teleport") && manager.canTeleport)
        {
            Debug.Log("Trying to teleport");
            tp = true;
            staminaCall = true;
            manager.moveCost += 15f;
        }

        if (Input.GetButtonDown("Crouch") && !Input.GetButtonDown("Run"))
        {
            crouch = true;
        }
        else if (Input.GetButtonUp("Crouch") || Input.GetButtonDown("Run")) { crouch = false; }

        if (Input.GetMouseButtonDown(0) && manager.mark != null)
        {
            CalculateThrowVector();
            SetArrow();
            drawingLine = true;
        }
        else if (Input.GetMouseButtonUp(0) && manager.mark != null)
        {
            RemoveArrow();
            Throw();
            thrown = true;
            drawingLine = false;
        }
    }

    void FixedUpdate()
    {
        controller.Move(horizontalMove * Time.fixedDeltaTime, verticalMove * Time.fixedDeltaTime, crouch, jump, run, roll, tp, horizDirection);
        jump = false;
        roll = false;
        tp = false;
    }

    
    public IEnumerator tiredState()
    {
        yield return new WaitForSeconds(manager.stamWaitTime);
        manager.action = false;
    }
    
    void CalculateThrowVector() //--Functions properly, totally cool (Actual Line drawing needs fixing)
    {
        //Figure out where the object will be thrown to
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 distance = mousePos - transform.position;
        Debug.Log(distance);
        throwDir = distance.normalized * throwPower;

        float shortestDist = float.MaxValue;
        //Figure out where the object will be thrown from
        for (int i = 0; i < controller.throwPoints.Count; i++)
        {
            float currentDist = Vector2.Distance(controller.throwPoints[i].transform.position, mousePos);
            if(currentDist < shortestDist)
            { 
                shortestDist = currentDist;
                throwOrigin = controller.throwPoints[i];
                originDir = controller.throwPoints[i].name;
            }
        }
    }

    void SetArrow() //--Line is correct when mousePos is not too close to the player (Further Testing?)
    {
        manager.line.positionCount = 2;
        manager.line.SetPosition(0, gameObject.transform.position);
        manager.line.SetPosition(1, mousePos);
        manager.line.textureMode = LineTextureMode.Tile;
        manager.line.enabled = true;
    }

    void RemoveArrow() { manager.line.enabled = false; }

    public void Throw()
    {

        manager.mark = Instantiate(manager.mark, throwOrigin.position, throwOrigin.rotation);
        manager.mark.GetComponent<Rigidbody2D>().AddForce(throwDir);
    }
}
