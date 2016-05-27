using System.IO;
using System.Collections.Generic;

namespace Docking {
	class Molecule {
		public string FileName;
		public int Size;
		public bool[] IsHetAtm;
		public int[] AtomId;
		public string[] AtomNames;
		public string[] AminoAcids;
		public int[] AminoAcidIds;
		public float[] X;
		public float[] Y;
		public float[] Z;
		public float[] Diameter;
		public float[] Charge;
		public Connections[] Connections;
		public int MaxAtomId;
		public int MaxAminoAcidId;
		public Molecule(string fileName) {
			FileName = fileName;
			
			StreamReader read = new StreamReader(File.Open(fileName, FileMode.Open));
			List<string> atoms = new List<string>();
			List<string> connects = new List<string>();
			while (read.Peek() >= 0) {
				string line = read.ReadLine();
				if (line.StartsWith("ATOM") || line.StartsWith("HETATM")) {
					atoms.Add(line);
				} else if (line.StartsWith("CONECT")) {
					connects.Add(line);
				}
			}
			Size = atoms.Count;
			IsHetAtm = new bool[Size];
			AtomId = new int[Size];
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
				IsHetAtm[i] = data[0] == "HETATM";
				AtomId[i] = int.Parse(data[1]);
				if (AtomId[i] > MaxAtomId) {
					MaxAtomId = AtomId[i];
				}
				AtomNames[i] = data[2];
				AminoAcids[i] = data[3];
				AminoAcidIds[i] = int.Parse(data[4]);
				if (AminoAcidIds[i] > MaxAminoAcidId) {
					MaxAminoAcidId = AminoAcidIds[i];
				}
				X[i] = Utils.ParseFloat(data[5]);
				Y[i] = Utils.ParseFloat(data[6]);
				Z[i] = Utils.ParseFloat(data[7]);
				Charge[i] = Utils.ParseFloat(data[8]);
				Diameter[i] = Utils.ParseFloat(data[9]) * 2;
				i++;
			}
			Connections = new Connections[connects.Count];
			i = 0;
			foreach (string r in connects) {
				List<string> data = SplitString(r);
				int from = int.Parse(data[1]);
				int[] to = new int[data.Count - 2];
				for (int j = 0; j < data.Count - 2; j++) {
					to[j] = int.Parse(data[j + 2]);
				}
				Connections[i] = new Connections(from, to);
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
	
	struct Connections {
		public int From;
		public int[] To;
		public Connections(int from, int[] to) {
			From = from;
			To = to;
		}
	}
}