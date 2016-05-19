using System;

namespace Docking {
	class Utils {
		static public float ParseFloat(string str) {
			return float.Parse(str, System.Globalization.CultureInfo.InvariantCulture);
		}
		static public string FloatToString(float x, string format = "0.####") {
			return x.ToString(format, System.Globalization.CultureInfo.InvariantCulture);
		}
	}
}