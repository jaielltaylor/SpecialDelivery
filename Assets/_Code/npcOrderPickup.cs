using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class npcOrderPickup : MonoBehaviour
{
    public int orderType; //0 is restaurant, 1 is cafe, 2 is grocery, 3 is post office
    public int orderCount; //number of orders to generate
    public Dictionary<int, List<Item>> orders = new Dictionary<int, List<Item>>();
    public List<Transform> deliveryLocations; //List of deliveryLocations
    public GameObject menu;
    public GameObject firstSelected;

    public Sprite itemSprite;

    private EventSystem system;
    private PlayerActions actions;    
    private int newID;

    private GameObject[] itemDisplays;
    private bool openedPrev = false;

    void Awake()
    {
        system = GameObject.FindGameObjectWithTag("eventSystem").GetComponent<EventSystem>();
        actions = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerActions>();
    }

    // Start is called before the first frame update
    void Start()
    {
        generateOrders(); //create the orders
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetActiveMenu()
    {
        system.SetSelectedGameObject(firstSelected);
        GameObject[] orderButtons = GameObject.FindGameObjectsWithTag("pickupButton");
        for (int i = 0; i < orderButtons.Length; i++)
        { 
            orderButtons[i].GetComponent<pickupMenuButton>().currPickupNpc = gameObject; 
        }
        displayDelivery();
    }

    void generateOrders()
    {
        for (int i = 0; i < orderCount; i++)
        {
            List<Item> newOrder = new List<Item>(); //create temp order
            int itemCount = UnityEngine.Random.Range(1, 3); //number of items in each order
            for (int j = 0; j < itemCount; j++) //generate items and add to the order
            {
                newID = (int)UnityEngine.Random.Range(findItemRange().x, findItemRange().y);
                if (orderType == 1 && newID == 5) { newID = roundNum(newID); }

                Item item = new Item();
                item.id = newID;
                Debug.Log(item.id);
                item.requiredTemp = item.idBaseTemp(newID);
                Debug.Log(item.requiredTemp);
                item.size = item.idBaseSize(newID);
                Debug.Log(item.size);
                item.sprite = itemSprite;
                newOrder.Add(item);
            }
            orders.Add(i, newOrder); //add the order to the list of orders
        }
    }

    public void displayDelivery() //onSelected
    {
        //depending on the number of the button, change the info on the display menu
        Debug.Log(EventSystem.current.currentSelectedGameObject.name);
        string buttonName = EventSystem.current.currentSelectedGameObject.name;
        if (buttonName.Contains("0")) { setOrderDisplay(0); }
        else if (buttonName.Contains("1")) { setOrderDisplay(1); }
        else if (buttonName.Contains("2")) { setOrderDisplay(2); }
        else if (buttonName.Contains("3")) { setOrderDisplay(3); }
        else if (buttonName.Contains("4")) { setOrderDisplay(4); }
    }

    void setOrderDisplay(int num)
    {
        if (GameObject.FindGameObjectWithTag("deliveryName") != null)
        {
            TextMeshProUGUI title = GameObject.FindGameObjectWithTag("deliveryName").GetComponent<TextMeshProUGUI>();
            Debug.Log(deliveryLocations[num].gameObject.name);
            title.SetText(deliveryLocations[num].gameObject.name); //set npc name from delivery Transform
            Debug.Log(title.text);
        }
        Sprite image = GameObject.FindGameObjectWithTag("deliveryIcon").GetComponent<Image>().sprite;
        image = deliveryLocations[num].GetComponent<SpriteRenderer>().sprite; //set npc icon from delivery transform (temp)

        if (!openedPrev) //need to check if menu was opened previously because if so, finding the itemDisplays again will cause errors.
        {
            itemDisplays = GameObject.FindGameObjectsWithTag("deliveryItems");
            openedPrev = true;
        }

        switch (orders[num].Count) //set item icons from order
        {
            default:
            case 1:
                itemDisplays[0].SetActive(true);
                itemDisplays[1].SetActive(false);
                itemDisplays[2].SetActive(false);
                itemDisplays[0].GetComponent<Image>().sprite = orders[num][0].sprite;
                itemDisplays[0].GetComponentInChildren<TextMeshProUGUI>().text = findSize(orders[num][0].size);
                break;
            case 2:
                itemDisplays[0].SetActive(true);
                itemDisplays[1].SetActive(true);
                itemDisplays[2].SetActive(false);
                itemDisplays[0].GetComponent<Image>().sprite = orders[num][0].sprite;
                itemDisplays[0].GetComponentInChildren<TextMeshProUGUI>().text = findSize(orders[num][0].size);
                itemDisplays[1].GetComponent<Image>().sprite = orders[num][1].sprite;
                itemDisplays[1].GetComponentInChildren<TextMeshProUGUI>().text = findSize(orders[num][1].size);
                break;
            case 3:
                itemDisplays[0].SetActive(true);
                itemDisplays[1].SetActive(true);
                itemDisplays[2].SetActive(true);
                itemDisplays[0].GetComponent<Image>().sprite = orders[num][0].sprite;
                itemDisplays[0].GetComponentInChildren<TextMeshProUGUI>().text = findSize(orders[num][0].size);
                itemDisplays[1].GetComponent<Image>().sprite = orders[num][1].sprite;
                itemDisplays[1].GetComponentInChildren<TextMeshProUGUI>().text = findSize(orders[num][1].size);
                itemDisplays[2].GetComponent<Image>().sprite = orders[num][2].sprite;
                itemDisplays[2].GetComponentInChildren<TextMeshProUGUI>().text = findSize(orders[num][2].size);
                break;
        }
    }

    Vector2 findItemRange()
    {
        switch(orderType)
        {
            default:
            case 0:
            case 1: 
                return new Vector2(3, 7);
            case 2: return new Vector2(0, 8);
            case 3: return new Vector2(0, 2);
        }
    }

    string findSize(int sizeNum)
    {
        switch(sizeNum)
        {
            default:
            case 1: return "Small";
            case 2: return "Medium";
            case 4: return "Large";
        }
    }

    int roundNum(int num)
    {
        int choice = UnityEngine.Random.Range(0, 1);
        if (choice == 1) { return num + 1; }
        else { return num - 1; }
    }
}
