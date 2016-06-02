using System.IO;

// PDB specification: http://deposit.rcsb.org/adit/docs/pdb_atom_format.html#ATOM
//  1 -  6        Record name     "ATOM  "
//  7 - 11        Integer         Atom serial number.
// 13 - 16        Atom            Atom name.
// 17             Character       Alternate location indicator.
// 18 - 20        Residue name    Residue name.
// 22             Character       Chain identifier.
// 23 - 26        Integer         Residue sequence number.
// 27             AChar           Code for insertion of residues.
// 31 - 38        Real(8.3)       Orthogonal coordinates for X in Angstroms.
// 39 - 46        Real(8.3)       Orthogonal coordinates for Y in Angstroms.
// 47 - 54        Real(8.3)       Orthogonal coordinates for Z in Angstroms.
// 55 - 60        Real(6.2)       Occupancy.
// 61 - 66        Real(6.2)       Temperature factor (Default = 0.0).
// 73 - 76        LString(4)      Segment identifier, left-justified.
// 77 - 78        LString(2)      Element symbol, right-justified.
// 79 - 80        LString(2)      Charge on the atom.
// Example:
// ATOM    145  N   VAL A  25      32.433  16.336  57.540  1.00 11.92      A1   N

namespace Docking {
	class Output {
		public static void Write(string fileName, Molecule moleculeA, Molecule moleculeB, Transformation transform, float value) {
			new Output(fileName, moleculeA, moleculeB, transform, value);
		}
		
		private StreamWriter writer;
		private int remarkId = 1;
		
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
					moleculeA.IsHetAtm[i],
					moleculeA.AtomId[i],
					moleculeA.AtomNames[i],
					moleculeA.AminoAcids[i],
					'A',
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
					moleculeB.IsHetAtm[i],
					moleculeA.MaxAtomId + moleculeB.AtomId[i],
					moleculeB.AtomNames[i],
					moleculeB.AminoAcids[i],
					'B',
					moleculeA.MaxAminoAcidId + moleculeB.AminoAcidIds[i],
					vector.X,
					vector.Y,
					vector.Z,
					moleculeB.Charge[i],
					moleculeB.Diameter[i]
				);
			}
			for (int i = 0; i < moleculeA.Connections.Length; i++) {
				addConnect(moleculeA.Connections[i], 0);
			}
			for (int i = 0; i < moleculeB.Connections.Length; i++) {
				addConnect(moleculeB.Connections[i], moleculeA.MaxAtomId);
			}
			writer.Write("END");
			writer.Flush();
		}
		
		private void addRemark(string[] lines) {
			string head = "REMARK " + prefix(remarkId.ToString(), 3) + " ";
			foreach (string line in lines) {
				writer.WriteLine(head + line);
			}
			remarkId++;
		}
		private void addAtom(bool isHetAtm, int atomId, string atom, string aminoAcid, char molecule, int aminoAcidId, float x, float y, float z, float charge, float diameter) {
			writer.WriteLine(
				(isHetAtm ? "HETATM" : "ATOM  ")
				+ prefix(atomId.ToString(), 5)
				+ " " + postfix(atom.Length > 4 ? atom : " " + atom, 5)
				+ postfix(aminoAcid, 3)
				+ " " + molecule
				+ " " + prefix(aminoAcidId.ToString(), 3)
				+ "    "
				+ prefix(Utils.FloatToString(x, "0.###"), 8)
				+ prefix(Utils.FloatToString(y, "0.###"), 8)
				+ prefix(Utils.FloatToString(z, "0.###"), 8)
				/* + prefix(Utils.FloatToString(charge), 7)
				+ prefix(Utils.FloatToString(diameter / 2), 6) */
				+ "  1.00 00.00           " + atom.Substring(0, 1)
			);
			atomId++;
		}
		private void addConnect(Connections connects, int idIncrease) {
			writer.Write("CONECT ");
			writer.Write(prefix((connects.From + idIncrease).ToString(), 4));
			foreach (int to in connects.To) {
				writer.Write(" ");
				writer.Write(prefix((to + idIncrease).ToString(), 4));
			}
			writer.WriteLine();
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