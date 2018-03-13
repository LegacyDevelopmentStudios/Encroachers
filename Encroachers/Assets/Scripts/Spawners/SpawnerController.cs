using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon;

public class SpawnerController : Photon.MonoBehaviour {
    // Public vars
    public float lifeTime = 20f;
    public float frequency = 2.95f;

    

    // Private vars
    private float freqCounter = 0f;


    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        lifeTime -= Time.deltaTime;
        freqCounter += Time.deltaTime;

        if (freqCounter >= frequency)
        {
            freqCounter = 0f;
            // instantiate enemy
            Vector3 newV = new Vector3(0f, 1.8f, 0f);
            GameObject faggot = (GameObject)Instantiate(Resources.Load("EnemyBOT"), transform.position + newV, Quaternion.identity);
        }

        if (lifeTime <= 0f)
        {
            Destroy(gameObject);
        }
	}
}
