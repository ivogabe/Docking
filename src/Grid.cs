using System;

namespace Docking {
	class Grid {
		private float dimension = 1; // Armstrong
		private float maxDistance = 8.5f; // Armstrong
		
		private Molecule molecule;
		
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
		
		public Grid(Molecule molecule) {
			this.molecule = molecule;
			
			for (int i = 0; i < molecule.Size; i++) {
				Block block = GetBlock(molecule.GetAtom(i));
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
			
			rangeX = maxX - minX;
			rangeY = maxY - minY;
			rangeZ = maxZ - minZ;
			
			blocks = rangeX * rangeY * rangeZ;
			atomsInBlock = new int[blocks];
			
			int radius = (int) Math.Ceiling(maxDistance / dimension);
			for (int i = 0; i < molecule.Size; i++) {
				Block block = GetBlock(molecule.GetAtom(i));
				for (int x = block.X - radius; x <= block.X + radius; x++) {
					for (int y = block.Y - radius; y <= block.Y + radius; y++) {
						for (int z = block.Z - radius; z <= block.Z + radius; z++) {
							
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