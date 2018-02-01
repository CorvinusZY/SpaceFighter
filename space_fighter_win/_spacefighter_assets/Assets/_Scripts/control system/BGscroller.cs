using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGscroller : MonoBehaviour {

	public float scrollspeed;
	public float tilesize;
	private Vector3 startPos;
	// Use this for initialization
	void Start () {
		startPos = transform.position;
		transform.position = startPos;
	}
	
	// Update is called once per frame
	void Update () {
		float newPos = Mathf.Repeat (Time.time * scrollspeed, tilesize);

		transform.position = startPos + Vector3.forward * newPos;

	}
}
