﻿using FEA2D.Materials;
using static System.Math;
namespace FEA2D.CrossSections
{
    /// <summary>
    /// Represents a Solid Circular Cross-Section.
    /// </summary>
    /// <seealso cref="FEA2D.CrossSections.IFrame2DSection" />
    [System.Serializable]
    public class CirclularSection : IFrame2DSection
    {
        /// <summary>
        /// Creates new instance of a <see cref="CirclularSection"/>.
        /// </summary>
        /// <param name="d">diameter of the <see cref="CirclularSection"/>.</param>
        /// <param name="material">Material of the cross-section.</param>
        public CirclularSection(double d, IMaterial material) : base()
        {
            this.D = d;
            this.Material = material;

            // calculate section properties and set them here 
            // to avoid recalculation in each time they are called to save time.
            SetSectionProperties(d);
        }

        /// <summary>
        /// Diameter of the circular section.
        /// </summary>
        public double D { get; set; }

        /// <summary>
        /// Sets the section properties.
        /// </summary>
        /// <param name="d">diameter of the <see cref="CirclularSection"/>.</param>
        private void SetSectionProperties(double d)
        {
            this.A = PI * d * d / 4.0;
            this.Ay = this.Ax = this.A * 0.9;
            this.Iy = this.Ix = PI * d * d * d * d / 64.0;
            this.J = 0.5 * this.Iy;
            base.MaxWidth = base.MaxHeight = d;
        }
    }
}
