using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class SecondUnit : MonoBehaviour
{ 
    public GameObject Target;
    public GameObject[] Positions;
    public GameObject Prefab;

    private Dictionary<GameObject, GameObject> Configure = new Dictionary<GameObject, GameObject>();

    public NavMeshAgent Agent;
    private NavMeshAgent Agent2;

    [HideInInspector]
    public Factory _factory;
    public bool _reachedTarget = false;

    // Start is called before the first frame update
    void Start()
    {
        Agent = this.GetComponent<NavMeshAgent>();
        Agent.SetDestination(Target.transform.position);
        foreach (GameObject pos in Positions)
        {
            Configure.Add(pos, null);
            Debug.Log("Add Done");
        }
    }

    public void StartConfigure(GameObject goo)
    {
        Debug.Log("Start Second Configuration");
        //make sure there is a spot to configure the agent
        List<GameObject> keys = Configure.Keys.ToList();
        foreach (GameObject key in keys)
        {
            //guard statement
            if (Configure[key] != null) { continue; }
            Configure[key] = goo;

            //disable agent, this agent is now the leader
            NavMeshAgent agent = goo.GetComponentInParent<NavMeshAgent>();
            CollisionDetection detect = goo.GetComponent<CollisionDetection>();
            SecondUnit mobile = goo.GetComponentInParent<SecondUnit>();
            agent.enabled = false;
            detect.enabled = false;
            mobile.enabled = false;
            break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_reachedTarget)
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

                cAgent.transform.position = Vector3.Lerp(cAgent.transform.position, pos, Time.deltaTime);
                cAgent.transform.rotation = Quaternion.Lerp(cAgent.transform.rotation, rot, Time.deltaTime);
                if (Vector3.Distance(cAgent.transform.position, pos) < 0.05f)
                {
                    //Debug.Log("Configured");
                    cAgent.transform.position = pos;
                    cAgent.transform.rotation = rot;
                }

                allConfigured = false;
            }

            if (allConfigured)
            {
                //destroy all agents
                List<GameObject> keys = Configure.Keys.ToList();
                foreach (GameObject key in keys)
                {
                    GameObject goo = Configure[key];
                    Destroy(goo);

                }
                //Destroy(Target);

                Debug.Log("All Configured");
            }
            return;
        }

        if (!Agent.pathPending)
        {
            if (Agent.remainingDistance <= Agent.stoppingDistance)
            {
                if (!Agent.hasPath || Agent.velocity.sqrMagnitude == 0f)
                {
                    // Done
                    //Debug.Log(gameObject.name + " has reached Target");
                    _reachedTarget = true;
                    Agent.enabled = false;
                }
            }
        }
    }
}
