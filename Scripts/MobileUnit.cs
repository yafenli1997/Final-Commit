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
        foreach (GameObject pos in Positions)  //��ʼ�Ͱ�3���������Configure
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
            if (Configure[key] != null) { continue; }  //������һ��ѭ��
            Configure[key] = go;  //���keyΪ��,�Ͱѵ�ǰ����ֵ��key      

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
            foreach (KeyValuePair<GameObject, GameObject> kvp in Configure)  //����Configure
            {
                //guard statement
                if (kvp.Value == null) 
                { 
                    allConfigured = false; 
                    continue;
                }
                //�����ֵΪ��,�ͱ�ʾû��������
                if (kvp.Key.transform.position == kvp.Value.transform.position) { 
                    continue; 
                }
                //�����Ϊ��, �Ͱ�λ�ø�ֵ��key

                //move object into position
                GameObject cAgent = kvp.Value;                  
                Vector3 pos = kvp.Key.transform.position;       //�õ�λ��
                Quaternion rot = kvp.Key.transform.rotation;  //�õ���ת

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

            if (allConfigured)   //������
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
                    //һ��ˮ����������һ��ˮ��
                    // Done
                    //Debug.Log(gameObject.name + " has reached Target");
                    _reachedTarget = true;
                    Agent.enabled = false;
                }
            }
        }
    }
}