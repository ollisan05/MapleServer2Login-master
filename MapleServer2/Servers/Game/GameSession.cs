using System.Diagnostics;
using MapleServer2.Enums;
using MapleServer2.Network;
using MapleServer2.Tools;
using MapleServer2.Types;
using Microsoft.Extensions.Logging;

namespace MapleServer2.Servers.Game {
    public class GameSession : Session {
        protected override SessionType Type => SessionType.Game;

        public readonly Inventory Inventory;

        public PlayerData Player { get; private set; }
        public FieldManager FieldManager { get; private set; }

        private readonly ManagerFactory<FieldManager> fieldManagerFactory;

        public GameSession(ManagerFactory<FieldManager> fieldManagerFactory, ILogger<GameSession> logger) : base(logger) {
            this.fieldManagerFactory = fieldManagerFactory;
            this.Inventory = new Inventory(48);
        }

        public void InitPlayer(PlayerData player) {
            Debug.Assert(this.Player == null, "Not allowed to reinitialize player.");
            this.Player = player;
        }

        public void EnterField(int newMapId) {
            FieldManager?.RemovePlayer(this, Player.ObjectId); // Leave previous field
            fieldManagerFactory.Release(Player.MapId);

            FieldManager = fieldManagerFactory.GetManager(newMapId);
            Player.MapId = newMapId;
            FieldManager.AddPlayer(this, Player); // Add player
        }
    }
}