//////////////////////////////////////////////////////////////////////////
//Constants: Constants for the game
//Author: Cheyanne Peters
//Last Updated: Nov 13, 2016
//Notes: 
//////////////////////////////////////////////////////////////////////////
using System;
using System.Drawing;

namespace AsteroidsByChey
{
    internal static class Constants
    {
        internal enum Detail { first, second, third }

        //death animation speed
        internal const int DSPD = 2;

        //asteroid  color, speed, and health
        internal static Color ASTCOL = Color.SlateBlue;
        internal const int ASTSPD = 3;
        internal const int ASTHLT = 100;

        //how fast ships can move, speed up/slow down, and rotate
        internal const float SSPDLIM = 10;
        internal const float SSPDCHG = 1f;
        internal const float SROTSPD = 20;
        //ship color
        internal static Color SCOL = Color.DeepPink;

        //size limit to use for shapes 
        internal const float TILSIZE = 25;

        //number of starting lives
        internal const int NUMLIVES = 3;

        //damage and color for basic ammo
        internal const int BASDMG = 10;
        internal const float BASSPD = 10;
        internal static Color BASCOL = Color.SteelBlue;

        //number of shots for multishot ammo
        internal const int NUMSHOTS = 3;

        //size multiplier for big ammo
        internal const float SIZMULTI = 1.5f;

        //damage multiplier for damage powerup and damage limit
        internal const float DMGMULTI = 1.1f;
        internal const int DDMGLIM = 100;

        //seconds to freeze enemies for pause time powerup
        internal const int FRZTIM = 3;

        //portion of damage to prevent and how much damage the shield can take
        internal const float FRZRAT = 0.3f;
        internal const int SDMGLIM = 100;

        //number of "stars" to add to background and their size
        internal const int BGSTARS = 1200;
        internal const int BGSIZ = 2;

        //random generator for game
        internal static Random RNG = new Random();

        //radian conversion value
        internal const double RADCNV = 0.0174533;
    }
}
