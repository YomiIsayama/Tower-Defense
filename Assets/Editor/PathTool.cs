using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PathTool : MonoBehaviour
{
    public static PathNode m_parent = null;

    [MenuItem("PathTool/Create PathNode")]
    static void CreatPathNode()
    {
        GameObject go = new GameObject();
        go.AddComponent<PathNode>();
        go.name = "pathnode";
        go.tag = "pathnode";
        Selection.activeTransform = go.transform;
    }


    [MenuItem("PathTool/Set Parent Wq")]
    static void SetParent()
    {
        if(!Selection.activeGameObject||Selection.GetTransforms(SelectionMode.Unfiltered).Length>1)
        {
            return;
        }
        if(Selection.activeGameObject.tag.CompareTo("pathnode")==0)
        {
            m_parent = Selection.activeGameObject.GetComponent<PathNode>();
            Debug.Log("set" + m_parent.name + "as parent");
        }
    }

    [MenuItem("PathTool/Set Next Wq")]
    static void SetNextChild()
    {
        if(!Selection.activeGameObject||m_parent == null || Selection.GetTransforms(SelectionMode.Unfiltered).Length>1)
        {
            return;
        }
        if (Selection.activeGameObject.tag.CompareTo("pathnode") == 0)
        {
            m_parent.SetNext(Selection.activeGameObject.GetComponent<PathNode>());
            m_parent = null;

            Debug.Log("set" + Selection.activeGameObject + "as child");
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
