using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagnumFoudation;

public class c_plcharacter : o_Mcharacter
{
    Vector2 last_position;
    Vector2 targetPos;
    public UnityEngine.UI.Text securityText;
    public AudioClip jumpSound;
    public AudioClip land;
    float walkSoundTimer = 0.6f;
    public AudioClip[] walksounds;
    public UnityEngine.UI.Text txt;
    public GameObject systgui;

    new void Start()
    {
        maxHealth = 4;
        health = maxHealth;
        Initialize();
        rendererObj = transform.GetChild(0).GetComponent<SpriteRenderer>();
        animHand = transform.GetChild(0).GetComponent<s_animhandler>();
        base.Start();
    }

    public override void OnGround()
    {
        s_soundmanager.sound.PlaySound(ref land, false);
    }

    public override void PlayerControl()
    {
        base.PlayerControl();
        switch (CHARACTER_STATE)
        {
            case CHARACTER_STATES.STATE_IDLE:

                if (ArrowKeyControl())
                    CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
                break;

            case CHARACTER_STATES.STATE_MOVING:

                if (grounded)
                {
                    terminalspd = terminalSpeedOrigin;
                    if (!ArrowKeyControl())
                        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
                    
                    if (Input.GetKeyDown(s_globals.arrowKeyConfig["jump"]))
                    {
                        s_soundmanager.sound.PlaySound(ref jumpSound, false);
                        Jump(2f);
                        //s_nGlobal.AddNoise(transform.position, 145, 0.5f);
                    }
                }
                else
                    terminalspd = terminalSpeedOrigin * 2 + 30f;
                break;
        }

        //MoveMotor(direction);

        //print(Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg);
        if (Input.GetKeyDown(KeyCode.V))
        {
            last_position = transform.position;
        }
    }

    new void FixedUpdate()
    {
        Collider2D trig = IfTouchingGetCol<o_trigger>(collision);
        if (trig != null)
        {
            if (trig.GetComponent<o_trigger>().TRIGGER_T == o_trigger.TRIGGER_TYPE.CONTACT_INPUT)
            {
                systgui.gameObject.SetActive(true);
                txt.text = "Press " + s_globals.arrowKeyConfig["select"].ToString();
            }
        }
        else
        {
            systgui.gameObject.SetActive(false);
            txt.text = "";
        }

        switch (health)
        {
            case 4:
                securityText.text = "Security level: 0%";
                break;


            case 3:
                securityText.text = "Security level: 25%";
                break;


            case 2:
                securityText.text = "Security level: 50%";
                break;


            case 1:
                securityText.text = "Security level: 75%";
                break;

            case 0:
                securityText.text = "Security level: 100%";
                break;

        }
        if (health == 0)
            s_triggerhandler.trig.JumpToEvent(0, true);

        int verticalDir = Mathf.RoundToInt(direction.y);
        int horizontalDir = Mathf.RoundToInt(direction.x);

        if (CHARACTER_STATE == CHARACTER_STATES.STATE_IDLE)
        {
            if (verticalDir == -1 && horizontalDir == 0)
                SetAnimation("idle_d", true);
            else if (verticalDir == 1 && horizontalDir == 0)
                SetAnimation("idle_u", true);
            else if (horizontalDir == -1 && verticalDir == 1 ||
                horizontalDir == -1 && verticalDir == -1 || horizontalDir == -1)
            {
                rendererObj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                SetAnimation("idle_s", true);
            }
            else if (horizontalDir == 1 && verticalDir == 1 ||
                horizontalDir == 1 && verticalDir == -1 || horizontalDir == 1)
            {
                rendererObj.transform.rotation = Quaternion.Euler(new Vector3(0, -180, 0));
                SetAnimation("idle_s", true);
            }

        }
        if (CHARACTER_STATE == CHARACTER_STATES.STATE_MOVING)
        {
            if (grounded)
            {
                if (walkSoundTimer > 0)
                {
                    walkSoundTimer -= Time.deltaTime;
                }
                else
                {
                    s_soundmanager.sound.PlaySound(ref walksounds[UnityEngine.Random.Range(0, 4)], false);
                    walkSoundTimer = 0.4f;
                }
                if (verticalDir == -1 && horizontalDir == 0)
                    SetAnimation("walk_d", true);
                else if (verticalDir == 1 && horizontalDir == 0)
                    SetAnimation("walk_u", true);
                else if (horizontalDir == -1 && verticalDir == 1 ||
                    horizontalDir == -1 && verticalDir == -1 || horizontalDir == -1)
                {
                    rendererObj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                    SetAnimation("walk_s", true);
                }
                else if (horizontalDir == 1 && verticalDir == 1 ||
                    horizontalDir == 1 && verticalDir == -1 || horizontalDir == 1)
                {
                    rendererObj.transform.rotation = Quaternion.Euler(new Vector3(0, -180, 0));
                    SetAnimation("walk_s", true);
                }
            }
        }

        s_camera.SetPlayer(GetComponent<c_plcharacter>());
        /*
        direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        targetPos = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
        angle = Mathf.Atan2(targetPos.x, targetPos.y) * Mathf.Rad2Deg;


        //Quaternion.
        if (angle <= (Mathf.Atan2(targetPos.x, targetPos.y) * Mathf.Rad2Deg))
        {
        }
        else
            angle -= Time.deltaTime;
        */
        base.FixedUpdate();
    }
    new void Update()
    {

        base.Update();
    }
    
}
