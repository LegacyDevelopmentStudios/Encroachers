using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AiScript : MonoBehaviour 
{
	private GameObject player;
	private float agentLifeTime = 300f;
	private float agentSpeed = 2.5f;

	private NavMeshAgent agent;
	private bool doCountdown = false;

    public float Lifetime
    {
        get
        {
            return agentLifeTime;
        }
        set
        {
            this.agentLifeTime = value;
			if (this.doCountdown == false) { this.doCountdown = true; }
        }
    }
    public float Speed
    {
        get
        {
            return agentSpeed;
        }
        set
        {
            this.agentSpeed = value;
			this.GetComponent<NavMeshAgent>().speed = value;
        }
    }

	void Start()
	{
		agent = GetComponent<NavMeshAgent>();
		agent.speed = agentSpeed;
		player = GameObject.FindGameObjectWithTag("Player");
	}

	void Update()
	{
		if (doCountdown == true)
		{
			agentLifeTime -= Time.deltaTime;

			if (agentLifeTime <= 0f)
			{
				Destroy(gameObject);
				return;
			}
		}
		
		agent.SetDestination(player.transform.position);
	}
}
