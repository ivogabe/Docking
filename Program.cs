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
			
			State best = new State(new Transformation(0, 0, 0, new Vector(0, 0, 0)), float.MaxValue);
			
			for (int i = 0; i < 5; i++) {
				Console.WriteLine("Run " + i);
				Search search = new Search(grid);
				search.Run(2000);
				Console.WriteLine();
				Console.WriteLine(" Best score " + Utils.FloatToString(search.Best.Value));
				if (search.Best.Value < best.Value) {
					best = search.Best;
				}
			}
			Console.WriteLine("Global best score " + Utils.FloatToString(best.Value));
			Output.Write(args[2], moleculeA, moleculeB, best.Transform, best.Value);
		}
	}
}
