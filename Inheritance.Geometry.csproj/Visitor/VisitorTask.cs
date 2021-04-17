using System;
using System.Collections.Generic;
using System.Linq;

namespace Inheritance.Geometry.Visitor
{
    public interface IVisitor<out T>
    {
        T Visit(Ball ball);
        T Visit(RectangularCuboid cuboid);
        T Visit(Cylinder cylinder);
        T Visit(CompoundBody body);
    }

    public abstract class Body
    {
        public Vector3 Position { get; }

        protected Body(Vector3 position)
        {
            Position = position;
        }

        public abstract Body Accept(IVisitor<Body> visitor);
    }

    public class Ball : Body
    {
        public double Radius { get; }

        public Ball(Vector3 position, double radius) : base(position)
        {
            Radius = radius;
        }

        public override Body Accept(IVisitor<Body> visitor) => visitor.Visit(this);
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

        public override Body Accept(IVisitor<Body> visitor) => visitor.Visit(this);
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

        public override Body Accept(IVisitor<Body> visitor) => visitor.Visit(this);
    }

    public class CompoundBody : Body
    {
        public IReadOnlyList<Body> Parts { get; }

        public CompoundBody(IReadOnlyList<Body> parts) : base(parts[0].Position)
        {
            Parts = parts;
        }

        public override Body Accept(IVisitor<Body> visitor) => visitor.Visit(this);
    }

    

    public class BoundingBoxVisitor :IVisitor<RectangularCuboid>
    {
        RectangularCuboid IVisitor<RectangularCuboid>.Visit(RectangularCuboid cuboid) => cuboid;

        RectangularCuboid IVisitor<RectangularCuboid>.Visit(Ball ball) =>
        new RectangularCuboid(ball.Position, 2 * ball.Radius, 2 * ball.Radius, 2 * ball.Radius);

        RectangularCuboid IVisitor<RectangularCuboid>.Visit(Cylinder cylinder) =>
        new RectangularCuboid(cylinder.Position, 2 * cylinder.Radius, 2 * cylinder.Radius, cylinder.SizeZ);

        RectangularCuboid IVisitor<RectangularCuboid>.Visit(CompoundBody body)
        {
            var parse = body.Parts.Select(s => s.Accept(this) as RectangularCuboid).ToList();
            var minVectors = parse.Select(s => new Vector3(s.Position.X - s.SizeX / 2, s.Position.Y - s.SizeY / 2, s.Position.Z - s.SizeZ / 2)).ToList();
            var maxVectors = parse.Select(s => new Vector3(s.Position.X + s.SizeX / 2, s.Position.Y + s.SizeY / 2, s.Position.Z + s.SizeZ / 2)).ToList();
            var minCoordinates = new Vector3(minVectors.Min(s => s.X), minVectors.Min(s => s.Y), minVectors.Min(s => s.Z));
            var maxCoordinates = new Vector3(maxVectors.Max(s => s.X), maxVectors.Max(s => s.Y), maxVectors.Max(s => s.Z));
            var position = maxCoordinates + minCoordinates;
            return new RectangularCuboid(new Vector3(position.X / 2, position.Y / 2, position.Z / 2),
                maxCoordinates.X - minCoordinates.X,
                maxCoordinates.Y - minCoordinates.Y,
                maxCoordinates.Z - minCoordinates.Z);
        }
    }

    public class BoxifyVisitor : IVisitor<Body>
    {
        Body IVisitor<Body>.Visit(Ball ball) => ball.Accept(new BoundingBoxVisitor());

        Body IVisitor<Body>.Visit(RectangularCuboid cuboid) => cuboid.Accept(new BoundingBoxVisitor());

        Body IVisitor<Body>.Visit(Cylinder cylinder) => cylinder.Accept(new BoundingBoxVisitor());

        Body IVisitor<Body>.Visit(CompoundBody body)
        {
            List<Body> transfer = body.Parts as List<Body>;
            for (int i = 0; i != transfer.Count; i++)
                transfer[i] = transfer[i].Accept(this);
            return body;
        }
    }
}