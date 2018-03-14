using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootManager : MonoBehaviour{

    public Transform cam;
    public GameObject cube;

    private ROFLimiter weap1, weap2;
    
    public void Start() {
        cam = transform.GetChild(0);
        
        weap1 = gameObject.AddComponent(typeof(ROFLimiter)) as ROFLimiter;
        weap2 = gameObject.AddComponent(typeof(ROFLimiter)) as ROFLimiter;
        
        weap1.FiresPerSecond = 10f;
        weap2.FiresPerSecond = 5f;
    }

    void Update () {
        if (weap1.CanFire(Input.GetMouseButton(0)))
        {
            Shoot();
        }
        if (weap2.CanFire(Input.GetMouseButton(1)))
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

    GameObject SpawnObject(Vector3 pos) {
        return GameObject.Instantiate(cube, pos, Quaternion.identity);
    }

    GameObject SpawnEnemySpawner(Vector3 pos)
    {
        return GameObject.Instantiate(Resources.Load<GameObject>("TestSpawner"), pos, Quaternion.identity);
    }
}
