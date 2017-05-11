
using UnityEngine;

public static class Layers
{
    public const string Ships = "Ships";
    public const string Characters = "Characters";
    public const string Worlds = "Worlds";

    public static class ID
    {
        public static int Ships
        {
            get
            {
                return LayerMask.NameToLayer(Layers.Ships);
            }
        }
        public static int Characters
        {
            get
            {
                return LayerMask.NameToLayer(Layers.Characters);
            }
        }
        public static int Worlds
        {
            get
            {
                return LayerMask.NameToLayer(Layers.Worlds);
            }
        }
    }
}
