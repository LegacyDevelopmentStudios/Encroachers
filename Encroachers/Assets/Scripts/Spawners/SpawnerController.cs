using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class SpawnerController : NetworkBehaviour {
    [Tooltip("How long the spawner will spawn things.")]
    public float lifeTime = 20f;

    [Tooltip("The number of seconds between spawning.")]
    public float frequency = 2.95f;

    [Tooltip("The object that will be spawned.")]
    [SerializeField] private GameObject aiPrefab;

    

    private float freqCounter = 0f;


    // Use this for initialization
    void Start () {
        Destroy(gameObject, lifeTime);
	}
	
	// Update is called once per frame
	void Update () {
        freqCounter += Time.deltaTime;

        if (freqCounter >= frequency)
        {
            freqCounter = 0f;
            // instantiate enemy
            GameObject go = GameObject.Instantiate(aiPrefab, transform.position, Quaternion.identity) as GameObject;
            NetworkServer.Spawn(go);

        }
	}
}
