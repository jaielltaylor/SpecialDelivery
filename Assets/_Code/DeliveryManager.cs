using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryManager : MonoBehaviour
{
    public List<Image> deliverySigs;
    public Sprite emptySig;
    public Sprite completeSig;

    float currDeliveries = 0f;
    public float maxDeliveries = 13f;

    float lerpSpeed;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (currDeliveries > maxDeliveries) { currDeliveries = maxDeliveries; }

        lerpSpeed = 3f * Time.deltaTime;

        CurrentProgress();
    }

    public void Return(float numReturned) { if (currDeliveries > 0) { currDeliveries -= numReturned; } }

    public void Deliver(float numDelivered) { if (currDeliveries < maxDeliveries) { currDeliveries += numDelivered; } } //Call this method whenever the player successfully delivers a package

    void CurrentProgress()
    {
        for (int i = 0; i < deliverySigs.Count; i++)
        {
            if (!DetermineDelivery(currDeliveries, i)) { deliverySigs[i].sprite = completeSig; } //Delivery successful!
            else { deliverySigs[i].sprite = emptySig; } //Delivery unsuccessful!
        }
    }

    bool DetermineDelivery(float delivery, int num) { return (num >= delivery); }
}
