using System.Numerics;
using GameServerCore.Enums;
using GameServerLib.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.StatsNS;

namespace LeagueSandbox.GameServer.GameObjects.AttackableUnits.Buildings.AnimatedBuildings
{
    public class BarrackDampener : ObjAnimatedBuilding
    {
        public DampenerState DampenerState { get; private set; }
        public Lane Lane { get; private set; }
        public float RespawnTime { get; set; }
        public float VisualStateChangeTime { get; set; }
        WeakReference<Region> VisionRegion;
        public bool RespawnAnimationAnnounced { get; set; }
        //WeakReference<ReplicationManagerI> mReplicationManager;
        //DampenerEventManager mEventManager;

        // TODO assists
        public BarrackDampener(
            Game game,
            string model,
            Lane laneId,
            TeamId team,
            int collisionRadius = 40,
            Vector2 position = new Vector2(),
            int visionRadius = 0,
            Stats stats = null,
            uint netId = 0
        ) : base(game, model, collisionRadius, position, visionRadius, netId, team, stats)
        {
            DampenerState = DampenerState.RespawningState;
            Lane = laneId;
        }

        internal BarrackDampener GetDampener(TeamId team, Lane lane) 
        {
            string searchStr = lane switch
            {
                Lane.LANE_L => "_L",
                Lane.LANE_C => "_C",
                _ => "_R",
            };


        }
    }
}
