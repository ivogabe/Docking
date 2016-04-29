using System;

namespace Docking {
	public class Program {
		public static void Main(string[] args) {
			if (args.Length != 2) {
				Console.WriteLine("Docking");
				Console.WriteLine("Usage:");
				Console.WriteLine("  dnx run protein.pdb ligand.pdb output.pdb");
				return;
			}
			Molecule moleculeA = new Molecule(args[0]);
			Molecule moleculeB = new Molecule(args[1]);
			Grid grid = new Grid(moleculeA, moleculeB);
			Search search = new Search(grid);
			search.Run(10000);
			Console.WriteLine("Best score " + search.Best.Value);
			Output.Write(args[2], moleculeA, moleculeB, search.Best.Transform);
		}
	}
}
