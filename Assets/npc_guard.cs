using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagnumFoudation;

[Serializable]
public class npc_guard : o_character
{
    public float angle;
    public BoxCollider2D lineOfSight;
    GameObject LOSSpr;
    
    public AudioClip[] walksounds = new AudioClip[2];

    public LayerMask layerMsk;
    public string teleportLoc;
    public List<int> marchDirection = new List<int>();

    float backtrackSearchTimer = 2f;
    float bulletShootTimer = 1.5f;
    float walkSoundTimer = 0.6f;
    const float startBulletShootTimer = 1.5f;

    public int marchPoint = 0;

    Vector2 lastSeenPos;
    public List<s_pathNode> path = new List<s_pathNode>();

    public enum AI_STATE
    {
        IDLE,
        ALERT,
        BACKTRACK,
        PATHFIND
    }
    public AI_STATE state = AI_STATE.IDLE;

    new void Start()
    {
        maxHealth = 20;
        target = GameObject.Find("Player").GetComponent<c_plcharacter>();
        LOSSpr = transform.GetChild(1).gameObject;
        lineOfSight.size *= LOSSpr.transform.lossyScale;
        health = maxHealth;
        terminalSpeedOrigin = 45;
        Initialize();
        rendererObj = transform.GetChild(2).GetComponent<SpriteRenderer>();
        animHand = transform.GetChild(2).GetComponent<s_animhandler>();
        base.Start();
    }

    public void ReturnToPatrol()
    {
        s_pathNode nod = s_ninjaloader.pathfind.FindPathNode(marchDirection[0]);
        if (nod != null)
        {
            Tuple<List<s_pathNode>, float> nodes = s_ninjaloader.pathfind.PathFind(transform.position, nod);
            path = nodes.Item1;
        }
    }

    public override void WalkControl()
    {
        /*
        if (marchDirection.Count > 0)
        {
            terminalspd = terminalSpeedOrigin;
            s_pathNode nod = s_ninjaloader.pathfind.FindPathNode(marchDirection[marchPoint]);
            path = s_ninjaloader.pathfind.PathFind(transform.position, nod.position);
            if (path != null)
            {
                if (!CheckTargetDistance(nod.position, 10))
                {
                    direction = LookAtTarget(nod.position);
                    CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
                }
                else
                {
                    if (marchPoint == marchDirection.Count - 1)
                        marchPoint = 0;
                    else
                        marchPoint++;
                }
            }
        }
        */
        if (path == null)
        {
            path = new List<s_pathNode>();
            for (int i = 0; i < marchDirection.Count; i++)
            {
                s_pathNode nod = s_ninjaloader.pathfind.FindPathNode(marchDirection[i]);
                path.Add(nod);
            }
        }
        else {

            if (path.Count == 0)
            {
                for (int i = 0; i < marchDirection.Count; i++)
                {
                    s_pathNode nod = s_ninjaloader.pathfind.FindPathNode(marchDirection[i]);
                    path.Add(nod);
                }
            }
        }
    }

    void PathWalk()
    {
        if (path != null)
            if (path.Count > 0)
            {
                if (path[0] != null)
                {
                    CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
                    if (!CheckTargetDistance(path[0].position, 10))
                    {
                        direction = LookAtTarget(path[0].position);
                        CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
                        terminalspd = 55;
                    }
                    else
                        path.RemoveAt(0);
                }
            }
    }

    public void ShootBullet(float duration)
    {
        o_bullet bullt = s_levelloader.LevEd.SpawnObject<o_bullet>("Bullet", transform.position, Quaternion.Euler(0, 0, angle));
        bullt.direction = direction;
        bullt.SetTimer();
        bullt.parent = this;
    }

    public override void ArtificialIntelleginceControl()
    {
        base.ArtificialIntelleginceControl();
        WalkControl();
        PathWalk();
    }

    new void FixedUpdate()
    {
        //o_noise noise = s_nGlobal.GetNoise(transform.position);
        Collider2D pl = IfTouchingGetCol<c_plcharacter>(lineOfSight, "Player");
        lineOfSight.transform.position = transform.position + (Vector3)(direction * 60);
        LOSSpr.transform.position = lineOfSight.transform.position;

        if (direction.x >= 0)
            rendererObj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        else
            rendererObj.transform.rotation = Quaternion.Euler(new Vector3(0, -180, 0));
        
        if (pl != null)
        {
            s_map.s_tileobj to = s_levelloader.LevEd.mapDat.tilesdata.Find(x => x.name == teleportLoc);
            if (to != null)
            {
                pl.GetComponent<c_plcharacter>().health--;
                pl.gameObject.transform.position = new Vector3(to.pos_x, to.pos_y);
            }
        }

        if (CheckTargetDistance(s_levelloader.LevEd.player, 450))
        {
            if (walkSoundTimer > 0)
            {
                walkSoundTimer -= Time.deltaTime;
            }
            else
            {
                s_soundmanager.sound.PlaySound(ref walksounds[UnityEngine.Random.Range(0, 5)], (450/TargetDistance(s_levelloader.LevEd.player)) * 0.55f, false);
                walkSoundTimer = 0.4f;
            }
        }

        /*
        if (pl != null)
        {
            path.Clear();
            target = pl.GetComponent<o_character>();

            GameObject go = GameObject.Find(teleportLoc);
            if (go != null)
            {
                pl.transform.position = go.transform.position;
            }

        }
        switch (state)
        {
            case AI_STATE.PATHFIND:

                if (path != null)
                {
                    if (path.Count > 0)
                    {
                        if (pl != null)
                            if (!Physics2D.Linecast(transform.position, target.transform.position, layerMsk))
                            {
                                path.Clear();
                            }
                        PathWalk();
                    }
                    else
                    {
                        if (Physics2D.Linecast(transform.position, target.transform.position, layerMsk))
                        {

                            path.Clear();
                            backtrackSearchTimer = 2f;
                            //int minleng = s_ninjaloader.pathfind.PathFind(transform.position, s_ninjaloader.pathfind.FindPathNode(closestPathID).position).Count;
                            state = AI_STATE.BACKTRACK;
                        }
                        else
                        {
                            direction = LookAtTarget(target);
                            lastSeenPos = target.transform.position;
                            if (CheckTargetDistance(target, 100))
                                CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
                            else
                                CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;

                            if (bulletShootTimer > 0)
                                bulletShootTimer -= Time.deltaTime;
                            else
                            {
                                ShootBullet(0.7f);
                                if (CHARACTER_STATE == CHARACTER_STATES.STATE_IDLE)
                                    bulletShootTimer = startBulletShootTimer;
                                if (CHARACTER_STATE == CHARACTER_STATES.STATE_MOVING)
                                    bulletShootTimer = startBulletShootTimer * 2;
                            }
                        }
                        if (pl == null)
                        {
                        }
                    }
                }
                break;

            case AI_STATE.IDLE:
                
                WalkControl();
                PathWalk();
                if (pl != null)
                {
                    path.Clear();
                    target = pl.GetComponent<o_character>();

                    GameObject go = GameObject.Find(teleportLoc);
                    if (go != null)
                    {
                        pl.transform.position = go.transform.position;
                    }
                    
                }

                break;
            case AI_STATE.BACKTRACK:
                if (backtrackSearchTimer > 0)
                {
                    Tuple<List<s_pathNode>, float> nodes = s_ninjaloader.pathfind.PathFind(transform.position, target.transform.position);

                    if (path == null)
                        path = new List<s_pathNode>();

                    if (nodes != null)
                        if (nodes.Item1 != null)
                            path = nodes.Item1;

                    backtrackSearchTimer -= Time.deltaTime;
                }
                else
                {
                    if (path.Count == 0)
                    {
                        path.Clear();
                        ReturnToPatrol();
                    }
                }
                PathWalk();
                if (pl != null)
                {
                    path.Clear();
                    state = AI_STATE.PATHFIND;
                }
                break;
                
        }
        */

        base.FixedUpdate();
    }

    new void Update()
    {
        base.Update();
        SetAnimation("walk_up", true);
    }
}
