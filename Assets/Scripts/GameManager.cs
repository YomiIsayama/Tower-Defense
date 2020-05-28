using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public bool m_debug = true;
    public List<Enemy> m_EnemyList = new List<Enemy>();
    public List<PathNode> m_PathNodes;
    public LayerMask m_groundlayer;
    public int m_wave = 1;
    public int m_waveMax = 10;
    public int m_life = 10;
    public int m_point = 30;

    Text m_txt_wave;
    Text m_txt_life;
    Text m_txt_point;
    Button m_but_try;

    bool m_isSelectedButton = false;

    void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        UnityAction<BaseEventData> downAction = new UnityAction<BaseEventData>(OnButCreateDefenderDown);
        UnityAction<BaseEventData> upAction = new UnityAction<BaseEventData>(OnButCreateDefenderUp);

        EventTrigger.Entry down = new EventTrigger.Entry();
        down.eventID = EventTriggerType.PointerDown;
        down.callback.AddListener(downAction);

        EventTrigger.Entry up = new EventTrigger.Entry();
        up.eventID = EventTriggerType.PointerUp;
        up.callback.AddListener(upAction);

        foreach(Transform t in this.GetComponentsInChildren<Transform>())
        {
            if (t.name.CompareTo("wave") == 0) 
            {
                m_txt_wave = t.GetComponent<Text>();
                SetWave(1);
            }
            else if (t.name.CompareTo("life") == 0)
            {
                m_txt_life = t.GetComponent<Text>();
                m_txt_life.text = string.Format("life:<color=yellow>{0}</color>", m_life);
            }
            else if (t.name.CompareTo("point") == 0)
            {
                m_txt_point = t.GetComponent<Text>();
                m_txt_point.text = string.Format("point:<color=yellow>{0}</color>", m_point);
            }
            else if (t.name.CompareTo("but_try")==0)
            {
                m_but_try = t.GetComponent<Button>();
                m_but_try.onClick.AddListener(delegate ()
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                });
            }
            else if (t.name.Contains("but_player"))
            {
                EventTrigger trigger = t.gameObject.AddComponent<EventTrigger>();
                trigger.triggers = new List<EventTrigger.Entry>();
                trigger.triggers.Add(down);
                trigger.triggers.Add(up);
            }
        }

        BuildPath();
    }

    // Update is called once per frame
    void Update()
    {
        if(m_isSelectedButton)
        {
            return;
        }
#if (UNITY_IOS || UNITY_ANDROID) && !UNITY_ENITOR
        bool press = Input.touches.Length>0?true:false;
        float mx = 0;
        float my = 0;
        if(press)
        {
            if(Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                mx = Input.GetTouch(0).deltaPosittion.x * 0.001f;
                my = Input.GetTouch(0).deltaPosittion.y * 0.001f; 
            }
        }
#else 
        bool press = Input.GetMouseButton(0);
        float mx = Input.GetAxis("Mouse X");
        float my = Input.GetAxis("Mouse Y");
#endif 
        GameCamera.Instance.Control(press, mx, my);
    }

    public void SetWave(int wave)
    {
        m_wave = wave;
        m_txt_wave.text = string.Format("wave:<color=yellow>{0}/{1}</color>", m_wave, m_waveMax);
    }

    public void SetDamage(int damage)
    {
        m_life -= damage;
        if (m_life <= 0)
        {
            m_life = 0;
            m_but_try.gameObject.SetActive(true);
        }
        m_txt_life.text = string.Format("life:<color=yellow>{0}</color>", m_life);
    }

    public bool SetPoint(int point)
    {
        if(m_point+point<0)
        {
            return false;
        }
        m_point += point;
        m_txt_point.text = string.Format("point:<color=yellow>{0}</color>", m_point);
        return true;
    }

    void OnButCreateDefenderDown(BaseEventData data)
    {
        m_isSelectedButton = true;
    }
    void OnButCreateDefenderUp(BaseEventData data)
    {
        //创建射线
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitinfo;
        //检测是否与地面相碰撞
        if (Physics.Raycast(ray, out hitinfo, 1000, m_groundlayer))
        {
            //如果选中的是一个可用的格子
            if (TileObject.Instance.getDataFromPosition(hitinfo.point.x, hitinfo.point.z) == (int)Defender.TileStatus.GUARD)
            {
                //获取碰撞点的位置
                Vector3 hitpos = new Vector3(hitinfo.point.x, 0, hitinfo.point.z);
                //获取Grid Object位置
                Vector3 gridPos = TileObject.Instance.transform.position;
                //获取格子大小
                float tilesize = TileObject.Instance.tileSize;
                //计算出所点击格子的中心位置
                hitpos.x = gridPos.x + (int)((hitpos.x - gridPos.x) / tilesize) * tilesize + tilesize * 0.5f;
                hitpos.z = gridPos.z + (int)((hitpos.z - gridPos.z) / tilesize) * tilesize + tilesize * 0.5f;

                //获取旋转的按钮Game Object，将简单通过按钮名字判断选择了哪个按钮
                GameObject go = data.selectedObject;
                if (go.name.Contains("1"))//如果按钮名字包括“1”
                {
                    if (SetPoint(-15))//减少15个铜钱，然后创建近战防守单位
                        Defender.Create<Defender>(hitpos, new Vector3(0, 180, 0));

                }
                else if (go.name.Contains("2"))//如果按钮名字包括“2”
                {
                    if (SetPoint(-20))//减少20个铜钱，然后创建远程防守单位
                        Defender.Create<Archer>(hitpos, new Vector3(0, 180, 0));
                }
            }
        }
        m_isSelectedButton = false;
    }

    [ContextMenu("BuildPath")]
    void BuildPath()
    {
        m_PathNodes = new List<PathNode>();
        GameObject[] objs = GameObject.FindGameObjectsWithTag("pathnode");
        for(int i = 0; i<objs.Length;i++)
        {
            m_PathNodes.Add(objs[i].GetComponent<PathNode>());
        }
    }

    void OnDrawGizmos()
    {
        if(!m_debug||m_PathNodes==null)
        {
            return;
        }
        Gizmos.color = Color.blue;  
        foreach (PathNode node in m_PathNodes)
        {
            if(node.m_next !=null)
            {
                Gizmos.DrawLine(node.transform.position, node.m_next.transform.position);
            }
        }
    }
}
