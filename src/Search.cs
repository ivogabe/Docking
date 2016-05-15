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
		private float sizeParameter = 0.1f;
		private float controlParameter;
		private Grid grid;
		
		public Search(Grid grid) {
			this.grid = grid;
			float value = float.NaN;
			Transformation transform;
			Console.WriteLine("Find starting position");
			do {
				Vector vector = new Vector(
					randomFloat(120) - 60,
					randomFloat(120) - 60,
					randomFloat(120) - 60
				);
				transform = new Transformation(
					randomFloat((float)Math.PI * 2),
					randomFloat((float)Math.PI * 2),
					randomFloat((float)Math.PI * 2),
					vector
				);
				value = grid.GetValue(transform);
			} while (float.IsNaN(value) || float.IsInfinity(value) || value > 10);
			controlParameter = 0.5f;
			Console.WriteLine("Start local search");
			Best = Current = new State(
				transform,
				value
			);
		}
		
		public void Run(int steps) {
			while (iteration < steps) {
				Step();
			}
		}
		
		private bool unfinishedLine = false;
		private void finishLine() {
			if (unfinishedLine) {
				unfinishedLine = false;
				Console.WriteLine();
			}
		}
		public void Step() {
			if (iteration % 100 == 0) {
				finishLine();
				Console.WriteLine("Iteration " + iteration + ", value " + Current.Value);
				controlParameter *= 0.9f;
				sizeParameter *= 0.7f;
			}
			if (stableSteps > 10) {
				unfinishedLine = false;
				controlParameter *= 1.5f;
				sizeParameter *= 1.8f;
				stableSteps = 0;
				Console.WriteLine(" Local minimum!");
			}
			iteration++;
			stableSteps++;
			State neighbour = GetNeighbour();
			if (float.IsNaN(neighbour.Value)) {
				return;
			}
			float delta = (neighbour.Value - Current.Value);
			if (delta <= 0) {
				// Improvement, accept always
				finishLine();
				Console.WriteLine(" [+] " + Current.Value + " -> " + neighbour.Value + (neighbour.Value <= Best.Value ? " best" : ""));
				Accept(neighbour);
			} else if (neighbour.Value < float.MaxValue && random.NextDouble() < Math.Exp(-delta / (Math.Max(Current.Value, 1) * controlParameter))) {
				finishLine();
				Console.WriteLine(" [-] " + Current.Value + " -> " + neighbour.Value);
				Accept(neighbour);
			} else {
				if (!unfinishedLine) {
					Console.Write(" ");
				}
				Console.Write("/");
				unfinishedLine = true;
			}
		}
		
		private void Accept(State neighbour) {
			if (Math.Abs(Current.Value - neighbour.Value) > 1) {
				stableSteps = 0;
			}
			Current = neighbour;
			if (Current.Value < Best.Value) {
				Best = Current;
			}
		}

		float nextX3 = -1;
		private State GetNeighbour() {
			float dX = randomFloat(sizeParameter * 20);
			float dY = randomFloat(sizeParameter * 20);
			float dZ = randomFloat(sizeParameter * 20);
			float dYaw = randomFloat(sizeParameter);
			float dPitch = randomFloat(sizeParameter);
			float dRoll = randomFloat(sizeParameter);
			
			// Choose 3 points (0, 1, x3) and plot a parabola between them.
			// Then search the minimum of the parabola, if it exists.
			
			Transformation a = Current.Transform.Modify(dX, dY, dZ, dYaw, dPitch, dRoll);
			float x3 = nextX3;
			nextX3 = 1 - x3; // 2 -> -1, -1 -> 2
			Transformation b = Current.Transform.Modify(x3 * dX, x3 * dY, x3 * dZ, x3 * dYaw, x3 * dPitch, x3 * dRoll);
			
			float aValue = grid.GetValue(a);
			float bValue = grid.GetValue(b);
			
			Transformation c = a;
			float cValue = float.MaxValue;
			
			if (aValue < float.MaxValue && bValue < float.MaxValue) {
				float x4 = findExtreme(0, 1, x3, Current.Value, aValue, bValue);
				if (!float.IsNaN(x4)) {
					c = Current.Transform.Modify(x4 * dX, x4 * dY, x4 * dZ, x4 * dYaw, x4 * dPitch, x4 * dRoll);
					cValue = grid.GetValue(c);
				}
			}

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