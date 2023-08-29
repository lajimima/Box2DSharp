using System.Collections.Generic;

namespace ET
{
    public enum EMapRegionType
    {
        Empty,

        BombShooter1 = 1,
        BombShooter2 = 2,
        BombShooter3 = 3,
        BombShooter4 = 4,
        BombShooter5 = 5,

        CastleWall = 6,
    }

    public enum EBattleCamp
    {
        Left = 1,
        Right = 2,
    }

    public class MapRegion
    {
        public EMapRegionType MapRegionType;
        public int ID;
        public string Name;
        public int ChannelColor;

        public int CellMinX = int.MaxValue;   // 含
        public int CellMinY = int.MaxValue;   // 含
        public int CellMaxX = int.MinValue;   // 含
        public int CellMaxY = int.MinValue;   // 含
        public int CellCountX;      // CellMaxX - CellMinX + 1
        public int CellCountY;      // CellMaxY - CellMinY + 1

        // 0: [CellMinX, CellMinY]
        // 1: [CellMinX + 1, CellMinY]
        public byte[] Datas;
    }

    public class CampMapRegion
    {
        public EBattleCamp Camp;

        public int CellMinX = int.MaxValue;   // 含
        public int CellMinY = int.MaxValue;   // 含
        public int CellMaxX = int.MinValue;   // 含
        public int CellMaxY = int.MinValue;   // 含
        public int CellCountX;      // CellMaxX - CellMinX + 1
        public int CellCountY;      // CellMaxY - CellMinY + 1

        public List<MapRegion> MapRegions = new List<MapRegion>(32);

        // 0: [CellMinX, CellMinY]
        // 1: [CellMinX + 1, CellMinY]
        public byte[] Datas;
    }

    public class BattleMapConfig
    {
        /// <summary>
        /// 地面（含）
        /// </summary>
        public int GroundCellY;

        public CampMapRegion LeftCampMapRegion = new CampMapRegion { Camp = EBattleCamp.Left };
        public CampMapRegion RightCampMapRegion = new CampMapRegion { Camp = EBattleCamp.Right };
    }
}
