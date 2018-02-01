using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]

public class Boundary 
{
	public float xmin, xmax, zmin, zmax;
}


public class playerControl : MonoBehaviour {

	public float speed;
	public Rigidbody rg;
	public Boundary bound;
	public float tilt;
	private float xmin, xmax, zmin, zmax;


	public AudioSource sound;
	public float shotrate;
	private float nextshot = 0.0f;
	public GameObject shot;
	public GameObject shotimprove;
	public Transform shotspawn;
	private gamecontrol mycontrol;


	// Use this for initialization
	void Start () {
		GameObject control = GameObject.FindWithTag ("gamecontrol");
		mycontrol = control.GetComponent<gamecontrol> ();
		rg = GetComponent<Rigidbody> ();
		sound = GetComponent<AudioSource> ();

	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButton("Fire1") && Time.time >= nextshot)
		{
			Instantiate (shot, shotspawn.position, shotspawn.rotation);
			nextshot = Time.time + shotrate;
			sound.Play ();

		}

		if (mycontrol.stageup()) {
			shot = shotimprove;
		}
	}

	void FixedUpdate()
	{
		float x_movement = Input.GetAxis ("Horizontal");
		float z_movement = Input.GetAxis ("Vertical");
		Vector3 speed_update = new Vector3 (x_movement, 0.0f, z_movement);
		rg.velocity = speed_update * speed;

		rg.position = new Vector3
		(
			Mathf.Clamp(rg.position.x, bound.xmin, bound.xmax),
			0.0f,
			Mathf.Clamp(rg.position.z, bound.zmin, bound.zmax)
		);

		rg.rotation = Quaternion.Euler (0.0f, 0.0f, rg.velocity.x * -tilt);

	}

}



