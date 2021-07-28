namespace Engine.MoveRepresentation
{
    /// <summary>
    /// This enum handles move state and is used for checking move progress.
    /// </summary>
    public enum MoveStatus
    {
        Done,
        Illegal,
        PlayerInCheck,
    }
}