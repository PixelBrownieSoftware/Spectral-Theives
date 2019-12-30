using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MagnumFoudation;

[CustomEditor(typeof(o_nodeobj))]
public class nodeobjEditor : Editor
{
    public List<s_map.s_tileobj> nodes;
    
    public override void OnInspectorGUI()
    {
        GameObject obj = Selection.activeGameObject;
        if (obj != null)
        {
            o_nodeobj anim = obj.GetComponent<o_nodeobj>();
            if (anim != null)
            {
                anim.nodeID = EditorGUILayout.IntField(anim.nodeID);
                EditorGUILayout.LabelField("Node neighbours");
                for(int i = 0; i < anim.nieghbours.Count; i++)
                {
                    o_nodeobj nod = anim.nieghbours[i];
                    if (nod == null)
                        continue;
                    if (GUILayout.Button(nod.ID + " " + nod.nodeID))
                    {
                        nod.nieghbours.Remove(anim);
                        anim.nieghbours.Remove(nod);
                    }
                }
                EditorGUILayout.LabelField("Nodes in map");
                if (nodes == null)
                {
                    s_ninjaloader nin = GameObject.Find("General").GetComponent<s_ninjaloader>();
                    s_ninjaloader.nl = nin;
                    nodes = nin.GetNodes();
                }
                else
                {
                    for(int i =0; i < nodes.Count; i++)
                    {
                        s_map.s_tileobj nod = nodes[i];
                        if (nod.TYPENAME != "Node")
                            continue;
                        if (i == anim.nodeID)
                            continue;
                        if (anim.nieghbours != null)
                        {
                            if (anim.nieghbours.Find(x => x.nodeID == i))
                                continue;
                        }
                        if (GUILayout.Button(i + ""))
                        {
                            o_nodeobj nud = s_ninjaloader.nl.GetNode(i);
                            anim.nieghbours.Add(nud);
                            nud.nieghbours.Add(anim);
                        }
                    }
                }

            }
        }
        Repaint();
    }
    
}