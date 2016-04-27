using System;

namespace Docking {
	struct Transformation {
		public Vector Transpose;
		
		// See http://planning.cs.uiuc.edu/node102.html
		public float Yaw;
		public float Pitch;
		public float Roll;
		
		private Matrix yawMatrix() {
			return new Matrix(
				(float)Math.Cos(Yaw), -(float)Math.Sin(Yaw), 0,
				(float)Math.Sin(Yaw), (float)Math.Cos(Yaw), 0,
				0, 0, 1
			);
		}
		private Matrix pitchMatrix() {
			return new Matrix(
				(float)Math.Cos(Pitch), 0, (float)Math.Sin(Pitch),
				0, 1, 0,
				-(float)Math.Sin(Pitch), 0, (float)Math.Cos(Pitch)
			);
		}
		private Matrix rollMatrix() {
			return new Matrix(
				1, 0, 0,
				0, (float)Math.Cos(Roll), -(float)Math.Sin(Roll),
				0, (float)Math.Sin(Roll), (float)Math.Cos(Roll)
			);
		}
		
		public Vector Transform(Vector point) {
			Vector rotated = yawMatrix().MultiplyVector(
				pitchMatrix().MultiplyVector(
					yawMatrix().MultiplyVector(point)
				)
			);
			return rotated.add(Transpose);
		}
	}
	struct Matrix {
		public Matrix(float a1, float a2, float a3, float b1, float b2, float b3, float c1, float c2, float c3) {
			A1 = a1;
			A2 = a2;
			A3 = a3;
			B1 = b1;
			B2 = b2;
			B3 = b3;
			C1 = c1;
			C2 = c2;
			C3 = c3;
		}
		
		public float A1, A2, A3;
		public float B1, B2, B3;
		public float C1, C2, C3;
		
		public Vector MultiplyVector(Vector vector) {
			return new Vector(
				A1 * vector.X + A2 * vector.Y + A3 * vector.Z,
				B1 * vector.X + B2 * vector.Y + B3 * vector.Z,
				C1 * vector.X + C2 * vector.Y + C3 * vector.Z
			);
		}
	}
	struct Vector {
		public Vector(float x, float y, float z) {
			X = x;
			Y = y;
			Z = z;
		}
		public float X;
		public float Y;
		public float Z;
		
		public Vector add(Vector other) {
			return new Vector(
				X + other.X,
				Y + other.Y,
				Z + other.Z
			);
		}
		
		private float square(float x) {
			return x * x;
		}
		public float DistanceSquared(Vector other) {
			return square(X - other.X)
				+ square(Y - other.Y)
				+ square(Z - other.Z);
		}
		public float Distance(Vector other) {
			return (float) Math.Sqrt(DistanceSquared(other));
		}
	}
}