﻿
namespace UNOCardGame
{
    partial class MainGame
    {
        /// <summary>
        /// Variabile di progettazione necessaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Pulire le risorse in uso.
        /// </summary>
        /// <param name="disposing">ha valore true se le risorse gestite devono essere eliminate, false in caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Codice generato da Progettazione Windows Form

        /// <summary>
        /// Metodo necessario per il supporto della finestra di progettazione. Non modificare
        /// il contenuto del metodo con l'editor di codice.
        /// </summary>
        private void InitializeComponent()
        {
            drawButton = new System.Windows.Forms.Button();
            colorLabel = new System.Windows.Forms.Label();
            players = new System.Windows.Forms.FlowLayoutPanel();
            playersLabel = new System.Windows.Forms.Label();
            turnDirection = new System.Windows.Forms.Label();
            cardsLabel = new System.Windows.Forms.Label();
            bluffButton = new System.Windows.Forms.Button();
            msgWriteBox = new System.Windows.Forms.TextBox();
            msgSendButton = new System.Windows.Forms.Button();
            chat = new System.Windows.Forms.RichTextBox();
            cards = new System.Windows.Forms.FlowLayoutPanel();
            tableCard = new System.Windows.Forms.Button();
            colorPic = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)colorPic).BeginInit();
            SuspendLayout();
            // 
            // drawButton
            // 
            drawButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            drawButton.Location = new System.Drawing.Point(479, 246);
            drawButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            drawButton.Name = "drawButton";
            drawButton.Size = new System.Drawing.Size(116, 66);
            drawButton.TabIndex = 2;
            drawButton.Text = "Pesca";
            drawButton.UseVisualStyleBackColor = true;
            drawButton.Click += drawButton_Click;
            // 
            // colorLabel
            // 
            colorLabel.AutoSize = true;
            colorLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F);
            colorLabel.Location = new System.Drawing.Point(774, 214);
            colorLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            colorLabel.Name = "colorLabel";
            colorLabel.Size = new System.Drawing.Size(88, 26);
            colorLabel.TabIndex = 6;
            colorLabel.Text = "Colore: ";
            // 
            // players
            // 
            players.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            players.Location = new System.Drawing.Point(14, 44);
            players.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            players.Name = "players";
            players.Size = new System.Drawing.Size(412, 382);
            players.TabIndex = 7;
            // 
            // playersLabel
            // 
            playersLabel.AutoSize = true;
            playersLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            playersLabel.Location = new System.Drawing.Point(14, 17);
            playersLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            playersLabel.Name = "playersLabel";
            playersLabel.Size = new System.Drawing.Size(84, 24);
            playersLabel.TabIndex = 8;
            playersLabel.Text = "Giocatori";
            // 
            // turnDirection
            // 
            turnDirection.AutoSize = true;
            turnDirection.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            turnDirection.Location = new System.Drawing.Point(391, 8);
            turnDirection.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            turnDirection.Name = "turnDirection";
            turnDirection.Size = new System.Drawing.Size(35, 33);
            turnDirection.TabIndex = 9;
            turnDirection.Text = "↞";
            // 
            // cardsLabel
            // 
            cardsLabel.AutoSize = true;
            cardsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            cardsLabel.Location = new System.Drawing.Point(14, 457);
            cardsLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            cardsLabel.Name = "cardsLabel";
            cardsLabel.Size = new System.Drawing.Size(71, 29);
            cardsLabel.TabIndex = 10;
            cardsLabel.Text = "Carte";
            // 
            // bluffButton
            // 
            bluffButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            bluffButton.Location = new System.Drawing.Point(479, 174);
            bluffButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            bluffButton.Name = "bluffButton";
            bluffButton.Size = new System.Drawing.Size(116, 66);
            bluffButton.TabIndex = 11;
            bluffButton.Text = "Bluff!";
            bluffButton.UseVisualStyleBackColor = true;
            bluffButton.Click += bluffButton_Click;
            // 
            // msgWriteBox
            // 
            msgWriteBox.Location = new System.Drawing.Point(900, 432);
            msgWriteBox.Name = "msgWriteBox";
            msgWriteBox.PlaceholderText = "Scrivi qui il tuo messaggio...";
            msgWriteBox.Size = new System.Drawing.Size(486, 23);
            msgWriteBox.TabIndex = 13;
            // 
            // msgSendButton
            // 
            msgSendButton.Location = new System.Drawing.Point(1392, 432);
            msgSendButton.Name = "msgSendButton";
            msgSendButton.Size = new System.Drawing.Size(76, 24);
            msgSendButton.TabIndex = 14;
            msgSendButton.Text = "Manda";
            msgSendButton.UseVisualStyleBackColor = true;
            msgSendButton.Click += msgSendButton_Click;
            // 
            // chat
            // 
            chat.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            chat.Font = new System.Drawing.Font("Lucida Console", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            chat.Location = new System.Drawing.Point(900, 12);
            chat.Name = "chat";
            chat.ReadOnly = true;
            chat.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            chat.Size = new System.Drawing.Size(568, 414);
            chat.TabIndex = 15;
            chat.Text = "";
            // 
            // cards
            // 
            cards.AutoScroll = true;
            cards.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            cards.Location = new System.Drawing.Point(14, 489);
            cards.Name = "cards";
            cards.Size = new System.Drawing.Size(1454, 345);
            cards.TabIndex = 17;
            // 
            // tableCard
            // 
            tableCard.Location = new System.Drawing.Point(602, 119);
            tableCard.Name = "tableCard";
            tableCard.Size = new System.Drawing.Size(150, 250);
            tableCard.TabIndex = 18;
            tableCard.UseVisualStyleBackColor = true;
            // 
            // colorPic
            // 
            colorPic.Location = new System.Drawing.Point(789, 243);
            colorPic.Name = "colorPic";
            colorPic.Size = new System.Drawing.Size(50, 50);
            colorPic.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            colorPic.TabIndex = 19;
            colorPic.TabStop = false;
            // 
            // MainGame
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1480, 846);
            Controls.Add(colorPic);
            Controls.Add(tableCard);
            Controls.Add(cards);
            Controls.Add(chat);
            Controls.Add(msgSendButton);
            Controls.Add(msgWriteBox);
            Controls.Add(bluffButton);
            Controls.Add(cardsLabel);
            Controls.Add(turnDirection);
            Controls.Add(playersLabel);
            Controls.Add(players);
            Controls.Add(colorLabel);
            Controls.Add(drawButton);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "MainGame";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "UNO";
            Load += Interface_Load;
            ((System.ComponentModel.ISupportInitialize)colorPic).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private System.Windows.Forms.Button drawButton;
        private System.Windows.Forms.Label colorLabel;
        private System.Windows.Forms.FlowLayoutPanel players;
        private System.Windows.Forms.Label playersLabel;
        private System.Windows.Forms.Label turnDirection;
        private System.Windows.Forms.Label cardsLabel;
        private System.Windows.Forms.Button bluffButton;
        private System.Windows.Forms.TextBox msgWriteBox;
        private System.Windows.Forms.Button msgSendButton;
        private System.Windows.Forms.RichTextBox chat;
        private System.Windows.Forms.FlowLayoutPanel cards;
        private System.Windows.Forms.Button tableCard;
        private System.Windows.Forms.PictureBox colorPic;
    }
}

