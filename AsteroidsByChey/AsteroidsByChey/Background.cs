//////////////////////////////////////////////////////////////////////////
//Background Class: Renders all background detail
//Author: Cheyanne Peters
//Last Updated: Nov 11, 2016
//Notes: number of stars assumes 3 colors. Change divisor in for loop if 
//      number of detail colors changes
//////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsteroidsByChey
{
    internal class Background
    {
        //detail colors and lists for the locations of the star details
        Color _dt1 = Color.Indigo;
        Color _dt2 = Color.DarkBlue;
        Color _dt3 = Color.DarkRed;
        List<PointF> _dt1Points = new List<PointF>();
        List<PointF> _dt2Points = new List<PointF>();
        List<PointF> _dt3Points = new List<PointF>();

        internal Background(Rectangle inLim)
        {
            //add the total number of star points required
            for (int addCount = 0; addCount < Constants.BGSTARS / 3; ++addCount)
            {
                _dt1Points.Add(new PointF(Constants.RNG.Next(inLim.Width), Constants.RNG.Next(inLim.Height)));
                _dt2Points.Add(new PointF(Constants.RNG.Next(inLim.Width), Constants.RNG.Next(inLim.Height)));
                _dt3Points.Add(new PointF(Constants.RNG.Next(inLim.Width), Constants.RNG.Next(inLim.Height)));
            }
        }

        //////////////////////////////////////////////////////////////////////////
        //GetPath Function: creates a path containing ellipses at each point in a
        //      list
        //Notes: 
        //////////////////////////////////////////////////////////////////////////
        protected GraphicsPath GetPath(List<PointF> inPoints)
        {
            GraphicsPath _gp = new GraphicsPath();
            inPoints.ForEach(o => _gp.AddEllipse(o.X, o.Y, Constants.BGSIZ, Constants.BGSIZ));
            return _gp;
        }

        //////////////////////////////////////////////////////////////////////////
        //Render Function: adds all "star" detail to drawing space
        //Notes: 
        //////////////////////////////////////////////////////////////////////////
        internal void Render(Graphics inGR)
        {
            inGR.FillPath(new SolidBrush(_dt1), GetPath(_dt1Points));
            inGR.FillPath(new SolidBrush(_dt2), GetPath(_dt2Points));
            inGR.FillPath(new SolidBrush(_dt3), GetPath(_dt3Points));
        }

        //////////////////////////////////////////////////////////////////////////
        //Expand Function: moves all "stars" to a position based on the resize 
        //      ratio
        //Notes: 
        //////////////////////////////////////////////////////////////////////////
        internal void Expand(float inXScale, float inYScale)
        {
            //temporary lists to hold the adjusted values
            List<PointF> new1 = new List<PointF>();
            List<PointF> new2 = new List<PointF>();
            List<PointF> new3 = new List<PointF>();

            //adjust all values and store them
            _dt1Points.ForEach(o => new1.Add(new PointF(o.X * inXScale, o.Y * inYScale)));
            _dt2Points.ForEach(o => new2.Add(new PointF(o.X * inXScale, o.Y * inYScale)));
            _dt3Points.ForEach(o => new3.Add(new PointF(o.X * inXScale, o.Y * inYScale)));

            //assign the new locations to the class list
            _dt1Points = new1.ToList();
            _dt2Points = new2.ToList();
            _dt3Points = new3.ToList();
        }
    }
}
