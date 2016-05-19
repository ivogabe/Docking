using System;

namespace Docking {
	public class Program {
		public static void Main(string[] args) {
			if (args.Length != 3) {
				Console.WriteLine("Docking");
				Console.WriteLine("Usage:");
				Console.WriteLine("  dotnet run protein.pdb ligand.pdb output.pdb");
				return;
			}
			Console.WriteLine("Read molecules");
			Molecule moleculeA = new Molecule(args[0]);
			Molecule moleculeB = new Molecule(args[1]);
			Console.WriteLine("Create grid");
			Grid grid = new Grid(moleculeA, moleculeB);
			Search search = new Search(grid);
			search.Run(10000);
			Console.WriteLine();
			Console.WriteLine("Best score " + search.Best.Value);
			Output.Write(args[2], moleculeA, moleculeB, search.Best.Transform, search.Best.Value);
		}
	}
}
