// This definition doesn't exist in SharpDX for some reason so I've added it

using System;
using System.Runtime.InteropServices;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

namespace Project2
{
    /// <summary>
    /// Describes a custom vertex format structure that contains position and color information.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct VertexPositionNormalColor : IEquatable<VertexPositionNormalColor>
    {
        /// <summary>
        /// Initializes a new <see cref="VertexPositionNormalColor"/> instance.
        /// </summary>
        /// <param name="position">The position of this vertex.</param>
        /// <param name="normal">The vertex normal.</param>
        /// <param name="color">The color of this vertex.</param>
        public VertexPositionNormalColor(Vector3 position, Vector3 normal, Color color) : this()
        {
            Position = position;
            Normal = normal;
            Color = color;
        }

        /// <summary>
        /// XYZ position.
        /// </summary>
        [VertexElement("SV_POSITION")]
        public Vector3 Position;

        /// <summary>
        /// The vertex normal.
        /// </summary>
        [VertexElement("NORMAL")]
        public Vector3 Normal;

        /// <summary>
        /// Color.
        /// </summary>
        [VertexElement("COLOR")]
        public Color Color;

        public bool Equals(VertexPositionNormalColor other)
        {
            return Position.Equals(other.Position) && Normal.Equals(other.Normal) && Color.Equals(other.Color);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is VertexPositionNormalColor&& Equals((VertexPositionNormalColor) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Position.GetHashCode();
                hashCode = (hashCode * 397) ^ Normal.GetHashCode();
                hashCode = (hashCode * 397) ^ Color.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(VertexPositionNormalColor left, VertexPositionNormalColor right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(VertexPositionNormalColor left, VertexPositionNormalColor right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return string.Format("Position: {0}, Normal: {1}, Color: {2}", Position, Normal, Color);
        }
    }
}
