using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class backpackButton : MonoBehaviour, ISelectHandler
{
    private Scrollbar bpScroll;
    void Awake()
    {
        bpScroll = GameObject.FindGameObjectWithTag("backpackScroll").GetComponent<Scrollbar>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnSelect(BaseEventData eventData)
    {
        switch(checkButtonNum() <= 4)
        {
            default:
            case true:
                bpScroll.value = 1;
                break;
            case false:
                bpScroll.value = 0;
                break;
        }
    }

    int checkButtonNum()
    {
        Regex re = new Regex(@"([a-zA-Z]+)(\d+)");
        Match result = re.Match(name);
        string numberPart = result.Groups[2].Value;
        return Convert.ToInt32(numberPart); 
    }
}
