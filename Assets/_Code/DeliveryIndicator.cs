using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryIndicator : MonoBehaviour
{
    public Transform target;
    public float hideDistance;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch(target == null)
        {
            case false:
                var dir = target.position - transform.position;
                if (dir.magnitude < hideDistance) { SetChildrenActive(false); }
                else
                {
                    SetChildrenActive(true);

                    var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                }
                break;
            case true:
                SetChildrenActive(false);
                break;
        }
    }

    void SetChildrenActive(bool value)
    {
        foreach (Transform child in transform) { child.gameObject.SetActive(value); }
    }
}
