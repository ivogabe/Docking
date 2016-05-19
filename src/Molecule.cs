using System.IO;
using System.Collections.Generic;

namespace Docking {
	class Molecule {
		public string FileName;
		public int Size;
		public string[] AtomNames;
		public string[] AminoAcids;
		public int[] AminoAcidIds;
		public float[] X;
		public float[] Y;
		public float[] Z;
		public float[] Diameter;
		public float[] Charge;
		public int MaxAminoAcidId;
		public Molecule(string fileName) {
			FileName = fileName;
			
			StreamReader read = new StreamReader(File.Open(fileName, FileMode.Open));
			List<string> atoms = new List<string>();
			while (read.Peek() >= 0) {
				string line = read.ReadLine();
				if (line.StartsWith("ATOM") || line.StartsWith("HETATM")) {
					atoms.Add(line);
				}
			}
			Size = atoms.Count;
			AtomNames = new string[Size];
			AminoAcids = new string[Size];
			AminoAcidIds = new int[Size];
			X = new float[Size];
			Y = new float[Size];
			Z = new float[Size];
			Diameter = new float[Size];
			Charge = new float[Size];
			int i = 0;
			foreach(string r in atoms) {
				List<string> data = SplitString(r);
				AtomNames[i] = data[2];
				AminoAcids[i] = data[3];
				AminoAcidIds[i] = int.Parse(data[4]);
				if (AminoAcidIds[i] > MaxAminoAcidId) {
					MaxAminoAcidId = AminoAcidIds[i];
				}
				X[i] = float.Parse(data[5], System.Globalization.CultureInfo.InvariantCulture);
				Y[i] = float.Parse(data[6], System.Globalization.CultureInfo.InvariantCulture);
				Z[i] = float.Parse(data[7], System.Globalization.CultureInfo.InvariantCulture);
				Charge[i] = float.Parse(data[8], System.Globalization.CultureInfo.InvariantCulture);
				Diameter[i] = float.Parse(data[9], System.Globalization.CultureInfo.InvariantCulture) * 2;
				i++;
			}
		}
		
		public Vector GetAtom(int id) {
			return new Vector(
				X[id],
				Y[id],
				Z[id]
			);
		}
		
		public static List<string> SplitString(string source) {
			List<string> result = new List<string>();
			int begin = 0;
			while (begin < source.Length) {
				int end = source.IndexOf(" ", begin);
				if (end == -1) end = source.Length;
				if (end != begin) {
					result.Add(source.Substring(begin, end - begin));
				}
				begin = end + 1;
			}
			return result;
		}
	}
}