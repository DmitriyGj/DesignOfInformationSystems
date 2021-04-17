using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

namespace Inheritance.Geometry
{
    public abstract class Body
    {
        public Vector3 Position { get; }

        public Body(Vector3 position)
        {
            Position = position;
        }

        public abstract bool ContainsPoint(Vector3 vector);

        public abstract RectangularCuboid GetBoundingBox();
    }

    public class Ball : Body
    {
        public double Radius { get; }

        public Ball(Vector3 position, double radius) : base(position)
        {
            Radius = radius;
        }

        public override bool ContainsPoint(Vector3 point)
        {
            var vector = point - Position;
            var length2 = vector.GetLength2();
            return length2 <= Radius * Radius;
        }

        public override RectangularCuboid GetBoundingBox() => new RectangularCuboid(Position, 2 * Radius, 2 * Radius, 2 * Radius);
    }

    public class RectangularCuboid : Body
    {
        public double SizeX { get; }
        public double SizeY { get; }
        public double SizeZ { get; }

        public RectangularCuboid(Vector3 position, double sizeX, double sizeY, double sizeZ) : base(position)
        {
            SizeX = sizeX;
            SizeY = sizeY;
            SizeZ = sizeZ;
        }

        public override bool ContainsPoint(Vector3 point)
        {
            var minPoint = new Vector3(
                 Position.X - SizeX / 2,
                 Position.Y - SizeY / 2,
                 Position.Z - SizeZ / 2);
            var maxPoint = new Vector3(
                Position.X + SizeX / 2,
                Position.Y + SizeY / 2,
                Position.Z + SizeZ / 2);

            return point >= minPoint && point <= maxPoint;
        }

        public override RectangularCuboid GetBoundingBox() => new RectangularCuboid(Position,SizeX,SizeY,SizeZ);
    }

    public class Cylinder : Body
    {
        public double SizeZ { get; }

        public double Radius { get; }

        public Cylinder(Vector3 position, double sizeZ, double radius) : base(position)
        {
            SizeZ = sizeZ;
            Radius = radius;
        }

        public override bool ContainsPoint(Vector3 point)
        {
            var vectorX = point.X - Position.X;
            var vectorY = point.Y - Position.Y;
            var length2 = vectorX * vectorX + vectorY * vectorY;
            var minZ = Position.Z - SizeZ / 2;
            var maxZ = minZ + SizeZ;

            return length2 <= Radius * Radius && point.Z >= minZ && point.Z <= maxZ;
        }

        public override RectangularCuboid GetBoundingBox() => new RectangularCuboid(Position, 2 * Radius, 2 * Radius, SizeZ);
    }

    public class CompoundBody : Body
    {
        public IReadOnlyList<Body> Parts { get; }

        public CompoundBody(IReadOnlyList<Body> parts) : base(parts[0].Position)
        {
            Parts = parts;
        }

        public override bool ContainsPoint(Vector3 point) => Parts.Any(body => body.ContainsPoint(point));
        
        public override RectangularCuboid GetBoundingBox()
        {
            var parse = Parts.Select(s => s.GetBoundingBox()).ToList();
            var minVectors = parse.Select(s =>new Vector3(s.Position.X - s.SizeX / 2, s.Position.Y - s.SizeY / 2, s.Position.Z - s.SizeZ / 2)).ToList();
            var maxVectors = parse.Select(s =>new Vector3(s.Position.X + s.SizeX / 2, s.Position.Y + s.SizeY / 2, s.Position.Z + s.SizeZ / 2)).ToList();
            var minCoordinates = new Vector3(minVectors.Min(s => s.X), minVectors.Min(s => s.Y), minVectors.Min(s => s.Z));
            var maxCoordinates = new Vector3(maxVectors.Max(s => s.X), maxVectors.Max(s => s.Y), maxVectors.Max(s => s.Z));
            var position = maxCoordinates + minCoordinates;
            return new RectangularCuboid(new Vector3(position.X / 2, position.Y / 2, position.Z / 2),
                maxCoordinates.X - minCoordinates.X,
                maxCoordinates.Y - minCoordinates.Y,
                maxCoordinates.Z - minCoordinates.Z);
        }
    }
}