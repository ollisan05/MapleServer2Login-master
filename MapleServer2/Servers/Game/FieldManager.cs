using System;
using System.Collections.Generic;
using MapleServer2.Packets;
using MapleServer2.Types;

namespace MapleServer2.Servers.Game {
    public class FieldManager {
        public readonly FieldState State;
        private readonly HashSet<GameSession> sessions;

        public FieldManager() {
            State = new FieldState();
            sessions = new HashSet<GameSession>();
        }

        public void AddPlayer(GameSession sender, PlayerData player) {
            sessions.Add(sender); // TODO: Send the initialization state of the field

            State.AddPlayer(player);
            // Add player
            Broadcast(session => {
                session.Send(FieldPacket.AddUser(session.Player.ObjectId, player));
            });
        }

        public void RemovePlayer(GameSession sender, int objectId) {
            sessions.Remove(sender);
            State.RemovePlayer(objectId);
            // Remove player
            Broadcast(session => {
                //session.Send(FieldPacket.RemoveUser(session.Player.ObjectId, objectId));
            });
        }

        public void SendMovement(GameSession sender) {
            // Send movement
        }

        public void SendChat(PlayerData player, string message) {
            Broadcast(session => {
                session.Send(ChatPacket.All(player, message));
            });
        }

        public void AddItem(GameSession sender, Item item) {
            FieldObject<Item> fieldItem = State.AddItem(item);

            Broadcast(session => {
                session.Send(FieldPacket.AddItem(fieldItem, session.Player.ObjectId));
            });
        }

        public bool RemoveItem(int objectId, out Item item) {
            if (!State.RemoveItem(objectId, out item)) {
                return false;
            }

            Broadcast(session => {
                session.Send(FieldPacket.PickupItem(objectId, session.Player.ObjectId));
                session.Send(FieldPacket.RemoveItem(objectId));
            });
            return true;
        }

        private void Broadcast(Action<GameSession> action) {
            lock (sessions) {
                foreach (GameSession session in sessions) {
                    action?.Invoke(session);
                }
            }
        }
    }
}