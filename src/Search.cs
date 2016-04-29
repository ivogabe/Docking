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
		public State Best;
		private int stableSteps;
		private int iteration = 0;
		private Random random = new Random();
		private float sizeParameter = 1;
		private float controlParameter = 10;
		private Grid grid;
		
		public Search(Grid grid) {
			this.grid = grid;
			Transformation transform = new Transformation(0, 0, 0, new Vector(0, 0, 0));
			Best = Current = new State(
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
				controlParameter *= 0.95f;
				sizeParameter *= 0.98f;
			}
			if (stableSteps > 200) {
				controlParameter *= 1.5f;
				sizeParameter *= 1.5f;
				stableSteps = 0;
			}
			iteration++;
			State neighbour = GetNeighbour();
			float delta = neighbour.Value - Current.Value;
			if (delta <= 0) {
				// Improvement, accept always
				Accept(neighbour);
				return;
			}
			if (random.NextDouble() < Math.Exp(-delta / controlParameter)) {
				Accept(neighbour);
			}
		}
		
		private void Accept(State neighbour) {
			Current = neighbour;
			if (Current.Value <= Best.Value) Best = Current;
			stableSteps = 0;
		}

		private State GetNeighbour() {
			float dX = randomFloat(sizeParameter);
			float dY = randomFloat(sizeParameter);
			float dZ = randomFloat(sizeParameter);
			float dYaw = randomFloat(sizeParameter / 2);
			float dPitch = randomFloat(sizeParameter / 2);
			float dRoll = randomFloat(sizeParameter / 2);
			
			// Choose 3 points (0, 1, x3) and plot a parabola between them.
			// Then search the minimum of the parabola, if it exists.
			
			Transformation a = Current.Transform.Modify(dX, dY, dZ, dYaw, dPitch, dRoll);
			float x3 = random.Next(1) == 1 ? 2 : -1;
			Transformation b = Current.Transform.Modify(x3 * dX, x3 * dY, x3 * dZ, x3 * dYaw, x3 * dPitch, x3 * dRoll);
			
			float aValue = grid.GetValue(a);
			float bValue = grid.GetValue(b);
			
			float x4 = findExtreme(0, 1, x3, Current.Value, aValue, bValue);
			Transformation c = Current.Transform.Modify(x4 * dX, x4 * dY, x4 * dZ, x4 * dYaw, x4 * dPitch, x4 * dRoll);
			float cValue = grid.GetValue(c);
			
			if (aValue <= bValue && aValue <= cValue) {
				return new State(a, aValue);
			} else if (bValue <= cValue) {
				return new State(b, bValue);
			} else {
				return new State(c, cValue);
			}
		}
		
		private float randomFloat(float size) {
			return size * (float) random.NextDouble();
		}
		
		/**
		 * Returns the extreme of a parabola
		 */
		private float findExtreme(float x1, float x2, float x3, float y1, float y2, float y3) {
			float a1 = x1 * (y2 - y3);
			float a2 = x2 * (y3 - y1);
			float a3 = x3 * (y1 - y2);
			return (x1 * a1 + x2 * a2 + x3 * a3) / (a1 + a2 + a3);
		}
	}
}