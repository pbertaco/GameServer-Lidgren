using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lidgren.Network;

using Newtonsoft.Json;

namespace Server
{
    class Game
    {
        string appIdentifier = "appIdentifier";
        int port = 3000;

        NetPeerConfiguration peerConfiguration;
        internal NetServer server;

        internal Dictionary<int, Player> connections;
        internal Dictionary<int, List<NetConnection>> rooms;

        public Game()
        {
            Console.WriteLine("Game");

            connections = new Dictionary<int, Player>();
            rooms = new Dictionary<int, List<NetConnection>>();

            peerConfiguration = new NetPeerConfiguration(appIdentifier)
            {
                MaximumConnections = 128,
                Port = port
            };

            server = new NetServer(peerConfiguration);
            server.Start();
        }

        internal void update()
        {
            NetIncomingMessage incomingMessage;

            while ((incomingMessage = server.ReadMessage()) != null)
            {
                switch (incomingMessage.MessageType)
                {
                    case NetIncomingMessageType.StatusChanged:
                        statusChanged(incomingMessage);
                        break;
                    case NetIncomingMessageType.Data:
                        data(incomingMessage);
                        break;
                    case NetIncomingMessageType.Receipt:
                        break;
                    case NetIncomingMessageType.DiscoveryRequest:
                        discoveryRequest(incomingMessage);
                        break;
                    case NetIncomingMessageType.DiscoveryResponse:
                        break;
                    case NetIncomingMessageType.DebugMessage:
                        break;
                    case NetIncomingMessageType.WarningMessage:
                        break;
                    case NetIncomingMessageType.ErrorMessage:
                        break;
                }

                server.Recycle(incomingMessage);
            }
        }

        void statusChanged(NetIncomingMessage incomingMessage)
        {
            NetConnectionStatus connectionStatus = (NetConnectionStatus)incomingMessage.ReadByte();

            switch (connectionStatus)
            {
                case NetConnectionStatus.None:
                    break;
                case NetConnectionStatus.InitiatedConnect:
                    break;
                case NetConnectionStatus.ReceivedInitiation:
                    break;
                case NetConnectionStatus.RespondedAwaitingApproval:
                    break;
                case NetConnectionStatus.RespondedConnect:
                    break;
                case NetConnectionStatus.Connected:
                    {
                        Player player = new Player(this, incomingMessage.SenderConnection);
                        connections.Add(incomingMessage.SenderConnection.GetHashCode(), player);
                    }
                    break;
                case NetConnectionStatus.Disconnecting:
                    break;
                case NetConnectionStatus.Disconnected:
                    {
                        Player player = connections[incomingMessage.SenderConnection.GetHashCode()];
                        player.disconnect();
                    }
                    break;
            }
        }

        void data(NetIncomingMessage incomingMessage)
        {
            Player player = connections[incomingMessage.SenderConnection.GetHashCode()];

            switch (incomingMessage.ReadString())
            {
                case "createRoom":
                    player.createRoom();
                    break;
                case "getAllRooms":
                    player.getAllRooms();
                    break;
                case "getRoomInfo":
                    player.getRoomInfo(incomingMessage.ReadInt32());
                    break;
                case "joinRoom":
                    player.joinRoom(incomingMessage.ReadInt32());
                    break;
                case "leaveAllRooms":
                    player.leaveAllRooms();
                    break;
                case "leaveRoom":
                    player.leaveRoom(incomingMessage.ReadInt32());
                    break;
                case "setName":
                    player.setName(incomingMessage.ReadString());
                    break;
                case "update":
                    player.update(incomingMessage.ReadString());
                    break;
            }
        }

        void discoveryRequest(NetIncomingMessage incomingMessage)
        {
            NetOutgoingMessage outgoingMessage = server.CreateMessage("Discovery Response");
            server.SendDiscoveryResponse(outgoingMessage, incomingMessage.SenderEndPoint);
        }
    }
}
