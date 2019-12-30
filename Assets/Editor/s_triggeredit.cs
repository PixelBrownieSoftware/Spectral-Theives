using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MagnumFoudation;

[CustomEditor(typeof(o_trigger))]
[CanEditMultipleObjects]
public class s_triggeredit : Editor
{
    s_ninjaloader mapdat;

    public override void OnInspectorGUI()
    {
        if (mapdat == null)
            mapdat = GameObject.Find("General").GetComponent<s_ninjaloader>();
        o_trigger tra = (o_trigger)target;

        base.OnInspectorGUI();

        for (int i = 0; i < Labelmap().Count; i++)
        {
            if (GUILayout.Button(Labelmap()[i].Item1))
            {
                tra.LabelToJumpTo = Labelmap()[i].Item2;
            }
        }
    }

    List<Tuple<string, int>> Labelmap()
    {
        List<MagnumFoudation.ev_details> te = mapdat.mapDat.Map_Script;

        List<Tuple<string, int>> maploc = new List<Tuple<string, int>>();
        for (int i = 0; i < te.Count; i++)
        {
            if (te[i].eventType == -1)
                maploc.Add(new Tuple<string, int>(te[i].string0, i));
        }
        return maploc;
    }

    List<string> LocationsMap()
    {
        //TODO: GET MAPS THEMSELVES INSTEAD
        List<TextAsset> te = mapdat.jsonMaps;

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
        TextAsset te = mapdat.jsonMaps.Find(x => x.name == n);

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
}
