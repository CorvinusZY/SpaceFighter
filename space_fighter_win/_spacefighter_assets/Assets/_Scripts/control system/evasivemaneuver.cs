using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class evasivemaneuver : MonoBehaviour {
	public float dodge;
	public float smoothing;
	public float tilt;
	public Vector2 startwait;
	public Vector2 maneuverTime;
	public Vector2 maneuverWait;
	public Boundary bound;


	private float targetmaneuver;
	private Rigidbody rg;
	private float currentspeed_z;
	// Use this for initialization
	void Start () {
		rg = GetComponent<Rigidbody> ();
		StartCoroutine (Evade());
		currentspeed_z = rg.velocity.z;
	}

	IEnumerator Evade(){
		yield return new WaitForSeconds (Random.Range(startwait.x, startwait.y));
		while (true) {
			targetmaneuver = Random.Range (1, dodge) * - Mathf.Sign(transform.position.x);
			yield return new WaitForSeconds (Random.Range(maneuverTime.x, maneuverTime.y));
			targetmaneuver = 0;
			yield return new WaitForSeconds (Random.Range(maneuverWait.x, maneuverWait.y));
		}

	}

	
	// Update is called once per frame
	void FixedUpdate () {
		float newmaneuver = Mathf.MoveTowards (rg.velocity.x, targetmaneuver, Time.deltaTime * smoothing);
		rg.velocity = new Vector3 (newmaneuver, 0.0f, currentspeed_z);
		rg.position = new Vector3(
			Mathf.Clamp(rg.position.x, bound.xmin, bound.xmax),
			0.0f,
			Mathf.Clamp(rg.position.z, bound.zmin, bound.zmax)
			);

		//rg.rotation = Quaternion.Euler(0.0f, 0.0f, rg.velocity.x * -tilt);
	}
}
