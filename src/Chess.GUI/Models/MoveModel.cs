namespace Chess.GUI.Models
{
    /// <summary>
    /// Stores string representation of moves
    /// </summary>
    public class MoveModel
    {
        // Properties (nullable)
        public string? MoveNumber { get; set; }
        public string? WhiteMove { get; set; }
        public string? BlackMove { get; set; }
    }
}