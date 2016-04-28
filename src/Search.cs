using System;

namespace Docking {
	struct State {
		public Transformation Transform;
		public float Value;
		
		public State(Transformation transform, float value) {
			Transform = transform;
			Value = value;
		}
	}
	
	class Search {
		public State Current;
		private int stableSteps;
		private int iteration = 0;
		private Random random = new Random();
		private float sizeParameter = 1;
		private float controlParameter = 10;
		private Grid grid;
		
		public Search(Grid grid) {
			this.grid = grid;
			Transformation transform = new Transformation(0, 0, 0, new Vector(0, 0, 0));
			Current = new State(
				transform,
				grid.GetValue(transform)
			);
		}
		
		public void Run(int steps) {
			while (iteration < steps) {
				Step();
			}
		}
		
		public void Step() {
			if (iteration % 100 == 0) {
				Console.WriteLine("Iteration " + iteration + ", value " + Current.Value);
			}
			iteration++;
			State neighbour = GetNeighbour();
			if (neighbour.Value <= Current.Value) {
				// Improvement, accept always
				Accept(neighbour);
				return;
			}
			// TODO: Simulated annealing
		}
		
		private void Accept(State neighbour) {
			Current = neighbour;
			stableSteps = 0;
		}

		private State GetNeighbour() {
			float dX = randomFloat(sizeParameter);
			float dY = randomFloat(sizeParameter);
			float dZ = randomFloat(sizeParameter);
			float dYaw = randomFloat(sizeParameter / 2);
			float dPitch = randomFloat(sizeParameter / 2);
			float dRoll = randomFloat(sizeParameter / 2);
			
			Transformation a = Current.Transform.Modify(dX, dY, dZ, dYaw, dPitch, dRoll);
			int factor = random.Next(2);
			if (factor == 0) factor = -1;
			Transformation b = Current.Transform.Modify(factor * dX, factor * dY, factor * dZ, factor * dYaw, factor * dPitch, factor * dRoll);
			
			float aValue = grid.GetValue(a);
			float bValue = grid.GetValue(b);
			
			// TODO: Plot graph and guess best position
			State best = aValue > bValue ? new State(b, bValue) : new State(a, aValue);
			return best;
		}
		
		private float randomFloat(float size) {
			return size * (float) random.NextDouble();
		}
	}
}