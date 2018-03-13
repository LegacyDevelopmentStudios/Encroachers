using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootManager : MonoBehaviour{

    public Transform cam;
    public GameObject cube;
    
    public void Start() {
        cam = transform.GetChild(0);
    }

    void Update () {
		if(Input.GetMouseButtonDown(0)) {
            Shoot();
        }

        if (Input.GetMouseButtonDown(1))
        {
            ShootAlternate();
        }
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

    void SpawnObject(Vector3 pos) {
        GameObject go = GameObject.Instantiate(cube, pos, Quaternion.identity);
    }

    GameObject SpawnEnemySpawner(Vector3 pos)
    {
        return GameObject.Instantiate(Resources.Load<GameObject>("TestSpawner"), pos, Quaternion.identity);
    }
}
