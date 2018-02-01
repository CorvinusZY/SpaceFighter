using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LaireonFramework
{
	public class destroy_if_collided : MonoBehaviour
	{
	
		public GameObject explode;
		public GameObject player_explode;
		public int scorevalue;
		private gamecontrol mycontroller;
		private BeatingHealthBar playerbar;
		private BeatingHealthBar stationbar;

		// Use this for initialization
		void Start ()
		{
			GameObject controller = GameObject.FindWithTag ("gamecontrol");
			GameObject bar1 = GameObject.FindWithTag ("playerbar");
			GameObject bar2 = GameObject.FindWithTag ("stationbar");
			if (controller != null) {
				mycontroller = controller.GetComponent<gamecontrol> ();
			}
			if (bar1 != null) {
				playerbar = bar1.GetComponent<BeatingHealthBar> ();
			}
			if (bar2 != null) {
				stationbar = bar2.GetComponent<BeatingHealthBar> ();
			}
		
		}
	
		// Update is called once per frame
		void Update ()
		{
		
		}

		void OnTriggerEnter (Collider other)
		{
			
			if (other.tag == "boundary" || other.tag == "enemy")
				return;

			if (explode != null) {
				Instantiate (explode, transform.position, transform.rotation);
			}
			Destroy (gameObject);

			if (other.tag == "Player") {
				mycontroller.playerhurt ();
				playerbar.currentValue = playerbar.maxValue * mycontroller.player_health_percent ();
				if (mycontroller.playerdead ()) {
					
					Instantiate (player_explode, other.transform.position, other.transform.rotation);
					mycontroller.gameover = true;
					Destroy (other.gameObject);
					playerbar.currentValue = 0;
				}
			}

			if (other.tag == "station") {
				mycontroller.stationhurt ();
				stationbar.currentValue = stationbar.maxValue * mycontroller.station_health_percent ();
				if (mycontroller.stationdead()) {
					Instantiate (player_explode, other.transform.position, other.transform.rotation);
					mycontroller.gameover = true;
					Destroy (other.gameObject);
					stationbar.currentValue = 0;
				}
			}
			mycontroller.addscore (scorevalue);



		}




	}

}
