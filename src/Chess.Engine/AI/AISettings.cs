namespace Engine.AI
{
    /// <summary>
    /// Static class holds global AI setting data.
    /// </summary>
    public static class AISettings
    {
        // Depth (used by both evaluators)
        public static int Depth = 4;
        
        // Properties used by the default evaluator
        public static int CheckmateBonus = 100000;
        public static int CheckBonus = 50;
        public static int CastleBonus = 25;
        public static int MobilityMultiplier = 1;
        public static int PieceMultiplier = 1;
        public static int AttackMultiplier = 1;
        public static int DepthMultiplier = 100;
        public static int TwoBishopsBonus = 20;
        public static int TwoRooksBonus = 50;

        // Piece values (used by the default evaluator)
        public static int PawnValue = 100;
        public static int KnightValue = 300;
        public static int BishopValue = 320;
        public static int RookValue = 500;
        public static int QueenValue = 900;

        // Used to decide whether the better evaluator will be used
        public static bool UseBetterEvaluator = false;
    }
}