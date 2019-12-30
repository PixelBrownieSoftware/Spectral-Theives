using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagnumFoudation;
using UnityEditor;

public class o_nodeobj : o_generic
{
    public int nodeID;
    public List<o_nodeobj> nieghbours = new List<o_nodeobj>();

    private void OnDrawGizmos()
    {
        foreach (o_nodeobj nod in nieghbours)
        {
            if(nod != null)
                Gizmos.DrawLine(transform.position, nod.transform.position);
        }

#if UNITY_EDITOR_WIN
        if (Application.isEditor)
            Handles.Label(transform.position, "Node ID: " + nodeID);
#endif
    }
}
