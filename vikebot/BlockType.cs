namespace vikebot
{
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
        /// 
        /// </summary>
        Dirt = 3,

        Gras = 4,

        Water = 5
    }
}
