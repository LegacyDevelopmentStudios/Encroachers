using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootManager : MonoBehaviour{

    public Transform cam;
    public GameObject cube;

    private float rateOfFire = 5f;
    private float rofTime, rofCounter;
    
    public void Start() {
        cam = transform.GetChild(0);
        rofTime = 1f / rateOfFire;
        rofCounter = rofTime;
    }

    void Update () {
		if(Input.GetMouseButton(0)) {

            if(CanFire()) { Shoot(); }
        }

        if (Input.GetMouseButton(1))
        {
            if(CanFire()) { ShootAlternate(); }
        }

        if (Input.GetMouseButtonUp(0)
         || Input.GetMouseButtonUp(1))
            rofCounter = rofTime;
	}

    private bool CanFire()
    {
        rofCounter += Time.deltaTime;
        if(rofCounter >= rofTime)
        {
            rofCounter = 0f;
            return true;
        }
        return false;
    }
    
    void Shoot() {
        RaycastHit hit;
        Debug.DrawLine(cam.position, cam.forward * 100, Color.red, 3f);
        if (Physics.Raycast(cam.position, cam.forward, out hit)) {
            //SpawnObject(hit.point);
            if (hit.transform.tag == "Enemy")
            {
                Destroy(hit.transform.gameObject);
            }
        }
    }

    private void ShootAlternate()
    {
        RaycastHit hit;
        Debug.DrawLine(cam.position, cam.forward * 100, Color.red, 3f);
        if (Physics.Raycast(cam.position, cam.forward, out hit))
        {
            if (hit.transform.tag != "Enemy")
            {
                SpawnEnemySpawner(hit.point);
            }
        }
    }

    GameObject SpawnObject(Vector3 pos) {
        return GameObject.Instantiate(cube, pos, Quaternion.identity);
    }

    GameObject SpawnEnemySpawner(Vector3 pos)
    {
        return GameObject.Instantiate(Resources.Load<GameObject>("TestSpawner"), pos, Quaternion.identity);
    }
}
