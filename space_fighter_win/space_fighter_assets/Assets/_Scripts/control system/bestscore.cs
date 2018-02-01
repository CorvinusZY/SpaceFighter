using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class bestscore : MonoBehaviour {

	public Text bestscore_text;
	public levelmanager mymanager;
	// Use this for initialization
	void Start () {
		int mybestscore = mymanager.getbestscore ();
		bestscore_text.text = "Best Score:\n" + mybestscore;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
