using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashController : MonoBehaviour {
	public Animator my_Anim;

	// Use this for initialization
	void Start () {
		my_Anim = gameObject.GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (my_Anim.GetCurrentAnimatorStateInfo (0).IsName ("Slash") && my_Anim.GetCurrentAnimatorStateInfo (0).normalizedTime >= 0.8f) {
			Destroy (gameObject);
		}
	}
}
