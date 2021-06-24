using System;
using System.Collections.Generic;

namespace Generics.Robots
{
    public interface IRobotAI<out T>
    {
        T GetCommand();
    }

    public interface IDevice<in T>
    {
        string ExecuteCommand(T com);
    }

    public class ShooterAI : IRobotAI<ShooterCommand>
    {
        int counter = 1;
        public ShooterCommand GetCommand() => ShooterCommand.ForCounter(counter++);
    }

    public class BuilderAI : IRobotAI<BuilderCommand>
    {
        int counter = 1;
        public BuilderCommand GetCommand() => BuilderCommand.ForCounter(counter++);
    }

    public class Mover : IDevice<IMoveCommand>
    {
        public string ExecuteCommand(IMoveCommand move_command)
        => $"MOV {move_command.Destination.X}, " +
                  $"{move_command.Destination.Y}";
    }

    public class ShooterMover : IDevice<IShooterMoveCommand>
    {
        public string ExecuteCommand(IShooterMoveCommand move_command)
        => $"MOV {move_command.Destination.X}," +
            $" {move_command.Destination.Y}, USE COVER " +
            $"{(move_command.ShouldHide ? "YES" : "NO")}";
    }

    public  class Robot<TCommand>
    {
        IRobotAI<TCommand> ai;
        IDevice<TCommand> device;

        public Robot(IRobotAI<TCommand> ai, IDevice<TCommand> executor)
        {
            this.ai = ai;
            this.device = executor;
        }

        public IEnumerable<string> Start(int steps)
        {
            for (int i = 0; i < steps; i++)
            {
                var command = ai.GetCommand();
                if (command == null)
                    break;
                yield return device.ExecuteCommand(command);
            }
        }
    }

    public static class Robot
    {
        public static Robot<TCom> Create<TCom>(IRobotAI<TCom> ai, IDevice<TCom> device) => new Robot<TCom>(ai, device);
    }
}
