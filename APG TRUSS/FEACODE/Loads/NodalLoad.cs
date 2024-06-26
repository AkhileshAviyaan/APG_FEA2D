﻿using FEA2D.Elements;
using System;

namespace FEA2D.Loads
{
    /// <summary>
    /// Represent a class for Nodal loads in Global or local coordinates system.
    /// </summary>
    [System.Serializable]
    public class NodalLoad
    {
        /// <summary>
        /// Creates a new class of <see cref="NodalLoad"/>.
        /// </summary>
        public NodalLoad()
        {
        }

        /// <summary>
        /// Creates a new class of <see cref="NodalLoad"/>
        /// </summary>
        /// <param name="fx">force parallel to Global X direction.</param>
        /// <param name="fy">force parallel to Global Y direction.</param>
        /// <param name="mz">Moment parallel to Global Z direction.</param>
        /// <param name="direction">load direction.</param>
        /// <param name="loadCase">load case.</param>
        public NodalLoad(double fx, double fy, double mz, LoadDirection direction, LoadCase loadCase) : this()
        {
            this.Fx = fx;
            this.Fy = fy;
            this.Mz = mz;
            this.LoadDirection = direction;
            this.LoadCase = loadCase;
        }

		public NodalLoad(double angle, double f)
		{
			this.Mz = 0;
            this.F = f;
            this.Angle = angle * Math.PI / 180;
			this.LoadDirection = LoadDirection.Global;

            this.CosAngle=(float)Math.Cos(Angle);
            this.SinAngle=(float)Math.Sin(Angle);
			this.CosPlusAngle = (float)Math.Cos(Angle+Math.PI/4);
			this.SinPlusAngle = (float)Math.Sin(Angle + Math.PI / 4);
			this.CosMinusAngle = (float)Math.Cos(Angle - Math.PI / 4);
			this.SinMinusAngle = (float)Math.Sin(Angle - Math.PI / 4);
			this.Fx= -f* Math.Cos(Angle);
			this.Fy=-f * Math.Sin(Angle);
			if (Math.Abs(Fx) < 1e-10) Fx = 0;
			if (Math.Abs(Fy) < 1e-10) Fy = 0;
		}


		/// <summary>
		/// Force in X-Direction.
		/// </summary>
		public double Fx { get; set; }
		public float CosAngle { get; set; }
		public float SinAngle { get; set; }
		public float CosPlusAngle { get; set; }
		public float SinPlusAngle { get; set; }
		public float CosMinusAngle { get; set; }
		public float SinMinusAngle { get; set; }

		/// <summary>
		/// Total Force.
		/// </summary>
		public double F { get; set; }
		/// <summary>
		/// Force direction Angle.
		/// </summary>
		public double Angle { get; set; }

		/// <summary>
		/// Force in Y-Direction.
		/// </summary>
		public double Fy { get; set; }

        /// <summary>
        /// Moment in Z-Direction.
        /// </summary>
        public double Mz { get; set; }

        /// <inheritdoc/>
        public LoadDirection LoadDirection { get; set; }

        /// <inheritdoc/>
        public LoadCase LoadCase { get; set; }

        /// <inheritdoc/>
        public double[] GetGlobalFixedEndForces(Node2D node)
        {
            // create force vector
            double[] Q = new double[3] { Fx, Fy, Mz };
            
            // if the forces is in global coordinate system of the node then return it.
            if (this.LoadDirection == LoadDirection.Global)
            {
                return Q;
            }
            // transform the load vector to the local coordinate of the node.
            double[] F = new double[3];
            node.TransformationMatrix.TransposeMultiply(Q, F);
            return F;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }
            if (obj.GetType() != typeof(NodalLoad))
            {
                return false;
            }
            NodalLoad nl = obj as NodalLoad;
            if (Fx != nl.Fx || Fy != nl.Fy || Mz != nl.Mz || LoadCase != nl.LoadCase)
            {
                return false;
            }
            return true;
        }

        /// <inheritdoc/>
        public static bool operator ==(NodalLoad nl1, NodalLoad nl2)
        {
            if (nl1 is null)
            {
                return false;
            }
            return nl1.Equals(nl2);
        }

        /// <inheritdoc/>
        public static bool operator !=(NodalLoad nl1, NodalLoad nl2)
        {
            if (ReferenceEquals(nl1, null))
            {
                return false;
            }
            return !nl1.Equals(nl2);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            int result = 0;
            result += (Fx + 1e-10).GetHashCode();
            result += (Fy + 2e-10).GetHashCode();
            result += (Mz + 6e-10).GetHashCode();
            result += LoadCase.GetHashCode();
            return result;
        }


    }
}
