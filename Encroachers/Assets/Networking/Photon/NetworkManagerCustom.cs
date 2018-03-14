using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


namespace LGDS 
{
	public class NetworkManagerCustom : NetworkManager {

		public Canvas mainMenu;

		private string ipAdress;
		private int port = 7777;
		public Text textConnectionInfo;
		public Text ipAdressTextInfo;
		public Scene currentScene;
		public GameObject[] panelsForUI;

		#region Unity Methods

		private void OnEnable()
		{
			SceneManager.sceneLoaded += OnMySceneLoaded;
		}

		private void OnDisable()
		{
			SceneManager.sceneLoaded -= OnMySceneLoaded;
		}

		public override void OnClientDisconnect(NetworkConnection conn)
		{
			base.OnClientDisconnect(conn);

			if(textConnectionInfo != null)
			{
				textConnectionInfo.text = "Disconnected Or Timed Out.";
				ActivePanel("MainMenu");
			}
		}
		#endregion

		#region My Methods

		void OnMySceneLoaded(Scene scene, LoadSceneMode mode)
		{
			SetInitialReferences();
		}


		void SetInitialReferences()
		{
			currentScene = SceneManager.GetActiveScene();
			if(currentScene.name == "MainMenu")
			{
				ActivePanel("PanelMain");
			}
			else
			{
				ActivePanel("PanelGame");
				OnClickClearConnectionTextInfo();
			}
		}

		public void ActivePanel(string panelName)
		{
			foreach (GameObject panelGO in panelsForUI)
			{
				if(panelGO.name.Equals(panelName))
				{
					panelGO.SetActive(true);
				}
				else
				{
					panelGO.SetActive(false);
				}
			}
		}

		void GetIpAdress()
		{
			ipAdress = ipAdressTextInfo.text;
		}

		void SetPort()
		{
			NetworkManager.singleton.networkPort = port;
		}

		void SetIpAdress()
		{
			NetworkManager.singleton.networkAddress = ipAdress;
		}

		public void OnClickClearConnectionTextInfo()
		{
			textConnectionInfo.text = string.Empty;
		}

		public void OnClickStartLanHost()
		{
			SetPort();
			NetworkManager.singleton.StartHost();
		}

		public void OnClickStartServerOnly()
		{
			SetPort();
			NetworkManager.singleton.StartServer();
		}

		public void OnClickJoinLANGame()
		{
			SetPort();
			GetIpAdress();
			SetIpAdress();
			NetworkManager.singleton.StartClient();
		}

		public void OnClickDisconnectFromNetwork()
		{
			NetworkManager.singleton.StopHost();
			NetworkManager.singleton.StopServer();
			NetworkManager.singleton.StopClient();
		}

		public void OnClickExitGame()
		{
			Application.Quit();
		}
		#endregion

	}
}
