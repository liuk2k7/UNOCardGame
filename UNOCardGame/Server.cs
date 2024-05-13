﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using UNOCardGame.Packets;

namespace UNOCardGame
{
    class Server
    {
        /// <summary>
        /// Timeout della connessione.
        /// </summary>
        private const int _TimeOutMillis = 50 * 1000;

        /// <summary>
        /// Il server continua ad aspettare connessioni finché questa flag non viene messa a false.
        /// </summary>
        private bool runFlag = true;

        //private bool hasStarted = false;

        //private bool turnId;

        /// <summary>
        /// Indirizzo IP su cui ascolta il server.
        /// </summary>
        private readonly IPAddress _Address;

        /// <summary>
        /// Porta su cui ascolta il server.
        /// </summary>
        private readonly ushort _Port;

        /// <summary>
        /// Tiene conto del numero degli id.
        /// Il numero degli ID deve essere ordinato per mantenere l'ordine dei turni.
        /// </summary>
        private uint _IdCount = 0;

        /// <summary>
        /// Handler del thread che gestisce le nuove connessioni.
        /// </summary>
        private Task _ListenerHandler;

        /// <summary>
        /// Handler del thread che gestisce il broadcasting.
        /// </summary>
        private Task _BroadcasterHandler;

        /// <summary>
        /// Handler del thread che gestisce il gioco.
        /// </summary>
        private Task _GameMasterHandler;

        /// <summary>
        /// Tutti i player del gioco, a parte l'host.
        /// Questo hashmap contiene tutti i dati necessari per comunicare con i client.
        /// </summary>
        private Dictionary<uint, PlayerData> _Players = new Dictionary<uint, PlayerData>();

        /// <summary>
        /// Mutex che coordina l'accesso a _Players
        /// </summary>
        private static Mutex _PlayersMutex = new Mutex();

        /// <summary>
        /// I dati di ogni player.
        /// Contiene il codice di accesso, il server per comunicare con il client
        /// e le informazioni del player.
        /// </summary>
        private class PlayerData
        {
            /// <summary>
            /// Constructor di PlayerData. Genera automaticamente l'access code.
            /// </summary>
            /// <param name="id"></param>
            /// <param name="client"></param>
            /// <param name="clientHandler"></param>
            /// <param name="player"></param>
            public PlayerData(uint id, Socket client, Task clientHandler, Player player)
            {
                var random = new Random();
                byte[] buffer = new byte[sizeof(ulong)];
                random.NextBytes(buffer);
                AccessCode = BitConverter.ToUInt64(buffer, 0);
                Deck = Card.GenerateDeck(7);
                Client = client;
                ClientHandler = clientHandler;
                Player = new Player(id, player.Name, player.Personalizations);
                IsOnline = true;
            }

            /// <summary>
            /// Il codice di accesso è necessario per evitare impersonificazioni.
            /// E' necessario anche in caso di riconnessione.
            /// </summary>
            public ulong AccessCode { get; }

            /// <summary>
            /// Socket della connessione al client. 
            /// </summary>
            public Socket Client { get; set; }

            /// <summary>
            /// Thread dell'handler del client.
            /// </summary>
            public Task ClientHandler { get; }

            /// <summary>
            /// Dati del player non legati alla connessione.
            /// </summary>
            public Player Player { get; }

            /// <summary>
            /// Il deck del giocatore.
            /// </summary>
            public List<Card> Deck { get; }

            /// <summary>
            /// Segna se il giocatore è online o meno.
            /// </summary>
            public bool IsOnline { get; set; }
        }

        public Server(string address, ushort port)
        {
            _Address = IPAddress.Parse(address);
            _Port = port;
        }

        ~Server()
        {
            StopServer();
            _ListenerHandler.Dispose();
            _BroadcasterHandler.Dispose();
            // TODO: Gamemaster
            foreach (var player in _Players)
                player.Value.ClientHandler.Dispose();
            _PlayersMutex.Dispose();
        }

        public void StartServer()
        {
            // TODO: Gamemaster

            // Broadcaster
            _BroadcasterHandler = new Task(async () => await Broadcaster());
            _BroadcasterHandler.Start();

            // Listener
            _ListenerHandler = new Task(async () => await Listen());
            _ListenerHandler.Start();
        }

        public void StopServer()
        {
            const int timeOut = 5000;

            Log.Info("Stopping listener...");
            if (!_ListenerHandler.IsCompleted)
                _ListenerHandler.Wait(timeOut);

            Log.Info("Stopping broadcaster...");
            if (!_BroadcasterHandler.IsCompleted)
                _BroadcasterHandler.Wait(timeOut);

            // TODO: Gamemaster

            foreach (var player in _Players)
            {
                Log.Info($"Terminating player '{player.Value.Player.Name}'...");
                player.Value.ClientHandler.Wait(timeOut);
                try
                {
                    if (player.Value.IsOnline)
                        player.Value.Client.Close();
                }
                catch (Exception e)
                {
                    Log.Error($"Failed to close client: {e}");
                }
            }
        }

        /// <summary>
        /// Thread che manda i pacchetti ai client.
        /// A seconda della richiesta del Canale può mandarli a un utente specifico o a tutti.
        /// </summary>
        private async Task Broadcaster() { }

        /// <summary>
        /// Gestisce le richieste di ogni client. Ogni client ha un suo thread apposito.
        /// </summary>
        /// <param name="client">Il client dato come parametro durantre la creazione del nuovo thread</param>
        private async Task ClientHandler(Socket client, uint id)
        {
            while (true)
            {
                try
                {
                    short packetType = await Packet.ReceiveType(client);
                    switch (packetType)
                    {
                        case Packet.ConnectionEnd:
                            goto close;
                        case Packet.ClientEnd:
                            goto abandon;
                        default:
                            if (packetType == ChatMessage.GetPacketId())
                            {
                                var packet = Packet.Receive<ChatMessage>(client);
                                // TODO: implementare handling pacchetti 
                            }
                            else
                            {
                                await Packet.CancelReceive(client);
                                var status = new Status<object, string>("Nome del pacchetto non valido");
                            }
                            break;
                    }
                    continue;
                }
                catch (PacketException e)
                {
                    Log.Error(client, $"Packet exception happened while handling client: {e}");
                    goto close;
                }
                catch (Exception e)
                {
                    Log.Error(client, $"Exception happened while handling client: {e}");
                    goto close;
                }

            // Disconnessione possibilmente temporanea del client
            // I dati del client vengono mantenuti in memoria.
            close:
                Log.Info(client, "Disconnecting client...");
                _PlayersMutex.WaitOne();
                if (_Players.TryGetValue(id, out var player))
                {
                    player.IsOnline = false;
                    player.Client = null;
                }
                _PlayersMutex.ReleaseMutex();
                client.Close();
                return;

            // Disconnessione permanente del client.
            // I dati del giocatore vengono rimossi dalla memoria.
            abandon:
                Log.Info(client, "Removing client...");
                _PlayersMutex.WaitOne();
                _Players.Remove(id);
                _PlayersMutex.ReleaseMutex();
                client.Close();
                return;
            }
        }

        /// <summary>
        /// Thread che ascolta le richieste in entrata e le gestisce.
        /// </summary>
        public async Task Listen()
        {
            // Binding e listening all'IP e alla porta specificati
            IPEndPoint ipEndpoint = new IPEndPoint(_Address, _Port);
            Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server.Bind(ipEndpoint);
            server.Listen(1000);
            while (runFlag)
            {
                // Accetta nuove connessioni 
                var client = await server.AcceptAsync();
                client.ReceiveTimeout = _TimeOutMillis;
                client.SendTimeout = _TimeOutMillis;
                Log.Info(client, "New connection");

                try
                {
                    // Riceve il nome del pacchetto, se non è di tipo "Join" chiude la connessione
                    short packetName = await Packet.ReceiveType(client);
                    if (packetName != Join.GetPacketId())
                    {
                        Log.Warn(client, "Client sent invalid packet while joining");
                        var status = new Status<Player, string>("Pacchetto non valido, una richiesta Join deve essere mandata");
                        await Packet.Send(client, status);
                        goto close;
                    }

                    // Riceve la richiesta e crea nuovi handler per ogni nuova connessione
                    var joinRequest = await Packet.Receive<Join>(client);
                    switch (joinRequest.Type)
                    {
                        case JoinType.Join:
                            if (!hasStarted)
                            {
                                // Nuovo ID del player
                                uint newID = _IdCount;
                                _IdCount++;

                                // Handler del player
                                var clientHandler = new Task(async () => await ClientHandler(client, newID));

                                // Creazione struct con i dati del giocatore
                                var playerData = new PlayerData(newID, client, clientHandler, joinRequest.NewPlayer);

                                // Aggiunta player
                                _PlayersMutex.WaitOne();
                                _Players.Add(newID, playerData);
                                _PlayersMutex.ReleaseMutex();

                                // Manda i nuovi dati generati dal server (ID e Access Code)
                                var status = new Status<NewPlayerData, string>(new NewPlayerData(playerData.Player, playerData.AccessCode));
                                await Packet.Send(client, status);

                                // Avvia handler
                                clientHandler.Start();
                            }
                            else
                            {
                                Log.Warn(client, "New client tried to connect while playing");
                                var status = new Status<NewPlayerData, string>("Il gioco è già iniziato");
                                await Packet.Send(client, status);
                                goto close;
                            }
                            continue;
                        case JoinType.Rejoin:
                            // TODO: implementare rejoin.
                            continue;
                        default:
                            goto close;
                    }
                }
                catch (PacketException e)
                {
                    Log.Error(client, $"Packet exception occurred while handling new connection: {e}");
                    goto close;
                }
                catch (Exception e)
                {
                    Log.Error(client, $"Exception occurred while handling a new connection: {e}");
                    goto close;
                }
            close:
                Log.Info(client, "Disconnecting...");
                client.Close();
                continue;
            }
        }
    }
}