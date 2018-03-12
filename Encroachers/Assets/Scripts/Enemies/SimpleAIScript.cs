using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SimpleAIScript : MonoBehaviour {

    public float agentSpeed = 2.5f;

    public GameObject gobj;

	// Use this for initialization
	void Start () 
    {
		
	}
	
	// Update is called once per frame
	void Update () {

        Vector3 direction = gobj.transform.position - transform.position;
        
        direction.y = 0.0f;

        transform.LookAt(gobj.transform.position, Vector3.up);
        transform.Translate(direction.normalized * agentSpeed * Time.deltaTime, Space.World);
	}
}
