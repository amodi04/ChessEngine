namespace Chess.GUI.Models;

/// <summary>
///     Stores string representation of moves
/// </summary>
public class MoveModel
{
    public string? MoveNumber { get; set; }
    public string? WhiteMove { get; init; }
    public string? BlackMove { get; init; }
}