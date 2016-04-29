using System.IO;
using System.Collections.Generic;

namespace Docking {
	class Molecule {
		public int Size;
		public float[] X;
		public float[] Y;
		public float[] Z;
		public float[] Diameter;
		public float[] Charge;
		public Molecule(string fileName) {
			// TODO: Load data from PDB file
			StreamReader read = new StreamReader(fileName);
			List<string> file = new List<string>();
			while (read.Peek() >= 0) {
				string line = read.ReadLine();
				if (line.StartsWith("ATOM")) {
					file.Add(line);
				}				
			}
			Size = file.Count;
			X = new float[Size];
			Y = new float[Size];
			Z = new float[Size];
			Diameter = new float[Size];
			Charge = new float[Size];
			int t = 0;
			foreach(string r in file) {
				X[t] = float.Parse(r.Substring(30, 8));
				Y[t] = float.Parse(r.Substring(38, 8));
				Z[t] = float.Parse(r.Substring(46, 8));
				Charge[t] = float.Parse(r.Substring(55, 8));
				Diameter[t] = float.Parse(r.Substring(62, 6)) * 2;
				t ++;
			}
			 
		}
		
		public Vector GetAtom(int id) {
			return new Vector(
				X[id],
				Y[id],
				Z[id]
			);
		}
	}
}