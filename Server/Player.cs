using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lidgren.Network;

using Newtonsoft.Json;

namespace Server
{
    class Player
    {
        static int nameCount;
        string name = $"Player {++nameCount}";

        Game game;
        NetConnection connection;

        int roomKey;

        public Player(Game game, NetConnection connection)
        {
            Console.WriteLine("Player");

            Player player = this;

            this.game = game;
            this.connection = connection;
        }

        internal void createRoom()
        {
            Console.WriteLine("createRoom");
            roomKey = connection.GetHashCode();
            game.rooms.Add(roomKey, new List<NetConnection>());
            joinRoom(roomKey);
        }

        internal void disconnect()
        {
            Console.WriteLine("disconnect");
            game.connections.Remove(connection.GetHashCode());
            leaveRoom(roomKey);
        }

        internal void getAllRooms()
        {
            Console.WriteLine("getAllRooms");
            string content = JsonConvert.SerializeObject(game.rooms.Keys);
            NetOutgoingMessage outgoingMessage = game.server.CreateMessage(content);
            game.server.SendMessage(outgoingMessage, connection, NetDeliveryMethod.ReliableUnordered);
        }

        internal void getRoomInfo(int roomKey)
        {
            Console.WriteLine("getRoomInfo");

            if (game.rooms.ContainsKey(roomKey))
            {
                string content = JsonConvert.SerializeObject(game.rooms[roomKey]);
                NetOutgoingMessage outgoingMessage = game.server.CreateMessage(content);
                game.server.SendMessage(outgoingMessage, connection, NetDeliveryMethod.ReliableUnordered);
            }
        }

        internal void joinRoom(int roomKey)
        {
            Console.WriteLine("joinRoom");

            if (game.rooms.ContainsKey(roomKey))
            {
                game.rooms[roomKey].Add(connection);
            }
        }

        internal void leaveAllRooms()
        {
            foreach (KeyValuePair<int, List<NetConnection>> item in game.rooms)
            {
                leaveRoom(item.Key);
            }
        }

        internal void leaveRoom(int roomKey)
        {
            if (game.rooms.ContainsKey(roomKey))
            {
                List<NetConnection> room = game.rooms[roomKey];
                room.Remove(connection);

                if (room.Count <= 0)
                {
                    game.rooms.Remove(roomKey);
                }
            }
        }

        internal void setName(string name)
        {
            Console.WriteLine("setName");
            this.name = name;
        }

        internal void update(string data)
        {
            foreach (NetConnection recipient in game.rooms[roomKey])
            {
                NetOutgoingMessage outgoingMessage = game.server.CreateMessage();
                outgoingMessage.Write(data);
                game.server.SendMessage(outgoingMessage, recipient, NetDeliveryMethod.ReliableUnordered);
            }
        }
    }
}
