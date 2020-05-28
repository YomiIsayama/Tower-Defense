using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TileWnd : EditorWindow
{
    protected static TileObject tileObject;

    [MenuItem("Tools/Tile Window")]
    static void Create()
    {
        EditorWindow.GetWindow(typeof(TileWnd));

        if(Selection.activeTransform!=null)
        {
            tileObject = Selection.activeTransform.GetComponent<TileObject>();
        }
    }

    void OnSelectionChange()
    {
        if(Selection.activeTransform!=null)
        {
            tileObject = Selection.activeTransform.GetComponent<TileObject>();
        }

        
    }

    void OnGUI()
    {
        if (tileObject == null)
        {
            return;
        }
        GUILayout.Label("Tile Editor");
        var tex = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/GUI/butPlayer1.png");
        GUILayout.Label(tex);
        tileObject.debug = EditorGUILayout.Toggle("Debug", tileObject.debug);

        string[] editDataStr = { "Dead", "Road", "Guard" };
        tileObject.dataID = GUILayout.Toolbar(tileObject.dataID, editDataStr);
        EditorGUILayout.Separator();

        if (GUILayout.Button("Reset"))
        {
            tileObject.Reset();
        }
    }
}
