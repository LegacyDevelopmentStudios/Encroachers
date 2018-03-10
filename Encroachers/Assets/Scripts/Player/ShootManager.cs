using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ShootManager : NetworkBehaviour{

    public Transform cam;
    public GameObject cube;
    
    public void Start() {
        cam = transform.GetChild(0);
    }

    void Update () {
		if(Input.GetMouseButtonDown(0) && isLocalPlayer) {
            Shoot();
        }
	}
    
    void Shoot() {
        RaycastHit hit;
        Debug.DrawLine(cam.position, cam.forward * 100, Color.red, 3f);
        if (Physics.Raycast(cam.position, cam.forward, out hit)) {
            CmdSpawnObject(hit.point);
        }
    }

    [Command]
    void CmdSpawnObject(Vector3 pos) {
        GameObject go = GameObject.Instantiate(cube, pos, Quaternion.identity);
        NetworkServer.Spawn(go);
    }

}
