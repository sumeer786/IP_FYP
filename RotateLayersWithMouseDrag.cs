using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RotateLayersWithMouseDrag : MonoBehaviour {
	public float minimumDragPixels = 20.0f;
    public float snapSpeed = 250.0f;
    public static bool canRotate, isRotating;

    Collider[] subCubes = new Collider[9];
    Vector3[] originalPositions = new Vector3[9];
    Quaternion[] originalOrientations = new Quaternion[9];

    RotateLayers rl;

    void Start() {
        canRotate = true;
        isRotating = false;
    	rl = GameObject.Find("Cube").GetComponent<RotateLayers>();
    	StartCoroutine(Rotate());
    }

    IEnumerator Rotate() {
    	while(true) {	
    		Vector3 camForward = Camera.main.transform.forward;
    		float axisSign = Mathf.Sign(camForward.x * camForward.y * camForward.z);
    		yield return null;

    		if(!Input.GetMouseButton(0) || EventSystem.current.IsPointerOverGameObject(0) || EventSystem.current.IsPointerOverGameObject() || RotateCube.isRotating || rl.queue.Count != 0 || !canRotate) {
    			continue;
			}

    		Vector2 clickPosition = Input.mousePosition;
    		Ray ray = Camera.main.ScreenPointToRay(clickPosition);

    		if(!Physics.Raycast(ray, out RaycastHit hit) || hit.transform.name == "Rotate") {
    			continue;
			}

    		int normalAxis = Mathf.Abs(Mathf.RoundToInt(Vector3.Dot(hit.normal, new Vector3(0, 1, 2))));

    		Vector3 rotationAxis = Vector3.zero;
    		Vector3 alternativeAxis = Vector3.zero;

    		rotationAxis[(normalAxis + 1) % 3] = 1;
    		alternativeAxis[(normalAxis + 2) % 3] = 1;

    		float signFlip = axisSign * Mathf.Sign(Vector3.Dot(rotationAxis, camForward) * Mathf.Sign(Vector3.Dot(alternativeAxis, camForward)));
    		Vector2 rotationDirection = signFlip* ScreenDirection(clickPosition, hit.point, alternativeAxis); 
    		Vector2 alternativeDirection = -signFlip * ScreenDirection(clickPosition, hit.point, rotationAxis);

    		float signedDistance;
    		do {
    			yield return null;

    			Vector2 mousePosition = Input.mousePosition;
    			signedDistance = DistanceAlong(clickPosition, mousePosition, rotationDirection);
    			if(Mathf.Abs(signedDistance) > minimumDragPixels)
    				break;

    			signedDistance = DistanceAlong(clickPosition, mousePosition, alternativeDirection);
    			if(Mathf.Abs(signedDistance) > minimumDragPixels) {
    				rotationAxis = alternativeAxis;
    				rotationDirection = alternativeDirection;
    				break;
    			}
    		} while(Input.GetMouseButton(0));

    		Vector3 extents = Vector3.one - 0.9f * rotationAxis;
    		extents = extents * 2.0f;
    		int subCubeCount = Physics.OverlapBoxNonAlloc(hit.collider.transform.position, extents, subCubes);
			if(subCubeCount < 8) {
				continue;
			}

			isRotating = true;

    		for(int i = 0; i < subCubeCount; i++) {
    			var subCube = subCubes[i].transform;
    			originalPositions[i] = subCube.position;
    			originalOrientations[i] = subCube.rotation;
    		}

			bool isFlick = true, atStart = true, signFlag = true;
    		float angle = 0.0f, time = 0.0f, lastAngle = 0.0f;
    		while(Input.GetMouseButton(0)) {
				lastAngle = angle;
    			angle = signedDistance * PlayerPrefs.GetFloat("layersRotationSensitivity", 0.1f);
				if(atStart) {
					signFlag = angle > 0 ? true : false;
					atStart = false;
				}

				if(angle != lastAngle) {
					bool currentSignFlag = (angle - lastAngle) > 0 ? true : false;
					if(signFlag != currentSignFlag) {
						isFlick = false;
					}
				}
    			RotateGroup(angle, rotationAxis, subCubeCount);

    			yield return null;
    			Vector2 mousePosition = Input.mousePosition;
    			signedDistance = DistanceAlong(clickPosition, mousePosition, rotationDirection);

				time += Time.deltaTime;
    		}

			float newAngle = angle;
			if(isFlick && time > 0.03f && time < 0.3f) {
				newAngle += angle > 0 ? 49 : -49;
			}

    		float snappedAngle = Mathf.Round(newAngle / 90.0f) * 90.0f;
    		while(angle != snappedAngle) {
    			angle = Mathf.MoveTowards(angle, snappedAngle, snapSpeed * Time.deltaTime);

    			RotateGroup(angle, rotationAxis, subCubeCount);
    			yield return null;
    		}
    		PerformOtherOperations(subCubeCount);
    	}
    }

	Vector2 ScreenDirection(Vector2 screenPoint, Vector3 worldPoint, Vector3 worldDirection) {
    	Vector2 shifted = Camera.main.WorldToScreenPoint(worldPoint + worldDirection);
    	return (shifted - screenPoint).normalized;
    }

    float DistanceAlong(Vector2 clickPosition, Vector2 currentPosition, Vector2 direction) {
    	return Vector2.Dot(currentPosition - clickPosition, direction);
    }

    void RotateGroup(float angle, Vector3 axis, int count) {
    	Quaternion rotation = Quaternion.AngleAxis(angle, axis);
    	for (int i = 0; i < count; i++) {
            var subCube = subCubes[i].transform;
            subCube.position = rotation * originalPositions[i];
            subCube.rotation = rotation * originalOrientations[i];
        }
    }

    bool Equal(Vector3 a, Vector3 b) {
        return Vector3.SqrMagnitude(a - b) < 0.001;
    }

    void PerformOtherOperations(int count) {
    	HashSet<GameObject> layer = new HashSet<GameObject>();
    	for(int i = 0; i < count; i++) {
    		layer.Add(subCubes[i].gameObject);
    	}

    	if(layer.Contains(rl.cube[0]) && layer.Contains(rl.cube[4])) {
    		if(Equal(rl.cube[0].transform.position, new Vector3(1, 1, -1))) {
    			rl.cfopSolver.U(0);
    			rl.SwapU(0);
    		}
    		else if(Equal(rl.cube[0].transform.position, new Vector3(-1, 1, 1))) {
    			rl.cfopSolver.U(1);
    			rl.SwapU(1);
    		}
    		else if(Equal(rl.cube[0].transform.position, new Vector3(-1, 1, -1))) {
    			rl.cfopSolver.U(2);
    			rl.SwapU(2);
    		}
    	}

    	else if(layer.Contains(rl.cube[18]) && layer.Contains(rl.cube[22])) {
    		if(Equal(rl.cube[18].transform.position, new Vector3(-1, -1, 1))) {
    			rl.cfopSolver.D(0);
    			rl.SwapD(0);
    		}
    		else if(Equal(rl.cube[18].transform.position, new Vector3(1, -1, -1))) {
    			rl.cfopSolver.D(1);
    			rl.SwapD(1);
    		}
    		else if(Equal(rl.cube[18].transform.position, new Vector3(-1, -1, -1))) {
    			rl.cfopSolver.D(2);
    			rl.SwapD(2);
    		}
    	}

    	else if(layer.Contains(rl.cube[6]) && layer.Contains(rl.cube[16])) {
    		if(Equal(rl.cube[6].transform.position, new Vector3(-1, 1, -1))) {
    			rl.cfopSolver.F(0);
    			rl.SwapF(0);
    		}
    		else if(Equal(rl.cube[6].transform.position, new Vector3(-1, -1, 1))) {
    			rl.cfopSolver.F(1);
    			rl.SwapF(1);
    		}
    		else if(Equal(rl.cube[6].transform.position, new Vector3(-1, -1, -1))) {
    			rl.cfopSolver.F(2);
    			rl.SwapF(2);
    		}
    	}

    	else if(layer.Contains(rl.cube[2]) && layer.Contains(rl.cube[10])) {
    		if(Equal(rl.cube[2].transform.position, new Vector3(1, 1, 1))) {
    			rl.cfopSolver.B(0);
    			rl.SwapB(0);
    		}
    		else if(Equal(rl.cube[2].transform.position, new Vector3(1, -1, -1))) {
    			rl.cfopSolver.B(1);
    			rl.SwapB(1);
    		}
    		else if(Equal(rl.cube[2].transform.position, new Vector3(1, -1, 1))) {
    			rl.cfopSolver.B(2);
    			rl.SwapB(2);
    		}
    	}

    	else if(layer.Contains(rl.cube[0]) && layer.Contains(rl.cube[12])) {
    		if(Equal(rl.cube[0].transform.position, new Vector3(-1, 1, 1))) {
    			rl.cfopSolver.L(0);
    			rl.SwapL(0);
    		}
    		else if(Equal(rl.cube[0].transform.position, new Vector3(1, -1, 1))) {
    			rl.cfopSolver.L(1);
    			rl.SwapL(1);
    		}
    		else if(Equal(rl.cube[0].transform.position, new Vector3(-1, -1, 1))) {
    			rl.cfopSolver.L(2);
    			rl.SwapL(2);
    		}
    	}

    	else if(layer.Contains(rl.cube[8]) && layer.Contains(rl.cube[14])) {
    		if(Equal(rl.cube[8].transform.position, new Vector3(1, 1, -1))) {
    			rl.cfopSolver.R(0);
    			rl.SwapR(0);
    		}
    		else if(Equal(rl.cube[8].transform.position, new Vector3(-1, -1, -1))) {
    			rl.cfopSolver.R(1);
    			rl.SwapR(1);
    		}
    		else if(Equal(rl.cube[8].transform.position, new Vector3(1, -1, -1))) {
    			rl.cfopSolver.R(2);
    			rl.SwapR(2);
    		}
    	}

    	else if(layer.Contains(rl.cube[4]) && layer.Contains(rl.cube[16])) {
    		if(Equal(rl.cube[4].transform.position, new Vector3(-1, 0, 0))) {
    			rl.cfopSolver.M(0);
    			rl.SwapM(0);
    		}
    		else if(Equal(rl.cube[4].transform.position, new Vector3(1, 0, 0))) {
    			rl.cfopSolver.M(1);
    			rl.SwapM(1);
    		}
    		else if(Equal(rl.cube[4].transform.position, new Vector3(0, -1, 0))) {
    			rl.cfopSolver.M(2);
    			rl.SwapM(2);
    		}
    	}

    	else if(layer.Contains(rl.cube[16]) && layer.Contains(rl.cube[14])) {
    		if(Equal(rl.cube[16].transform.position, new Vector3(0, 0, -1))) {
    			rl.cfopSolver.E(0);
    			rl.SwapE(0);
    		}
    		else if(Equal(rl.cube[16].transform.position, new Vector3(0, 0, 1))) {
    			rl.cfopSolver.E(1);
    			rl.SwapE(1);
    		}
    		else if(Equal(rl.cube[16].transform.position, new Vector3(1, 0, 0))) {
    			rl.cfopSolver.E(2);
    			rl.SwapE(2);
    		}
    	}

    	else if(layer.Contains(rl.cube[4]) && layer.Contains(rl.cube[14])) {
    		if(Equal(rl.cube[4].transform.position, new Vector3(0, 0, -1))) {
    			rl.cfopSolver.S(0);
    			rl.SwapS(0);
    		}
    		else if(Equal(rl.cube[4].transform.position, new Vector3(0, 0, 1))) {
    			rl.cfopSolver.S(1);
    			rl.SwapS(1);
    		}
    		else if(Equal(rl.cube[4].transform.position, new Vector3(0, -1, 0))) {
    			rl.cfopSolver.S(2);
    			rl.SwapS(2);
    		}
    	}
        isRotating = false;
    }
}