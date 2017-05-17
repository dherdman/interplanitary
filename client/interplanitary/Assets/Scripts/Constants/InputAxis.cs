
using UnityEngine;

public static class InputAxis
{
    public static class PlayerControl
    {
        public const string HORIZONTAL = "PlayerHorizontal";
        public const string JUMP = "PlayerJump";
        public const string INTERACT = "PlayerInteract";
    }
    public static class ShipControl
    {
        public const string HORIZONTAL = "ShipHorizontal";
        public const string VERTICAL = "ShipVertical";
    }
    public static class Debug
    {
        public const KeyCode PAUSE = KeyCode.LeftBracket;
        public const KeyCode STEP = KeyCode.RightBracket;
    }
}