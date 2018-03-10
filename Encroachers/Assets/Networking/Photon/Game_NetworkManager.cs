using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Game_NetworkManager : Photon.MonoBehaviour {

	[SerializeField] private GameObject player;
	[SerializeField] private Text connectionText;
	[SerializeField] private GameObject lobbyCam;
	[SerializeField] private Transform spawnPoint;

	private void Start () 
	{
		PhotonNetwork.ConnectUsingSettings("testing branch");
		
	}

	public virtual void OnJoinedLobby()
	{
		Debug.Log("Connected To Lobby");

		PhotonNetwork.JoinOrCreateRoom("New", null, null);
	}

	public virtual void OnJoinedRoom()
	{
		PhotonNetwork.Instantiate(player.name, spawnPoint.position, spawnPoint.rotation, 0);

		lobbyCam.SetActive(false);
	}
	
	private void Update () 
	{
		connectionText.text = PhotonNetwork.connectionStateDetailed.ToString();
	}
}
