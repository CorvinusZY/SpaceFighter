using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotMover : MonoBehaviour {

	public float speed;
	public Rigidbody rg;
	// Use this for initialization
	void Start () {
		rg = GetComponent<Rigidbody> ();
		rg.velocity = Vector3.forward * speed;

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
