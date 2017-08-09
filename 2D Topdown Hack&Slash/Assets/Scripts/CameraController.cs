using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	public GameObject target;

	public float followAhead;
	public float smoothing;

	private Vector3 targetPosition;

	public bool followTarget;

	// Use this for initialization
	void Start () {
		followAhead = 3f;
		smoothing = 2f;
		target = GameObject.Find ("Player");
		followTarget = true;

	}

	// Update is called once per frame
	void Update () {
		if (followTarget) {
			targetPosition = new Vector3 (target.transform.position.x, target.transform.position.y, -10f);



			//transform.position = targetPosition;

			transform.position = Vector3.Lerp (transform.position, targetPosition, smoothing * Time.deltaTime);
		}

	}
}