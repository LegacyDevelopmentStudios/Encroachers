using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class SpawnerController : NetworkBehaviour {
    // Public vars
    public float lifeTime = 20f;
    public float frequency = 2.95f;
    [SerializeField] private GameObject aiPrefab;

    

    // Private vars
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
            Vector3 newV = new Vector3(0f, 0.8f, 0f);
            GameObject go = GameObject.Instantiate(aiPrefab, transform.position, Quaternion.identity) as GameObject;
            NetworkServer.Spawn(go);

        }
	}
}
