using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetection : MonoBehaviour
{
    public string CollisionTag = "Agent";
    private MobileUnit _mobileUnit;
    //private YT_second _YT_second;

    // Start is called before the first frame update
    private void Start()
    {
        //Debug.Log("CollisionDetection.Start()");
        _mobileUnit = transform.parent.GetComponent<MobileUnit>();
        //_YT_second = transform. parent.GetComponent<YT_second>();
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("OnTriggerEnter()");
        
        if (_mobileUnit == null) { return; }
        if (!_mobileUnit._reachedTarget) { return; }
        GameObject go = other.gameObject;
        if (go.tag != CollisionTag) { return; }
        _mobileUnit.StartConfigure(go);
        //Debug.Log("CDWork");
    }
    //private void OnTriggerEnter(Collider other)
    //{
        //if (_YT_second == null) { return; }
       // if (!_YT_second._reachedTargetforsecond) { return; }
        //GameObject going = other.gameObject;
        //if (going.tag != CollisionTag) { return; }
        //_YT_second.StartSecondConfigure(going);

    //}

}