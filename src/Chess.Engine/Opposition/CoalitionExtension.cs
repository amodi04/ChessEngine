namespace Engine.Opposition
{
    public static class CoalitionExtension
    {
        public static int GetDirection(this Coalition coalition)
        {
            return coalition == Coalition.White ? 1 : -1;
        }

        public static bool IsWhite(this Coalition coalition)
        {
            return coalition == Coalition.White;
        }
    }
}