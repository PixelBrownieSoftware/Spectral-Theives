using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagnumFoudation;
using UnityEngine.Tilemaps;

public class s_ninjaloader : s_levelloader
{
    public Canvas canvas;
    public List<s_pathNode> pathNodes = new List<s_pathNode>();
    public List<o_nodeobj> debugNodes = new List<o_nodeobj>();
    public static s_ninjaloader nl;
    public static s_pathfind pathfind;

    void Awake()
    {
        canvas.gameObject.SetActive(true);
        pathfind = GetComponent<s_pathfind>();
        nl = this;
        InEditor = false;
        InitializeLoader();
        InitializeGameWorld();
        colmp.color = Color.clear;
        foreach (KeyValuePair<string, int> v in s_globals.GlobalFlags)
        {
            s_globals.SetGlobalFlag(v.Key, 0);
        }
        //LoadMap(maps[0]);
    }

    public override void NewMap()
    {
        base.NewMap();
        pathNodes.Clear();
    }

    public List<s_map.s_tileobj> GetNodes()
    {
        if (nl == null)
            nl = this;
        s_object[] tiles = GetTileObjects();
        List<s_map.s_tileobj> t = new List<s_map.s_tileobj>();
        int nodeID = 0;
        foreach (s_object c in tiles)
        {
            s_map.s_tileobj til = new s_map.s_tileobj(c.ID);
            til.pos_x = (int)c.transform.position.x;
            til.pos_y = (int)c.transform.position.y;

            switch (c.ID)
            {
                case "Node":
                    o_nodeobj nod = c.GetComponent<o_nodeobj>();
                    if (nod.nieghbours != null)
                    {
                        foreach (o_nodeobj no in nod.nieghbours)
                        {
                            if (no == null)
                                continue;
                            if(til.CustomTypes != null)
                                til.CustomTypes.Add(new s_map.s_customType("node", no.nodeID));
                        }
                    }
                    t.Add(til);
                    nodeID++;
                    continue;
                default:
                    continue;
            }
        }
        return t;
    }

    public override List<s_map.s_tileobj> GetTiles(s_object[] tiles)
    {
        List<s_map.s_tileobj> t = new List<s_map.s_tileobj>();
        int nodeID = 0;
        foreach (s_object c in tiles)
        {
            s_map.s_tileobj til = new s_map.s_tileobj(c.ID);
            til.pos_x = (int)c.transform.position.x;
            til.pos_y = (int)c.transform.position.y;
            til.name = c.name;

            switch (c.ID)
            {
                case "Node":
                    o_nodeobj nod = c.GetComponent<o_nodeobj>();

                    if (til.CustomTypes == null)
                        til.CustomTypes = new List<s_map.s_customType>();

                    til.CustomTypes.Add(new s_map.s_customType("nodeID", nod.nodeID));
                    foreach (o_nodeobj no in nod.nieghbours)
                    {
                        if (no == null)
                            continue;
                        if (til.CustomTypes != null)
                        {
                            //print("node " + no.nodeID);
                            til.CustomTypes.Add(new s_map.s_customType("node", no.nodeID));
                        }
                        else {
                            til.CustomTypes = new List<s_map.s_customType>();
                            //print("node " + no.nodeID);
                            til.CustomTypes.Add(new s_map.s_customType("node", no.nodeID));
                        }
                    }
                    t.Add(til);
                    nodeID++;
                    continue;

                default:
                    t.Add(til);
                    break;
            }
        }
        return t;
    }
    public override s_map.s_trig AddTrigger(o_trigger obj)
    {
        return new s_map.s_trig(obj.transform.localScale);
    }
    public override List<s_map.s_trig> GetTriggers(s_object[] triggers)
    {
        List<s_map.s_trig> t = new List<s_map.s_trig>();
        foreach (o_trigger c in triggers)
        {
            s_map.s_trig tr = new s_map.s_trig(c.ID, c.transform.position);
            tr.name = c.name;
            tr.labelToJumpTo = c.LabelToJumpTo;
            tr.trigtye = c.TRIGGER_T;
            t.Add(tr);
        }
        return t;
    }
    public override void SetEntities(List<s_map.s_chara> characters)
    {
        foreach (s_map.s_chara c in characters)
        {
            if (InEditor)
            {
                if (c.charType == "guard")
                {
                    npc_guard guar = Instantiate<npc_guard>(FindOBJ<npc_guard>("guard"), new Vector3(c.pos_x, c.pos_y), Quaternion.identity);
                    guar.name = c.charname;
                    guar.marchDirection.Clear();
                    List<s_map.s_customType> custom = c.CustomTypes.FindAll(x => x.name == "node");
                    s_map.s_customType location = c.CustomTypes.Find(x => x.name == "location");
                    guar.teleportLoc = location.type3;

                    foreach (s_map.s_customType cu in custom)
                    {
                        guar.marchDirection.Add(cu.type);
                    }
                    guar.transform.SetParent(entitiesObj.transform);
                }
            }
            else
            {
                if (c.charType == "guard")
                {
                    npc_guard guar = SpawnObject<npc_guard>("guard", new Vector3(c.pos_x, c.pos_y), Quaternion.identity);
                    guar.name = c.charname;
                    guar.marchDirection.Clear();
                    if (guar.path != null)
                        guar.path.Clear();
                    guar.marchPoint = 0;
                    guar.state = npc_guard.AI_STATE.IDLE;
                    List<s_map.s_customType> custom = c.CustomTypes.FindAll(x => x.name == "node");
                    s_map.s_customType location = c.CustomTypes.Find(x => x.name == "location");
                    guar.teleportLoc = location.type3;
                    guar.marchDirection = new List<int>();
                    foreach (s_map.s_customType cu in custom)
                    {
                        guar.marchDirection.Add(cu.type);
                    }
                    guar.transform.SetParent(entitiesObj.transform);
                    allcharacters.Add(guar);
                }
            }
        }
    }
    public override List<s_map.s_chara> GetEntities(o_character[] characters)
    {
        List<s_map.s_chara> t = new List<s_map.s_chara>();
        foreach (o_character c in characters)
        {
            s_map.s_chara ch = new s_map.s_chara(c.transform.position, "", c.name, c.ID, true, true, true, "fjks");
            npc_guard g = c.GetComponent<npc_guard>();
            for (int i = 0; i < g.marchDirection.Count; i++)
            {
                ch.AddCustomTag("node", g.marchDirection[i]);
                ch.AddCustomTag("location", g.teleportLoc);
            }
            t.Add(ch);
        }
        return t;
    }
    /*
    public override GameObject SetTrigger(s_map.s_trig trigger)
    {
        GameObject trigObj = null;
        Vector2 pos = new Vector2(trigger.pos_x, trigger.pos_y);

        trigObj = SpawnObject<s_object>(FindOBJ("trigger"), pos, Quaternion.identity).gameObject;
        trigObj.transform.SetParent(triggersObj.transform);
        o_trigger trig = trigObj.GetComponent<o_trigger>();
        s_utility util = null;
        if (trig == null)
            util = trigObj.GetComponent<s_utility>();
        
        if (trig != null)
        {
            trig.ev_num = 0;    //TODO: IF THE TRIGGER DOES NOT STATICALLY STORE ITS EVENT NUMBER, SET IT TO 0

            if (trigger.name != "")
                trig.name = trigger.name;
            trig.isactive = false;
            trig.TRIGGER_T = trigger.trigtye;
            trig.LabelToJumpTo = trigger.labelToJumpTo;
            //trig.TRIGGER_T = mapdat.triggerdata[i].trigtye;

            s_save_vector ve = trigger.trigSize;

            trig.GetComponent<BoxCollider2D>().size = new Vector2(ve.x, ve.y);

            trig.transform.SetParent(trig.transform);
        }
        else if (util != null)
        {
            if (trigger.name != "")
                util.name = trigger.name;
            util.istriggered = false;
            //trig.TRIGGER_T = mapdat.triggerdata[i].trigtye;

            s_save_vector ve = trigger.trigSize;

            if (trigger.util != "u_boundary")
                util.GetComponent<BoxCollider2D>().size = new Vector2(ve.x, ve.y);

            util.transform.SetParent(util.transform);
        }
        return trigObj;
    }
    */
    public override GameObject SetTrigger(s_map.s_trig trigger)
    {
        GameObject trigObj = null;
        Vector2 pos = new Vector2(trigger.pos_x, trigger.pos_y);

        if (!InEditor)
            trigObj = SpawnObject<s_object>("trigger", pos, Quaternion.identity).gameObject;
        else
            trigObj = Instantiate(FindOBJ("trigger"), new Vector3(trigger.pos_x, trigger.pos_y), Quaternion.identity);

        trigObj.transform.SetParent(triggersObj.transform);
        o_trigger trig = trigObj.GetComponent<o_trigger>();
        
        if (trig != null)
        {
            if (trigger.name != "")
                trig.name = trigger.name;
            trig.isactive = false;
            trig.TRIGGER_T = trigger.trigtye;
            trig.LabelToJumpTo = trigger.labelToJumpTo;

            s_save_vector ve = trigger.trigSize;

            trig.GetComponent<BoxCollider2D>().size = new Vector2(ve.x, ve.y);

            trig.transform.SetParent(trig.transform);
        }
        return trigObj;
    }
    
    public override List<s_save_item> GetItems(o_itemObj[] itemsInMap)
    {
        return new List<s_save_item>();
    }

    public override void GetTileDat(ref s_map mapfil)
    {
        Tile[] tiles = new Tile[(int)mapsizeToKeep.x * (int)mapsizeToKeep.y];
        Tile[] colTiles = new Tile[(int)mapsizeToKeep.x * (int)mapsizeToKeep.y];
        Vector2Int vec = new Vector2Int(0, 0);

        for (int x = 0; x < mapsizeToKeep.x; x++)
        {
            for (int y = 0; y < mapsizeToKeep.y; y++)
            {
                Tile coltil = colmp.GetTile<Tile>(new Vector3Int(x, y, 0));
                if (coltil != null)
                {
                    colTiles[(int)(x + (mapsizeToKeep.x * y))] = coltil;
                    if (colTiles[(int)(x + (mapsizeToKeep.x * y))] != null)
                    {
                        string tileName = coltil.name;
                        s_map.s_tileobj tilo = new s_map.s_tileobj(new Vector2(x * 20, y * 20), null);
                        tilo.name = tileName;

                        mapfil.tilesdata.Add(tilo);
                    }

                }

                Tile til = tm.GetTile<Tile>(new Vector3Int(x, y, 0));
                if (til != null)
                {
                    mapfil.graphicTiles.Add(
                               new s_map.s_block(til.sprite.name,
                               new Vector2(x * 20, y * 20)));
                }

                Tile tilmid = tm2.GetTile<Tile>(new Vector3Int(x, y, 0));
                if (tilmid != null)
                {
                    mapfil.graphicTilesMiddle.Add(
                               new s_map.s_block(tilmid.sprite.name,
                               new Vector2(x * 20, y * 20)));
                }

                Tile tiltop = tm3.GetTile<Tile>(new Vector3Int(x, y, 0));
                if (tiltop != null)
                {
                    mapfil.graphicTilesTop.Add(
                               new s_map.s_block(tiltop.sprite.name,
                               new Vector2(x * 20, y * 20)));
                }
            }
        }
    }

    public override void SetTileMap(s_map mapdat)
    {
        pathNodes.Clear();
        debugNodes.Clear();

        s_map mp = mapdat;
        List<s_map.s_block> tile = mp.graphicTiles;
        List<s_map.s_block> tileMid = mp.graphicTilesMiddle;
        List<s_map.s_block> tileTop = mp.graphicTilesTop;
        List<s_map.s_tileobj> coll = mp.tilesdata;
        // base.SetTileMap(mapdat);
        /*
        */
        foreach (s_map.s_block b in tile)
        {
            tm.SetTile(new Vector3Int((int)b.position.x / 20, (int)b.position.y / 20, 0), tilesNew.Find(ti => ti.name == b.sprite));
        }
        foreach (s_map.s_block b in tileMid)
        {
            tm2.SetTile(new Vector3Int((int)b.position.x / 20, (int)b.position.y / 20, 0), tilesNew.Find(ti => ti.name == b.sprite));
        }
        foreach (s_map.s_block b in tileTop)
        {
            tm3.SetTile(new Vector3Int((int)b.position.x / 20, (int)b.position.y / 20, 0), tilesNew.Find(ti => ti.name == b.sprite));
        }

        int nodeID = 0;
        for (int i = 0; i < coll.Count; i++)
        {
            s_map.s_tileobj b = coll[i];
            string tilename = "";
            COLLISION_T tileType = (COLLISION_T)b.enumthing;
            Tile t = null;
            s_pathNode nod2 = new s_pathNode();
            GameObject go = null;
            
            if (InEditor)
            {
                switch (b.TYPENAME)
                {
                    case "teleport_object":
                    case "keyObj":
                        go = Instantiate(
                        FindOBJ(b.TYPENAME),
                        new Vector3(b.pos_x, b.pos_y),
                        Quaternion.identity);
                        if (go != null)
                        {
                            go.transform.SetParent(tilesObj.transform);
                            go.name = b.name;
                        }
                        break;

                    case "Node":
                        go = Instantiate(FindOBJ(b.TYPENAME),
                    new Vector3(b.pos_x, b.pos_y),
                    Quaternion.identity);
                        if (go != null)
                        {
                            go.transform.SetParent(tilesObj.transform);
                            go.name = b.name;
                        }

                        o_nodeobj nod = go.GetComponent<o_nodeobj>();

                        nod.nodeID = nodeID;
                        nod.name = "Node_" + nod.nodeID;
                        nod.transform.position = new Vector3(b.pos_x, b.pos_y);
                        nod.transform.SetParent(tilesObj.transform);
                        debugNodes.Add(nod);

                        /*
                        o_nodeobj nod = go.GetComponent<o_nodeobj>();
                        nod.nodeID = nodeID;
                        nod.name = "Node_" + nod.nodeID;
                        debugNodes.Add(nod);
                        */

                        nod2 = new s_pathNode();
                        nod2.position = new Vector2(b.pos_x, b.pos_y);
                        if (b.CustomTypes.Find(x => x.name == "nodeID").name == "")
                            nod2.id = nodeID;
                        else
                            nod2.id = b.CustomTypes.Find(x => x.name == "nodeID").type;
                        pathNodes.Add(nod2);
                        nodeID++;
                        break;
                }
                /*
                switch (tileType)
                {
                    case COLLISION_T.WALL:
                        if (b.TYPENAME == "teleport_object")
                            continue;
                        colmp.SetTile(new Vector3Int(b.pos_x / 20, b.pos_y / 20, 0), collisionTile);
                        break;
                }
                */
            }
            else
            {
                switch (b.TYPENAME)
                {
                    case "teleport_object":
                        continue;

                    case "keyObj":
                        SpawnObject<o_generic>("keyObj", new Vector2(b.pos_x, b.pos_y), Quaternion.identity);
                        continue;

                    case "Node":
                        nod2 = new s_pathNode();
                        nod2.position = new Vector2(b.pos_x, b.pos_y);
                        nod2.id = nodeID;
                        pathNodes.Add(nod2);
                        nodeID++;
                        break;
                }
            }
            tilename = b.name;
            if (tilename != "")
                colmp.SetTile(new Vector3Int(b.pos_x / 20, b.pos_y / 20, 0), collisionList.Find(ti => ti.name == tilename));
            else
                colmp.SetTile(new Vector3Int(b.pos_x / 20, b.pos_y / 20, 0), t);

        }

        for (int i = 0; i < pathNodes.Count; i++)
        {
            s_pathNode p = pathNodes[i];
            List<s_pathNode> nbrs = new List<s_pathNode>();
            List<int> GetNeighbours = new List<int>();

            s_map.s_tileobj t = coll[p.id];

            //Gets all the neighbours
            foreach (s_map.s_customType no in t.CustomTypes)
            {
                //Find all the custom values with "node" which resemble the neighbours
                if (no.name == "node")
                {
                    s_pathNode path = pathNodes.Find(x => x.id == no.type);
                    nbrs.Add(path);
                }
            }
            if (InEditor)
            {
                o_nodeobj currentNode = debugNodes.Find(x => x.nodeID == p.id);
                foreach (s_pathNode n in nbrs)
                {
                    o_nodeobj nod = debugNodes.Find(x => x.nodeID == n.id);
                    if (nod != null)
                    {
                        currentNode.nieghbours.Add(nod);
                        GetNeighbours.Add(n.id);

                        //nod.nieghbours.Add(currentNode);
                        if (!currentNode.nieghbours.Find(x => nod))
                        {
                        }
                    }
                }
                foreach (s_pathNode n in nbrs)
                {
                    GetNeighbours.Add(n.id);
                }
                p = new s_pathNode(p.id, new Vector2(p.position.x, p.position.y), GetNeighbours);
                pathNodes[i] = p;
            }
            else
            {
                foreach (s_pathNode n in nbrs)
                {
                    GetNeighbours.Add(n.id);
                }
                p = new s_pathNode(p.id, new Vector2(p.position.x, p.position.y), GetNeighbours);
                pathNodes[i] = p;
            }
        }
        pathfind = GetComponent<s_pathfind>();
        pathfind.SetNodes(ref pathNodes);
    }

    public o_nodeobj GetNode(int id)
    {
        s_object[] objs = GetTileObjects();
        foreach (s_object obj in objs)
        {
            o_nodeobj nod = obj.GetComponent<o_nodeobj>();
            if (nod != null)
            {
                if (nod.nodeID == id)
                    return nod;
            }
            else
            {
                continue;
            }
        }
        return null;
    }

    new private void OnDrawGizmos()
    {
        /*
#if UNITY_EDITOR_WIN
        foreach (s_pathNode pn in pathNodes)
        {
            if (Application.isEditor)
                UnityEditor.Handles.Label(pn.position, "Node ID: " + pn.id);

            foreach (int n in pn.neighbours)
            {
                s_pathNode neigh = pathNodes.Find(x => x.id == n);

                if (neigh != null)
                    Gizmos.DrawLine(pn.position, neigh.position);
            }
#endif
        }
            */
    }
    
}


public partial class o_Mcharacter : o_character
{
    public s_pathNode path { get; set; }
}

[System.Serializable]
public class s_pathNode
{
    public List<int> neighbours = new List<int>();

    public bool isLight;
    public bool isgoal = false;

    public s_pathNode parent;
    public int list;
    public Vector2 position;
    public int id;
    public float g_cost = 0, h_cost = 0;
    public float f_cost
    {
        get
        {
            return g_cost + h_cost;
        }
    }

    public s_pathNode(int id, Vector2 position, List<int> path)
    {
        neighbours = path;
        this.id = id;
        this.position = position;
    }
    public s_pathNode()
    {
    }
}