using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AiScript : MonoBehaviour 
{
	public GameObject player;
	NavMeshAgent agent;
	public float agentLifeTime = 120f;
	public float agentSpeed = 2f;

	void Start()
	{
		agent = GetComponent<NavMeshAgent>();
		player = GameObject.FindGameObjectWithTag("Player");
	}

	void Update()
	{
		agent.SetDestination(player.transform.position);
	}
}
