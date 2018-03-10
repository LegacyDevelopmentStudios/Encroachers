﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Game_NetworkManager : Photon.MonoBehaviour {


	[SerializeField]
	private Text connectionText;

	private void Start () 
	{
		PhotonNetwork.ConnectUsingSettings("testing branch");
		
	}
	
	private void Update () 
	{
		connectionText.text = PhotonNetwork.connectionStateDetailed.ToString();
	}
}
