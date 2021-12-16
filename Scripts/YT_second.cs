using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;


public class YT_second : MonoBehaviour
{

    public GameObject Targetforsecond;
    public GameObject[] Positions;
    public GameObject Prefab;

    private Dictionary<GameObject, GameObject> Configure = new Dictionary<GameObject, GameObject>();
    private NavMeshAgent agent;

    [HideInInspector]
    public Factory _factory;
    public bool _reachedTargetforsecond = false;


    // Start is called before the first frame update

    private void Start()
    {
        GameObject[] points = GameObject.FindGameObjectsWithTag("Targetforsecond");
        /*foreach (GameObject item in points)
        {
            //Debug.Log(item.name);
        }*/
        int intR = Random.Range(0, points.Length);
        Targetforsecond = points[intR];

        //Targetforsecond = GameObject.FindWithTag("Targetforsecond");
        agent = this.GetComponent<NavMeshAgent>();
        agent.SetDestination(Targetforsecond.transform.position);
        foreach (GameObject pos in Positions)
        {
            Configure.Add(pos, null);
        }
        GameObject going = Prefab;
    }
    // Update is called once per frame


    public void StartSecondConfigure(GameObject going)
    {

        //Debug.Log("Start secondConfiguration");
        //make sure there is a spot to configure the agent
        List<GameObject> keys = Configure.Keys.ToList();
        foreach (GameObject key in keys)
        {
            //guard statement
            if (Configure[key] != null) { continue; }
            Configure[key] = going;
            //disable agent, this agent is now the leader
            //NavMeshAgent agent = going.GetComponentInParent<NavMeshAgent>();
            //Collision2 detect = going.GetComponent<Collision2>();
            //YT_second yT_Second = going.GetComponentInParent<YT_second>();

            NavMeshAgent agent = going.GetComponent<NavMeshAgent>();
            Collision2 detect = going.GetComponentInChildren<Collision2>();
            YT_second yT_Second = going.GetComponent<YT_second>();

            Destroy(going.GetComponentInChildren<Rigidbody>());  //把刚体删了避免到处漂浮


            foreach (BoxCollider item in going.GetComponentsInChildren<BoxCollider>())
            {
                Destroy(item);
            }

            

            agent.enabled = false;
            detect.enabled = false;
            yT_Second.enabled = false;
             
            //Debug.Log("---going----" + going.name);
            break;
        }
    }
    private void Update()
    {
        if (_reachedTargetforsecond)
        {
            //Debug.Log("Waiting");

            //put object into place
            bool allConfigured = true;
            foreach (KeyValuePair<GameObject, GameObject> kvp in Configure)
            {
                //guard statement
                if (kvp.Value == null) { allConfigured = false; continue; }
                if (kvp.Key.transform.position == kvp.Value.transform.position) { continue; }

                //move object into position
                GameObject cAgent = kvp.Value;
                Vector3 pos = kvp.Key.transform.position;
                Quaternion rot = kvp.Key.transform.rotation;

                //cAgent.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
                cAgent.transform.position = Vector3.Lerp(cAgent.transform.position, pos, Time.deltaTime);
                cAgent.transform.rotation = Quaternion.Lerp(cAgent.transform.rotation, rot, Time.deltaTime);
                //if (Vector3.Distance(cAgent.transform.position, pos) < 0.05f)
                //{
                    Debug.Log("Configured222");
                    cAgent.transform.position = pos;
                    cAgent.transform.rotation = rot;
                 //}

                allConfigured = false;
            }

            if (allConfigured)
            {
                Debug.Log("All Configuredsecond---222");
                //destroy all agents
                List<GameObject> keys = Configure.Keys.ToList();
                foreach (GameObject key in keys)
                {
                    GameObject going = Configure[key];
                    Destroy(going);  //把父对象消除掉

                }

                Destroy(this.gameObject);
                //instantiate factory

                GameObject prefab = Instantiate(Prefab, transform.position, transform.rotation, null);
                //float moveY = prefab.transform.localScale.y / 2;
                //prefab.transform.position += new Vector3(0, 0, 0);
                //prefab.transform.position += new Vector3(0, 0, 0);
                //prefab.transform.position += new Vector3(0, moveY, 0);

                //Debug.Log("All Configuredsecond---2" + prefab.name);
                //Debug.Log("2222222222------");
                //Destroy(gameObject); //destroy this last, because it will destroy this script
            }
            return;
        }


        if (!agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    // Done
                    //Debug.Log(gameObject.name + " has reached Target----2");
                    _reachedTargetforsecond = true;
                    agent.enabled = false;
                }
            }
        }
    }
}

