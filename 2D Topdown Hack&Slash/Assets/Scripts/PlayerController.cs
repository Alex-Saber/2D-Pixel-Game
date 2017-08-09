using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {


	public float horizontal_Move_Speed;
	public float vertical_Move_Speed;

	public float dash_Speed;
	public float slash_Speed;

	public Animator player_Anim;

	public Rigidbody2D player_RigidBody;

	public Transform player_Transform;

	public bool is_Dashing;
	public bool is_Attacking;
	public bool is_Taking_Damage;


	// Use this for initialization
	void Start () {
		player_Anim = gameObject.GetComponent<Animator> ();
		player_RigidBody = gameObject.GetComponent<Rigidbody2D> ();
		player_Transform = gameObject.GetComponent<Transform> ();

		horizontal_Move_Speed = 1.5f;
		vertical_Move_Speed = 1.1f;

		dash_Speed = 2f;
		slash_Speed = 0.8f;

	}
	
	// Update is called once per frame
	void Update () {

		//Can move if not taking damage, dashing and/or attacking.
		if (!is_Dashing && !is_Attacking && !is_Taking_Damage) 
		{
			//Checking for Horizontal Movement
			if (Input.GetAxisRaw ("Horizontal") > 0) {
				player_RigidBody.velocity = new Vector3 (horizontal_Move_Speed, player_RigidBody.velocity.y, 0f);
				player_Anim.SetFloat ("Speed", 1f);
				player_Transform.localScale = new Vector3 (1, 1, 1);
			} else if (Input.GetAxisRaw ("Horizontal") < 0) {
				player_RigidBody.velocity = new Vector3 (-horizontal_Move_Speed, player_RigidBody.velocity.y, 0f);
				player_Anim.SetFloat ("Speed", 1f);
				player_Transform.localScale = new Vector3 (-1, 1, 1);
			} else {
				player_RigidBody.velocity = new Vector3 (0f, player_RigidBody.velocity.y, 0f);
			}

			//Checking for Vertical Movement
			if (Input.GetAxisRaw ("Vertical") > 0) {
				player_RigidBody.velocity = new Vector3 (player_RigidBody.velocity.x, vertical_Move_Speed, 0f);
				player_Anim.SetFloat ("Speed", 1f);

			} else if (Input.GetAxisRaw ("Vertical") < 0) {
				player_RigidBody.velocity = new Vector3 (player_RigidBody.velocity.x, -vertical_Move_Speed, 0f);
				player_Anim.SetFloat ("Speed", 1f);

			} else {
				player_RigidBody.velocity = new Vector3 (player_RigidBody.velocity.x, 0f, 0f);
			}

			//Checking for no Movement
			if (Input.GetAxisRaw ("Horizontal") == 0f && Input.GetAxisRaw ("Vertical") == 0f) {
				player_Anim.SetFloat ("Speed", 0f);
			}
		}

	
		//Dash Control
		if (Input.GetKeyDown (KeyCode.Space) && is_Dashing == false) {
			is_Dashing = true;
			player_Anim.SetBool ("Dash", true);
			player_RigidBody.velocity = Vector3.zero;


			//Control Player Movement During Dash
			Vector3 movementDirection = (Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0f,0f,10f)) - player_Transform.position;
			player_RigidBody.AddForce ((movementDirection/movementDirection.magnitude) * dash_Speed, ForceMode2D.Impulse);

			//Changing character direction
			//If clicking to the left of the character make him face left
			if (transform.localScale.x > 0 && (Camera.main.ScreenToWorldPoint (Input.mousePosition) + new Vector3 (0f, 0f, 10f)).x < transform.position.x) {
				transform.localScale = new Vector3 (-1, 1, 1);
				transform.position = new Vector3 (transform.position.x - 0.08f,transform.position.y,transform.position.z);
			} 
			//If clicking to the right of the character make him face right
			else if (transform.localScale.x < 0 && (Camera.main.ScreenToWorldPoint (Input.mousePosition) + new Vector3 (0f, 0f, 10f)).x > transform.position.x) {
				transform.localScale = new Vector3 (1, 1, 1);
				transform.position = new Vector3 (transform.position.x + 0.08f,transform.position.y,transform.position.z);
			}
		}

		if (is_Dashing == true && player_Anim.GetCurrentAnimatorStateInfo (0).IsName("Dash") && player_Anim.GetCurrentAnimatorStateInfo (0).normalizedTime >= 0.8f) {
			is_Dashing = false;
			player_Anim.SetBool ("Dash", false);
		}


		//Slash Control
		if (Input.GetKeyDown (KeyCode.Mouse0) && is_Dashing == false && is_Attacking == false) {
			is_Attacking = true;
			player_Anim.SetBool ("Slash", true);
			player_RigidBody.velocity = Vector3.zero;


			//Control Player Movement During Slash
			Vector3 movementDirection = (Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0f,0f,10f)) - player_Transform.position;
			player_RigidBody.AddForce ((movementDirection/movementDirection.magnitude) * slash_Speed, ForceMode2D.Impulse);


			//Changing character direction
			//If clicking to the left of the character make him face left
			if (transform.localScale.x > 0 && (Camera.main.ScreenToWorldPoint (Input.mousePosition) + new Vector3 (0f, 0f, 10f)).x < transform.position.x) {
				transform.localScale = new Vector3 (-1, 1, 1);
				transform.position = new Vector3 (transform.position.x - 0.08f,transform.position.y,transform.position.z);
			} 
			//If clicking to the right of the character make him face right
			else if (transform.localScale.x < 0 && (Camera.main.ScreenToWorldPoint (Input.mousePosition) + new Vector3 (0f, 0f, 10f)).x > transform.position.x) {
				transform.localScale = new Vector3 (1, 1, 1);
				transform.position = new Vector3 (transform.position.x + 0.08f,transform.position.y,transform.position.z);
			}

			//****************** Spawn Slash Object in direction of mouse click ********************//
			Vector3 slash_Direction = (Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0f,0f,10f)) - player_Transform.position;
			slash_Direction = slash_Direction/(slash_Direction.magnitude * 15);

			GameObject slash = (GameObject)Instantiate(Resources.Load ("Slash_0"));
			slash.transform.position = player_Transform.position + slash_Direction;
			slash.transform.up = slash.transform.position - player_Transform.position;
			

		}

		if (is_Attacking == true && player_Anim.GetCurrentAnimatorStateInfo (0).IsName("Slash 1") && player_Anim.GetCurrentAnimatorStateInfo (0).normalizedTime >= 0.8f) {
			is_Attacking = false;
			player_Anim.SetBool ("Slash", false);
		}





		//END OF UPDATE
	}
		
}
