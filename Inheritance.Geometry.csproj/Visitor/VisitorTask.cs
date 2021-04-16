using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using NUnit.Framework;


namespace Inheritance.Geometry.Visitor
{
    public interface IVisitor
    {
        Body Visit(Ball ball);
        Body Visit(RectangularCuboid cuboid);
        Body Visit(Cylinder cylinder);
        Body Visit(CompoundBody compoundBody);

    }
    public abstract class Body
    {
        public Vector3 Position { get; }

        protected Body(Vector3 position)
        {
            Position = position;
        }

        public abstract Body Accept(IVisitor visitor);
    }

    public class Ball : Body
    {
        public double Radius { get; }

        public Ball(Vector3 position, double radius) : base(position)
        {
            Radius = radius;
        }

        public override Body Accept(IVisitor visitor)
        {
           return visitor.Visit(this);
        }
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

        public override Body Accept(IVisitor visitor)
        {
           return visitor.Visit(this);
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

        public override Body Accept(IVisitor visitor)
        {
            return visitor.Visit(this);
        }
    }

    public class CompoundBody : Body
    {
        public IReadOnlyList<Body> Parts { get; }

        public CompoundBody(IReadOnlyList<Body> parts) : base(parts[0].Position)
        {
            Parts = parts;
        }

        public override Body Accept(IVisitor visitor)
        {
           return visitor.Visit(this);
        }
    }

    public class BoundingBoxVisitor : IVisitor
    {
        public Body Visit(Ball ball)=>new RectangularCuboid(ball.Position, 2 * ball.Radius, 2 * ball.Radius, 2 * ball.Radius);

        public Body Visit(RectangularCuboid cuboid) => cuboid;

        public Body Visit(Cylinder cylinder) => new RectangularCuboid(cylinder.Position,2 * cylinder.Radius, 2 * cylinder.Radius, cylinder.SizeZ );

        public Body Visit(CompoundBody compoundBody)
        {
            var minVector = new Vector3(double.MaxValue, double.MaxValue, double.MaxValue);
            var maxVector = new Vector3(double.MinValue, double.MinValue, double.MinValue);
            List<Body> transfer = compoundBody.Parts as List<Body>;
            for(int i =0; i != transfer.Count;i++)
                transfer[i] = transfer[i].Accept(new BoundingBoxVisitor());
            for (int i =0; i != transfer.Count;i++)
            {
                var minOfBox = (transfer[i] as RectangularCuboid).GetMinPoint();
                var maxOfBox = (transfer[i] as RectangularCuboid).GetMaxPoint();
                minVector = minVector.GetSmallestCoordinatesFrom(minOfBox);
                maxVector = minVector.GetBiggestCoordinatesFrom(maxOfBox);
            }
            var position = maxVector + minVector;
            return new RectangularCuboid(new Vector3(position.X / 2, position.Y / 2, position.Z / 2)
                , maxVector.X - minVector.X
                , maxVector.Y - minVector.Y
                , maxVector.Z - minVector.Z);
        }
    }

    public class BoxifyVisitor : IVisitor
    {
        public Body Visit(Ball ball)
        {
            return ball.Accept(new BoundingBoxVisitor());
        }

        public Body Visit(RectangularCuboid cuboid)
        {
            return cuboid.Accept(new BoundingBoxVisitor());
        }

        public Body Visit(Cylinder cylinder)
        {
            return cylinder.Accept(new BoundingBoxVisitor());
        }

        public Body Visit(CompoundBody compoundBody)
        {
            var a = compoundBody.Accept(new BoundingBoxVisitor());
            return compoundBody;
        }
    }
    static public class Vector3Extension
    {
        public static Vector3 GetMiddleWith(this Vector3 vector1, Vector3 vector2)
        {
            var transfer = vector1 + vector2;
            return new Vector3(transfer.X / 2, transfer.Y / 2, transfer.Z / 2);
        }

        public static Vector3 GetSmallestCoordinatesFrom(this Vector3 vector1, Vector3 vector2)
        {
            return new Vector3(Math.Min(vector1.X, vector2.X), Math.Min(vector1.Y, vector2.Y), Math.Min(vector1.Z, vector2.Z));
        }

        public static Vector3 GetBiggestCoordinatesFrom(this Vector3 vector1, Vector3 vector2)
        {
            return new Vector3(Math.Max(vector1.X, vector2.X), Math.Max(vector1.Y, vector2.Y), Math.Max(vector1.Z, vector2.Z));
        }
    }
}