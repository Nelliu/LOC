using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Respawn : MonoBehaviour
{
    public Transform[] RespawnPoints;
    private short indexOfChkpointReached = -1;
    private Controller controller;

    public Text DeathCounter; // counts number of deaths.
    public string CurrentCheckpoint; // stores name of current checkpoint

    public GameObject FinishLine; // stores GO with finish line of game
    public GameObject OnStartLevel; // stores information for start of level 2
    private SpriteRenderer spriterender; // loads Sprite Renderer component to change sprite later on
    private Player player;

    public void Start()
    {
        spriterender = GetComponent<SpriteRenderer>();
        player = GetComponent<Player>();
        controller = GetComponent<Controller>();
        RespawnPointFill();
        MoveToCheckpoint(ref indexOfChkpointReached);
    }

    public void Update()
    {
        if (controller.spikesHit)
        {
            MoveToCheckpoint(ref indexOfChkpointReached);
            DeathCounter.text = (int.Parse(DeathCounter.text) + 1).ToString();
            controller.spikesHit = false;

        }

        if (Input.GetKeyDown(KeyCode.P) && controller.canMoveToNext)
        {
            MoveToNextLevel();
            controller.canMoveToNext = false;
        }

        if (controller.checkpointHit && indexOfChkpointReached == RespawnPoints.Length - 1)
        {
            FinishLine.SetActive(true);
        }
       


    }

    private void RespawnPointFill()
    {
        if (RespawnPoints != null)
        {
            if (indexOfChkpointReached != RespawnPoints.Length - 1) // ?
            {
                indexOfChkpointReached++;
                //print(indexOfChkpointReached);
                CurrentCheckpoint = RespawnPoints[indexOfChkpointReached].name;
            }
        }
    }

    public void UpdateCheckPointOnHit()
    {
     
        if (RespawnPoints != null && controller.nameOfCheckpoint != null)
        {
            if (controller.nameOfCheckpoint != RespawnPoints[indexOfChkpointReached].name)
            {
                for (short i = 0; i < RespawnPoints.Length; i++)
                {
                    if (RespawnPoints[i].name == controller.nameOfCheckpoint && i > indexOfChkpointReached)
                    {
                        indexOfChkpointReached = i;
                        CurrentCheckpoint = RespawnPoints[i].name;
                    }
                }
            }
        }
    }
    private void MoveToCheckpoint(ref short checkpointIndex)
    {
        gameObject.transform.position = RespawnPoints[checkpointIndex].position;
    }

    internal void MoveToNextLevel() // currently moves only to level 2 and noowhere else
    {
        controller.finish.SetActive(false);
        gameObject.transform.position = RespawnPoints[3].position;
        indexOfChkpointReached = 3;
        CurrentCheckpoint = RespawnPoints[3].name;

        controller.isSquare = true;
        player.MaxJumpVelocity = 0;
        player.MinJumpVelocity = 0;
        player.wallJumpUp = Vector2.zero;
        player.wallJumpLeap = Vector2.zero;
        spriterender.sprite = player.sprite2;

        OnStartLevel.SetActive(true);


        

      
    }

}

