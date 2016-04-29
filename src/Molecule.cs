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