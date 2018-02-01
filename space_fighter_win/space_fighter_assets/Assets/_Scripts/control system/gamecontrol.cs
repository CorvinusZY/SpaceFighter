using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class gamecontrol : MonoBehaviour
{

	public Vector3 spawn;
	public GameObject[] hazards;
	public float spawnwait;
	public levelmanager mymanager;

	public Text score_text;
	public Text restart_text;
	public Text gameover_text;

	public bool restart;
	public bool gameover;

	private float playerhealth = 10;
	private float stationhealth = 50;
	private int score = 0;

	public GameObject panel;
	private bool panelon = false;


	// Use this for initialization
	void Start ()
	{
		restart = false;
		gameover = false;
		restart_text.text = "";
		restart_text.GetComponent<Button> ().interactable = false;
		gameover_text.text = "";
		StartCoroutine (spawnwave ());
		addscore (0);
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Escape)) {
			if (panelon) {
				Time.timeScale = 1;
				panel.SetActive (false);
				panelon = false;
			} else {
				Time.timeScale = 0;
				panel.SetActive (true);
				panelon = true;
			}
		}


	}

	IEnumerator spawnwave ()
	{
		while (true) {
			GameObject hazard = hazards [Random.Range (0, hazards.Length)];
			yield return new WaitForSeconds (spawnwait);
			Quaternion spawn_rotate = Quaternion.identity;
			Vector3 spawn_posn = new Vector3 (Random.Range (-6, 6), spawn.y, spawn.z);
			Instantiate (hazard, spawn_posn, spawn_rotate);

			if (gameover) {
				mymanager.updatescore (score);
				gameover_text.text = "Game Over!";
				restart_text.GetComponent<Button> ().interactable = true;
				restart_text.text = "Restart\n";
				restart = true;
				break;
			}
		}
	}

	public void addscore (int netchange)
	{
		score += netchange;
		updatescore ();
	}

	void updatescore ()
	{
		score_text.text = "Score: " + score.ToString ();
	}

	public void playerhurt ()
	{
		playerhealth -= 1;

	}

	public void stationhurt ()
	{
		stationhealth -= 1;
	}

	public bool playerdead ()
	{
		
		if (playerhealth <= 0) {
			
			return true;
		} else {
			return false;
		}
	}

	public bool stationdead ()
	{
		
		return (stationhealth <= 0);
	}

	public float player_health_percent ()
	{
		float x = playerhealth / 10;
		return x;
	}

	public float station_health_percent ()
	{
		float x = stationhealth / 50;
		return x;
	}

	public bool stageup ()
	{
		if (score >= 100)
			return true;
		else
			return false;
	}
		
}
