using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MagnumFoudation;

[CanEditMultipleObjects]
[CustomEditor(typeof(npc_guard))]
public class ed_guard : Editor
{
    public List<s_map.s_tileobj> nodes;

    void Start()
    {
        
    }
    
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GameObject obj = Selection.activeGameObject;
        if (obj != null)
        {
            npc_guard anim = obj.GetComponent<npc_guard>();
            if (anim != null)
            {
                anim.ID =  EditorGUILayout.TextField(anim.ID);
                anim.faction = EditorGUILayout.TextField(anim.faction);
                EditorGUILayout.LabelField("Teleport location");
                anim.teleportLoc = EditorGUILayout.TextField(anim.teleportLoc);
                anim.marchPoint = EditorGUILayout.IntField(anim.marchPoint);
                
                EditorGUILayout.LabelField("Pathfinding");
                if (anim.path != null)
                    for (int i = 0; i < anim.path.Count; i++)
                    {
                        EditorGUILayout.LabelField(anim.path[i].id + "");
                    }
                EditorGUILayout.LabelField("Path to follow");
                for (int i = 0; i < anim.marchDirection.Count; i++)
                {
                    int nod = anim.marchDirection[i];
                    if (GUILayout.Button("ID: " + nod))
                    {
                        anim.marchDirection.Remove(nod);
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
                    for (int i = 0; i < nodes.Count; i++)
                    {
                        s_map.s_tileobj nod = nodes[i];
                        if (nod.TYPENAME != "Node")
                            continue;
                        
                        if (GUILayout.Button(i + ""))
                        {
                            anim.marchDirection.Add(i);
                        }
                    }
                }
                EditorGUILayout.LabelField("Targets");
                if (anim.targets != null)
                    for (int i = 0; i < anim.targets.Count; i++)
                    {
                        EditorGUILayout.LabelField(anim.targets[i].name + "");
                    }

            }
        }
        Repaint();
    }
}
