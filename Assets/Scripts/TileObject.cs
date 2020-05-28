using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileObject : MonoBehaviour
{
    public static TileObject Instance = null;

    public LayerMask tileLayer;
    public float tileSize = 1;
    public int xTileCount = 2;
    public int zTileCount = 2;
    public int[] data;

    [HideInInspector]
    public int dataID = 0;
    [HideInInspector]
    public bool debug = false;

    void Awake()
    {
        Instance = this;
    }

    public void Reset()
    {
        data = new int[xTileCount * zTileCount];
    }

    public int getDataFromPosition(float pox, float poz)
    {
        int index = (int)((pox - transform.position.x) / tileSize) * zTileCount + (int)((poz - transform.position.z) / tileSize);
        if (index < 0 || index >= data.Length)
        {
            return 0;
        }
        return data[index];
    }
    public void setDataFromPosition(float pox, float poz,int number)
    {
        int index = (int)((pox - transform.position.x) / tileSize) * zTileCount + (int)((poz - transform.position.z) / tileSize);
        if (index < 0 || index >= data.Length)
        {
            return;
        }
        data[index]=number;
    }

    void OnDrawGizmos()
    {
        if(!debug)
        {
            return;
        }
        if(data==null)
        {
            Debug.Log("please reset data first");
            return;
        }

        Vector3 pos = transform.position;

        for (int i = 0; i < xTileCount; i++)
        {
            Gizmos.color = new Color(0, 0, 1, 1);
            Gizmos.DrawLine(pos + new Vector3(tileSize * i, pos.y, 0), transform.transform.TransformPoint(tileSize * i, pos.y, tileSize * zTileCount));
        
            for(int k =0; k < zTileCount; k++)
            {
                if ((i * zTileCount + k) < data.Length && data[i * zTileCount + k] == dataID) 
                {
                    Gizmos.color = new Color(1, 0, 0, 0.3f);
                    Gizmos.DrawCube(new Vector3(pos.x + i * tileSize + tileSize * 0.5f, pos.y, pos.z + k * tileSize + tileSize * 0.5f), new Vector3(tileSize, 0.2f, tileSize));
                }
            }

        }

        for (int k = 0; k < zTileCount; k++)
        {
            Gizmos.color = new Color(0, 0, 1, 1);
            Gizmos.DrawLine(pos + new Vector3(tileSize * k, pos.y, 0), transform.transform.TransformPoint(tileSize * k, pos.y, tileSize * xTileCount));
        }
    }
}
