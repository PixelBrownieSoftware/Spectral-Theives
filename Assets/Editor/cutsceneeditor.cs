using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine;
using System.Threading.Tasks;
using MagnumFoudation;
using MagnumFoundationEditor;

public class ed_cutscene : MagnumFoundationEditor.ed_cutscene
{
    s_object objectItem;
    s_object[] objlist = null;
    o_character selectedCharacter = null;
    Vector2 scrollview;
    Vector2 scrollview2;    //Used for individual events 
    TextAsset utilityobj;
    TextAsset textobj;
    Vector2 mousepos;

    GameObject mouseArea;
    Vector2 dialogue_scroll;
    string label_text;
    List<string> locations;
    List<string> posLocation;
    int label_point;
    int leng;
    s_map eventMap;
    bool eventMapDisp = false;
    bool putlabel = false;

    Vector2 pos;
    Color evcolour; Event e;
    s_ninjaloader ed;

    Tool lasttool = Tool.None;

    //Pixel's Outstandingly Ultumate Cutscene Handler
    [MenuItem("Brownie/POUCH")]
    static void init()
    {
        GetWindow<ed_cutscene>("POUCH");
    }

    private void OnEnable()
    {
        if (mouseArea == null)
            mouseArea = GameObject.Find("Gizmo");

        SceneView.onSceneGUIDelegate += SceneGUI;
        lasttool = Tools.current;
    }

    void SceneGUI(SceneView sv)
    {
        e = Event.current;
        if (e.keyCode == KeyCode.S)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            pos = ray.origin;
        }
    }

    List<string> LocationsMap()
    {
        if (ed == null)
            return null;
        if (ed.jsonMaps == null)
            return null;
        List<TextAsset> te = ed.jsonMaps;

        List<string> maploc = new List<string>();
        for (int i = 0; i < te.Count; i++)
        {
            maploc.Add(te[i].name);
        }
        return maploc;
    }

    List<string> PositionsOnMap(string n)
    {
        List<string> maploc = new List<string>();
        TextAsset te = ed.jsonMaps.Find(x => x.name == n);

        s_map m = JsonUtility.FromJson<s_map>(te.text);

        for (int i = 0; i < m.tilesdata.Count; i++)
        {
            //print((s_map.s_mapteleport)mapdat.tilesdata[i]);
            switch (m.tilesdata[i].TYPENAME)
            {
                case "teleport_object":
                    maploc.Add(m.tilesdata[i].name);
                    break;
            }
        }
        return maploc;
    }

    Vector2 TeleporterPos(string n, string t)
    {
        List<string> maploc = new List<string>();
        TextAsset te = ed.jsonMaps.Find(x => x.name == n);

        s_map m = JsonUtility.FromJson<s_map>(te.text);

        s_map.s_tileobj til = m.tilesdata.Find(x => x.TYPENAME == "teleport_object" && x.name == t);
        if (til != null)
            return new Vector2(til.pos_x, til.pos_y);
        return new Vector2(0, 0);
    }

    bool FindLabel(int labelPt)
    {
        for (int l = 0; l < ed.mapDat.map_script_labels.Count; l++)
        {
            if (ed.mapDat.map_script_labels[l].index == labelPt)
            {
                return true;
            }
        }
        return false;
    }

    private new void OnGUI()
    {
        ed = GameObject.Find("General").GetComponent<s_ninjaloader>();
        base.OnGUI();
    }

    void WriteCode()
    {
        s_map map = ed.mapDat;
        string str = "";
        EditorGUILayout.Space();
        int ind = 0;
        foreach (ev_details d in map.Map_Script)
        {
            bool findLabel = FindLabel(ind);
            if (findLabel)
            {
                ev_label l = map.map_script_labels.Find(x => x.index == ind);
                EditorGUILayout.LabelField(l.name + ": \n");
            }
            ind++;
            EditorGUILayout.LabelField(ind + ". " + str + "\n");
        }
    }

    void SetStringToObjectName(ref ev_details det)
    {
        objectItem = (s_object)EditorGUILayout.ObjectField(objectItem, typeof(s_object), true);
        if (GUILayout.Button("Set string to object"))
        {
            if (objectItem != null)
                det.string0 = objectItem.name;
        }
    }

    public override void EnumChange(ref ev_details ev)
    {
        ev.eventType = (int)(MagnumFoudation.EVENT_TYPES)EditorGUILayout.EnumPopup((MagnumFoudation.EVENT_TYPES)ev.eventType);
    }
    /*
    void DrawDetailsTrigger(o_trigger trig)
    {
        int EVNUM = 0;
        ev_details[] details = trig.Events.ev_Details;
        for (int i = 0; i < details.Length; i++)
        {
            ev_details ev = details[i];
            if (ev != null)
            {
                EditorGUILayout.BeginHorizontal();
                foldoutlist[i] = EditorGUILayout.Foldout(foldoutlist[i], "Event Numeber: " + EVNUM, true);
                

                EditorGUILayout.EndHorizontal();
            }

            if (foldoutlist[i])
            {
                switch (ev.eventType)
                {


                }
                EditorGUILayout.Space();
            }
            EditorGUILayout.Space();
            EVNUM++;

        }
    }
    
    void LoadTempLevel(string dir)
    {
        s_leveledit ed = GameObject.Find("Main Camera").GetComponent<s_leveledit>();
        ed.LoadTempMap(ed.JsonToObj(dir));
    }
    */
    /*
    void DrawDetailsTrigger()
    {
        int EVNUM = 0;
        List<ev_label> labels = eventMap.map_script_labels;
        List<ev_details> details = eventMap.Map_Script;
        if (foldoutlist2 != null)
        {
            for (int i = 0; i < details.Count; i++)
            {
                ev_details ev = details[i];
                if (ev != null)
                {
                    bool findLabel = false;
                    ev_label lab = new ev_label(null, -1);
                    int l = 0;
                    for (l = 0; l < labels.Count; l++)
                    {
                        if (labels[l].index == i)
                        {
                            lab = labels[l];
                            findLabel = true;
                            break;
                        }
                    }
                    if (findLabel)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Label: ");
                        lab.name = EditorGUILayout.TextArea(lab.name);
                        labels[l] = lab;
                        EditorGUILayout.Space();
                        EditorGUILayout.EndHorizontal();
                    }

                    EditorGUILayout.BeginHorizontal();
                    //EditorGUILayout.LabelField("Event Numeber: " + EVNUM);
                    

                    foldoutlist2[i] = EditorGUILayout.Foldout(foldoutlist2[i], "Event Numeber: " + EVNUM, true);

                    #region REMOVE EVENT
                    if (GUILayout.Button("-"))
                    {
                        details.RemoveAt(i);
                        foldoutlist2 = new bool[details.Count];
                    }
                    #endregion

                    #region MOVE EVENT
                    if (0 < i)
                    {
                        if (GUILayout.Button("^"))
                        {
                            int index = i - 1; //For the index of the new object above
                            ev_details det = details[i];
                            details.RemoveAt(i);
                            details.Insert(index, det);
                            foldoutlist2 = new bool[details.Count];
                        }
                    }
                    if (details.Count > i + 1)
                    {
                        if (GUILayout.Button("v"))
                        {
                            int index = i + 1;
                            ev_details det = details[i];
                            details.RemoveAt(i);
                            details.Insert(index, det);
                            foldoutlist2 = new bool[details.Count];
                        }
                    }
                    #endregion

                    //ev.simultaneous = EditorGUILayout.Toggle("Simultaneous?", ev.simultaneous);

                    ev.eventType = (EVENT_TYPES)EditorGUILayout.EnumPopup(ev.eventType);

                    EditorGUILayout.EndHorizontal();
                }
                if (foldoutlist2[i])
                {
                    switch (ev.eventType)
                    {

                    }
                    EditorGUILayout.Space();
                }

                EVNUM++;

            }

        }
    }
    */
}