using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2Shoot : MonoBehaviour 
{

	public Camera cam;

	void Update()
	{
		if(Input.GetMouseButton(0))
		{
			Ray ray = cam.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if(Physics.Raycast(ray, out hit, 100))
			{
				Debug.Log(hit.collider.name);
			}
		}
	}
}
