using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using Kociemba;

public class TimerHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
	public bool clickedDown, timerRunning;
	float holdTime, elapsedTime;
	public float requiredHoldTime;
	public Text time;
	public GameObject cubeMap, previousButton;
	public Text scramble;
	CFOPSolver cfopSolver;
	GameObject[,] cube = new GameObject[6, 9];

	void Start() {
		for(int i = 0; i < 6; i++) {
			for(int j = 0; j < 9; j++) {
				cube[i, j] = GameObject.Find("" + i + j);
			}
		}

		previousButton.SetActive(false);
		cfopSolver = new CFOPSolver();
		NextScrambleHelper();
	}

	public GameObject scramblePanel;
	public void OnPointerDown(PointerEventData eventData) {
		if(timerRunning) {
			StopTimer();
		}
		else {
			time.color = new Color32(255, 60, 60, 255);
			clickedDown = true;
		}
	}

	public void StopTimer() {
		runTimer = timerRunning = false;
		elapsedTime = 0f;
		cubeMap.SetActive(true);
		scramblePanel.SetActive(true);
		NextScramble();
	}

	bool runTimer;
	public void OnPointerUp(PointerEventData eventData) {
		if(time.color == new Color32(121, 238, 92, 255)) {
			runTimer = true;
			timerRunning = true;
		}
		
		clickedDown = false;
		time.color = Color.white;
		holdTime = 0;
	}

	void Update() {
		if(clickedDown) {
			holdTime += Time.deltaTime;
			if(holdTime >= requiredHoldTime) {
				time.color = new Color32(121, 238, 92, 255);
				time.text = "00:00:00";
				cubeMap.SetActive(false);
				scramblePanel.SetActive(false);
			}
		}
		if(runTimer) {
			elapsedTime += Time.deltaTime;
			time.text = TimeSpan.FromSeconds(elapsedTime).ToString("mm':'ss':'ff");
		}
	}

	Stack<string> scrambles = new Stack<string>();
	System.Random rand = new System.Random();
	public void NextScramble() {
		scrambles.Push(scramble.text);
		previousButton.SetActive(true);
		NextScrambleHelper();
	}

	public void NextScrambleHelper() {
		string randomScramble = Tools.randomCube().Substring(0, 25);

		List<string> scrambleAsList = new List<string>();
		for(int i = 0; i < randomScramble.Length; i++) {
			scrambleAsList.Add(randomScramble[i].ToString());
		}

		cfopSolver.Clean(ref scrambleAsList);
		randomScramble = "";
		for(int i = 0; i < scrambleAsList.Count; i++) {
			if(rand.Next(2) == 1 && scrambleAsList[i].Length == 1) {
				randomScramble += scrambleAsList[i] + "' ";
			}
			else {
				randomScramble += scrambleAsList[i] + " ";
			}
		}

		scramble.text = randomScramble;
		Perform(randomScramble);
	}

	public void PreviousScramble() {
		scramble.text = scrambles.Pop();
		if(scrambles.Count == 0) {
			previousButton.SetActive(false);
		}
		Perform(scramble.text);
	}

	void Perform(string scramble) {
		cfopSolver.Fill();
		cfopSolver.Perform(scramble);

		for(int i = 0; i < 6; i++) {
			for(int j = 0; j < 9; j++) {
				cube[i, j].GetComponent<Image>().color = cfopSolver.reverseColorMap[cfopSolver.c[i][j]];
			}
		}
	}

	public GameObject timer;
	public void HideTimer() {
		StopTimer();
		timer.SetActive(false);
		AdManager.adManager.HideBannerAds();
	}
}