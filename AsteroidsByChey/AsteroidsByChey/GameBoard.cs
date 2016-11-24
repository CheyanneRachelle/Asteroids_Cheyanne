//////////////////////////////////////////////////////////////////////////
//Class GameBoard: Holds all current game data
//Author: Cheyanne Peters
//Last Updated: Nov 13, 2016
//Notes: requires Constants, Sprites and Player classes
//////////////////////////////////////////////////////////////////////////
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace AsteroidsByChey
{
    internal class GameBoard
    {
        #region Members
        //////////////////////////////////////////////////////////////////////////
        //Base class variables
        //  _player - manages all player related data
        //  _ost - manages all obstacles
        //  _bgnd - background to draw the game on
        //  _asts - manages all asteroids
        //  _currGameSize - holds current game bounds
        //////////////////////////////////////////////////////////////////////////
        Player _player;
        List<Obstacle> _obst;
        Background _bgnd;
        List<Asteroid> _asts;
        List<AsteroidDeath> _dths;
        Rectangle _currGameSiz;
        #endregion

        #region Constructor
        //////////////////////////////////////////////////////////////////////////
        //Constructor: Sets initial game size, connects to provided player, 
        //      creates a background, and adds asteroids
        //Notes: 
        //////////////////////////////////////////////////////////////////////////
        internal GameBoard(Rectangle inRect, Player inPlayer)
        {
            _currGameSiz = inRect;
            _player = inPlayer;
            _bgnd = new Background(_currGameSiz);
            _asts = new List<Asteroid>();
            _dths = new List<AsteroidDeath>();
            _asts.Add(new Asteroid(new PointF(50, 50), new PointF(_player.PlayerLocation.X, _player.PlayerLocation.Y)));
        }
        #endregion

        #region Support Methods
        //////////////////////////////////////////////////////////////////////////
        //Function Render: Draws all game objects
        //Notes: 
        //////////////////////////////////////////////////////////////////////////
        internal void Render(Graphics inGR)
        {
            //double buffering
            using (BufferedGraphicsContext bgc = new BufferedGraphicsContext())
            {
                using (BufferedGraphics bg = bgc.Allocate(inGR, _currGameSiz))
                {
                    //smooth all edges
                    bg.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                    //background first
                    _bgnd.Render(bg.Graphics);

                    //asteroids
                    _asts.ForEach(o => o.Render(bg.Graphics));

                    //deaths
                    _dths.ForEach(o => o.Render(bg.Graphics));
                    //player items last
                    _player.Render(bg.Graphics);

                    //draw all added items
                    bg.Render();
                }
            }
        }

        //////////////////////////////////////////////////////////////////////////
        //Function UpdateGameArea: Gets x and y scaled values based on difference 
        //      between new and last client size and updates positions based on
        //      those values
        //Notes: 
        //////////////////////////////////////////////////////////////////////////
        internal void UpdateGameArea(Rectangle inRect)
        {
            //conversion necessary since width and height are ints
            float xScale = (float)inRect.Width / _currGameSiz.Width;
            float yScale = (float)inRect.Height / _currGameSiz.Height;

            //update current size
            _currGameSiz = inRect;
            _player.GameSize = inRect;

            //move background and player items
            _bgnd.Expand(xScale, yScale);
            _player.Expand(xScale, yScale);
            _asts.ForEach(o => o.Scale(xScale, yScale));
        }

        //////////////////////////////////////////////////////////////////////////
        //Function Tick: Moves player items, then asteroids, then removes dead 
        //      asteroids
        //Notes: 
        //////////////////////////////////////////////////////////////////////////
        internal void Tick(Graphics inGR)
        {
            _player.Tick(inGR, _asts);
            _asts.ForEach(o => o.Move(_currGameSiz));
            _dths.ForEach(o => o.Move(_currGameSiz));

            _asts.ForEach(o =>
            {
                if(o.IsDead)
                {
                    _dths.Add(new AsteroidDeath(o.Location, o.BaseColor));
                }
            });
            _asts.RemoveAll(o => o.IsDead);
            _dths.RemoveAll(o => o.IsDead);
        }
        #endregion
    }
}
