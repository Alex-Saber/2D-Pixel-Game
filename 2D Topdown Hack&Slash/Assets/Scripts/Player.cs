using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour {

	/*
		Inventory
		Stats Skills and Levels
	*/

	public float moveSpeed;

	public Rigidbody2D myRigidBody;

	// Use this for initialization
	void Start () {
		moveSpeed = 5f;

		myRigidBody = gameObject.GetComponent<Rigidbody2D> ();
	}
	
	// Update is called once per frame
	void Update () {

		if (isLocalPlayer) {
			if (Input.GetAxisRaw ("Horizontal") > 0f) {
				myRigidBody.velocity = new Vector3 (moveSpeed, myRigidBody.velocity.y, 0f);
			} else if (Input.GetAxisRaw ("Horizontal") < 0f) {
				myRigidBody.velocity = new Vector3 (-moveSpeed, myRigidBody.velocity.y, 0f);
			} else {
				myRigidBody.velocity = new Vector3 (0f, myRigidBody.velocity.y, 0f);
			}

			if (Input.GetAxisRaw ("Vertical") > 0f) {
				myRigidBody.velocity = new Vector3 (myRigidBody.velocity.x, moveSpeed, 0f);
			} else if (Input.GetAxisRaw ("Vertical") < 0f) {
				myRigidBody.velocity = new Vector3 (myRigidBody.velocity.x, -moveSpeed, 0f);
			} else {
				myRigidBody.velocity = new Vector3 (myRigidBody.velocity.x, 0f, 0f);
			}
		}
	}

	public override void OnStartLocalPlayer() {
		gameObject.GetComponent<SpriteRenderer> ().color = Color.cyan;
	}
}
