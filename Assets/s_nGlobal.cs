using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagnumFoudation;
using UnityEditor;

[System.Serializable]
public class o_noise
{
    public o_noise(Vector2 position, float range, float duration)
    {
        this.position = position;
        this.range = range;
        this.duration = duration;
    }
    public Vector2 position;
    public float range;
    public float duration;
}

public class s_nGlobal : s_globals 
{
    public static List<o_noise> noises = new List<o_noise>();
    s_map curmap;

    private void OnGUI()
    {
        int x = 0, y = 0;
        List<s_map> maps = s_ninjaloader.LevEd.maps;
        for (int i = 0; i < maps.Count; i++)
        {
            s_map ma = maps[i];

            if (curmap == ma)
            {
                if (GUI.Button(new Rect(150, 50 * 1, 160, 50), "Back"))
                {
                    curmap = null;
                }
                if (GUI.Button(new Rect(160 * x, 50 * (y + 2), 160, 50), ma.name))
                {
                    s_ninjaloader.LevEd.current_area = i;
                    s_ninjaloader.LevEd.NewMap();
                    s_ninjaloader.LevEd.LoadMap(curmap);
                }
            }
            else
            {
                if (curmap == null)
                {

                    if (y == 10)
                    {
                        y = 0;
                        x++;
                    }
                    if (GUI.Button(new Rect(160 * x, 50 * (y + 3), 160, 50), ma.name))
                    {
                        curmap = ma;
                    }
                    y++;

                }
            }

        }

    }

    public static void AddNoise(Vector2 position, float range, float duration)
    {
        noises.Add(new o_noise(position, range, duration));
    }
    public static o_noise GetNoise(Vector2 position)
    {
        foreach (o_noise n in noises)
        {
            print(Vector2.Distance(n.position, position) < n.range);
            if (Vector2.Distance(n.position, position) < n.range)
                return n;
        }
        return null;
    }

    void Update()
    {
        for (int i = 0; i < noises.Count; i++)
        {
            o_noise n = noises[i];
            n.duration -= Time.deltaTime;
            if (n.duration <= 0)
            {
                noises.Remove(n);
            }
        }
    }

    private void OnDrawGizmos()
    {
        foreach (o_noise n in noises)
        {
            Gizmos.DrawWireSphere(n.position, n.range);
        }
    }
}
