using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class levelmanager : MonoBehaviour {

	private static int bestscore;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void loadlevel(string level) {
		Application.LoadLevel (level);
	}

	public void quit(){
		Application.Quit ();
	}

	public int getbestscore(){
		return bestscore;
	}

	public void updatescore(int lastscore){
		if (bestscore < lastscore) {
			bestscore = lastscore;
		}
	}
}
