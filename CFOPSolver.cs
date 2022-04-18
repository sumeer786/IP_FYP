using System;
using UnityEngine;
using System.Collections.Generic;

public class CFOPSolver {
	public char[][] c = new char[6][];
	public char[] up, down, front, back, left, right;
	public Dictionary<Color, char> colorsMap = new Dictionary<Color, char>();
	public Dictionary<char, Color> reverseColorMap = new Dictionary<char, Color>();
	Dictionary<char[], char[]> leftLayer = new Dictionary<char[], char[]>();
	Dictionary<char[], char[]> rightLayer = new Dictionary<char[], char[]>();
	Dictionary<int, char[]> above = new Dictionary<int, char[]>();
	Dictionary<int, char[]> below = new Dictionary<int, char[]>();
	string[] f2lAlgorithms = new string[41];
	string[] ollAlgorithms = new string[57];
	string[] pllAlgorithms = new string[21];
	char[] colorsChar = {'a', 'b', 'c', 'd', 'e', 'f'};
	

	public CFOPSolver() {
		Color[] colors = {new Color32(255, 255, 255, 255), new Color32(255, 255, 0, 255), new Color32(0, 173, 0, 255), new Color32(0, 85, 255, 255), new Color32(255, 125, 0, 255), new Color32(255, 20, 0, 255)};

		for(int i = 0; i < 6; i++) {
			colorsMap.Add(colors[i], colorsChar[i]);
			reverseColorMap.Add(colorsChar[i], colors[i]);
		}

		for(int i = 0; i < 6; i++) {
			c[i] = new char[9];
			for(int j = 0; j < 9; j++) 
				c[i][j] = colorsChar[i];
		}

		up = c[0];
		down = c[1];
		front = c[2];
		back = c[3];
		left = c[4];
		right = c[5];

		rightLayer.Add(front, right);
		rightLayer.Add(right, back);
		rightLayer.Add(back, left);
		rightLayer.Add(left, front);

		leftLayer.Add(right, front);
		leftLayer.Add(back, right);
		leftLayer.Add(left, back);
		leftLayer.Add(front, left);

		above.Add(1, front);
		above.Add(5, right);
		above.Add(7, back);
		above.Add(3, left);

		below.Add(1, back);
		below.Add(5, right);
		below.Add(7, front);
		below.Add(3, left);

		// Load algorithms
		f2lAlgorithms = ((TextAsset)Resources.Load("F2L")).text.Split('\n');
		ollAlgorithms = ((TextAsset)Resources.Load("OLL")).text.Split('\n');
		pllAlgorithms = ((TextAsset)Resources.Load("PLL")).text.Split('\n');
	}

	List<string> solution;
	List<List<string>> finalSolution;
	public List<List<string>> Solve() {
		solution = new List<string>();
		finalSolution = new List<List<string>>();
		
		// Create copy of cube
		char[,] copy = new char[6, 9];
		for(int i = 0; i < 6; i++) {
			for(int j = 0; j < 9; j++) 
				copy[i, j] = c[i][j];
		}
		
		Cross();
		F2L();
		OLL();
		PLL();

		if(!IsSolved()) {
			return null;
		}
			
		RestoreCube(copy);
		return finalSolution;
	}

	public void Fill() {
		for(int i = 0; i < 6; i++) {
			for(int j = 0; j < 9; j++) {
				c[i][j] = colorsChar[i];
			}
		}
	}

	void AddSolution() {
		Clean(ref solution);
		List<string> temp = new List<string>();
		foreach(string s in solution) {
			temp.Add(s);
		}

		finalSolution.Add(temp);
		solution.Clear();
	}

	public bool IsSolved() {
		for(int i = 0; i < 6; i++) {
			for(int j = 0; j < 9; j++) {
				if(c[i][j] != c[i][4])
					return false;
			}
		}
		return true;
	}

	void RestoreCube(char[,] temp) {
		for(int i = 0; i < 6; i++) {
			for(int j = 0; j < 9; j++) 
				c[i][j] = temp[i, j];
		}
	}

	int[] GetPositionOfNextCrossEdge() {
		for(int i = 0; i < 6; i++) {
			for(int j = 1; j < 8; j += 2) {
				if(c[i][j] == c[1][4]) {
					if(i == 1 && solvedCrossEdges[j])
						continue;

					return new int[] {i, j};
				}
			}
		}
		return null;
	}

	void MarkSolvedCrossSide(ref char[] temp) {
		for(int i = 1; i < 8; i += 2) {
			if(temp == above[i]) {
				solvedCrossEdges[i] = true;
				return;
			}
		}
	}

	void SolveMiddleLeftCrossEdge(ref char[] temp, int i) {
		if(i == 0) {
			RotateLayer(ref temp, 0);
			int count = 0;
			while(temp[4] != temp[7] && count < 4) {
				D(0);
				solution.Add("D");

				temp = rightLayer[temp];
				count++;
			}
		}

		else {
			int rotatationCount = 0, targetColor = temp[5], count = 0;
			while(temp[4] != targetColor && count < 4) {
				temp = rightLayer[temp];
				rotatationCount++;
				count++;
			}
			
			if(rotatationCount == 0) 
				RotateLayer(ref temp, 0);

			else if(rotatationCount == 1) {
				D(1);
				solution.Add("D'");

				char[] temp2 = leftLayer[temp];
				RotateLayer(ref temp2, 0);
				D(0);
				solution.Add("D");
			}

			else if(rotatationCount == 2) {
				char[] temp2 = leftLayer[temp];
				RotateLayer(ref temp2, 2);
				RotateLayer(ref temp, 1);
				RotateLayer(ref temp2, 2);
			}

			else {
				D(0);
				solution.Add("D");

				char[] temp2 = rightLayer[temp];
				RotateLayer(ref temp2, 0);
				D(1);
				solution.Add("D'");
			}
		}
		MarkSolvedCrossSide(ref temp);
	}

	void SolveMiddleRightCrossEdge(ref char[] temp, int i) {
		if(i == 0) {
			RotateLayer(ref temp, 1);
			int count = 0;
			while(temp[4] != temp[7] && count < 4) {
				D(0);
				solution.Add("D");

				temp = rightLayer[temp];
				count++;
			}
		}

		else {
			int rotatationCount = 0, targetColor = temp[3], count = 0;
			while(temp[4] != targetColor && count < 4) {
				temp = leftLayer[temp];
				rotatationCount++;
				count++;
			}

			if(rotatationCount == 0) 
				RotateLayer(ref temp, 1);

			else if(rotatationCount == 1) {
				D(0);
				solution.Add("D");

				char[] temp2 = rightLayer[temp];
				RotateLayer(ref temp2, 1);
				D(1);
				solution.Add("D'");
			}
			
			else if(rotatationCount == 2) {
				char[] temp2 = rightLayer[temp];
				RotateLayer(ref temp2, 2);
				RotateLayer(ref temp, 0);
				RotateLayer(ref temp2, 2);
			}

			else {
				D(1);
				solution.Add("D'");

				char[] temp2 = leftLayer[temp];
				RotateLayer(ref temp2, 1);
				D(0);
				solution.Add("D");
			}
		}
		MarkSolvedCrossSide(ref temp);
	}

	bool[] solvedCrossEdges = new bool[8];
	void Cross() {
		solvedCrossEdges = new bool[8];
		for(int i = 0; i < 4; i++) {
			int[] p = GetPositionOfNextCrossEdge();
			if(p == null)
				break;

			if(p[0] == 0) {
				char[] temp = below[p[1]];
				int count = 0;
				while(temp[1] != temp[4] && count < 4) {
					U(0);
					solution.Add("U");

					temp = leftLayer[temp];
					count++;
				}
				RotateLayer(ref temp, 2);
				MarkSolvedCrossSide(ref temp);
			}

			else if(p[0] == 1) {
				char[] temp = above[p[1]];
				if(temp[4] == temp[7])
					MarkSolvedCrossSide(ref temp);
				else {
					RotateLayer(ref temp, 0);
					SolveMiddleRightCrossEdge(ref temp, i);
				}
			}

			else if(p[1] == 3) {
				char[] temp2 = leftLayer[c[p[0]]];
				SolveMiddleLeftCrossEdge(ref temp2, i);
			}

			else if(p[1] == 5) {
				char[] temp2 = rightLayer[c[p[0]]];
				SolveMiddleRightCrossEdge(ref temp2, i);
			}

			else if(p[1] == 1) {
				char color = 'g';
				char[] temp = c[p[0]];
				for(int j = 1; j < 8; j += 2) {
					if(below[j] == temp) {
						color = up[j];
						break;
					}
				}

				int count = 0;
				while(leftLayer[temp][4] != color && count < 4) {
					U(0);
					solution.Add("U");

					temp = leftLayer[temp];
					count++;
				}

				char[] temp2 = leftLayer[temp];
				RotateLayer(ref temp, 1);
				RotateLayer(ref temp2, 0);
				RotateLayer(ref temp, 0);
				MarkSolvedCrossSide(ref temp2);
			}

			else {
				char[] temp2 = leftLayer[c[p[0]]];
				RotateLayer(ref c[p[0]], 0);
				SolveMiddleLeftCrossEdge(ref temp2, i);
			}
		}
		AddSolution();
	}

	int[] GetCornerPosition(char a, char b) {
		for(int i = 0; i < 6; i++) {
			for(int j = 0; j < 9; j += 2) {
				if(j == 4) 
					continue;

				if(c[i][j] == down[4]) {
					char c = 'g', d = 'g';
					if(i == 0) {
						if(j == 0) {
							c = left[0];
							d = back[2];
						} else if(j == 2) {
							c = right[2];
							d = back[0];
						} else if(j == 6) {
							c = front[0];
							d = left[2];
						} else if(j == 8) {
							c = front[2];
							d = right[0];
						}
					}
					else if(i == 1) {
						if(j == 0) {
							c = front[6];
							d = left[8];
						} else if(j == 2) {
							c = right[6];
							d = front[8];
						} else if(j == 6) {
							c = left[6];
							d = back[8];
						} else if(j == 8) {
							c = right[8];
							d = back[6];
						}
					}
					else if(i == 2) {
						if(j == 0) {
							c = up[6];
							d = left[2];
						} else if(j == 2) {
							c = up[8];
							d = right[0];
						} else if(j == 6) {
							c = left[8];
							d = down[0];
						} else if(j == 8) {
							c = right[6];
							d = down[2];
						}
					}
					else if(i == 3) {
						if(j == 0) {
							c = up[2];
							d = right[2];
						} else if(j == 2) {
							c = up[0];
							d = left[0];
						} else if(j == 6) {
							c = right[8];
							d = down[8];
						} else if(j == 8) {
							c = left[6];
							d = down[6];
						}
					}
					else if(i == 4) {
						if(j == 0) {
							c = up[0];
							d = back[2];
						} else if(j == 2) {
							c = up[6];
							d = front[0];
						} else if(j == 6) {
							c = down[6];
							d = back[8];
						} else if(j == 8) {
							c = front[6];
							d = down[0];
						}
					}
					else if(i == 5) {
						if(j == 0) {
							c = up[8];
							d = front[2];
						} else if(j == 2) {
							c = up[2];
							d = back[0];
						} else if(j == 6) {
							c = front[8];
							d = down[2];
						} else if(j == 8) {
							c = back[6];
							d = down[8];
						}
					}
					if((a == c && b == d) || (a == d && b == c))
						return new int[] {i, j};
				}
			}
		}
		return null;
	}

	int[] GetEdgePosition(char a, char b) {
		for(int i = 0; i < 6; i++) {
			if(i == 1) 
				continue;
			for(int j = 1; j < 8; j += 2) {
				if(i != 0 && j == 7) 
					continue;
				
				if(c[i][j] == a) {
					if(i == 0) {
						if(below[j][1] == b)
							return new int[] {i, j};
					} else if(j == 1) {
						if((c[i] == front && up[7] == b) || (c[i] == left && up[3] == b) || (c[i] == back && up[1] == b) || (c[i] == right && up[5] == b))
							return new int[] {i, j};
					} else if(j == 3) {
						if(leftLayer[c[i]][5] == b)
							return new int[] {i, j};
					} else if(j == 5) {
						if(rightLayer[c[i]][3] == b)
							return new int[] {i, j};
					}
				}
			}
		}
		return null;
	}

	bool IsCurrentF2LPairSolved() {
		if(front[4] == front[5] && front[4] == front[8] && right[3] == right[4] && right[6] == right[4] && down[2] == down[4])
			return true;
		return false;
	}

	void F2L() {
		for(int k = 0; k < 4; k++) {
			if(IsSolved()) {
				return;
			}
			if(!IsCurrentF2LPairSolved()) {
				int[] p = GetCornerPosition(front[4], right[4]);
				if(p == null) {
					AddSolution();
					continue;
				}

				int i = p[0], j = p[1];

				if((i == 0 && j == 0) || (i == 4 && j == 0) || (i == 3 && j == 2)) {
					U(2);
					solution.Add("U2");
				} else if((i == 0 && j == 2) || (i == 3 && j == 0) || (i == 5 && j == 2)) {
					U(0);
					solution.Add("U");
				} else if((i == 0 && j == 6) || (i == 4 && j == 2) || (i == 2 && j == 0)) {
					U(1);
					solution.Add("U'");
				} else if((i == 1 && j == 6) || (i == 4 && j == 6) || (i == 3 && j == 8)) {
					L(0); U(2); L(1);
					solution.Add("L");
					solution.Add("U2");
					solution.Add("L'");
				} else if((i == 1 && j == 8) || (i == 5 && j == 8) || (i == 3 && j == 6)) {
					R(1); U(2); R(0); U(1);
					solution.Add("R'");
					solution.Add("U2");
					solution.Add("R");
					solution.Add("U'");
				} else if((i == 1 && j == 0) || (i == 4 && j == 8) || (i == 2 && j == 6)) {
					L(1); U(1); L(0);
					solution.Add("L'");
					solution.Add("U'");
					solution.Add("L");
				}

				bool flag = ((i == 2 && j == 8) || (i == 5 && j == 6) || (i == 1 && j == 2));

				int[] q = GetEdgePosition(front[4], right[4]);
				if(q == null) {
					AddSolution();
					continue;
				}
				i = q[0]; j = q[1];

				flag = flag && !((i == 2 && j == 5) || (i == 5 && j == 3));

				if((i == 2 && j == 3) || (i == 4 && j == 5)) {
					L(1); U(1); L(0); U(0);
					solution.Add("L'");
					solution.Add("U'");
					solution.Add("L");
					solution.Add("U");
				} else if((i == 4 && j == 3) || (i == 3 && j == 5)) {
					L(0); U(1); L(1); U(0);
					solution.Add("L");
					solution.Add("U'");
					solution.Add("L'");
					solution.Add("U");
				} else if((i == 5 && j == 5) || (i == 3 && j == 3)) {
					R(1); U(2); R(0);
					solution.Add("R'");
					solution.Add("U2");
					solution.Add("R");
				}

				if(flag) {
					q = GetEdgePosition(front[4], right[4]);
					i = q[0]; j = q[1];
					
					if(i == 0) {
						char[] temp2 = below[j];
						int count = 0;
						while(temp2[1] != temp2[4] && count < 4) {
							temp2 = leftLayer[temp2];
							U(0);
							solution.Add("U");
							count++;
						}
					}
					else {
						char[] temp2 = c[i];
						int count = 0;
						while(temp2[1] != temp2[4] && count < 4) {
							temp2 = leftLayer[temp2];
							U(0);
							solution.Add("U");
							count++;
						}
					}
				}

				char[,] temp = new char[6, 9];
				for(i = 0; i < 6; i++) {
					for(j = 0; j < 9; j++)
						temp[i, j] = c[i][j];
				}

				for(i = 0; i < 41; i++) {
					Perform(f2lAlgorithms[i]);
					if(IsCurrentF2LPairSolved()) {
						foreach(string s in f2lAlgorithms[i].Split(null))
							solution.Add(s);
						break;
					}
					RestoreCube(temp);
				}
			}
			if(k != 3) {
				y(0);
				solution.Add("y");
			}
			AddSolution();
		}
	}

	bool IsOriented() {
		for(int i = 0; i < 9; i++) {
			if(up[i] != up[4])
				return false;
		}
		return true;
	}

	void OLL() {
		if(IsSolved()) {
			return;
		}
		if(IsOriented()) {
			AddSolution();
			return;
		}

		for(int i = 0; i < 4; i++) {
			char[,] temp = new char[6, 9];
			for(int k = 0; k < 6; k++) {
				for(int l = 0; l < 9; l++)
					temp[k, l] = c[k][l];
			}

			for(int j = 0; j < 57; j++) {
				Perform(ollAlgorithms[j]);
				if(IsOriented()) {
					foreach(string a in ollAlgorithms[j].Split(null)) {
						solution.Add(a);
					}
					AddSolution();
					return;
				}	
				RestoreCube(temp);
			}
			U(0);
			solution.Add("U");
		}
	}

	bool IsPermuted() {
		for(int i = 2; i < 6; i++) {
			if(c[i][0] != c[i][1] || c[i][1] != c[i][2])
				return false;
		}
		return true;
	}

	void PLL() {
		if(IsSolved()) {
			return;
		}
		if(!IsPermuted()) {
			for(int i = 0; i < 4; i++) {
				char[,] temp = new char[6, 9];
				for(int k = 0; k < 6; k++) {
					for(int l = 0; l < 9; l++)
						temp[k, l] = c[k][l];
				}

				for(int j = 0; j < 21; j++) {
					Perform(pllAlgorithms[j]);
					if(IsPermuted()) {
						foreach(string a in pllAlgorithms[j].Split(null))
							solution.Add(a);
						goto exit;
					}
					RestoreCube(temp);
				}
				U(0);
				solution.Add("U");
			}
		}

		exit:
		for(int i = 0; i < 4 && front[1] != front[4]; i++){
			U(0);
			solution.Add("U");
		}
		AddSolution();
	}

	public void Clean(ref List<string> list) {
		for(int i = 0; i < list.Count - 1;) {
			string a = list[i], b = list[i + 1];
			if(a[0] == b[0]) {
				if(a.Length == 1 && b.Length == 1) {
					list.RemoveAt(i);
					list[i] = a[0] + "2";
					i = i == 0 ? 0 : i - 1;
				}
				else if(a.Length == 1 && b.Length == 2) {
					if(b[1] == '\'') {
						list.RemoveAt(i);
						list.RemoveAt(i);
						i = i == 0 ? 0 : i - 1;	
					}
					else if(b[1] == '2') {
						list.RemoveAt(i);
						list[i] = a[0] + "'";
						i = i == 0 ? 0 : i - 1;	
					}
				}
				else if(a.Length == 2 && b.Length == 1) {
					if(a[1] == '\'') {
						list.RemoveAt(i);
						list.RemoveAt(i);
						i = i == 0 ? 0 : i - 1;	
					}
					else if(a[1] == '2') {
						list.RemoveAt(i);
						list[i] = a[0] + "'";
						i = i == 0 ? 0 : i - 1;	
					}
				}
				else if(a.Length == 2 && b.Length == 2) {
					if(a[1] == '\'' && b[1] == '\'') {
						list.RemoveAt(i);
						list[i] = a[0] + "2";
						i = i == 0 ? 0 : i - 1;	
					}
					else if(a[1] == '2' && b[1] == '2') {
						list.RemoveAt(i);
						list.RemoveAt(i);
						i = i == 0 ? 0 : i - 1;	
					}
					else {
						list.RemoveAt(i);
						list[i] = a[0] + "";
						i = i == 0 ? 0 : i - 1;	
					}
				}
				else 
					i++;
			}
			else
				i++;
		}
	}

	public void Perform(string s) {
		string[] input = s.Split(null);
		for(int i = 0; i < input.Length; i++) {
			if(input[i] == "U") {
				U(0);
			} else if(input[i] == "F") {
				F(0);
			} else if(input[i] == "R") {
				R(0);
			} else if(input[i] == "D") {
				D(0);
			} else if(input[i] == "B") {
				B(0);
			} else if(input[i] == "L") {
				L(0);
			} else if(input[i] == "M") {
				M(0);
			} else if(input[i] == "E") {
				E(0);
			} else if(input[i] == "U'") {
				U(1);
			} else if(input[i] == "F'") {
				F(1);
			} else if(input[i] == "R'") {
				R(1);
			} else if(input[i] == "D'") {
				D(1);
			} else if(input[i] == "B'") {
				B(1);
			} else if(input[i] == "L'") {
				L(1);
			} else if(input[i] == "M'") {
				M(1);
			} else if(input[i] == "E'") {
				E(1);
			} else if(input[i] == "U2") {
				U(2);
			} else if(input[i] == "F2") {
				F(2);
			} else if(input[i] == "R2") {
				R(2);
			} else if(input[i] == "D2") {
				D(2);
			} else if(input[i] == "B2") {
				B(2);
			} else if(input[i] == "L2") {
				L(2);
			} else if(input[i] == "M2") {
				M(2);
			} else if(input[i] == "E2") {
				E(2);
			} else if(input[i] == "u") {
				WideU(0);
			} else if(input[i] == "r") {
				WideR(0);
			} else if(input[i] == "l") {
				WideL(0);
			} else if(input[i] == "u'") {
				WideU(1);
			} else if(input[i] == "r'") {
				WideR(1);
			} else if(input[i] == "l'") {
				WideL(1);
			} else if(input[i] == "u2") {
				WideU(2);
			} else if(input[i] == "r2") {
				WideR(2);
			} else if(input[i] == "l2") {
				WideL(2);
			} else if(input[i] == "x") {
				x(0);
			} else if(input[i] == "x'") {
				x(1);
			} else if(input[i] == "x2") {
				x(2);
			} else if(input[i] == "y") {
				y(0);
			} else if(input[i] == "y'") {
				y(1);
			} else if(input[i] == "y2") {
				y(2);
			}
		}
	}

	void Swap(ref char[] arr, int a, int b, int c, int d) {
		char temp = arr[a];
		arr[a] = arr[d];
		arr[d] = arr[c];
		arr[c] = arr[b];
		arr[b] = temp;
	}

	void Swap(ref char[] arr, ref char[] brr, ref char[] crr, ref char[] drr, int a, int b, int c, int d) {
		char temp = arr[a];
		arr[a] = drr[d];
		drr[d] = crr[c];
		crr[c] = brr[b];
		brr[b] = temp;
	}

	public void U(int rotation) {
		if(rotation == 0) {
			Swap(ref up, 0, 2, 8, 6);
			Swap(ref up, 1, 5, 7, 3);
			Swap(ref back, ref right, ref front, ref left, 2, 2, 2, 2);
			Swap(ref back, ref right, ref front, ref left, 1, 1, 1, 1);
			Swap(ref back, ref right, ref front, ref left, 0, 0, 0, 0);
		}
		else if(rotation == 1) {
			Swap(ref up, 0, 6, 8, 2);
			Swap(ref up, 1, 3, 7, 5);
			Swap(ref back, ref left, ref front, ref right, 2, 2, 2, 2);
			Swap(ref back, ref left, ref front, ref right, 1, 1, 1, 1);
			Swap(ref back, ref left, ref front, ref right, 0, 0, 0, 0);
		}
		else {
			U(0); U(0);
		}
	}

	public void WideU(int rotation) {
		if(rotation == 0) {
			U(0); E(1);
		}
		else if(rotation == 1) {
			U(1); E(0);
		}
		else {
			U(2); E(2);
		}
	}

	public void D(int rotation) {
		if(rotation == 0) {
			Swap(ref down, 0, 2, 8, 6);
			Swap(ref down, 1, 5, 7, 3);
			Swap(ref front, ref right, ref back, ref left, 6, 6, 6, 6);
			Swap(ref front, ref right, ref back, ref left, 7, 7, 7, 7);
			Swap(ref front, ref right, ref back, ref left, 8, 8, 8, 8);
		}
		else if(rotation == 1) {
			Swap(ref down, 0, 6, 8, 2);
			Swap(ref down, 1, 3, 7, 5);
			Swap(ref front, ref left, ref back, ref right, 6, 6, 6, 6);
			Swap(ref front, ref left, ref back, ref right, 7, 7, 7, 7);
			Swap(ref front, ref left, ref back, ref right, 8, 8, 8, 8);
		}
		else {
			D(0); D(0);
		}
	}

	public void F(int rotation) {
		if(rotation == 0) {
			Swap(ref front, 0, 2, 8, 6);
			Swap(ref front, 1, 5, 7, 3);
			Swap(ref up, ref right, ref down, ref left, 6, 0, 2, 8);
			Swap(ref up, ref right, ref down, ref left, 7, 3, 1, 5);
			Swap(ref up, ref right, ref down, ref left, 8, 6, 0, 2);
		}
		else if(rotation == 1) {
			Swap(ref front, 0, 6, 8, 2);
			Swap(ref front, 1, 3, 7, 5);
			Swap(ref up, ref left, ref down, ref right, 6, 8, 2, 0);
			Swap(ref up, ref left, ref down, ref right, 7, 5, 1, 3);
			Swap(ref up, ref left, ref down, ref right, 8, 2, 0, 6);
		}
		else {
			F(0); F(0);
		}
	}

	public void B(int rotation) {
		if(rotation == 0) {
			Swap(ref back, 0, 2, 8, 6);
			Swap(ref back, 1, 5, 7, 3);
			Swap(ref up, ref left, ref down, ref right, 2, 0, 6, 8);
			Swap(ref up, ref left, ref down, ref right, 1, 3, 7, 5);
			Swap(ref up, ref left, ref down, ref right, 0, 6, 8, 2);
		}
		else if(rotation == 1) {
			Swap(ref back, 0, 6, 8, 2);
			Swap(ref back, 1, 3, 7, 5);
			Swap(ref up, ref right, ref down, ref left, 2, 8, 6, 0);
			Swap(ref up, ref right, ref down, ref left, 1, 5, 7, 3);
			Swap(ref up, ref right, ref down, ref left, 0, 2, 8, 6);
		}
		else {
			B(0); B(0);
		}
	}

	public void L(int rotation) {
		if(rotation == 0) {
			Swap(ref left, 0, 2, 8, 6);
			Swap(ref left, 1, 5, 7, 3);
			Swap(ref up, ref front, ref down, ref back, 0, 0, 0, 8);
			Swap(ref up, ref front, ref down, ref back, 3, 3, 3, 5);
			Swap(ref up, ref front, ref down, ref back, 6, 6, 6, 2);
		}
		else if(rotation == 1) {
			Swap(ref left, 0, 6, 8, 2);
			Swap(ref left, 1, 3, 7, 5);
			Swap(ref up, ref back, ref down, ref front, 0, 8, 0, 0);
			Swap(ref up, ref back, ref down, ref front, 3, 5, 3, 3);
			Swap(ref up, ref back, ref down, ref front, 6, 2, 6, 6);
		}
		else {
			L(0); L(0);
		}
	}

	public void WideL(int rotation) {
		L(rotation); M(rotation);
	}

	public void R(int rotation) {
		if(rotation == 0) {
			Swap(ref right, 0, 2, 8, 6);
			Swap(ref right, 1, 5, 7, 3);
			Swap(ref up, ref back, ref down, ref front, 8, 0, 8, 8);
			Swap(ref up, ref back, ref down, ref front, 5, 3, 5, 5);
			Swap(ref up, ref back, ref down, ref front, 2, 6, 2, 2);
		}
		else if(rotation == 1) {
			Swap(ref right, 0, 6, 8, 2);
			Swap(ref right, 1, 3, 7, 5);
			Swap(ref up, ref front, ref down, ref back, 8, 8, 8, 0);
			Swap(ref up, ref front, ref down, ref back, 5, 5, 5, 3);
			Swap(ref up, ref front, ref down, ref back, 2, 2, 2, 6);
		}
		else {
			R(0); R(0);
		}
	}

	public void WideR(int rotation) {
		if(rotation == 0) {
			R(0); M(1);
		}
		else if(rotation == 1) {
			R(1); M(0);
		}
		else {
			R(2); M(2);
		}
	}

	public void M(int rotation) {
		if(rotation == 0) {
			Swap(ref up, ref front, ref down, ref back, 1, 1, 1, 7);
			Swap(ref up, ref front, ref down, ref back, 4, 4, 4, 4);
			Swap(ref up, ref front, ref down, ref back, 7, 7, 7, 1);
		}
		else if(rotation == 1) {
			Swap(ref up, ref back, ref down, ref front, 1, 7, 1, 1);
			Swap(ref up, ref back, ref down, ref front, 4, 4, 4, 4);
			Swap(ref up, ref back, ref down, ref front, 7, 1, 7, 7);
		}
		else {
			M(0); M(0);
		}
	}

	public void E(int rotation) {
		if(rotation == 0) {
			Swap(ref front, ref right, ref back, ref left, 3, 3, 3, 3);
			Swap(ref front, ref right, ref back, ref left, 4, 4, 4, 4);
			Swap(ref front, ref right, ref back, ref left, 5, 5, 5, 5);
		}
		else if(rotation == 1) {
			Swap(ref front, ref left, ref back, ref right, 3, 3, 3, 3);
			Swap(ref front, ref left, ref back, ref right, 4, 4, 4, 4);
			Swap(ref front, ref left, ref back, ref right, 5, 5, 5, 5);
		}
		else {
			E(0); E(0);
		}
	}

	public void S(int rotation) {
		if(rotation == 0) {
			Swap(ref up, ref right, ref down, ref left, 3, 1, 5, 7);
			Swap(ref up, ref right, ref down, ref left, 4, 4, 4, 4);
			Swap(ref up, ref right, ref down, ref left, 5, 7, 3, 1);
		}
		else if(rotation == 1) {
			Swap(ref up, ref left, ref down, ref right, 3, 7, 5, 1);
			Swap(ref up, ref left, ref down, ref right, 4, 4, 4, 4);
			Swap(ref up, ref left, ref down, ref right, 5, 1, 3, 7);
		}
		else {
			S(0); S(0);
		}
	}

	void RotateLayer(ref char[] arr, int rotation) {
		string add = "";
		if(rotation == 1) {
			add = "'";
		} else if(rotation == 2) {
			add = "2";
		}

		if(arr == front) {
			F(rotation);
			solution.Add("F" + add);
		} else if(arr == back) {
			B(rotation);
			solution.Add("B" + add);
		} else if(arr == up) {
			U(rotation);
			solution.Add("U" + add);
		} else if(arr == down) {
			D(rotation);
			solution.Add("D" + add);
		} else if(arr == left) {
			L(rotation);
			solution.Add("L" + add);
		} else {
			R(rotation);
			solution.Add("R" + add);
		}
	}

	public void x(int rotation) {
		if(rotation == 0) {
			WideR(0); L(1);
		}
		else if(rotation == 1) {
			WideR(1); L(0);
		}
		else {
			WideR(2); L(2);
		}
	}

	public void y(int rotation) {
		if(rotation == 0) {
			WideU(0); D(1);
		}
		else if(rotation == 1) {
			WideU(1); D(0);
		}
		else {
			WideU(2); D(2);
		}
	}

	public void z(int rotation) {
		if(rotation == 0) {
			F(0); S(0); B(1);
		} 
		else if(rotation == 1) {
			F(1); S(1); B(0);
		}
		else {
			F(2); S(2); B(2);
		}
	}
}