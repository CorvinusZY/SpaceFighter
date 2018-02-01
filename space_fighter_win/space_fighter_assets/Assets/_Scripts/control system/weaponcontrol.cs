using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weaponcontrol : MonoBehaviour {

	public GameObject shot;
	public Transform shotspawn;
	public float delay;
	public float firerate;


	private AudioSource audio;
	// Use this for initialization
	void Start () {
		audio = GetComponent<AudioSource> ();
		InvokeRepeating ("Fire", delay, firerate);
	}

	void Fire(){
		Instantiate (shot, shotspawn.position, shotspawn.rotation);
		audio.Play ();
	}

	// Update is called once per frame
	void Update () {
		
	}
}
