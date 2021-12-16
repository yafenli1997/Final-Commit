using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Factory : MonoBehaviour
{
    private bool _prefab = false;
    public GameObject Prefab1;
    //public GameObject Prefab2;
    //public GameObject Prefab3;
    public GameObject[] Prefabs;

    public string TargetTag;

    public GameObject Target;

    public int MakeLimit = 4;
    private int _madeCount = 0;

    public double MakeRate = 2.0f;

    private float _lastMake = 0;

    

    // Start is called before the first frame update
    private void Start()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag(TargetTag);
        Target = targets[Random.Range(0, targets.Length)];
    }

    // Update is called once per frame
    private void Update()
    {
        if (Target == null) { return; }

        //when factory is done making (i.e. reached it's limit)
        //if (_madeCount >= MakeLimit)
        {
            //Destroy(gameObject);
        }

        //instantiate Agent
        _lastMake += Time.deltaTime; //_lastMake = _lastMake + Time.deltaTime;
        if (_lastMake > MakeRate)
        {
            //Debug.Log("Make");
            _lastMake = 0; //reset time counter
            _madeCount++;

            GameObject prefab = Prefabs[Random.Range(0, Prefabs.Length)];
            GameObject go = Instantiate(prefab, this.transform.position, Quaternion.identity);

            //≤‚ ‘boxÀÆ≤›----
            //YT_second mu = go.GetComponent<YT_second>();
            //mu.Targetforsecond = Target;
            //≤‚ ‘boxÀÆ≤›----

            MobileUnit mu = go.GetComponent<MobileUnit>();
            mu.Target = Target;

            //mu.Target = Manager.Instance.GetTarget();
            mu._factory = this;
            
        }
    }

    //public void DestroyFactory()
    //{
        //Manager.Instance.RemoveTargetFromList(Target);
        //Destroy(this.gameObject);
    //}

}