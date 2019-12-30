using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagnumFoudation;
using UnityEngine.UI;

public class s_trig : s_triggerhandler
{
    Image fade;
    private gui_dialogue Dialogue;
    const float shutterdepth = 1.55f;
    string current_label;
    bool first_move_event = true;

    public o_character player;
    public o_character host;

    s_ninjaloader leveled;

    bool activated_shutters = false;

    /*
    public override IEnumerator EventPlayMast()
    {
        doingEvents = true;
        Image sh1 = GameObject.Find("Shutter1").GetComponent<Image>();
        Image sh2 = GameObject.Find("Shutter2").GetComponent<Image>();

        while (pointer != -1 || pointer < Events.Count)
        {
            yield return StartCoroutine(EventPlay());

            // print("Pointer at: " + pointer);
            if (pointer == -1)
                break;
            pointer++;
        }
        if (activated_shutters)
        {
            for (int i = 0; i < 30; i++)
            {
                sh1.rectTransform.position += new Vector3(0, shutterdepth);
                sh2.rectTransform.position += new Vector3(0, -shutterdepth);
                yield return new WaitForSeconds(Time.deltaTime);
            }
        }
        current_trigger = null;
        doingEvents = false;
        //isskipping = false;
        Time.timeScale = 1;
        first_move_event = true;
        activated_shutters = false;
    }
    */

    new IEnumerator EventPlay()
    {
        yield return base.EventPlay();
        //yield return new WaitForSeconds(0.5f);
    }

    private void Update()
    {
    }

}