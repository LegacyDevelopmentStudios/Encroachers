﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class SimpleAIScript : MonoBehaviour 
{
    
    public float agentSpeed = 2.5f;

    private GameObject plr;
    private Rigidbody rb;

    private float lifeTime = 120f;

    //private PhotonPlayer[] plrList;

    //private PhotonPlayer target;

	// Use this for initialization
	void Start () 
    {
        rb = GetComponent<Rigidbody>();
        //plrList = PhotonNetwork.otherPlayers;
        plr = GameObject.FindGameObjectWithTag("Player");
	}
	
	// Update is called once per frame
	void Update () {
        lifeTime -= Time.deltaTime;

        if (lifeTime <= 0f)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 direction = plr.transform.position - transform.position;
        
        direction.y = 0.0f;


        transform.LookAt(plr.transform.position, Vector3.up);
        //transform.Translate(direction.normalized * agentSpeed * Time.deltaTime, Space.World);

        if (rb.velocity.magnitude < agentSpeed)
        {
            rb.AddForce(direction.normalized * Time.deltaTime * rb.mass * 10f, ForceMode.VelocityChange);
        }
    }
}