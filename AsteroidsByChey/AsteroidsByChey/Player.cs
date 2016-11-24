//////////////////////////////////////////////////////////////////////////
//Class Player: Holds and manages player related data and objects
//Author: Cheyanne Peters
//Last Updated: Nov 13, 2016
//Notes: requires Constants and Sprites classes
//////////////////////////////////////////////////////////////////////////
using System.Collections.Generic;
using System.Drawing;
using InputAbstraction;
using System;

namespace AsteroidsByChey
{
    class Player
    {
        #region Members
        //////////////////////////////////////////////////////////////////////////
        //Base class variables
        //  _lvs - number of lives a player has left
        //  _pts - number of points a player has acquired
        //  _mAmm - whether a player has multiammo equipped
        //  _bAmm - whether a player has bigammo equipped
        //  _currentGameSize - bounds of the current playing area
        //  _dmgPU - manages the damage pickup
        //  _ptPU - manages the pause time pickup
        //  _ship - manages the player's ship which represents them in game and is 
        //          worth 1 life
        //  _moves - holds movement data for the ship 
        //  _allAmmo - holds a collection of all firing ammo
        //////////////////////////////////////////////////////////////////////////
        byte _lvs = Constants.NUMLIVES;
        int _pts = 0;
        bool _mAmm = false;
        bool _bAmm = false;
        Rectangle _currentGameSize;
        DamagePU _dmgPU = new DamagePU();
        PauseTimePU _ptPU = new PauseTimePU();
        ShieldPU _shPU = new ShieldPU();
        Ship _ship;
        OnePlayer _moves;
        List<Ammo> _allAmm = new List<Ammo>();
        List<DeathAnimation> _allDeaths = new List<DeathAnimation>();

        //allow the current game size to be updated externally for new ship creations
        internal Rectangle GameSize { set { _currentGameSize = value; } }
        //retrieve ship location for asteroid goal
        internal PointF PlayerLocation { get { return _ship.Location; } }
        #endregion

        #region Constructor
        //////////////////////////////////////////////////////////////////////////
        //Constructor: sets initial game size, creates a player ship, and connects
        //      to matching player input 
        //Notes: -MAKE SURE ONEPLAYER IS ACTING AS REFERENCE TYPE
        //////////////////////////////////////////////////////////////////////////
        internal Player(Rectangle inClient, OnePlayer inMoves)
        {
            _currentGameSize = inClient;
            _ship = new Ship(new PointF(_currentGameSize.Width/2, _currentGameSize.Height/2));
            _moves = inMoves;
        }
        #endregion

        #region Support Methods
        //////////////////////////////////////////////////////////////////////////
        //Tick Method: Moves player related objects
        //Notes: 
        //////////////////////////////////////////////////////////////////////////
        internal void Tick(Graphics inGR, List<Asteroid> inAsts)
        {
            //look at each asteroid
            inAsts.ForEach(o =>
            {
                //check if the asteroid has been hit by a missile (if they're close)
                _allAmm.ForEach(p => 
                {
                    if ((Math.Sqrt(Math.Pow(o.Location.X - p.Location.X, 2) 
                                + Math.Pow(o.Location.Y - p.Location.Y, 2)) <= Constants.TILSIZE))
                    {
                        //get asteroid region
                        Region _hitCheck = new Region(o.GPath);

                        //compare it to the missile
                        if (p.CheckHit(inGR, _hitCheck))
                        {
                            //damage asteroid and kill missile if there's a hit
                            o.DamageHealth(p.Damage);
                            p.IsDead = true;
                            _allDeaths.Add(new AmmoDeath(p.Location, p.BaseColor));
                        }
                    } 
                });
            });

            //remove dead ammo
            _allAmm.RemoveAll(o => o.IsDead);
            //remove dead explosions
            _allDeaths.RemoveAll(o => o.IsDead);

            //move explosions
            _allDeaths.ForEach(o => o.Move(_currentGameSize));
            //move missiles
            FireMissiles();
            //adjust the ship's movement fields and move it
            _ship.UpdateMoveChange(_moves.Right, _moves.Left, _moves.Forward, _moves.Backward);
            _ship.Move(_currentGameSize);
        }

        //////////////////////////////////////////////////////////////////////////
        //Render Method: Draws player objects
        //Notes: 
        //////////////////////////////////////////////////////////////////////////
        internal void Render(Graphics inGR)
        {
            _allAmm.ForEach(o => o.Render(inGR));
            _ship.Render(inGR);
            _allDeaths.ForEach(o => o.Render(inGR));
        }

        //////////////////////////////////////////////////////////////////////////
        //Expand Method: Updates the scale of player objects
        //Notes: 
        //////////////////////////////////////////////////////////////////////////
        internal void Expand(float inXScale, float inYScale)
        {
            _ship.Scale(inXScale, inYScale);
        }

        //////////////////////////////////////////////////////////////////////////
        //Fire Missiles Method: Fires missiles
        //Notes: 
        //////////////////////////////////////////////////////////////////////////
        internal void FireMissiles()
        {
            //add a new missile if requested
            if(_moves.Shoot == true)
            {
                _allAmm.Add(new BasicAmmo(_ship.GetTurretPos(), _ship.Rotation));
                _moves.Shoot = false;
            }
            //move all existing missiles
            _allAmm.ForEach(o => o.Move(_currentGameSize));
        }
        #endregion
    }
}
