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

        public Vector3 GetMinPoint()
        {
            return new Vector3(
                 Position.X - SizeX / 2,
                 Position.Y - SizeY / 2,
                 Position.Z - SizeZ / 2);
        }

        public Vector3 GetMaxPoint()
        {
            return new Vector3(Position.X + SizeX / 2, 
                               Position.Y + SizeY / 2, 
                               Position.Z + SizeZ / 2);
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
            var minVector = new Vector3(double.MaxValue, double.MaxValue, double.MaxValue);
            var maxVector = new Vector3(double.MinValue, double.MinValue, double.MinValue);
            var minCoordinatesVector = new Vector3(double.MaxValue, double.MaxValue, double.MaxValue);
            var maxCoordinatesVector = new Vector3(double.MinValue, double.MinValue, double.MinValue);
            foreach (var part in Parts)
            {
                var box = part.GetBoundingBox();
                var minOfBox = box.GetMinPoint();
                var maxOfBox = box.GetMaxPoint();
                minVector = minOfBox <= minVector?minOfBox:minVector ;
                maxVector = maxOfBox >= maxVector?maxOfBox:maxVector;
                minCoordinatesVector = minCoordinatesVector.GetSmallestCoordinatesFrom(minOfBox);
                maxCoordinatesVector = maxCoordinatesVector.GetBiggestCoordinatesFrom(maxOfBox);
            }
            return new RectangularCuboid(minVector.GetMiddleWith(maxVector)
                , maxCoordinatesVector.X - minCoordinatesVector.X
                , maxCoordinatesVector.Y - minCoordinatesVector.Y
                , maxCoordinatesVector.Z - minCoordinatesVector.Z); ;
        }
   
    }
    static public class Vector3Extension
    {
        public static Vector3 GetMiddleWith(this Vector3 vector1, Vector3 vector2)
        {
            var transfer = vector1 + vector2;
            return  new Vector3(transfer.X / 2, transfer.Y / 2, transfer.Z / 2);
        }

        public static Vector3 GetSmallestCoordinatesFrom(this Vector3 vector1,Vector3 vector2)
        {
            return new Vector3(Math.Min(vector1.X,vector2.X), Math.Min(vector1.Y,vector2.Y), Math.Min(vector1.Z,vector2.Z));
        }

        public static Vector3 GetBiggestCoordinatesFrom(this Vector3 vector1, Vector3 vector2)
        {
            return new Vector3(Math.Max(vector1.X, vector2.X), Math.Max(vector1.Y, vector2.Y), Math.Max(vector1.Z, vector2.Z));
        }
    }
}