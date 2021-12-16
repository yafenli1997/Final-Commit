using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class MobileUnit : MonoBehaviour
{
    public GameObject Target;
    public GameObject[] Positions;
    //public GameObject Factory;
    public GameObject Prefab;

    private Dictionary<GameObject, GameObject> Configure = new Dictionary<GameObject, GameObject>();

    public NavMeshAgent Agent;
    private NavMeshAgent Agent2;

    [HideInInspector]
    public Factory _factory;
    public bool _reachedTarget = false;

    // Start is called before the first frame update
    private void Start()
    {
        Agent = this.GetComponent<NavMeshAgent>();
        Agent.SetDestination(Target.transform.position);
        foreach (GameObject pos in Positions)  //开始就把3个坐标加入Configure
        {
            Configure.Add(pos, null);
        }
    }

    public void StartConfigure(GameObject go)
    {
        //Debug.Log("Start Configuration");
        //make sure there is a spot to configure the agent
        List<GameObject> keys = Configure.Keys.ToList();
        foreach (GameObject key in keys)
        {
            //guard statement
            if (Configure[key] != null) { continue; }  //进入下一次循环
            Configure[key] = go;  //如果key为空,就把当前对象赋值给key      

            //disable agent, this agent is now the leader
            NavMeshAgent agent = go.GetComponentInParent<NavMeshAgent>();
            CollisionDetection detect = go.GetComponent<CollisionDetection>();
            MobileUnit mobile = go.GetComponentInParent<MobileUnit>();
            agent.enabled = false;
            detect.enabled = false;
            mobile.enabled = false;
            break;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (_reachedTarget)
        {
            //Debug.Log("Waiting");

            //put object into place
            bool allConfigured = true;
            foreach (KeyValuePair<GameObject, GameObject> kvp in Configure)  //遍历Configure
            {
                //guard statement
                if (kvp.Value == null) 
                { 
                    allConfigured = false; 
                    continue;
                }
                //如果有值为空,就表示没有组合完毕
                if (kvp.Key.transform.position == kvp.Value.transform.position) { 
                    continue; 
                }
                //如果不为空, 就把位置赋值给key

                //move object into position
                GameObject cAgent = kvp.Value;                  
                Vector3 pos = kvp.Key.transform.position;       //拿到位置
                Quaternion rot = kvp.Key.transform.rotation;  //拿到旋转

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

            if (allConfigured)   //组合完毕
            {
                //destroy all agents
                List<GameObject> keys = Configure.Keys.ToList();
                foreach (GameObject key in keys)
                {
                    GameObject go = Configure[key];
                    Destroy(go);

                }
                //Destroy(Target);

                //instantiate factory
                GameObject prefab = Instantiate(Prefab, transform.position, transform.rotation, null);
                float moveY = prefab.transform.localScale.y / 2;
                prefab.transform.position += new Vector3(0, 0, 0);
                prefab.transform.position += new Vector3(0, moveY, 0);

                //Debug.Log("All Configured");
                //Destroy(gameObject); //destroy this last, because it will destroy this script
            }
            return;
        }

        //Debug.DrawLine(this.transform.position, Target.transform.position, Color.black);
        //Debug.DrawRay(this.transform.position, this.transform.forward, Color.red);

        //test if agent has reached target (do this first)
        if (!Agent.pathPending)
        {
            if (Agent.remainingDistance <= Agent.stoppingDistance)
            {
                if (!Agent.hasPath || Agent.velocity.sqrMagnitude == 0f)
                {
                    //一根水草碰到另外一根水草
                    // Done
                    //Debug.Log(gameObject.name + " has reached Target");
                    _reachedTarget = true;
                    Agent.enabled = false;
                }
            }
        }
    }
}