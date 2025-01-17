﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.Versioning;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using UNOCardGame.Packets;

namespace UNOCardGame
{
    [SupportedOSPlatform("windows")]
    public partial class MainGame : Form
    {
        /// <summary>
        /// Il client di questo gioco.
        /// </summary>
        public Client Client = null;

        /// <summary>
        /// Il server di questo gioco (solo se si sta hostando la partita)
        /// </summary>
        private readonly Server Server = null;

        /// <summary>
        /// Se la chiusura è stata forzata, specifica se successivamente è possibile riunirsi o no
        /// </summary>
        private bool? Abandon = null;

        /// <summary>
        /// Dice chi ha il turno e lo evidenzia quando vengono mostrati i giocatori
        /// </summary>
        private uint playerTurnId = Server.ADMIN_ID;

        /// <summary>
        /// Tiene conto del numero di carte di ogni player
        /// </summary>
        private Dictionary<uint, int> playerCardsNum = [];

        /// <summary>
        /// Giocatori nel gioco
        /// </summary>
        private Dictionary<uint, Player> gamePlayers = [];

        /// <summary>
        /// Font normale della chat
        /// </summary>
        private Font _ChatNormal;

        /// <summary>
        /// Font della chat in italic
        /// </summary>
        private Font _ChatItalic;

        /// <summary>
        /// Font della chat in bold
        /// </summary>
        private Font _ChatBold;

        /// <summary>
        /// Inizializza i componenti legati allo stile della UI
        /// </summary>
        private void InitStyleComponents()
        {
            _ChatNormal = chat.Font;
            _ChatItalic = new Font(chat.Font, FontStyle.Italic);
            _ChatBold = new Font(chat.Font, FontStyle.Bold);
        }

        private void InitClient(Player player, string address, ushort port, bool isDNS, long? prevAccessCode)
        {
            if (prevAccessCode is long _prevAccessCode)
                Client = new Client(player, address, port, isDNS, _prevAccessCode);
            else
                Client = new Client(player, address, port, isDNS);

            // Progress che fa visualizzare i nuovi messaggi
            var appendMsg = new Progress<MessageDisplay>();
            appendMsg.ProgressChanged += (s, message) => AppendMessage(message);
            Client.AddMsg = appendMsg;

            // Progress che fa visualizzare i nuovi player
            var playerUpdate = new Progress<Dictionary<uint, Player>>();
            playerUpdate.ProgressChanged += (s, message) => UpdatePlayers(message);
            Client.UpdatePlayers = playerUpdate;

            // Progress che chiude il gioco
            var close = new Progress<(string, bool)>();
            close.ProgressChanged += (s, message) => ForceCloseGame(message.Item1, message.Item2);
            Client.ForceClose = close;

            // Aggiorna il turno del gioco
            var turnUpdate = new Progress<TurnUpdate>();
            turnUpdate.ProgressChanged += (s, message) => TurnUpdate(message);
            Client.TurnUpdate = turnUpdate;

            // Messaggio da parte del server riguardo il gioco
            var gameMsg = new Progress<GameMessage>();
            gameMsg.ProgressChanged += (s, message) => ShowGameMessage(message);
            Client.GameMessage = gameMsg;

            // Resetta allo stato iniziale dopo una partita
            var gameEnd = new Progress<GameEnd>();
            gameEnd.ProgressChanged += (s, message) => ResetGame(message);
            Client.ResetGame = gameEnd;
        }

        /// <summary>
        /// Si riconnette a un server
        /// </summary>
        /// <param name="player"></param>
        /// <param name="address"></param>
        /// <param name="port"></param>
        /// <param name="isDNS"></param>
        /// <param name="prevAccessCode"></param>
        public MainGame(Player player, string address, ushort port, bool isDNS, long prevAccessCode)
        {
            InitializeComponent();
            InitStyleComponents();
            InitClient(player, address, port, isDNS, prevAccessCode);
        }

        /// <summary>
        /// Si connette a un server
        /// </summary>
        /// <param name="player"></param>
        /// <param name="address"></param>
        /// <param name="port"></param>
        /// <param name="isDNS"></param>
        public MainGame(Player player, string address, ushort port, bool isDNS)
        {
            InitializeComponent();
            InitStyleComponents();
            InitClient(player, address, port, isDNS, null);
        }

        /// <summary>
        /// Hosta il server
        /// </summary>
        /// <param name="player"></param>
        /// <param name="address"></param>
        /// <param name="port"></param>
        public MainGame(Player player, string address, ushort port)
        {
            InitializeComponent();
            InitStyleComponents();
            InitClient(player, address, port, false, null);
            Server = new Server(address, port);
        }

        /// <summary>
        /// Imposta i messaggi di default prima del gioco
        /// </summary>
        /// <param name="_end"></param>
        private void ResetGame(GameEnd _end)
        {
            playerCardsNum = [];
            try
            {
                tableCard.Image = (Image)Properties.Resources.ResourceManager.GetObject("None");
                colorPic.Hide();
            }
            catch (Exception)
            {
                tableCard.Text = "None";
            }
            Label infoLabel = new();
            infoLabel.AutoSize = true;
            infoLabel.Font = new Font(chat.Font.FontFamily, 14f, FontStyle.Bold);
            if (Server == null)
                infoLabel.Text = "Aspettando che il server inizi la partita...";
            else
                infoLabel.Text = "Per far iniziare la partita scrivi '.start' in chat.";
            cards.Controls.Clear();
            cards.Controls.Add(infoLabel);
            if (_end is GameEnd end)
                MessageBox.Show(end.ToString(), "Partita terminata. Classifica finale:");
            ShowPlayers();
        }

        private void Interface_Load(object sender, EventArgs e)
        {
            Enabled = false;
            long? hostAccessCode = null;
            if (Server is Server server)
            {
                try
                {
                    hostAccessCode = server.Start(Client.Player);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Impossibile far partire il server: {ex.Message}");
                    Abandon = true;
                    Close();
                    return;
                }
                ServiceMessage("Avvio del server riuscito.");
            }

            try
            {
                if (Client.Start(hostAccessCode))
                {
                    ServiceMessage($"Connessione al server ({(Client.ServerDNS ?? Client.ServerIP)}:{Client.ServerPort}) riuscita.");
                    Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Impossibile far partire il client: {ex.Message}");
                Server?.Stop();
                Abandon = false;
                Close();
            }
            ResetGame(null);
        }

        private void msgSendButton_Click(object sender, EventArgs e)
        {
            string msg = msgWriteBox.Text;
            if (msg == "")
                return;
            msgWriteBox.Clear();
            Client.Send(new ChatMessage(msg));
        }

        private void drawButton_Click(object sender, EventArgs e)
        {
            Client.Send(new ActionUpdate(null, null, ActionType.Draw));
        }

        private void bluffButton_Click(object sender, EventArgs e)
        {
            Client.Send(new ActionUpdate(null, null, ActionType.CallBluff));
        }

        private int OnCardButtonClick(Card card)
        {
            Colors? color = null;
            if (card.Type == Type.Special)
                color = ColorSelection.SelectColor();
            Client.Send(new ActionUpdate(card.Id, color, null));
            return 0;
        }

        private void TurnUpdate(TurnUpdate turn)
        {
            if (turn.IsLeftToRight is bool isLeftToRight && turn.TableCard is Card _tableCard && turn.PlayerId is uint _playerTurnId && turn.PlayersCardsNum is Dictionary<uint, int> _playerCardsNum)
            {
                if (isLeftToRight)
                    turnDirection.Text = "→";
                else
                    turnDirection.Text = "←";
                try
                {
                    tableCard.Image = (Image)Properties.Resources.ResourceManager.GetObject(_tableCard.ToString());
                    colorPic.Show();
                    colorPic.Image = (Image)Properties.Resources.ResourceManager.GetObject(_tableCard.Color.ToString());
                }
                catch (Exception)
                {
                    tableCard.Text = _tableCard.ToString();
                    tableCard.Image = null;
                    colorPic.Image = null;
                }
                playerTurnId = _playerTurnId;
                playerCardsNum = _playerCardsNum;
                ShowPlayers();
            }
            else if (turn.NewCards is List<Card> newCards)
            {
                cards.Controls.Clear();
                foreach (var card in newCards)
                    cards.Controls.Add(card.GetAsButton(OnCardButtonClick));
            }
            else Client.Log.Warn($"Il server ha mandato un TurnUpdate non valido: {turn.Serialize()}");
        }

        private void ShowGameMessage(GameMessage gameMessage)
        {
            MessageBoxIcon icon;
            if (gameMessage.Type == MessageType.Error)
                icon = MessageBoxIcon.Error;
            else
                icon = MessageBoxIcon.Information;
            Enabled = false;
            MessageBox.Show(gameMessage.ToString(), gameMessage.Type.ToString(), MessageBoxButtons.OK, icon);
            Enabled = true;
        }

        /// <summary>
        /// Messaggio di servizio messo nella chat dal client stesso.
        /// </summary>
        /// <param name="msg">Messaggio di servizio</param>
        private void ServiceMessage(string msg)
        {
            chat.SelectionStart = chat.TextLength;
            chat.SelectionLength = 0;
            chat.SelectionFont = _ChatItalic;
            chat.SelectionColor = Color.DimGray;
            chat.AppendText(msg + Environment.NewLine);
        }

        /// <summary>
        /// Aggiunge alla chat un messaggio ricevuto dal server
        /// </summary>
        /// <param name="msg">Messaggio ricevuto dal server</param>
        private void AppendMessage(MessageDisplay msg)
        {
            chat.SelectionStart = chat.TextLength;
            chat.SelectionLength = 0;
            if (msg.Name is var name && msg.NameColor is Color nameColor)
            {
                chat.SelectionColor = nameColor;
                chat.SelectionFont = _ChatBold;
                chat.AppendText(name);
                chat.SelectionColor = chat.ForeColor;
                chat.AppendText(": ");
                chat.SelectionFont = _ChatNormal;
            }
            else
            {
                chat.SelectionFont = _ChatItalic;
                chat.SelectionColor = Color.DimGray;
            }
            chat.AppendText(msg.Message + Environment.NewLine);
        }

        /// <summary>
        /// Aggiorna la lista dei player
        /// </summary>
        /// <param name="_players"></param>
        private void UpdatePlayers(Dictionary<uint, Player> _players)
        {
            gamePlayers = _players;
            ShowPlayers();
        }

        /// <summary>
        /// Mostra la vista aggiornata dei player
        /// </summary>
        private void ShowPlayers()
        {
            players.Controls.Clear();
            foreach (var player in gamePlayers)
            {
                if (playerCardsNum.TryGetValue(player.Key, out var cardsNum))
                    players.Controls.Add(player.Value.GetAsLabel(player.Value.Id == playerTurnId, cardsNum));
                else
                    players.Controls.Add(player.Value.GetAsLabel(false, 0));
            }
        }

        /// <summary>
        /// Chiude il gioco in maniera forzata, riportando errori in caso ci siano stati
        /// </summary>
        void ForceCloseGame(string errMsg, bool abandon)
        {
            Abandon = abandon;
            Enabled = false;
            if (errMsg != null)
                MessageBox.Show(errMsg);
            Close();
        }

        /// <summary>
        /// Comportamento alla chiusura
        /// </summary>
        /// <param name="e"></param>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            cards.Controls.Clear();
            Enabled = false;
            if (Abandon is bool abandon)
                Client.Close(abandon);
            else if (Server == null)
                if (MessageBox.Show("Se clicchi 'No' potrai riunirti successivamente", "Vuoi chiudere definitivamente la connessione?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    Client.Close(true);
                else Client.Close(false);
            else if (MessageBox.Show("Tutti i giocatori verranno disconnessi.", "Vuoi chiudere definitivamente il server?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                Client.Close(true);
            else return;
            if (Server is Server server)
                server.Stop();
            base.OnFormClosing(e);
        }
    }
}
