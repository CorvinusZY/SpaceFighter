using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class musicControl : MonoBehaviour {

	public AudioSource[] audiolist;
	public Scrollbar mybar;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(mybar.IsActive()){
			for (int i = 0; i < audiolist.Length; i++) {
			audiolist [i].volume = mybar.value;
			}
		}
	}
}
