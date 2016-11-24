//////////////////////////////////////////////////////////////////////////
//Abstract Class Sprites: Framework for all renderable game objects
//Author: Cheyanne Peters
//Last Updated: Nov 13, 2016
//Notes: requires supporting Constants.cs file
//////////////////////////////////////////////////////////////////////////
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace AsteroidsByChey
{
    #region Base Classes
    internal abstract class Sprites
    {
        #region Members
        //////////////////////////////////////////////////////////////////////////
        //Base class variables
        //  _loc - holds object's position in world
        //  _gp - the graphic path to be transformed and rendered
        //  _col - the main color of the object
        //  _dead - represents whether the object has lost all health/power
        //  _spd - rate at which objects move each tick
        //  _rot - how far around a circle an object is turned in degrees
        //  _xScale - change in height since the game opened
        //  _yScale - change in width since the game opened
        //////////////////////////////////////////////////////////////////////////

        //all objects start at origin point
        protected PointF _loc = new PointF(0, 0);
        protected GraphicsPath _gp;
        protected Color _col;
        protected bool _dead;
        protected float _spd;
        protected float _rot;
        protected static float _xScale;
        protected static float _yScale;

        //external access to location
        internal PointF Location { get { return _loc; } set { _loc = value; } } 
        //access to graphicspath for hit detection
        internal GraphicsPath GPath { get { return _gp; } }
        //access to dead state
        internal bool IsDead { get { return _dead; } set { _dead = value; } }
        //access to color
        internal Color BaseColor { get { return _col; } }
        #endregion

        #region Constructor
        //////////////////////////////////////////////////////////////////////////
        //Static Constructor: Sets initial window size to the base scale
        //Notes: -must create a Sprites derived object on first game window load
        //////////////////////////////////////////////////////////////////////////
        static Sprites()
        {
            _xScale = 1;
            _yScale = 1;
        }
        #endregion

        #region Base Methods
        //////////////////////////////////////////////////////////////////////////
        //Method Scale: Adjusts scale value and sets a new position
        //Notes: -location must be updated here as the position is saved, not only
        //      transformed
        //      -any values that do not have to be saved can be transformed in 
        //      GetPath overrides
        //          *speed affected by scale in Move function
        //////////////////////////////////////////////////////////////////////////
        internal void Scale(float inScaleX, float inScaleY)
        {
            _xScale = inScaleX;
            _yScale = inScaleY;
            _loc = new PointF(_loc.X * _xScale, _loc.Y * _yScale);
        }

        //////////////////////////////////////////////////////////////////////////
        //Transform Method: Copies path detail to renderable path, resizes and 
        //      moves it
        //Notes: 
        //////////////////////////////////////////////////////////////////////////
        internal void Transform(GraphicsPath inPath)
        {
            //copy the provided graphics path
            _gp = (GraphicsPath)inPath.Clone();
            Matrix matMove = new Matrix();
            Matrix matScale = new Matrix();
            Matrix matRot = new Matrix();

            //move, rotate, and scale the path
            matMove.Translate(_loc.X, _loc.Y);
            matScale.Scale(_xScale, _yScale);
            matRot.Rotate(_rot);

            matMove.Multiply(matScale);
            matMove.Multiply(matRot);

            //apply transformations
            _gp.Transform(matMove);
        }
        #endregion

        #region Virtual Methods
        //////////////////////////////////////////////////////////////////////////
        //Virtual Method CheckHit: Determines if the object was hit
        //Notes: 
        //////////////////////////////////////////////////////////////////////////
        internal virtual bool CheckHit(Graphics inGR, Region inRegion)
        {
            UpdatePath();
            //make a region representing current object
            Region myRegion = new Region(_gp);

            //test against incoming region
            myRegion.Intersect(inRegion);

            //return whether or not an intersection was found
            if (myRegion.IsEmpty(inGR))
                return false;
            else
                return true;
        }

        //////////////////////////////////////////////////////////////////////////
        //Virtual Method Render: Adds the object to a drawing area
        //Notes: -add a member if unique brushes are needed
        //////////////////////////////////////////////////////////////////////////
        internal virtual void Render(Graphics inGR)
        {
            inGR.FillPath(new SolidBrush(_col), _gp);
        }

        //////////////////////////////////////////////////////////////////////////
        //Virtual Method Die: Kills the object
        //Notes: -use base to set the bool, child classes should add their own 
        //      additional functionality
        //////////////////////////////////////////////////////////////////////////
        protected virtual void Die()
        {
            _dead = true;
        }

        //////////////////////////////////////////////////////////////////////////
        //Virtual Method Move: Updates object location based on current direction
        //Notes: -inverted to account for y axis
        //////////////////////////////////////////////////////////////////////////
        internal virtual void Move(Rectangle inBounds)
        {
            _loc = new PointF(_loc.X + _spd * (float)Math.Sin(_rot * Constants.RADCNV) * _xScale,
                            _loc.Y - _spd * (float)Math.Cos(_rot * Constants.RADCNV) * _yScale);
            UpdatePath();
        }
        #endregion

        #region Abstract Methods
        //////////////////////////////////////////////////////////////////////////
        //Abstract Method GetPath: Requires child classes to return a GraphicsPath
        //Notes: 
        //////////////////////////////////////////////////////////////////////////
        protected abstract void UpdatePath();
        #endregion
    }

    //////////////////////////////////////////////////////////////////////////
    //Ammo: All fireable objects
    //Author: Cheyanne Peters
    //Last Updated: Nov 13, 2016
    //Notes: 
    //////////////////////////////////////////////////////////////////////////
    internal abstract class Ammo : Sprites
    {
        #region Members
        //////////////////////////////////////////////////////////////////////////
        //Base Variables
        //  -dmg - represents amount of damage ammo can cause
        //  -aGP - shape common to all ammo
        //////////////////////////////////////////////////////////////////////////
        protected int _dmg;
        protected static GraphicsPath _aGP = new GraphicsPath();

        //access to damage stat
        internal int Damage { get { return _dmg; } }
        #endregion

        #region Constructors
        //////////////////////////////////////////////////////////////////////////
        //Static Constructor: Shape for all ammo
        //Notes: - maintain 2:1 ratio if changing size ratio
        //////////////////////////////////////////////////////////////////////////
        static Ammo()
        {
            _aGP.AddEllipse(-Constants.TILSIZE / 16, -Constants.TILSIZE / 8, 
                Constants.TILSIZE / 8, Constants.TILSIZE / 4);
        }

        //////////////////////////////////////////////////////////////////////////
        //Constructor: Sets initial position and rotation and marks the ammo as 
        //      dead
        //Notes: -make infinite ammo not dead in their constructors
        //      -rotation should never be changed after initialization
        //////////////////////////////////////////////////////////////////////////
        public Ammo(PointF inPoint, float inRot)
        {
            _loc = inPoint;
            _rot = inRot;
            _dead = true;
            UpdatePath();
        }
        #endregion

        #region Overrides
        //////////////////////////////////////////////////////////////////////////
        //UpdatePath Override: passes the ammo path to the transform function
        //Notes: 
        //////////////////////////////////////////////////////////////////////////
        protected override void UpdatePath()
        {
            Transform(_aGP);
        }

        //////////////////////////////////////////////////////////////////////////
        //Move Override: also kills ammo when out of bounds
        //Notes: 
        //////////////////////////////////////////////////////////////////////////
        internal override void Move(Rectangle inBounds)
        {
            base.Move(inBounds);
            if (_loc.X < 0 || _loc.X > inBounds.Width || _loc.Y < 0 || _loc.Y > inBounds.Height)
                Die();
        }
        #endregion
    }

    //////////////////////////////////////////////////////////////////////////
    //PowerUp: All stat influencing objects
    //Author: Cheyanne Peters
    //Last Updated: Nov 11, 2016
    //Notes: 
    //////////////////////////////////////////////////////////////////////////
    internal abstract class PowerUp : Sprites
    {
        protected int _pwr;

        internal PowerUp()
        {
            _dead = false;
        }
    }

    //////////////////////////////////////////////////////////////////////////
    //Entity: All "living" objects (can take damage)
    //Author: Cheyanne Peters
    //Last Updated: Nov 13, 2016
    //Notes: 
    //////////////////////////////////////////////////////////////////////////
    internal abstract class Entity : Sprites
    {
        #region Members
        //////////////////////////////////////////////////////////////////////////
        //Base Variables
        //  -_hlt - amount of health an entity has
        //////////////////////////////////////////////////////////////////////////
        protected int _hlt;
        #endregion

        #region Methods
        //////////////////////////////////////////////////////////////////////////
        //DamageHealth Function: reduces health by provided amount and kills 
        //      entity if health too low
        //Notes: 
        //////////////////////////////////////////////////////////////////////////
        internal void DamageHealth(int inDMG)
        {
            _hlt -= inDMG;
            if (_hlt <= 0)
                Die();
        }
        #endregion
    }

    //////////////////////////////////////////////////////////////////////////
    //DeathAnimation: All "living" objects (can take damage)
    //Author: Cheyanne Peters
    //Last Updated: Nov 13, 2016
    //Notes: 
    //////////////////////////////////////////////////////////////////////////
    internal abstract class DeathAnimation : Sprites
    {
        #region Members
        //////////////////////////////////////////////////////////////////////////
        //Base Variables
        //  -_alpha - influence death animation color and also represents health
        //////////////////////////////////////////////////////////////////////////
        protected byte _alpha = 255;
        #endregion

        #region Constructor
        //////////////////////////////////////////////////////////////////////////
        //Constructor: sets point to provided point, speed common to all death 
        //      animations
        //Notes: -base color and location same as whatever died
        //////////////////////////////////////////////////////////////////////////
        internal DeathAnimation(PointF inPoint, Color inCol)
        {
            _loc = inPoint;
            _spd = Constants.DSPD;
            _dead = false;
            _col = inCol;
            UpdatePath();
        }
        #endregion

        #region Overrides 
        //////////////////////////////////////////////////////////////////////////
        //Move Override: expands size instead of moving location
        //Notes: 
        //////////////////////////////////////////////////////////////////////////
        internal override void Move(Rectangle inBounds)
        {
            _spd *= 2;
            _alpha -= (byte)_spd;
            if (_alpha <= _spd)
                Die();
            else
                _col = Color.FromArgb(_alpha, _col);
        }
        #endregion
    }
    #endregion

    #region Ammo Derivations
    //////////////////////////////////////////////////////////////////////////
    //BasicAmmo: Fires at average speed with average damage
    //Author: Cheyanne Peters
    //Last Updated: Nov 12, 2016
    //Notes: 
    //////////////////////////////////////////////////////////////////////////
    internal class BasicAmmo : Ammo
    {
        #region Constructor
        //////////////////////////////////////////////////////////////////////////
        //Constructor: set ammo to alive (infinite), damage and color to constants
        //Notes: 
        //////////////////////////////////////////////////////////////////////////
        internal BasicAmmo(PointF inPoint, float inRot) : base(inPoint, inRot)
        {
            _dead = false;
            _dmg = Constants.BASDMG;
            _col = Constants.BASCOL;
            _spd = Constants.BASSPD;
        }
        #endregion
    }

    //////////////////////////////////////////////////////////////////////////
    //MultiAmmo: Fires multiple shots at average speed and average damage
    //Author: Cheyanne Peters
    //Last Updated: Nov 12, 2016
    //Notes: 
    //////////////////////////////////////////////////////////////////////////
    internal class MultiAmmo : Ammo
    {
        internal MultiAmmo(PointF inPoint, int inRot) : base(inPoint, inRot)
        {
        }

        //USE NUMSHOTS CONSTANT
        protected override void UpdatePath()
        {
            throw new NotImplementedException();
        }
    }

    //////////////////////////////////////////////////////////////////////////
    //BigAmmo: Fires a larger shot at average speed and average damage
    //Author: Cheyanne Peters
    //Last Updated: Nov 12, 2016
    //Notes: 
    //////////////////////////////////////////////////////////////////////////
    internal class BigAmmo : Ammo
    {
        internal BigAmmo(PointF inPoint, int inRot) : base(inPoint, inRot)
        {
        }
        //USE SIZMULTI CONSTANT
        protected override void UpdatePath()
        {
            throw new NotImplementedException();
        }
    }
    #endregion

    #region PowerUp Derivations
    //////////////////////////////////////////////////////////////////////////
    //DamagePU: Multiplies damage by a certain amount
    //Author: Cheyanne Peters
    //Last Updated: Nov 11, 2016
    //Notes: 
    //////////////////////////////////////////////////////////////////////////
    internal class DamagePU : PowerUp
    {
        private float _dMult = Constants.DMGMULTI;
        internal DamagePU() : base()
        {
            _pwr = Constants.DDMGLIM;
        }
        //USE DMGMULTI CONST
        protected override void UpdatePath()
        {
            throw new NotImplementedException();
        }
    }

    //////////////////////////////////////////////////////////////////////////
    //PauseTimePU: Freezes enemies for a period of time
    //Author: Cheyanne Peters
    //Last Updated: Nov 11, 2016
    //Notes: _freezeTim is in seconds
    //////////////////////////////////////////////////////////////////////////
    internal class PauseTimePU : PowerUp
    {
        internal PauseTimePU() : base()
        {
            _pwr = Constants.FRZTIM;
        }
        //USE FRZTIM CONST
        protected override void UpdatePath()
        {
            throw new NotImplementedException();
        }
    }

    //////////////////////////////////////////////////////////////////////////
    //ShieldPU: Prevents a percentage of damage for a certain amount of damage
    //Author: Cheyanne Peters
    //Last Updated: Nov 11, 2016
    //Notes: 
    //////////////////////////////////////////////////////////////////////////
    internal class ShieldPU : PowerUp
    {
        float _bDmg = Constants.FRZRAT;

        internal ShieldPU() : base()
        {
            _pwr = Constants.SDMGLIM;
        }
        //USE FRZRAT AND DMGLIM CONSTS
        protected override void UpdatePath()
        {
            throw new NotImplementedException();
        }
    }
    #endregion

    #region Entity Derivations
    //////////////////////////////////////////////////////////////////////////
    //Tank: Represents the player in game space
    //Author: Cheyanne Peters
    //Last Updated: Nov 13, 2016
    //Notes: 
    //////////////////////////////////////////////////////////////////////////
    internal class Ship : Entity
    {
        #region Members
        //////////////////////////////////////////////////////////////////////////
        //Base Variables
        //  -_tGP - the base shape for all ships
        //////////////////////////////////////////////////////////////////////////
        static GraphicsPath _tGP;

        //access to rotation to help ammo initialization
        internal float Rotation { get { return _rot; } }
        #endregion

        #region Constructors
        //////////////////////////////////////////////////////////////////////////
        //Static Constructor: Common shape for all ships
        //Notes: 
        //////////////////////////////////////////////////////////////////////////
        static Ship()
        {
            _tGP = new GraphicsPath();
            _tGP.AddPolygon(new PointF[] { new PointF(0, -Constants.TILSIZE/2),
                                            new PointF(Constants.TILSIZE/4, -Constants.TILSIZE/4),
                                            new PointF(Constants.TILSIZE/2, Constants.TILSIZE/2),
                                            new PointF(0, Constants.TILSIZE/3),
                                            new PointF(-Constants.TILSIZE/2, Constants.TILSIZE/2),
                                            new PointF(-Constants.TILSIZE/4, -Constants.TILSIZE/4)});
        }

        //////////////////////////////////////////////////////////////////////////
        //Constructor: Unique color and spawn point for each ship
        //Notes: 
        //////////////////////////////////////////////////////////////////////////
        public Ship(PointF inSpawnPoint)
        {
            _loc = inSpawnPoint;
            _col = Constants.SCOL;
        }
        #endregion

        #region Overrides
        //////////////////////////////////////////////////////////////////////////
        //UpdatePath Override: Passes ship shape
        //      moves it, and returns the path for use
        //Notes:
        //////////////////////////////////////////////////////////////////////////
        protected override void UpdatePath()
        {
            Transform(_tGP);
        }

        //////////////////////////////////////////////////////////////////////////
        //Render Override: Renders the ship but also adds a border
        //Notes: 
        //////////////////////////////////////////////////////////////////////////
        internal override void Render(Graphics inGR)
        {
            UpdatePath();
            base.Render(inGR);
            inGR.DrawPath(new Pen(Color.DarkSlateGray), _gp);
        }
        #endregion

        #region Support Functions
        //////////////////////////////////////////////////////////////////////////
        //Move Change Method: Updates values used to determine movement
        //Notes: 
        //////////////////////////////////////////////////////////////////////////
        internal void UpdateMoveChange(bool inR, bool inL, bool inF, bool inB)
        {
            //adjust rotation right or left
            if(inR)
            {
                //Avoid wrapping around
                if (_rot == 360)
                    _rot = Constants.SROTSPD;
                //avoid jumping over the wraparound point
                else if (_rot > 360 - Constants.SROTSPD)
                    _rot = Constants.SROTSPD - (360 - _rot);
                //simply update if not at wraparound point
                else
                    _rot += Constants.SROTSPD;
            }
            else if (inL)
            {
                // Avoid wrapping around
                if (_rot == 0)
                    _rot = 360 - Constants.SROTSPD;
                //avoid jumping over the wraparound point
                else if (_rot < Constants.SROTSPD)
                    _rot = 360 - (Constants.SROTSPD - _rot);
                //simply update if not at wraparound point
                else
                    _rot -= Constants.SROTSPD;
            }

            //adjust speed forward or backward
            if(inF)
            {
                //avoid exceeding speed limit
                if (_spd > Constants.SSPDLIM - Constants.SSPDCHG)
                    _spd = Constants.SSPDLIM;
                else
                    _spd += Constants.SSPDCHG;
            }
            else if (inB)
            {
                //avoid exceeding speed limit
                if (_spd - Constants.SSPDCHG < -Constants.SSPDLIM)
                    _spd = -Constants.SSPDLIM;
                else
                    _spd -= Constants.SSPDCHG;
            }
            else
            {
                //move back to 0 speed if there's no forward/back
                _spd -= Math.Sign(_spd) * Constants.SSPDCHG/2;
            }
        }

        //////////////////////////////////////////////////////////////////////////
        //Get Turret Position Method: finds turret position based on tank loc and 
        //      tilesize
        //Notes: 
        //////////////////////////////////////////////////////////////////////////
        internal PointF GetTurretPos()
        {
            float x = _loc.X + Constants.TILSIZE / 2 * (float)Math.Sin(_rot * Constants.RADCNV);
            float y = _loc.Y - Constants.TILSIZE / 2 * (float)Math.Cos(_rot * Constants.RADCNV);
            return new PointF(x, y);
        }
        #endregion
    }

    //////////////////////////////////////////////////////////////////////////
    //Asteroid: "Enemies" that can be killed and can damage the player
    //Author: Cheyanne Peters
    //Last Updated: Nov 12, 2016
    //Notes: 
    //////////////////////////////////////////////////////////////////////////
    internal class Asteroid: Entity
    {
        #region Members
        //////////////////////////////////////////////////////////////////////////
        //Base Variables
        //  -_astGP - variable shape for asteroids
        //  -_sds - number of sides on the asteroid
        //////////////////////////////////////////////////////////////////////////
        GraphicsPath _astGP;
        protected int _sds = 0;

        //access to number of sides
        internal int SideCount { get { return _sds; } }
        #endregion

        #region Constructor
        //////////////////////////////////////////////////////////////////////////
        //Constructor: Inits fields with asteroid unique values
        //Notes: 
        //////////////////////////////////////////////////////////////////////////
        internal Asteroid(PointF inPoint, PointF inTarget)
        {
            //get the difference between the target and asteroid spawn point for rotation 
            float xDiff = -(inPoint.X - inTarget.X);
            float yDiff = inPoint.Y - inTarget.Y;

            _astGP = new GraphicsPath();
            _loc = inPoint;
            _col = Constants.ASTCOL;
            _spd = Constants.ASTSPD;
            _rot = (float)Math.Atan2(xDiff, yDiff) * 180 / (float)Math.PI;
            _hlt = Constants.ASTHLT;

            //random points and angles for each asteroid
            _sds = Constants.RNG.Next(4, 13);
            PointF[] points = new PointF[_sds];
            float division = (float)(2 * Math.PI / (points.Length));
            for (int count = 0; count < points.Length; ++count)
            {
                float angle = division * count;
                points[count] = new PointF(Constants.RNG.Next((int)Constants.TILSIZE / 4, (int)Constants.TILSIZE / 2) * (float)Math.Cos(angle),
                                        Constants.RNG.Next((int)Constants.TILSIZE / 4, (int)Constants.TILSIZE / 2) * (float)Math.Sin(angle));
            }

            //add points to create asteroid shape
            _astGP.AddPolygon(points);

            //ensure the renderable path gets filled
            UpdatePath();
        }
        #endregion

        //////////////////////////////////////////////////////////////////////////
        //UpdatePath Override: sends asteroid path
        //Notes: 
        //////////////////////////////////////////////////////////////////////////
        protected override void UpdatePath()
        {
            Transform(_astGP);
        }
    }

    //////////////////////////////////////////////////////////////////////////
    //Obstacle: Items that can block movement and can be killed
    //Author: Cheyanne Peters
    //Last Updated: Nov 11, 2016
    //Notes: 
    //////////////////////////////////////////////////////////////////////////
    internal class Obstacle : Entity
    {
        protected override void UpdatePath()
        {
            throw new NotImplementedException();
        }
    }
    #endregion

    #region DeathAnimation Derivations
    //////////////////////////////////////////////////////////////////////////
    //AmmoDeath: Animates the death of an ammo
    //Author: Cheyanne Peters
    //Last Updated: Nov 13, 2016
    //Notes: 
    //////////////////////////////////////////////////////////////////////////
    internal class AmmoDeath : DeathAnimation
    {
        #region Members
        //////////////////////////////////////////////////////////////////////////
        //Base Variables
        //  -_amdGP - shape for ammo deaths
        //  -_amdGP2 - secondary shape to draw on top of the base shape
        //  -_gp2 - renderable path for the secondary shape
        //  -_col2 - color for the secondary shape
        //////////////////////////////////////////////////////////////////////////
        protected static GraphicsPath _amdGP;
        protected static GraphicsPath _amdGP2;
        protected GraphicsPath _gp2;
        Color _col2;
        #endregion

        #region Constructors
        //////////////////////////////////////////////////////////////////////////
        //Static Constructor: Adds ellipses to the base and secondary paths
        //Notes:
        //////////////////////////////////////////////////////////////////////////
        static AmmoDeath()
        {
            _amdGP = new GraphicsPath();
            _amdGP2 = new GraphicsPath();
            _amdGP.AddEllipse(-Constants.TILSIZE / 4, -Constants.TILSIZE / 4, 
                                Constants.TILSIZE / 2, Constants.TILSIZE / 2);
            _amdGP2.AddEllipse(-Constants.TILSIZE / 6, -Constants.TILSIZE / 6, 
                                Constants.TILSIZE / 3, Constants.TILSIZE / 3);
        }

        //////////////////////////////////////////////////////////////////////////
        //Constructor: Calls base constructor and also inits secondary color
        //Notes:
        //////////////////////////////////////////////////////////////////////////
        internal AmmoDeath(PointF inPoint, Color inCol) : base(inPoint, inCol)
        {
            //a more red version of the base color for the secondary if not already high
            if(_col.R < 200)
                _col2 = Color.FromArgb(255, _col.G, _col.B);
            //otherwise a more green version (will result in more yellow due to the red)
            else
                _col2 = Color.FromArgb(_col.R, 255, _col.B);
        }
        #endregion

        #region Overrides
        //////////////////////////////////////////////////////////////////////////
        //UpdatePath Override: Ammo death animations only scale, and have 2 paths
        //      to scale
        //Notes:
        //////////////////////////////////////////////////////////////////////////
        protected override void UpdatePath()
        {
            //copy the graphics paths
            _gp = (GraphicsPath)_amdGP.Clone();
            _gp2 = (GraphicsPath)_amdGP2.Clone();
            Matrix matTrans = new Matrix();
            Matrix matScale = new Matrix();

            //scale and move the path
            matTrans.Translate(_loc.X, _loc.Y);
            matScale.Scale(_xScale * _spd, _yScale * _spd);

            //apply transformation, same for both
            matTrans.Multiply(matScale);
            _gp.Transform(matTrans);
            _gp2.Transform(matTrans);
        }

        //////////////////////////////////////////////////////////////////////////
        //Render Override: call base render and then add the smaller path on top
        //Notes:
        //////////////////////////////////////////////////////////////////////////
        internal override void Render(Graphics inGR)
        {
            //draw larger path first, then smaller on top
            base.Render(inGR);
            inGR.FillPath(new SolidBrush(_col2), _gp2);
        }

        //////////////////////////////////////////////////////////////////////////
        //Move Override: call base move and also update secondary color
        //Notes:
        //////////////////////////////////////////////////////////////////////////
        internal override void Move(Rectangle inBounds)
        {
            base.Move(inBounds);
            _col2 = Color.FromArgb(_alpha, _col2);
        }
        #endregion
    }

    internal class AsteroidDeath : AmmoDeath
    {
        internal AsteroidDeath(PointF inPoint, Color inCol) : base(inPoint, inCol)
        {
            _spd = 500;
        }
    }


    #endregion
}
