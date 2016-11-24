//////////////////////////////////////////////////////////////////////////
//Game: Game events and initializations
//Author: Cheyanne Peters
//Last Updated: Nov 12, 2016
//Notes: 
//////////////////////////////////////////////////////////////////////////
using System;
using System.Drawing;
using System.Windows.Forms;
using InputAbstraction;

namespace AsteroidsByChey
{
    public partial class Game : Form
    {
        #region Members
        //manages the player's game and all related data
        GameBoard currentGame;
        Player currentPlayer;
        OnePlayer playerMovement;
        #endregion

        #region Init
        public Game()
        {
            InitializeComponent();

            //start a game
            //MAKE THIS RESPOND TO THE USER SAYING THEY WANT TO START A GAME
            
            playerMovement = new OnePlayer();
            currentPlayer = new Player(ClientRectangle, playerMovement);
            currentGame = new GameBoard(ClientRectangle, currentPlayer);
        }
        #endregion

        #region Events
        //////////////////////////////////////////////////////////////////////////
        //Tick Event: Moves and Renders game on every time tick
        //Notes: 
        //////////////////////////////////////////////////////////////////////////
        private void _t_gameTick_Tick(object sender, EventArgs e)
        {
            currentGame.Tick(CreateGraphics());
            currentGame.Render(CreateGraphics());
        }

        //////////////////////////////////////////////////////////////////////////
        //Size Changed Event: Updates scales when window is resized
        //Notes: 
        //////////////////////////////////////////////////////////////////////////
        private void Game_SizeChanged(object sender, EventArgs e)
        {
            //only update if there is a game or if the screen is not minimized
            if (currentGame != null && (Size != new Size(160, 39)))
                currentGame.UpdateGameArea(ClientRectangle);
        }

        //////////////////////////////////////////////////////////////////////////
        //Key Down Event: initiates movement and weapon switching
        //Notes: 
        //////////////////////////////////////////////////////////////////////////
        private void Game_KeyDown(object sender, KeyEventArgs e)
        {
            playerMovement.KeyboardInput(e, true);
        }

        //////////////////////////////////////////////////////////////////////////
        //Key Up Event: stops movement and initiates weapon shooting
        //Notes: 
        //////////////////////////////////////////////////////////////////////////
        private void Game_KeyUp(object sender, KeyEventArgs e)
        {
            playerMovement.KeyboardInput(e, false);
        }
        #endregion
    }
}
