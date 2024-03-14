using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class npcOrderDropoff : MonoBehaviour
{
    public GameObject menu;
    public Button firstSelected;
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
        EventSystem system = GameObject.FindGameObjectWithTag("eventSystem").GetComponent<EventSystem>();
        system.firstSelectedGameObject = firstSelected.gameObject;

    }
}
