using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnerController : MonoBehaviour {
    // Public vars
    public float lifeTime = 20f;
    public float frequency = 2.95f;

    

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
            GameObject faggot = (GameObject)Instantiate(Resources.Load("EnemyBOTAI"), transform.position + newV, Quaternion.identity);
            AiScript ais = faggot.GetComponent<AiScript>();
            ais.Lifetime = 10f;
            ais.Speed = 5f;
        }
	}
}
