using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defender : MonoBehaviour
{
    public enum TileStatus
    {
        DEAD = 0,
        ROAD = 1,
        GUARD = 2,
    }

    public float m_attackArea = 2.0f;
    public int m_power = 1;
    public float m_attackInterval = 2.0f;
    protected Enemy m_targetEnemy;
    protected bool m_isFaceEnemy;
    protected GameObject m_model;
    protected Animator m_ani;

    public static T Create<T> (Vector3 pos,Vector3 angle) where T : Defender
    {
        GameObject go = new GameObject("defender");
        go.transform.position = pos;
        go.transform.eulerAngles = angle;
        T d = go.AddComponent<T>();
        d.Init();
        return d;
    }

    protected virtual void Init()
    {
        m_attackArea = 2.0f;
        m_power = 2;
        m_attackInterval = 2.0f;
        CreateModel("swordman");
        StartCoroutine(Attack());
    }

    protected virtual void CreateModel(string myname)
    {
        GameObject model = Resources.Load<GameObject>(myname);
        m_model = (GameObject)Instantiate(model, this.transform.position, this.transform.rotation, this.transform);
        m_ani = m_model.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        FindEnemy();
        RotateTo();
        Attack();
    }

    public void RotateTo()
    {
        if(m_targetEnemy == null)
        {
            return;
        }
        var targetdir = m_targetEnemy.transform.position - transform.position;
        targetdir.y = 0;
        Vector3 rot_delta = Vector3.RotateTowards(this.transform.forward, targetdir, 20.0f * Time.deltaTime, 0.0f);
        Quaternion targetrotaion = Quaternion.LookRotation(rot_delta);

        float angle = Vector3.Angle(targetdir, transform.forward);

        if(angle<1.0f)
        {
            m_isFaceEnemy = true;
        }
        else
        {
            m_isFaceEnemy = false;
        }
        transform.rotation = targetrotaion;
    }

    void FindEnemy()
    {
        if(m_targetEnemy!=null)
        {
            return;
        }
        m_targetEnemy = null;
        int minlife = 0;
        foreach (Enemy enemy in GameManager.Instance.m_EnemyList)
        {
            if (enemy.m_life == 0)
            {
                continue;
            }
            Vector3 pos1 = this.transform.position;
            pos1.y = 0;
            Vector3 pos2 = enemy.transform.position;
            pos2.y = 0;
            float dist = Vector3.Distance(pos1, pos2);

            if (dist > m_attackArea)
            {
                continue;
            }

            if(minlife == 0 || minlife > enemy.m_life)
            {
                m_targetEnemy = enemy;
                minlife = enemy.m_life;
            }

        }
    }

    protected virtual IEnumerator Attack()
    {
        while(m_targetEnemy == null || !m_isFaceEnemy)// if null target , Keep on standby
        {
            yield return 0;
        }
        m_ani.CrossFade("attack", 0.1f); // play the animation "attack"

        while(!m_ani.GetCurrentAnimatorStateInfo(0).IsName("attack")) // if not in attack state, wait to enter state
        {
            yield return 0;
        }
        float ani_leght = m_ani.GetCurrentAnimatorStateInfo(0).length;// get animation play time
        yield return new WaitForSeconds(ani_leght * 0.5f);// wait for play half of animation
        if (m_targetEnemy != null)
        {
            m_targetEnemy.SetDamage(m_power);
        }
        yield return new WaitForSeconds(ani_leght * 0.5f);//wait for play end
        m_ani.CrossFade("idle", 0.1f);

        yield return new WaitForSeconds(m_attackInterval);
        StartCoroutine(Attack());
    }
}
