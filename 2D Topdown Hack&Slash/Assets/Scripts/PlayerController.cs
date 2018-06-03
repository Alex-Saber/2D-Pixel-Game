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

	public Vector3 anim_Scale;

	Vector3 Offset;

	public Animator player_Anim;

	public Rigidbody2D player_RigidBody;

	public Transform player_Transform;

	public bool is_Dashing;
	public bool is_Attacking;
	public bool is_Taking_Damage;

	// Direction is 1 for right and -1 for left.
	public int direction;

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

		dash_Speed = 3f;
		slash_Speed = 0.8f;

		anim_Scale = new Vector3(1.105f, 1.105f, 1);

		direction = 1;

		Offset = new Vector3(0.235f,-0.23f,0);
		
		//net_anim = transform.gameObject.GetComponent<NetworkAnimator>();
		//net_anim.ani = transform.gameObject.GetComponent<Animator>();
	}
	
	// This is called everytime a new player connects to the server
	public override void OnStartLocalPlayer() {
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
			
			bool moving;
			//Checking for Horizontal Movement (Left and Right)
			if (Input.GetAxisRaw ("Horizontal") > 0) 
			{
				player_RigidBody.velocity = new Vector3 (horizontal_Move_Speed, player_RigidBody.velocity.y, 0f);
				player_Anim.SetFloat ("Speed", 1f);
				player_Anim.SetInteger ("direction", 1);
				moving = true;
				direction = 1;
			} 
			else if (Input.GetAxisRaw ("Horizontal") < 0) 
			{
				player_RigidBody.velocity = new Vector3 (-horizontal_Move_Speed, player_RigidBody.velocity.y, 0f);
				player_Anim.SetFloat ("Speed", -1f);
				player_Anim.SetInteger ("direction", -1);
				moving = true;
				direction = -1;
			} 
			else 
			{
				moving = false;
				player_RigidBody.velocity = new Vector3 (0f, player_RigidBody.velocity.y, 0f);
			}

			//Checking for Vertical Movement (Up and Down)
			if (Input.GetAxisRaw ("Vertical") > 0) 
			{
				player_RigidBody.velocity = new Vector3 (player_RigidBody.velocity.x, vertical_Move_Speed, 0f);
				player_Anim.SetFloat ("Speed", direction);
				moving = true;
			} 
			else if (Input.GetAxisRaw ("Vertical") < 0) 
			{
				player_RigidBody.velocity = new Vector3 (player_RigidBody.velocity.x, -vertical_Move_Speed, 0f);
				player_Anim.SetFloat ("Speed", direction);
				moving = true;
			} 
			else 
			{
				player_RigidBody.velocity = new Vector3 (player_RigidBody.velocity.x, 0f, 0f);
			}

			//Checking for no Movement
			if (Input.GetAxisRaw ("Horizontal") == 0f && Input.GetAxisRaw ("Vertical") == 0f) 
			{
				moving = false;
				player_Anim.SetFloat ("Speed", 0f);
			}

			//Adjusting size of animation (Will be fixed later in animations).
			if (moving) 
			{
				player_Transform.localScale = anim_Scale;
			}
			else 
			{
				player_Transform.localScale = new Vector3 (1,1,1);
			}
		}

		/***********************
		 |	   Dash Control    |
		 ***********************/
		if (Input.GetKeyDown (KeyCode.Space) && is_Dashing == false) 
		{
			is_Dashing = true;
			
			player_RigidBody.velocity = Vector3.zero;

			Vector3 mousePos = Input.mousePosition;
			mousePos.z = 10;
			//Control Player Movement During Dash
			Vector3 movementDirection = (Camera.main.ScreenToWorldPoint(mousePos)) - (player_Transform.position + Offset);
			player_RigidBody.AddForce ((movementDirection/movementDirection.magnitude) * dash_Speed, ForceMode2D.Impulse);

			//Changing character direction based on dash direction
			//If clicking to the left of the character make him face left
			if (movementDirection.x < 0) 
			{
				direction = -1;
				player_Anim.SetInteger("direction", -1);
				player_Anim.SetBool ("Dash", true);
			} 
			//If clicking to the right of the character make him face right
			else if (movementDirection.x > 0) {
				direction = 1;
				player_Anim.SetInteger("direction", 1);	
				player_Anim.SetBool ("Dash", true);
			}

			
		}

		if (is_Dashing == true && 
			(player_Anim.GetCurrentAnimatorStateInfo (0).IsName("Dash Right") ||
			player_Anim.GetCurrentAnimatorStateInfo (0).IsName("Dash Left")) &&
			player_Anim.GetCurrentAnimatorStateInfo (0).normalizedTime >= 0.5f) 
		{

			is_Dashing = false;
			player_Anim.SetBool ("Dash", false);
		}



		/***********************
		 |	   Slash Control   |
		 ***********************/
		if (Input.GetKeyDown (KeyCode.Mouse0) && !is_Dashing && !is_Attacking) {
			is_Attacking = true;
			
			player_RigidBody.velocity = Vector3.zero;

			Vector3 mousePos = Input.mousePosition;
			mousePos.z = 10;
			//Control Player Movement During Slash
			Vector3 slashDirection = (Camera.main.ScreenToWorldPoint(mousePos)) - (player_Transform.position + Offset);

			//GameObject point = (GameObject)Instantiate(Resources.Load("point"));

			
			//point.transform.position = player_Transform.position + Offset;
			

			player_RigidBody.AddForce ((slashDirection/slashDirection.magnitude) * slash_Speed, ForceMode2D.Impulse);


			//Changing character direction
			//If clicking to the left of the character make him face left
			if (slashDirection.x < 0) {
				direction = -1;
				player_Anim.SetInteger("direction", -1);
				player_Anim.SetBool ("Slash", true);
			}
			//If clicking to the right of the character make him face right
			else if (slashDirection.x > 0) {
				direction = 1;
				player_Anim.SetInteger("direction", 1);
				player_Anim.SetBool ("Slash", true);
			}


			//****************** Spawn Slash Object in direction of mouse click ********************
			
			slashDirection = slashDirection/(slashDirection.magnitude * 10);

			GameObject slash = (GameObject)Instantiate(Resources.Load ("Slash_0"));
			slash.transform.position = (player_Transform.position + Offset) + slashDirection;
			slash.transform.up = slash.transform.position - (player_Transform.position + Offset);
		}

		//If player is currently attacking allow animation to terminate.
		if (is_Attacking == true && (player_Anim.GetCurrentAnimatorStateInfo (0).IsName("Slash 1 Left") || player_Anim.GetCurrentAnimatorStateInfo (0).IsName("Slash 1 Right")) && player_Anim.GetCurrentAnimatorStateInfo (0).normalizedTime >= 0.5f) {
			is_Attacking = false;
			player_Anim.SetBool ("Slash", false);
		}
		


		//END OF UPDATE
	}
		
}
