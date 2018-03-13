using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


namespace Networking
{
	public class PlayerNetworking : NetworkBehaviour 
	{
		public GameObject fpsCharacter;
		public GameObject[] characterModel;

		public override void OnStartLocalPlayer()
		{
			GetComponent<PlayerMovement>().enabled = true;
			fpsCharacter.SetActive(true);

			foreach(GameObject go in characterModel)
			{
				go.SetActive(false);
			}
		}
	}
}
