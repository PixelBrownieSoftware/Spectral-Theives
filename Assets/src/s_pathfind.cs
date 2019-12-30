using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class s_pathfind : MonoBehaviour
{
    public List<s_pathNode> nodes = new List<s_pathNode>();

    public void SetNodes(ref List<s_pathNode> nodes)
    {
        this.nodes = nodes;
    }
    public void ResetNodes()
    {
        foreach (s_pathNode no in nodes)
        {
            no.g_cost = 0;
            no.h_cost = 0;
            no.parent = null;
            no.list = 0;
            no.isgoal = false;
            no.isLight = false;
        }
    }

    void Start()
    {
        
    }
    
    void Update()
    {

    }
    private void OnDrawGizmos()
    {
        /*
        foreach (s_pathNode nod in nodes)
        {
            Vector3 nodepos = new Vector3(nod.position.x, nod.position.y);

            if (nod.isgoal)
                Handles.Label(nodepos, "Goal");

            if (nod.parent != null)
                Handles.Label(nodepos, "Step: " + nod.list);
            Handles.Label(nodepos, "ID: " + nod.id);

            Gizmos.DrawSphere(nodepos, 20);
            if (nod.neighbours != null)
            {
                foreach (int idN in nod.neighbours)
                {
                    s_pathNode neighbour = nodes.Find(x => x.id == idN);
                    Vector3 neighPos = new Vector3(neighbour.position.x, neighbour.position.y);
                    Gizmos.DrawLine(nodepos, neighPos);
                }
            }
        }
        */
    }

    public s_pathNode FindPathNode(int id)
    {
        foreach (s_pathNode p in nodes)
        {
            if (p.id == id)
                return p;
        }
        return null;
    }

    public Tuple< List<s_pathNode>, float> PathFind(Vector2 start, Vector2 end)
    {
        ResetNodes();
        int round = 0;
        List<s_pathNode> openList = new List<s_pathNode>();
        HashSet<s_pathNode> closedList = new HashSet<s_pathNode>();

        s_pathNode startNode = FindNodeAtPosition(start);
        s_pathNode goal = FindNodeAtPosition(end);

        if(goal == null)
            return null;

        goal.isgoal = true;

        openList.Add(startNode);

        while (openList.Count > 0)
        {
            if (round == 200)
                return null;

            s_pathNode current = openList[0];
            for (int i = 1; i < openList.Count; i++)
            {
                s_pathNode pa = openList[i];
                if (pa.f_cost < current.f_cost)
                {
                    //FindPathNode(pa.id)
                    current = FindNodeAtPosition(pa.position);
                }
            }

            if (current.id == goal.id)
            {
                List<s_pathNode> pathList = new List<s_pathNode>();
                s_pathNode path = goal;
                int l = 0;
                while (path != startNode)
                {
                    path.list = l;
                    pathList.Add(path);
                    l++;
                    path = path.parent;
                }
                pathList.Reverse();
                return new Tuple<List<s_pathNode>, float>(pathList, current.f_cost);
            }

            openList.Remove(current);
            closedList.Add(current);
            /*
            if (current.neighbours == null)
                continue;
            */

            foreach (int n in current.neighbours)
            {
                s_pathNode no = nodes.Find(x => x.id == n);
                float g_cost = current.g_cost + Vector2.Distance(no.position, current.position);
                float h_cost = current.h_cost + Vector2.Distance(no.position, goal.position);

                if (closedList.Contains(no))
                    continue;

                if (!openList.Contains(no) || g_cost < no.g_cost)
                {
                    no.g_cost = g_cost;
                    no.h_cost = h_cost;
                    no.parent = current;

                    if (!openList.Contains(no))
                        openList.Add(no);
                }
            }

            round++;
        }
        return null;
    }
    public Tuple<List<s_pathNode>, float> PathFind(Vector2 start, s_pathNode end)
    {
        ResetNodes();
        int round = 0;
        List<s_pathNode> openList = new List<s_pathNode>();
        HashSet<s_pathNode> closedList = new HashSet<s_pathNode>();

        s_pathNode startNode = FindNodeAtPosition(start);
        s_pathNode goal = end;

        if (goal == null)
            return null;

        goal.isgoal = true;

        openList.Add(startNode);

        while (openList.Count > 0)
        {
            if (round == 200)
                return null;

            s_pathNode current = openList[0];
            for (int i = 1; i < openList.Count; i++)
            {
                s_pathNode pa = openList[i];
                if (pa.f_cost < current.f_cost)
                {
                    //FindPathNode(pa.id)
                    current = FindNodeAtPosition(pa.position);
                }
            }

            if (current.id == goal.id)
            {
                List<s_pathNode> pathList = new List<s_pathNode>();
                s_pathNode path = goal;
                int l = 0;
                while (path != startNode)
                {
                    path.list = l;
                    pathList.Add(path);
                    l++;
                    path = path.parent;
                }
                pathList.Reverse();
                return new Tuple<List<s_pathNode>, float>(pathList, current.f_cost);
            }

            openList.Remove(current);
            closedList.Add(current);
            /*
            if (current.neighbours == null)
                continue;
            */

            foreach (int n in current.neighbours)
            {
                s_pathNode no = nodes.Find(x => x.id == n);
                float g_cost = current.g_cost + Vector2.Distance(no.position, current.position);
                float h_cost = current.h_cost + Vector2.Distance(no.position, goal.position);

                if (closedList.Contains(no))
                    continue;

                if (!openList.Contains(no) || g_cost < no.g_cost)
                {
                    no.g_cost = g_cost;
                    no.h_cost = h_cost;
                    no.parent = current;

                    if (!openList.Contains(no))
                        openList.Add(no);
                }
            }

            round++;
        }
        return null;
    }

    public s_pathNode FindNodeAtPosition(Vector2 pos)
    {
        s_pathNode returnNode = null;
        float smallest = float.MaxValue;
        for (int i = 0; i < nodes.Count; i++)
        {
            s_pathNode p = nodes[i];
            Vector2 nodepos = new Vector2(p.position.x, p.position.y);
            float dist = Vector2.Distance(nodepos, pos);
            if (dist < smallest)
            {
                returnNode = p;
                smallest = dist;
            }
        }
        return returnNode;
    }
}
