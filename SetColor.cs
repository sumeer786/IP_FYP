using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetColor : MonoBehaviour {
	bool mouseDown = false;

	void OnMouseDown() {
		mouseDown = true;
	}

	void Update() {
		if(Input.GetMouseButtonUp(0)) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if(mouseDown && Physics.Raycast(ray, out RaycastHit hit) && hit.collider.gameObject == gameObject) {
				gameObject.GetComponent<Renderer>().material.color = GetSelectedColor.selectedColor;
			}
			mouseDown = false;
		}
	}
}