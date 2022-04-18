using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RotateLayers : MonoBehaviour {
	public GameObject[] cube;
	public GameObject guideCanvas;
	public Transform rubiksCube, uLayer, dLayer, fLayer, bLayer, lLayer, rLayer, mLayer, eLayer, sLayer;
	public bool isRotating;
	public Queue<string> queue = new Queue<string>();
	public CFOPSolver cfopSolver;

	void Start() {
		cube = new GameObject[27];
		for(int i = 0; i < 27; i++) {
			if(i != 13) {
				cube[i] = GameObject.Find("Piece" + i);
			}
		}

		cfopSolver = new CFOPSolver();
	}

	void Update() {
		if(Input.anyKeyDown && !guideCanvas.activeSelf) {
			string input = Input.inputString;
			if(input == "u" || input == "d" || input == "f" || input == "b" || input == "l" || input == "r" || input == "m" || input == "e" || input == "s") {
				queue.Enqueue(input.ToUpper());
			} else if(input == "U" || input == "D" || input == "F" || input == "B" || input == "L" || input == "R" || input == "M" || input == "E"|| input == "S") {
				queue.Enqueue(input + "'");
			} else if(input == "X" || input == "Y" || input == "Z") {
				queue.Enqueue(input.ToLower() + "'");
			} else {
				queue.Enqueue(input);
			}
		}

		if(!isRotating && !RotateLayersWithMouseDrag.isRotating && queue.Count > 0) {
			isRotating = true;
			string input = queue.Dequeue();
			if(input == "U") {
				U(0);
				cfopSolver.U(0);
			} else if(input == "F") {
				F(0);
				cfopSolver.F(0);
			} else if(input == "R") {
				R(0);
				cfopSolver.R(0);
			} else if(input == "D") {
				D(0);
				cfopSolver.D(0);
			} else if(input == "B") {
				B(0);
				cfopSolver.B(0);
			} else if(input == "L") {
				L(0);
				cfopSolver.L(0);
			} else if(input == "M") {
				M(0);
				cfopSolver.M(0);
			} else if(input == "E") {
				E(0);
				cfopSolver.E(0);
			} else if(input == "S") {
				S(0);
				cfopSolver.S(0);
			} else if(input == "U'") {
				U(1);
				cfopSolver.U(1);
			} else if(input == "F'") {
				F(1);
				cfopSolver.F(1);
			} else if(input == "R'") {
				R(1);
				cfopSolver.R(1);
			} else if(input == "D'") {
				D(1);
				cfopSolver.D(1);
			} else if(input == "B'") {
				B(1);
				cfopSolver.B(1);
			} else if(input == "L'") {
				L(1);
				cfopSolver.L(1);
			} else if(input == "M'") {
				M(1);
				cfopSolver.M(1);
			} else if(input == "E'") {
				E(1);
				cfopSolver.E(1);
			} else if(input == "S'") {
				S(1);
				cfopSolver.S(1);
			} else if(input == "U2") {
				U(2);
				cfopSolver.U(2);
			} else if(input == "F2") {
				F(2);
				cfopSolver.F(2);
			} else if(input == "R2") {
				R(2);
				cfopSolver.R(2);
			} else if(input == "D2") {
				D(2);
				cfopSolver.D(2);
			} else if(input == "B2") {
				B(2);
				cfopSolver.B(2);
			} else if(input == "L2") {
				L(2);
				cfopSolver.L(2);
			} else if(input == "M2") {
				M(2);
				cfopSolver.M(2);
			} else if(input == "E2") {
				E(2);
				cfopSolver.E(2);
			} else if(input == "S2") {
				S(2);
				cfopSolver.S(2);
			} else if(input == "u") {
				U(0); E(1);
				cfopSolver.WideU(0);
			} else if(input == "r") {
				R(0); M(1);
				cfopSolver.WideR(0);
			} else if(input == "l") {
				L(0); M(0);
				cfopSolver.WideL(0);
			} else if(input == "u'") {
				U(1); E(0);
				cfopSolver.WideU(1);
			} else if(input == "r'") {
				R(1); M(0);
				cfopSolver.WideR(1);
			} else if(input == "l'") {
				L(1); M(1);
				cfopSolver.WideL(1);
			} else if(input == "u2") {
				U(2); E(2);
				cfopSolver.WideU(2);
			} else if(input == "r2") {
				R(2); M(2);
				cfopSolver.WideR(2);
			} else if(input == "l2") {
				L(2); M(2);
				cfopSolver.WideL(2);
			} else if(input == "x") {
				R(0); M(1); L(1);
				cfopSolver.x(0);
			} else if(input == "x'") {
				R(1); M(0); L(0);
				cfopSolver.x(1);
			} else if(input == "x2") {
				R(2); M(2); L(2);
				cfopSolver.x(2);
			} else if(input == "y") {
				U(0); E(1); D(1);
				cfopSolver.y(0);
			} else if(input == "y'") {
				U(1); E(0); D(0);
				cfopSolver.y(1);
			} else if(input == "y2") {
				U(2); E(2); D(2);
				cfopSolver.y(2);
			} else if(input == "z") {
				F(0); S(0); B(1);
				cfopSolver.z(0);
			} else if(input == "z'") {
				F(1); S(1); B(0);
				cfopSolver.z(1);
			} else {
				isRotating = false;
			}
		}
	}

	void Swap(ref GameObject a, ref GameObject b, ref GameObject c, ref GameObject d) {
		GameObject temp = a; a = d; d = c; c = b; b = temp;
	}

	void Swap(ref GameObject a, ref GameObject b) {
		GameObject temp = a; a = b; b = temp;
	}

	IEnumerator PerformTheRotation(Vector3 axis, float newAngle, Transform layer, Action<int> Swap, int rotation) {
		Vector3 originalPosition = layer.position;
		Quaternion originalRotation = layer.rotation;

		float oldAngle = 0f;
		while(oldAngle != newAngle) {
			oldAngle = Mathf.MoveTowards(oldAngle, newAngle, PlayerPrefs.GetFloat("layersRSK", 100f) * Time.deltaTime);
			Quaternion q = Quaternion.AngleAxis(oldAngle, axis);
			layer.position = q * originalPosition;
			layer.rotation = q * originalRotation;

			yield return null;
		}

		for(int i = 0; i < 27; i++) {
			if(i != 13) {
				cube[i].transform.parent = rubiksCube;
			}
		}
		Swap(rotation);
		isRotating = false;
	}

	public void U(int rotation) {
		for(int i = 0; i < 9; i++)
			cube[i].transform.parent = uLayer;

		float newAngle = rotation == 0 ? 90f : rotation == 1 ? -90f : 180f;
		StartCoroutine(PerformTheRotation(Vector3.up, newAngle, uLayer, SwapU, rotation));
	}

	public void SwapU(int rotation) {
		if(rotation == 0) {
			Swap(ref cube[0], ref cube[2], ref cube[8], ref cube[6]);
			Swap(ref cube[1], ref cube[5], ref cube[7], ref cube[3]);
		}
		else if(rotation == 1) {
			Swap(ref cube[6], ref cube[8], ref cube[2], ref cube[0]);
			Swap(ref cube[3], ref cube[7], ref cube[5], ref cube[1]);
		}
		else {
			Swap(ref cube[0], ref cube[8]);
			Swap(ref cube[2], ref cube[6]);

			Swap(ref cube[1], ref cube[7]);
			Swap(ref cube[3], ref cube[5]);
		}
	}

	public void D(int rotation) {
		for(int i = 18; i < 27; i++) 
			cube[i].transform.parent = dLayer;

		float newAngle = rotation == 0 ? -90f : rotation == 1 ? 90f : 180f;
		StartCoroutine(PerformTheRotation(Vector3.up, newAngle, dLayer, SwapD, rotation));
	}

	public void SwapD(int rotation) {
		if(rotation == 0) {
			Swap(ref cube[24], ref cube[26], ref cube[20], ref cube[18]);
			Swap(ref cube[25], ref cube[23], ref cube[19], ref cube[21]);
		}
		else if(rotation == 1) {
			Swap(ref cube[18], ref cube[20], ref cube[26], ref cube[24]);
			Swap(ref cube[21], ref cube[19], ref cube[23], ref cube[25]);
		}
		else {
			Swap(ref cube[20], ref cube[24]);
			Swap(ref cube[18], ref cube[26]);

			Swap(ref cube[19], ref cube[25]);
			Swap(ref cube[21], ref cube[23]);
		}
	}

	public void F(int rotation) {
		for(int i = 6; i < 9; i++) {
			cube[i].transform.parent = fLayer;
			cube[i + 9].transform.parent = fLayer;
			cube[i + 18].transform.parent = fLayer;
		}

		float newAngle = rotation == 0 ? -90f : rotation == 1 ? 90f : 180f;
		StartCoroutine(PerformTheRotation(Vector3.right, newAngle, fLayer, SwapF, rotation));
	}

	public void SwapF(int rotation) {
		if(rotation == 0) {
			Swap(ref cube[6], ref cube[8], ref cube[26], ref cube[24]);
			Swap(ref cube[7], ref cube[17], ref cube[25], ref cube[15]);
		}
		else if(rotation == 1) {
			Swap(ref cube[24], ref cube[26], ref cube[8], ref cube[6]);
			Swap(ref cube[15], ref cube[25], ref cube[17], ref cube[7]);
		}
		else {
			Swap(ref cube[6], ref cube[26]);
			Swap(ref cube[8], ref cube[24]);

			Swap(ref cube[7], ref cube[25]);
			Swap(ref cube[15], ref cube[17]);
		}
	}

	public void B(int rotation) {
		for(int i = 0; i < 3; i++) {
			cube[i].transform.parent = bLayer;
			cube[i + 9].transform.parent = bLayer;
			cube[i + 18].transform.parent = bLayer;
		}

		float newAngle = rotation == 0 ? 90f : rotation == 1 ? -90f : 180f;
		StartCoroutine(PerformTheRotation(Vector3.right, newAngle, bLayer, SwapB, rotation));
	}

	public void SwapB(int rotation) {
		if(rotation == 0) {
			Swap(ref cube[2], ref cube[0], ref cube[18], ref cube[20]);
			Swap(ref cube[1], ref cube[9], ref cube[19], ref cube[11]);
		} 
		else if(rotation == 1) {
			Swap(ref cube[20], ref cube[18], ref cube[0], ref cube[2]);
			Swap(ref cube[11], ref cube[19], ref cube[9], ref cube[1]);
		}
		else {
			Swap(ref cube[2], ref cube[18]);
			Swap(ref cube[0], ref cube[20]);

			Swap(ref cube[1], ref cube[19]);
			Swap(ref cube[9], ref cube[11]);
		}
	}

	public void L(int rotation) {
		for(int i = 0; i <= 18; i += 9) {
			cube[0 + i].transform.parent = lLayer;
			cube[3 + i].transform.parent = lLayer;
			cube[6 + i].transform.parent = lLayer;
		}

		float newAngle = rotation == 0 ? 90f : rotation == 1 ? -90f : 180f;
		StartCoroutine(PerformTheRotation(Vector3.forward, newAngle, lLayer, SwapL, rotation));
	}

	public void SwapL(int rotation) {
		if(rotation == 0) {
			Swap(ref cube[0], ref cube[6], ref cube[24], ref cube[18]);
			Swap(ref cube[3], ref cube[15], ref cube[21], ref cube[9]);
		}
		else if(rotation == 1) {
			Swap(ref cube[18], ref cube[24], ref cube[6], ref cube[0]);
			Swap(ref cube[9], ref cube[21], ref cube[15], ref cube[3]);
		}
		else {
			Swap(ref cube[0], ref cube[24]);
			Swap(ref cube[6], ref cube[18]);

			Swap(ref cube[3], ref cube[21]);
			Swap(ref cube[9], ref cube[15]);
		}
	}

	public void R(int rotation) {
		for(int i = 0; i <= 18; i += 9) {
			cube[2 + i].transform.parent = rLayer;
			cube[5 + i].transform.parent = rLayer;
			cube[8 + i].transform.parent = rLayer;
		}

		float newAngle = rotation == 0 ? -90f : rotation == 1 ? 90f : 180f;
		StartCoroutine(PerformTheRotation(Vector3.forward, newAngle, rLayer, SwapR, rotation));
	}

	public void SwapR(int rotation) {
		if(rotation == 0) {
			Swap(ref cube[8], ref cube[2], ref cube[20], ref cube[26]);
			Swap(ref cube[5], ref cube[11], ref cube[23], ref cube[17]);
		}
		else if(rotation == 1) {
			Swap(ref cube[26], ref cube[20], ref cube[2], ref cube[8]);
			Swap(ref cube[17], ref cube[23], ref cube[11], ref cube[5]);
		}
		else {
			Swap(ref cube[8], ref cube[20]);
			Swap(ref cube[2], ref cube[26]);

			Swap(ref cube[5], ref cube[23]);
			Swap(ref cube[11], ref cube[17]);
		}
	}

	public void M(int rotation) {
		for(int i = 0; i <= 18; i += 9) {
			cube[1 + i].transform.parent = mLayer;
			if(i != 9)
				cube[4 + i].transform.parent = mLayer;
			cube[7 + i].transform.parent = mLayer;
		}

		float newAngle = rotation == 0 ? 90f : rotation == 1 ? -90f : 180f;
		StartCoroutine(PerformTheRotation(Vector3.forward, newAngle, mLayer, SwapM, rotation));
	}

	public void SwapM(int rotation) {
		if(rotation == 0) {
			Swap(ref cube[1], ref cube[7], ref cube[25], ref cube[19]);
			Swap(ref cube[4], ref cube[16], ref cube[22], ref cube[10]);
		}
		else if(rotation == 1) {
			Swap(ref cube[19], ref cube[25], ref cube[7], ref cube[1]);
			Swap(ref cube[10], ref cube[22], ref cube[16], ref cube[4]);
		}
		else {
			Swap(ref cube[1], ref cube[25]);
			Swap(ref cube[7], ref cube[19]);

			Swap(ref cube[4], ref cube[22]);
			Swap(ref cube[10], ref cube[16]);
		}
	}

	public void E(int rotation) {
		for(int i = 9; i < 18; i++) {
			if(i != 13)
				cube[i].transform.parent = eLayer;
		}

		float newAngle = rotation == 0 ? -90f : rotation == 1 ? 90f : 180f;
		StartCoroutine(PerformTheRotation(Vector3.up, newAngle, eLayer, SwapE, rotation));
	}

	public void SwapE(int rotation) {
		if(rotation == 0) {
			Swap(ref cube[15], ref cube[17], ref cube[11], ref cube[9]);
			Swap(ref cube[16], ref cube[14], ref cube[10], ref cube[12]);
		}
		else if(rotation == 1) {
			Swap(ref cube[9], ref cube[11], ref cube[17], ref cube[15]);
			Swap(ref cube[12], ref cube[10], ref cube[14], ref cube[16]);
		}
		else {
			Swap(ref cube[11], ref cube[15]);
			Swap(ref cube[9], ref cube[17]);

			Swap(ref cube[10], ref cube[16]);
			Swap(ref cube[12], ref cube[14]);
		}
	}

	public void S(int rotation) {
		for(int i = 3; i < 6; i++) {
			cube[i].transform.parent = sLayer;
			if(i != 4) 
				cube[i + 9].transform.parent = sLayer;
			cube[i + 18].transform.parent = sLayer;
		}

		float newAngle = rotation == 0 ? -90f : rotation == 1 ? 90f : 180f;
		StartCoroutine(PerformTheRotation(Vector3.right, newAngle, sLayer, SwapS, rotation));
	}

	public void SwapS(int rotation) {
		if(rotation == 0) {
			Swap(ref cube[3], ref cube[5], ref cube[23], ref cube[21]);
			Swap(ref cube[4], ref cube[14], ref cube[22], ref cube[12]);
		}
		else if(rotation == 1) {
			Swap(ref cube[21], ref cube[23], ref cube[5], ref cube[3]);
			Swap(ref cube[12], ref cube[22], ref cube[14], ref cube[4]);
		}
		else {
			Swap(ref cube[3], ref cube[23]);
			Swap(ref cube[5], ref cube[21]);

			Swap(ref cube[4], ref cube[22]);
			Swap(ref cube[14], ref cube[12]);
		}
	}
}