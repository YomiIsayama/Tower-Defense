using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public PathNode m_currentNode;
    public int m_life = 15;
    public int m_maxlife = 15;
    public float m_speed = 2;
    public System.Action<Enemy> onDeath;
    protected Transform m_lifebarObj;
    UnityEngine.UI.Slider m_lifebar;
    void Start()
    {
        SetUp();
    }
    public void SetUp()
    {
        //更新List中的敌人
        GameManager.Instance.m_EnemyList.Add(this);
        //读取生命条prefab
        GameObject prefab = (GameObject)Resources.Load("Canvas3D");
        //创建生命条
        m_lifebarObj = ((GameObject)Instantiate(prefab, Vector3.zero, Camera.main.transform.rotation, this.transform)).transform;
        m_lifebarObj.localPosition = new Vector3(0.0f, 140.0f, 30);
        m_lifebarObj.localScale = new Vector3(2.0f, 2.0f, 2.0f);
        m_lifebar = m_lifebarObj.GetComponentInChildren<UnityEngine.UI.Slider>();
        //更新生命条位置和角度
        StartCoroutine(UpDateLifebar());

    }
    IEnumerator UpDateLifebar()
    {
        //更新生命条的值
        m_lifebar.value = (float)m_life / (float)m_maxlife;
        //更新角度，如终面向摄像机
        m_lifebarObj.transform.eulerAngles = Camera.main.transform.eulerAngles;
        yield return 0; //没有任何等待
        StartCoroutine(UpDateLifebar());//执行循环
    }
    // Update is called once per frame
    void Update()
    {
        RotateTo();
        MoveTo();
    }

    public void RotateTo()
    {
        var position = m_currentNode.transform.position - transform.position;
        position.y = 0;
        var targetRotation = Quaternion.LookRotation(position);
        float next = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetRotation.eulerAngles.y, 120 * Time.deltaTime);
        this.transform.eulerAngles = new Vector3(0, next, 0);
    }

    public void MoveTo()
    {
        Vector3 pos1 = this.transform.position;
        Vector3 pos2 = m_currentNode.transform.position;
        float dist = Vector2.Distance(new Vector2(pos1.x, pos1.z), new Vector2(pos2.x, pos2.z));
        if (dist < 1.0f)
        {
            if(m_currentNode.m_next==null)
            {
                GameManager.Instance.SetDamage(1);
                DestroyMe();
            }
            else
            {
                m_currentNode = m_currentNode.m_next;
            }
        }
        this.transform.Translate(new Vector3(0, 0, m_speed * Time.deltaTime));
    }

    public void SetDamage(int damage)
    {
        m_life -= damage;
        if(m_life <0)
        {
            m_life = 0;
            GameManager.Instance.SetPoint(5);
            DestroyMe();
        }
    }


    public void DestroyMe()
    {
        GameManager.Instance.m_EnemyList.Remove(this);
        onDeath(this);
        Destroy(this.gameObject);
    }


}
