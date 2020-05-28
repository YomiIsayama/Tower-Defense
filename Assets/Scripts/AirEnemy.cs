using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirEnemy : Enemy
{
    // Start is called before the first frame update
    void Start()
    {
        SetUp();
        m_lifebarObj.localPosition = new Vector3(0, 2.0f, 0);
        m_lifebarObj.localScale = new Vector3(0.02f, 0.02f, 0.02f);
    }

    // Update is called once per frame
    void Update()
    {
        RotateTo();
        MoveTo();
        Fly();
    }

    public void Fly()
    {
        float flyspeed = 0;
        if (this.transform.position.y<2.0)
        {
            flyspeed = 1.0f;
        }
        this.transform.Translate(new Vector3(0, flyspeed * Time.deltaTime, 0));
    }
}
