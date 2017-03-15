namespace vikebot
{
    /// <summary>
    /// Represents a single block entity in the game map.
    /// </summary>
    public enum BlockType
    {
        /// <summary>
        /// A block which isn't accessible by the player. You can't run onto these
        /// blocks.
        /// </summary>
        EndOfMap = 0,

        /// <summary>
        /// The player itself
        /// </summary>
        Self = 1,

        /// <summary>
        /// Points out that there is another player on this block. You can't run onto
        /// this block, but you attack it.
        /// </summary>
        Opponent = 2,

        /// <summary>
        /// Default
        /// </summary>
        Dirt = 3,

        /// <summary>
        /// Use gras to hide yourself.
        /// </summary>
        Gras = 4,

        /// <summary>
        /// Don't run into water. You will drowning.
        /// </summary>
        Water = 5
    }
}
