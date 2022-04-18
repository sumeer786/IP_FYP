using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GetSelectedColor : MonoBehaviour {
	public GameObject[] colors, indicators;

	void Start() {
		foreach(GameObject indicator in indicators) {
			indicator.SetActive(false);
		}
	}

	public static Color selectedColor = new Color(0.4f, 0.4f, 0.4f);
	public void SelectColor() {
		foreach(GameObject indicator in indicators) {
			indicator.SetActive(false);
		}

		GameObject current = EventSystem.current.currentSelectedGameObject;
		current.transform.GetChild(0).gameObject.SetActive(true);
		selectedColor = current.GetComponent<Button>().colors.normalColor;
	}
}