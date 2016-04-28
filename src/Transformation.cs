using System;

namespace Docking {
	struct Transformation {
		public Vector Transpose;
		public Matrix Rotate;
		
		float yaw, pitch, roll;
		
		/**
		 * Creates a new transformation.
		 * Yaw, pitch and roll are the rotation around axes, see http://planning.cs.uiuc.edu/node102.html
		 */
		public Transformation(float yaw, float pitch, float roll, Vector transpose) {
			this.yaw = yaw;
			this.pitch = pitch;
			this.roll = roll;
			Transpose = transpose;
			Rotate = Matrix.Zero;
			Rotate = yawMatrix().Multiply(pitchMatrix()).Multiply(rollMatrix());
		}
		
		private Matrix yawMatrix() {
			return new Matrix(
				(float)Math.Cos(yaw), -(float)Math.Sin(yaw), 0,
				(float)Math.Sin(yaw), (float)Math.Cos(yaw), 0,
				0, 0, 1
			);
		}
		private Matrix pitchMatrix() {
			return new Matrix(
				(float)Math.Cos(pitch), 0, (float)Math.Sin(pitch),
				0, 1, 0,
				-(float)Math.Sin(pitch), 0, (float)Math.Cos(pitch)
			);
		}
		private Matrix rollMatrix() {
			return new Matrix(
				1, 0, 0,
				0, (float)Math.Cos(roll), -(float)Math.Sin(roll),
				0, (float)Math.Sin(roll), (float)Math.Cos(roll)
			);
		}
		
		public Vector Transform(Vector vector) {
			return Rotate.MultiplyVector(vector).Add(Transpose);
		}
		
		public Transformation Modify(float dX, float dY, float dZ, float dYaw, float dPitch, float dRoll) {
			return new Transformation(
				yaw + dYaw,
				pitch + dPitch,
				roll + dRoll,
				Transpose.Add(new Vector(dX, dY, dZ))
			);
		}
	}
	struct Matrix {
		public static Matrix Zero = new Matrix(0, 0, 0, 0, 0, 0, 0, 0, 0);
		
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
		
		public Matrix Multiply(Matrix other) {
			return new Matrix(
				A1 * other.A1 + A2 * other.B1 + A3 * other.C1,
				A2 * other.A2 + A2 * other.B2 + A3 * other.C2,
				A3 * other.A3 + A2 * other.B3 + A3 * other.C3,
				
				B1 * other.A1 + B2 * other.B1 + B3 * other.C1,
				B1 * other.A2 + B2 * other.B2 + B3 * other.C2,
				B1 * other.A3 + B2 * other.B3 + B3 * other.C3,
				
				C1 * other.A1 + C2 * other.B1 + C3 * other.C1,
				C1 * other.A2 + C2 * other.B2 + C3 * other.C2,
				C1 * other.A3 + C2 * other.B3 + C3 * other.C3
			);
		}
		
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
		
		public Vector Add(Vector other) {
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