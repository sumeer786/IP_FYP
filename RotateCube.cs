using UnityEngine;
using UnityEngine.EventSystems;

public class RotateCube : MonoBehaviour {
	public static bool isRotating;
	public static bool canRotate;
	bool overUI = false;
	public Rigidbody rb;

	void Start() {
		isRotating = false;
		canRotate = true;
	}

	void Update() {
		// Finished Rotating
		if(Input.GetMouseButtonUp(0)) {
			overUI = false;
			isRotating = false;
		}

		if(EventSystem.current.IsPointerOverGameObject(0) || EventSystem.current.IsPointerOverGameObject()) {
			overUI = true;
			isRotating = true; 
        }

		if(Input.GetMouseButtonDown(0)) {
			Vector2 clickPosition = Input.mousePosition;
    		Ray ray = Camera.main.ScreenPointToRay(clickPosition);

    		if(!Physics.Raycast(ray, out RaycastHit hit)) {
				isRotating = true;
			}
		}
	}
	
	void FixedUpdate() {
		if(isRotating && !overUI && canRotate) {
			float x = Input.GetAxis("Mouse X") * PlayerPrefs.GetFloat("cubeRotationSensitivity", 1000f) * Time.fixedDeltaTime;
			float y = Input.GetAxis("Mouse Y") * PlayerPrefs.GetFloat("cubeRotationSensitivity", 1000f) * Time.fixedDeltaTime;

			rb.AddTorque(Camera.main.transform.up * x);	
			rb.AddTorque(Camera.main.transform.right * -y);
		}
	}
}