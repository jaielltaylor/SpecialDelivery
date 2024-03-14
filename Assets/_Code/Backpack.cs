using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Backpack : MonoBehaviour
{
    public GameObject menu;
    public GameObject firstSelected;

    private EventSystem system;
    private void Awake()
    {
        system = GameObject.FindGameObjectWithTag("eventSystem").GetComponent<EventSystem>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetActiveMenu()
    {
        system.SetSelectedGameObject(firstSelected);
        highlightOrder();
    }

    public void highlightOrder()
    {

    }
}
