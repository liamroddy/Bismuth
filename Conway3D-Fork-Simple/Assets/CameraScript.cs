using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
	// Start is called before the first frame update
	void Start()
	{
		// setup the camera to look at the zero vector from a suitable distance and angle:
		Camera.main.transform.position = new Vector3(50f, 50f, 50f);
		Camera.main.transform.LookAt(Vector3.zero);
	}

	// Update is called once per frame
	void Update()
	{
		// MOUSE CONTROLS		
		if (Input.GetMouseButton(0))
		{

			Camera.main.transform.RotateAround(Vector3.zero, Camera.main.transform.up, 500f * Time.deltaTime * Input.GetAxis("Mouse X"));
			Camera.main.transform.RotateAround(Vector3.zero, Camera.main.transform.right, -500f * Time.deltaTime * Input.GetAxis("Mouse Y"));
		}

		float scrollAmount = Input.GetAxis("Mouse ScrollWheel");


		if ((scrollAmount < 0 && Vector3.Distance(Vector3.zero, Camera.main.transform.position) > 25f) || scrollAmount > 0) // don't allow to zoom to far in, ooherwise can't zoom back out
			Camera.main.transform.position = Vector3.MoveTowards(Camera.main.transform.position, Vector3.zero, (1 / scrollAmount) * -70f * Time.deltaTime);


		// KEYBOARD CONTROLS:
		Vector3 centre = Vector3.zero;

		// Zoom in and out
		if (Input.GetKey("up") && Vector3.Distance(Vector3.zero, Camera.main.transform.position) > 25f) // don't allow to zoom to far in, ooherwise can't zoom back out
		{
			Camera.main.transform.position = Vector3.MoveTowards(Camera.main.transform.position, centre, 50f * Time.deltaTime);
		}
		if (Input.GetKey("down"))
		{
			Camera.main.transform.position = Vector3.MoveTowards(Camera.main.transform.position, centre, -50f * Time.deltaTime);
		}

		// Rotate camera around on x plane (left to right)
		if (Input.GetKey(KeyCode.A))
		{
			Camera.main.transform.RotateAround(Vector3.zero, Camera.main.transform.up, 50f * Time.deltaTime);
		}
		if (Input.GetKey(KeyCode.D))
		{
			Camera.main.transform.RotateAround(Vector3.zero, Camera.main.transform.up, -50f * Time.deltaTime);
		}

		// Rotate camera around on y plane (top to bottom)
		if (Input.GetKey(KeyCode.W))
		{
			Camera.main.transform.RotateAround(Vector3.zero, Camera.main.transform.right, 50f * Time.deltaTime);
		}
		if (Input.GetKey(KeyCode.S))
		{
			Camera.main.transform.RotateAround(Vector3.zero, Camera.main.transform.right, -50f * Time.deltaTime);
		}
	}
}
