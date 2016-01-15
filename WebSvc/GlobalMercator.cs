using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;

namespace MoreMapsTileFetcher
{
    /// <summary>
    /// Conversion routines for Google, TMS, and Microsoft Quadtree tile representations, derived from
    /// http://www.maptiler.org/google-maps-coordinates-tile-bounds-projection/ 
    /// </summary>
    public class GlobalMercator
    {
        private const int TileSize = 256;
        private const int EarthRadius = 6378137;
        private const double InitialResolution = 2 * Math.PI * EarthRadius / TileSize;
        private const double OriginShift = 2 * Math.PI * EarthRadius / 2;


        /// <summary>
        /// A two dimensional point in space
        /// </summary>
        public struct Point
        {
            public Point(double x, double y) : this()
            {
                X = x;
                Y = y;
            }

            public double X { get; set; }
            public double Y { get; set; }
        }

        /// <summary>
        /// Reference to a Tile X, Y index
        /// </summary>
        public class Tile
        {
            public Tile() { }
            public Tile(int x, int y)
            {
                X = x;
                Y = y;
            }

            public int X { get; set; }
            public int Y { get; set; }
        }

        //Resolution (meters/pixel) for given zoom level (measured at Equator)
        public static double Resolution(int zoom)
        {
            return InitialResolution / (Math.Pow(2, zoom));
        }

        //Converts pixel coordinates in given zoom level of pyramid to EPSG:900913
        public static Point PixelsToMeters(Point p, int zoom)
        {
            // Convert to meters
            var res = Resolution(zoom);
            var meters = new Point();
            meters.X = p.X * res - OriginShift;
            meters.Y = p.Y * res;// - OriginShift;
                        
            // The abstraction of Google having origo top left and BBOX having origo center need to go here
            //meters.Y = 2 * Math.PI * EarthRadius - meters.Y;
            meters.Y = meters.Y - OriginShift;

            return meters;
        }

        //Returns bounds of the given tile in EPSG:900913 coordinates
        public static Rect TileBounds(Tile t, int zoom)
        {
            // Convert from origo top left to origo bottom left
            t.Y = (int) Math.Pow(2, zoom) - 1 - t.Y;

            var min = PixelsToMeters(new Point(t.X * TileSize, t.Y * TileSize), zoom);
            var max = PixelsToMeters(new Point((t.X + 1) * TileSize, (t.Y + 1) * TileSize), zoom);

            return new Rect(min, max);
        }

        /// <summary>
        /// A rectangle defined by top left and bottom right points. Origo bottom left.
        /// </summary>
        public struct Rect
        {
            public Rect(Point p1, Point p2) : this()
            {
                P1 = p1;
                P2 = p2;

                // The Google maps coordinate system works with upper (north) left as origo
                Left = P1.X;
                Right = P2.X;
                Top = P2.Y;
                Bottom = P1.Y;
            }

            // NB: Google tile system is top left (x,y) = (0,0)
            public double Left { get; internal set; }
            public double Right { get; internal set; }
            public double Top { get; internal set; }
            public double Bottom { get; internal set; }

            public Point P1 { get; set; }
            public Point P2 { get; set; }

        }
    }
}
