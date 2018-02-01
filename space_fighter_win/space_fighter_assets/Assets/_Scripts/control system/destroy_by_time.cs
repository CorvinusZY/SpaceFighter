using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destroy_by_time : MonoBehaviour {

	public float destroy_time;
	// Use this for initialization
	void Start () {
		Destroy (gameObject, destroy_time);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
