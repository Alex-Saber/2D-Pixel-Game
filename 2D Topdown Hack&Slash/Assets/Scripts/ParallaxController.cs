using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxController : MonoBehaviour {

	public Transform[] backgrounds;			// A list of all the transforms to parallax
	private float[] parallax_Scales; 		// The float corresponding to each background with the speed of movement for the parallax effect
	public float smoothing;					

	public Transform cam;
	public Vector3 previous_Cam_Pos;  

	// Called before Start() but after all GameObjects are created (used for references)
	void Awake() {
		cam = Camera.main.transform;
	}

	// Use this for initialization
	void Start () {
		previous_Cam_Pos = cam.position;

		parallax_Scales = new float[backgrounds.Length];

		smoothing = 3f;

		for (int i = 0; i < backgrounds.Length; i++) {
			parallax_Scales [i] = (-smoothing)/(i + 1);
		}
	}
	
	// Update is called once per frame
	void Update () {

		// for each background
		for (int i = 0; i < backgrounds.Length; i++) {
			// The parallax is the opposite movement of the camera 
			float y_parallax = (cam.position.y - previous_Cam_Pos.y) * parallax_Scales[i];
			float x_parallax = (cam.position.x - previous_Cam_Pos.x) * parallax_Scales[i];

			// Create Target position for backgrounds
			float y_pos = backgrounds[i].position.y + y_parallax;
			float x_pos = backgrounds[i].position.x + x_parallax;

			// Create Vector3 target for background to move to
			Vector3 target = new Vector3(x_pos, y_pos, backgrounds[i].position.z);

			//Now lerp between current background position and target position
			backgrounds[i].position = Vector3.Lerp(backgrounds[i].position, target, smoothing * Time.deltaTime);
		}
		previous_Cam_Pos = cam.position;
	}
}
