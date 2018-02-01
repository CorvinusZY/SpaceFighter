using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotator : MonoBehaviour {
	public float tumble;
	public Rigidbody rg;

	// Use this for initialization
	void Start () {
		rg = GetComponent<Rigidbody> ();
		rg.angularVelocity = Random.insideUnitSphere * tumble;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
