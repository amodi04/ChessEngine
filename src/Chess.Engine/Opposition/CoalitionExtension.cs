namespace Engine.Opposition
{
    public static class CoalitionExtension
    {
        public static int GetDirection(this Coalition coalition)
        {
            return coalition == Coalition.WHITE ? 1 : -1;
        }

        public static int GetOppositeDirection(this Coalition coalition)
        {
            return -GetDirection(coalition);
        }
    }
}