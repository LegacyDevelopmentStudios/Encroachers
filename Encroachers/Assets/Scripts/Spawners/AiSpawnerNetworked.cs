using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AiSpawnerNetworked : NetworkBehaviour 
{
	[SerializeField] GameObject agentObject;
	[SerializeField] GameObject agentSpawn;
	private int counter;
	private int numberOfAgents;

	public override void OnStartServer()
	{
		for(int i = 0; i > numberOfAgents; i++)
		{
			spawnAgents();
		}
	}

	void spawnAgents()
	{

		if(Input.GetButton("Fire"))
		{
			GameObject go = GameObject.Instantiate(agentObject, agentSpawn.transform.position, Quaternion.identity) as GameObject;
			NetworkServer.Spawn(go);
		}
	}


}
