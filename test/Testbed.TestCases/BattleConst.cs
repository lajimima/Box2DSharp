namespace ET
{
    public static class BattleConst
    {
        public const int FixedWidthInPixel = 1920;
        public const int FixedHeightInPixel = 1080;
        public const int CellSize = 4;
        public const int ServerCellWidth = FixedWidthInPixel / CellSize;
        public const int ServerCellHeight = FixedHeightInPixel / CellSize;
    }

    public enum EBattleType
    {
        None,
        Local,
        Real,
    }

    public enum EMatchStage
    {
        None,

        Matching,

        SetupBattle,

        InBattle,
    }

    public enum EMatchType
    {
        None = 0,

        /// <summary>
        /// 本地对打
        /// </summary>
        Local = 1,

        /// <summary>
        /// 随机匹配真人
        /// </summary>
        RandomReal = 2,

        /// <summary>
        /// 创建key等待真人加入对打
        /// </summary>
        CreateKeyReal = 3,

        /// <summary>
        /// 使用key加入真人对打
        /// </summary>
        UseKeyReal = 4,
    }
}
