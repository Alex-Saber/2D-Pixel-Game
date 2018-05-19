using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour 
{


	public float horizontal_Move_Speed;
	public float vertical_Move_Speed;

	public float dash_Speed;
	public float slash_Speed;

	public float anim_Scale;

	public Animator player_Anim;

	public Rigidbody2D player_RigidBody;

	public Transform player_Transform;

	public bool is_Dashing;
	public bool is_Attacking;
	public bool is_Taking_Damage;

	public int left;

	// Use this for initialization
	void Start () {

		if (!isLocalPlayer)
		{
			Destroy(this);
			return;
		}

		player_Anim = gameObject.GetComponent<Animator> ();
		player_RigidBody = gameObject.GetComponent<Rigidbody2D> ();
		player_Transform = gameObject.GetComponent<Transform> ();

		horizontal_Move_Speed = 1.5f;
		vertical_Move_Speed = 1.1f;

		dash_Speed = 2f;
		slash_Speed = 0.8f;

		anim_Scale = 1.105f;

		transform.gameObject.name = transform.gameObject.name + " " + (Random.Range(0, 100)).ToString();

		//net_anim = transform.gameObject.GetComponent<NetworkAnimator>();
		//net_anim.ani = transform.gameObject.GetComponent<Animator>();
	}
	
	public override void OnStartLocalPlayer(){
		Debug.Log("Player connected!");	
		CameraController c = Camera.main.GetComponent<CameraController>();
		if (c != null)
			c.target = 	gameObject;
		}

	// Update is called once per frame
	void FixedUpdate () {
		
		/***********************
		 |	Movement Control   |
		 ***********************/

		//Can move if not taking damage, dashing and attacking.
		if (!is_Dashing && !is_Attacking && !is_Taking_Damage) 
		{
			

			//Checking for Horizontal Movement (Left and Right)
			if (Input.GetAxisRaw ("Horizontal") > 0) {
				left = 1;
				player_RigidBody.velocity = new Vector3 (horizontal_Move_Speed, player_RigidBody.velocity.y, 0f);
				player_Anim.SetFloat ("Speed", 1f);
				player_Transform.localScale = new Vector3 (anim_Scale, anim_Scale, 1);
			} else if (Input.GetAxisRaw ("Horizontal") < 0) {
				left = -1;
				player_RigidBody.velocity = new Vector3 (-horizontal_Move_Speed, player_RigidBody.velocity.y, 0f);
				player_Anim.SetFloat ("Speed", 1f);
				player_Transform.localScale = new Vector3 (-anim_Scale, anim_Scale, 1);
			} else {
				player_RigidBody.velocity = new Vector3 (0f, player_RigidBody.velocity.y, 0f);
				player_Transform.localScale = (left == 1) ? new Vector3 (1, 1, 1) : new Vector3 (-1, 1, 1);
			}

			//Checking for Vertical Movement (Up and Down)
			if (Input.GetAxisRaw ("Vertical") > 0) {
				player_RigidBody.velocity = new Vector3 (player_RigidBody.velocity.x, vertical_Move_Speed, 0f);
				player_Anim.SetFloat ("Speed", 1f);
				player_Transform.localScale = (left == 1) ? new Vector3 (anim_Scale, anim_Scale, 1) : new Vector3 (-anim_Scale, anim_Scale, 1);
			} else if (Input.GetAxisRaw ("Vertical") < 0) {
				player_RigidBody.velocity = new Vector3 (player_RigidBody.velocity.x, -vertical_Move_Speed, 0f);
				player_Anim.SetFloat ("Speed", 1f);
				player_Transform.localScale = (left == 1) ? new Vector3 (anim_Scale, anim_Scale, 1) : new Vector3 (-anim_Scale, anim_Scale, 1);
			} else {
				player_RigidBody.velocity = new Vector3 (player_RigidBody.velocity.x, 0f, 0f);
				if (Input.GetAxisRaw ("Horizontal") == 0) 
				{
					player_Transform.localScale = (left == 1) ? new Vector3 (1, 1, 1) : new Vector3 (-1, 1, 1);
				}
			}

			//Checking for no Movement
			if (Input.GetAxisRaw ("Horizontal") == 0f && Input.GetAxisRaw ("Vertical") == 0f) {
				player_Anim.SetFloat ("Speed", 0f);
			}
		}
		/*****************************
		 |	  Movement Control End   |
		 *****************************/

		/***********************
		 |	   Dash Control    |
		 ***********************/
		if (Input.GetKeyDown (KeyCode.Space) && is_Dashing == false) {
			is_Dashing = true;
			player_Anim.SetBool ("Dash", true);
			player_RigidBody.velocity = Vector3.zero;


			//Control Player Movement During Dash
			Vector3 movementDirection = (Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0f,0f,10f)) - player_Transform.position;
			player_RigidBody.AddForce ((movementDirection/movementDirection.magnitude) * dash_Speed, ForceMode2D.Impulse);

			//Changing character direction based on dash direction
			//If clicking to the left of the character make him face left
			if (transform.localScale.x > 0 && (Camera.main.ScreenToWorldPoint (Input.mousePosition) + new Vector3 (0f, 0f, 10f)).x < transform.position.x) {
				left = -1;
				transform.localScale = new Vector3 (-1, 1, 1);
				transform.position = new Vector3 (transform.position.x - 0.08f,transform.position.y,transform.position.z);
			} 
			//If clicking to the right of the character make him face right
			else if (transform.localScale.x < 0 && (Camera.main.ScreenToWorldPoint (Input.mousePosition) + new Vector3 (0f, 0f, 10f)).x > transform.position.x) {
				left = 1;
				transform.localScale = new Vector3 (1, 1, 1);
				transform.position = new Vector3 (transform.position.x + 0.08f,transform.position.y,transform.position.z);
			}
		}

		if (is_Dashing == true && player_Anim.GetCurrentAnimatorStateInfo (0).IsName("Dash") && player_Anim.GetCurrentAnimatorStateInfo (0).normalizedTime >= 0.8f) {
			is_Dashing = false;
			player_Anim.SetBool ("Dash", false);
		}
		/**************************
		 |	  Dash Control End    |
		 **************************/


		/***********************
		 |	   Slash Control   |
		 ***********************/
		if (Input.GetKeyDown (KeyCode.Mouse0) && !is_Dashing && !is_Attacking) {
			is_Attacking = true;
			player_Anim.SetBool ("Slash", true);
			player_RigidBody.velocity = Vector3.zero;


			//Control Player Movement During Slash
			Vector3 movementDirection = (Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0f,0f,10f)) - player_Transform.position;
			player_RigidBody.AddForce ((movementDirection/movementDirection.magnitude) * slash_Speed, ForceMode2D.Impulse);


			//Changing character direction
			//If clicking to the left of the character make him face left
			if (transform.localScale.x > 0 && (Camera.main.ScreenToWorldPoint (Input.mousePosition) + new Vector3 (0f, 0f, 10f)).x < transform.position.x) {
				left = 1;
				transform.localScale = new Vector3 (-1, 1, 1);
				transform.position = new Vector3 (transform.position.x - 0.08f,transform.position.y,transform.position.z);
			} 
			//If clicking to the right of the character make him face right
			else if (transform.localScale.x < 0 && (Camera.main.ScreenToWorldPoint (Input.mousePosition) + new Vector3 (0f, 0f, 10f)).x > transform.position.x) {
				left = 0;
				transform.localScale = new Vector3 (1, 1, 1);
				transform.position = new Vector3 (transform.position.x + 0.08f,transform.position.y,transform.position.z);
			}

			player_Transform.localScale = (left == 1) ? new Vector3 (1, 1, 1) : new Vector3 (-1, 1, 1);

			//****************** Spawn Slash Object in direction of mouse click ********************//
			Vector3 slash_Direction = (Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0f,0f,10f)) - player_Transform.position;
			slash_Direction = slash_Direction/(slash_Direction.magnitude * 15);

			GameObject slash = (GameObject)Instantiate(Resources.Load ("Slash_0"));
			slash.transform.position = player_Transform.position + slash_Direction;
			slash.transform.up = slash.transform.position - player_Transform.position;
		}
		/**************************
		 |	  Slash Control End   |
		 **************************/


		//If player is currently attacking allow animation to terminate.
		if (is_Attacking == true && player_Anim.GetCurrentAnimatorStateInfo (0).IsName("Slash 1") && player_Anim.GetCurrentAnimatorStateInfo (0).normalizedTime >= 0.8f) {
			is_Attacking = false;
			player_Anim.SetBool ("Slash", false);
		}



		//END OF UPDATE
	}
		
}
