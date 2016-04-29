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