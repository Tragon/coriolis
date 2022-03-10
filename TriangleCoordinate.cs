using System;
using System.Text.RegularExpressions;
using UnityEngine;

[System.Serializable]
public struct TriangleCoordinate
{
	[SerializeField]
	private int x, z;

	public int X {
		get {
			return x;
		}
	}

	public int Z {
		get {
			return z;
		}
	}
	public TriangleCoordinate(int x, int z) {
		this.x = x;
		this.z = z;
	}

	public int Y {
		get {
			return -X - Z;
		}
	}

	public override string ToString() {
		return Z.ToString() + GetColNameFromIndex(X);
	}

	public string convert(int n) {
		int offset = n / 26;
		string t = "";
		int baseChar = (int)'A';
		if (offset > 0) {
			t += (char)(baseChar + offset);
		}
		t += (char)(n % 26);
		return t;
	}

	public static string GetColNameFromIndex(int columnNumber) {
		if (columnNumber == 0) { return "-"; }
		int dividend = Math.Abs(columnNumber);
		bool negative = columnNumber < 0;
		string columnName = String.Empty;
		int modulo;

		while(dividend > 0) {
			modulo = (dividend - 1) % 26;
			columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
			dividend = (int)((dividend - modulo) / 26);
		}

		return negative ? "-" + columnName : columnName;
	}

	// (A = 1, B = 2...AA = 27...AAA = 703...)
	public static int GetColNumberFromName(string columnName) {
		bool negative = false;
		if (columnName.Length > 0) {
			negative = columnName[0].Equals('-');
			if(negative) {
				columnName = columnName.Substring(1);
			}
        }
		char[] characters = columnName.ToUpperInvariant().ToCharArray();
		int sum = 0;
		for(int i = 0; i < characters.Length; i++) {
			sum *= 26;
			sum += (characters[i] - 'A' + 1);
		}
		return (negative ? -1 : 1) * sum;
	}

	public string ToTupleString() {
		return X.ToString() + ", " + Z.ToString();
	}

	public string ToTripleString() {
		return "(" +
			X.ToString() + ", " + Y.ToString() + ", " + Z.ToString() + ")";
	}

	public string ToStringOnSeparateLines() {
		return X.ToString() + "\n" + Y.ToString() + "\n" + Z.ToString();
	}

	public static TriangleCoordinate FromOffsetCoordinates(float x, float z) {
		return new TriangleCoordinate((int)Math.Round(x * 2 + z % 2), (int)Math.Round(-z * 3));
	}

	public static TriangleCoordinate FromWorldCoordinates(Vector3 v) {
		//int x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
		//int z = z * (HexMetrics.outerRadius * 1.5f);
		// 3 * 4 = 12
		// 12 / 4 = 3
		// (1 + 2) * 3 = 9
		// 9 / 3 - 2 = 1
		//float z = v.z / (HexMetrics.outerRadius * 1.5f);
		//float x = v.x / (HexMetrics.innerRadius * 2f) - (int)(z * 0.5f - z / 2);
		int x = (int)Math.Round(v.x / (HexMetrics.radFactor * 10f));
		int z = (int)Math.Round(v.z / 5f);
		return new TriangleCoordinate(x, -z);
	}

	public static TriangleCoordinate FromGlobalCoordinates(Vector3 v) {
		int x = (int)Math.Round(v.x / (HexMetrics.radFactor));
		int z = (int)Math.Round(v.z * 2f);
		return new TriangleCoordinate(x, z);
	}

	public Vector3 ToWorldCoordinates() {
		float x = X * (HexMetrics.radFactor * 10f);
		float z = Z * 5f;
		return new Vector3(x, 0f, -z);
	}

	public Vector3 ToGlobalCoordinates() {
		float x = X * (HexMetrics.radFactor);
		float z = Z / 2f;
		return new Vector3(x, 0f, z);
	}

	public static TriangleCoordinate FromTextInput(string coord) {
		Match match = Regex.Match(coord, @"(-?[\d]*)(-?\w*)");
		if (match.Success) {
			string vx = match.Groups[1].Value;
			int z = String.Empty.Equals(vx) ? 0 : int.Parse(vx);
			int x = GetColNumberFromName(match.Groups[2].Value);
			Debug.LogWarning("Z: " + match.Groups[1].Value + " = " + z);
			Debug.LogWarning("X: " + match.Groups[2].Value + " = " + x);
			return new TriangleCoordinate(x, -z);
		}
		return new TriangleCoordinate();
	}
}
