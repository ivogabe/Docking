using System.IO;

namespace Docking {
	class Output {
		public static void Write(string fileName, Molecule moleculeA, Molecule moleculeB, Transformation transform, float value) {
			new Output(fileName, moleculeA, moleculeB, transform, value);
		}
		
		private StreamWriter writer;
		private int remarkId = 1;
		private int atomId = 1;
		
		private Output(string fileName, Molecule moleculeA, Molecule moleculeB, Transformation transform, float value) {
			writer = new StreamWriter(File.Open(fileName, FileMode.Create));
			
			addRemark(new string [] { "Binding of " + moleculeA.FileName, "with " + moleculeB.FileName });
			addRemark(new string [] { "Energy between molecules: " + Utils.FloatToString(value) });
			addRemark(new string [] {
				"Transformation:",
				"rotate " + transform.RotationString(),
				"transpose "
					+ Utils.FloatToString(transform.Transpose.X)
					+ ", " + Utils.FloatToString(transform.Transpose.Y)
					+ ", " + Utils.FloatToString(transform.Transpose.Z)
			});
			
			for (int i = 0; i < moleculeA.Size; i++) {
				addAtom(
					moleculeA.AtomNames[i],
					moleculeA.AminoAcids[i],
					moleculeA.AminoAcidIds[i],
					moleculeA.X[i],
					moleculeA.Y[i],
					moleculeA.Z[i],
					moleculeA.Charge[i],
					moleculeA.Diameter[i]
				);
			}
			for (int i = 0; i < moleculeB.Size; i++) {
				Vector vector = transform.Transform(moleculeB.GetAtom(i));
				addAtom(
					moleculeB.AtomNames[i],
					moleculeB.AminoAcids[i],
					moleculeB.AminoAcidIds[i],
					vector.X,
					vector.Y,
					vector.Z,
					moleculeB.Charge[i],
					moleculeB.Diameter[i]
				);
			}
			addRemark(new string [] { "End" });
			writer.Flush();
		}
		
		private void addRemark(string[] lines) {
			string head = "REMARK " + prefix(remarkId.ToString(), 3) + " ";
			foreach (string line in lines) {
				writer.WriteLine(head + line);
			}
			remarkId++;
		}
		private void addAtom(string atom, string aminoAcid, int aminoAcidId, float x, float y, float z, float charge, float diameter) {
			writer.WriteLine(
				"ATOM " + prefix(atomId.ToString(), 6)
				+ " " + postfix(atom.Length > 4 ? atom : " " + atom, 3)
				+ " " + postfix(aminoAcid, 3)
				+ " " + prefix(aminoAcidId.ToString(), 5)
				+ " " + prefix(Utils.FloatToString(x, "0.###"), 7)
				+ " " + prefix(Utils.FloatToString(x, "0.###"), 7)
				+ " " + prefix(Utils.FloatToString(x, "0.###"), 7)
				+ " " + prefix(Utils.FloatToString(charge), 7)
				+ " " + prefix(Utils.FloatToString(diameter / 2), 6)
			);
			atomId++;
		}
		
		private string prefix(string str, int length) {
			if (str.Length > length) {
				return str.Substring(0, length);
			}
			while (str.Length < length) {
				str = " " + str;
			}
			return str;
		}
		private string postfix(string str, int length) {
			if (str.Length > length) {
				return str.Substring(0, length);
			}
			while (str.Length < length) {
				str = str + " ";
			}
			return str;
		}
	}
}