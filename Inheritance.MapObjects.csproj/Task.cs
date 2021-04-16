using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inheritance.MapObjects
{
    public interface IFightable
    {
        Army Army { get; set; }
    }

    public interface ICollectable
    {
        Treasure Treasure { get; set; }
    }

    public interface IOwnerble
    {
        int Owner { get; set; }
    }

    public class Dwelling : IOwnerble
    {
        public int Owner { get; set; }
    }

    public class Mine : IOwnerble, IFightable, ICollectable
    {
        public int Owner { get; set; }
        public Army Army { get; set; }
        public Treasure Treasure { get; set; }
    }

    public class Creeps : IFightable, ICollectable
    {
        public Army Army { get; set; }
        public Treasure Treasure { get; set; }
    }

    public class Wolves : IFightable
    {
        public Army Army { get; set; }
    }

    public class ResourcePile : ICollectable
    {
        public Treasure Treasure { get; set; }
    }

    public static class Interaction
    {
        public static void Make(Player player, object mapObject)
        {
            if (mapObject is IFightable armyObj
            && !player.CanBeat(armyObj.Army))
            {
                player.Die();
                return;
            }
            if (mapObject is IOwnerble ownObj)
                ownObj.Owner = player.Id;

            if (mapObject is ICollectable ollectableObj)
                player.Consume(ollectableObj.Treasure);
        }
    }
}
