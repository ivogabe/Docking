using System;
namespace Docking {
	class Grid {
		private float dimension = 1f; // Ångstrom
		private float scale; // Ångstrom ^ -1
		private float maxDistance = 8.5f; // Ångstrom
		private float maxDistanceSquared;
		
		/**
		 * Constant for electrostatic energy.
		 */
		private float epsilon0 = 1;
		/**
		 * Constant for Vanderwaals energy.
		 */
		private float epsilon = 25;
		
		private Molecule moleculeA, moleculeB;
		
		private int minX;
		private int maxX;
		private int minY;
		private int maxY;
		private int minZ;
		private int maxZ;
		private int rangeX;
		private int rangeY;
		private int rangeZ;
		
		private long blocks;
		private int[] atomsInBlock;
		private int[] blockStartIndex;
		private int[] atomIndices;
		
		public Grid(Molecule moleculeA, Molecule moleculeB) {
			scale = 1 / dimension;
			maxDistanceSquared = maxDistance * maxDistance;
			
			this.moleculeA = moleculeA;
			this.moleculeB = moleculeB;
			
			for (int i = 0; i < moleculeA.Size; i++) {
				Block block = GetBlock(moleculeA.GetAtom(i));
				if (block.X < minX) minX = block.X;
				if (block.X > maxX) maxX = block.X;
				if (block.Y < minY) minY = block.Y;
				if (block.Y > maxY) maxY = block.Y;
				if (block.Z < minZ) minZ = block.Z;
				if (block.Z > maxZ) maxZ = block.Z;
			}
			// Add small buffer to all edges
			int edge = (int) Math.Ceiling(4 * maxDistance * scale);
			minX -= edge;
			minY -= edge;
			minZ -= edge;
			maxX += edge;
			maxY += edge;
			maxZ += edge;
			
			rangeX = maxX - minX + 1;
			rangeY = maxY - minY + 1;
			rangeZ = maxZ - minZ + 1;
			
			blocks = rangeX * rangeY * rangeZ;
			atomsInBlock = new int[blocks];
			blockStartIndex = new int[blocks];
			int atomIndicesSize = 0;
			forEachBlockAndAtom((block, atom) => {
				atomsInBlock[GetIndex(block)]++;
				atomIndicesSize++;
			});
			int pos = 0;
			for (int i = 0; i < blocks; i++) {
				blockStartIndex[i] = pos;
				pos += atomsInBlock[i];
				atomsInBlock[i] = 0;
			}
			atomIndices = new int[atomIndicesSize];
			forEachBlockAndAtom((block, atom) => {
				int i = GetIndex(block);
				atomIndices[blockStartIndex[i] + atomsInBlock[i]] = atom;
				atomsInBlock[i]++;
			});
		}
		
		void forEachBlockAndAtom(Action<Block, int> callback) {
			int radius = (int) Math.Ceiling(maxDistance * scale);
			float radiusSquared = maxDistance * maxDistance + dimension * dimension;
			for (int i = 0; i < moleculeA.Size; i++) {
				Vector vector = moleculeA.GetAtom(i);
				Block block = GetBlock(vector);
				// Console.WriteLine("atom  " + vector);
				// Console.WriteLine("block " + BlockCenter(block));
				
				for (int x = block.X - radius; x <= block.X + radius; x++) {
					for (int y = block.Y - radius; y <= block.Y + radius; y++) {
						for (int z = block.Z - radius; z <= block.Z + radius; z++) {
							Block current = new Block(x, y, z);
							// Console.WriteLine("atom  " + vector);
							// Console.WriteLine("block " + BlockCenter(current));
							// Console.WriteLine("Distance squared " + vector.DistanceSquared(BlockCenter(current)) + ", max " + radiusSquared);
							// if (vector.DistanceSquared(BlockCenter(current)) <= radiusSquared) {
								callback(current, i);
							// }
						}
					}
				}
			}
		}
		
		Block GetBlock(Vector point) {
			return new Block(
				round(point.X * scale),
				round(point.Y * scale),
				round(point.Z * scale)
			);
		}
		int round(float x) {
			if (x >= 0) return (int)x;
			return (int) (x - 1);
		}
		
		int GetIndex(Block block) {
			int x = block.X - minX;
			int y = block.Y - minY;
			int z = block.Z - minZ;
			return x + (y * rangeX + z * rangeY);
		}
		bool isOutOfRange(Block block) {
			return block.X < minX || block.X > maxX
				|| block.Y < minY || block.Y > maxY
				|| block.Z < minZ || block.Z > maxZ;
		}
		
		Vector BlockCenter(Block block) {
			return new Vector(
				(block.X + 0.5f) * dimension,
				(block.Y + 0.5f) * dimension,
				(block.Z + 0.5f) * dimension
			);
		}
		
		public float GetValue(Transformation transform) {
			float result = 0;
			bool near = false;
			
			for (int i = 0; i < moleculeB.Size; i++) {
				Vector atom = transform.Transform(moleculeB.GetAtom(i));
				Block block = GetBlock(atom);
				
				if (isOutOfRange(block)) continue;
				
				int blockId = GetIndex(block);
				int end = blockStartIndex[blockId] + atomsInBlock[blockId];
				// Console.WriteLine("start = " + blockStartIndex[blockId] + ", end = " + end + ", " + blockId);
				for (int j = blockStartIndex[blockId]; j < end; j++) {
					float energy = energyBetween(transform, atomIndices[j], i);
					if (energy != 0) {
						near = true;
						result += energy;
					}
				}
			}
			// If the molecules don't interact with eachother,
			// return MaxValue so we won't choose this configuration
			if (!near) {
				// Console.WriteLine("Too far " + transform);
				return float.MaxValue;
			} else {
				// Console.WriteLine("Ok " + result + ", " + transform);
			}
			return result;
		}
		
		private float energyBetween(Transformation transform, int idA, int idB) {
			Vector atomA = moleculeA.GetAtom(idA);
			Vector atomB = transform.Transform(moleculeB.GetAtom(idB));
			float distanceSquared = atomA.DistanceSquared(atomB);
			if (distanceSquared > maxDistanceSquared) return 0;
			
			float distance = Math.Max((float) Math.Sqrt(distanceSquared), 0.01f);
			float electrostatic = moleculeA.Charge[idA] * moleculeB.Charge[idB] / (4 * (float) Math.PI * epsilon0 * distance);
			float sigma = (moleculeA.Diameter[idA] + moleculeB.Diameter[idB]) * 0.5f;
			float pow6 = power6(sigma / distance);
			float vanderwaals = 4 * epsilon * pow6 * (pow6 - 1);
			
			return electrostatic + vanderwaals;
		}
		
		private float power6(float value) {
			return value * value * value * value * value * value;
		}
	}
	
	struct Block {
		public Block(int x, int y, int z) {
			X = x;
			Y = y;
			Z = z;
		}
		public int X;
		public int Y;
		public int Z;
	}
}