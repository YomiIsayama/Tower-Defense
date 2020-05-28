using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(TileObject))]
public class TileEditor : Editor
{
    protected bool editMode = false;

    protected TileObject tileObject;

    void OnEnable()
    {
        tileObject = (TileObject)target;
    }

    public void OnSceneGUI()
    {
        if(editMode)
        {
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
            tileObject.debug = true;
            Event e = Event.current;

            if (e.button == 0 && (e.type == EventType.MouseDown || e.type == EventType.MouseDrag) && !e.alt)
            {
                Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);

                RaycastHit hitinfo;
                if(Physics.Raycast(ray,out hitinfo,2000,tileObject.tileLayer))
                {
                    tileObject.setDataFromPosition(hitinfo.point.x, hitinfo.point.z, tileObject.dataID);
                }
            }
        }
        HandleUtility.Repaint();
    }

    public override void OnInspectorGUI()
    {
        GUILayout.Label("Tile Editor");
        editMode = EditorGUILayout.Toggle("Edit", editMode);
        tileObject.debug = EditorGUILayout.Toggle("Debug", tileObject.debug);

        string[] editDataStr = { "Dead", "Road", "Guard" };
        tileObject.dataID = GUILayout.Toolbar(tileObject.dataID, editDataStr);

        EditorGUILayout.Separator();

        if (GUILayout.Button("Reset"))
        {
            tileObject.Reset();
        }
        DrawDefaultInspector();
    }
}
