using System;

namespace Docking {
	class Grid {
		private float dimension = 1; // Ångstrom
		private float maxDistance = 8.5f; // Ångstrom
		
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
		
		private int blocks;
		private int[] atomsInBlock;
		private int[] blockStartIndex;
		private int[] atomIndices;
		
		public Grid(Molecule moleculeA, Molecule moleculeB) {
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
			// Add 9 Armstrong on all edges
			minX -= (int) (9 / dimension);
			minY -= (int) (9 / dimension);
			minZ -= (int) (9 / dimension);
			maxX += (int) (9 / dimension);
			maxY += (int) (9 / dimension);
			maxZ += (int) (9 / dimension);
			
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
			int radius = (int) Math.Ceiling(maxDistance / dimension);
			int radiusSquared = radius * radius;
			for (int i = 0; i < moleculeA.Size; i++) {
				Vector vector = moleculeA.GetAtom(i);
				Block block = GetBlock(vector);
				for (int x = block.X - radius; x <= block.X + radius; x++) {
					for (int y = block.Y - radius; y <= block.Y + radius; y++) {
						for (int z = block.Z - radius; z <= block.Z + radius; z++) {
							Block current = new Block(x, y, z);
							if (vector.DistanceSquared(BlockCenter(current)) <= radiusSquared) {
								callback(current, i);
							}
						}
					}
				}
			}
		}
		
		Block GetBlock(Vector point) {
			return new Block(
				round(point.X / dimension),
				round(point.Y / dimension),
				round(point.Z / dimension)
			);
		}
		int round(float x) {
			if (x >= 0) return (int)x;
			return (int) (x - 1);
		}
		
		int GetIndex(Block block) {
			int x = block.X - rangeX;
			int y = block.Y - rangeY;
			int z = block.Z - rangeZ;
			return x + (y * rangeX + z * rangeY);
		}
		
		Vector BlockCenter(Block block) {
			return new Vector(
				(block.X + 0.5f) * dimension,
				(block.Y + 0.5f) * dimension,
				(block.Z + 0.5f) * dimension
			);
		}
		
		public float GetValue(Transformation transformation) {
			float result = 0;
			// TODO
			return result;
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