using System.Collections.Generic;
using Crimson.Collections;
using Microsoft.Xna.Framework;

namespace Crimson
{
    public static class Bezier
    {
        /// <summary>
        /// Evaluate a quadratic bezier
        /// </summary>
        public static Vector2 GetPoint(Vector2 p0, Vector2 p1, Vector2 p2, float t)
        {
            t = Mathf.Clamp01(t);
            var oneMinusT = 1f - t;
            return oneMinusT * oneMinusT * p0 +
                   2f * oneMinusT * t * p1 +
                   t * t * p2;
        }

        /// <summary>
        /// Get the first derivative for a quadratic bezier
        /// </summary>
        public static Vector2 GetFirstDerivative(Vector2 p0, Vector2 p1, Vector2 p2, float t)
        {
            return 2f * (1f - t) * (p1 - p0) +
                   2f * t * (p2 - p1);
        }

        public static Vector2 GetPoint(Vector2 start, Vector2 firstControlPoint, Vector2 secondControlPoint,
            Vector2 end, float t)
        {
            t = Mathf.Clamp01(t);
            var oneMinusT = 1f - t;
            return oneMinusT * oneMinusT * oneMinusT * start +
                   3f * oneMinusT * oneMinusT * t * firstControlPoint +
                   3f * oneMinusT * t * t * secondControlPoint +
                   t * t * t * end;
        }

        public static Vector2 GetFirstDerivative(Vector2 start, Vector2 firstControlPoint, Vector2 secondControlPoint,
            Vector2 end, float t)
        {
            t = Mathf.Clamp01(t);
            var oneMinusT = 1f - t;
            return 3f * oneMinusT * oneMinusT * (firstControlPoint - start) +
                   6f * oneMinusT * t * (secondControlPoint - firstControlPoint) +
                   3f * t * t * (end - secondControlPoint);
        }

        private static void RecursiveGetOptimizedDrawingPoints(Vector2 start, Vector2 firstCtrlPoint,
            Vector2 secondCtrlPoint, Vector2 end, List<Vector2> points, float distanceTolerance)
            
        {
            // calculate all the mid-points of the line segments
            var pt12 = (start + firstCtrlPoint) / 2;
            var pt23 = (firstCtrlPoint + secondCtrlPoint) / 2;
            var pt34 = (secondCtrlPoint + end) / 2;

            // calculate the mid-points of the new half lines
            var pt123 = (pt12 + pt23) / 2;
            var pt234 = (pt23 + pt34) / 2;

            // finally subdivide the last two midpoints. if we met our distance tolerance this will be the final point we use.
            var pt1234 = (pt123 + pt234) / 2;

            // try to approximate the full cubic curve by a single straight line
            var deltaLine = end - start;

            var d2 = System.Math.Abs(((firstCtrlPoint.X - end.X) * deltaLine.Y -
                                      (firstCtrlPoint.Y - end.Y) * deltaLine.X));
            var d3 = System.Math.Abs(((secondCtrlPoint.X - end.X) * deltaLine.Y -
                                      (secondCtrlPoint.Y - end.Y) * deltaLine.X));

            if ((d2 + d3) * (d2 + d3) < distanceTolerance * (deltaLine.X * deltaLine.X + deltaLine.Y * deltaLine.Y))
            {
                points.Add(pt1234);
                return;
            }

            // Continue subdivision
            RecursiveGetOptimizedDrawingPoints(start, pt12, pt123, pt1234, points, distanceTolerance);
            RecursiveGetOptimizedDrawingPoints(pt1234, pt234, pt34, end, points, distanceTolerance);
        }
        
        /// <summary>
        /// Recursively subdivides a bezier curve until distanceTolerance is met. Flat sections will have fewer points
        /// than curved with this algorithm. Returns a pooled list that should be returned to the ListPool when done.
        /// </summary>
        public static List<Vector2> GetOptimizedDrawingPoints(Vector2 start, Vector2 firstCtrlPoint,
            Vector2 secondCtrlPoint, Vector2 end, float distanceTolerance = 1f)
        {
            var points = ListPool<Vector2>.Obtain();
            points.Add(start);
            RecursiveGetOptimizedDrawingPoints(start, firstCtrlPoint, secondCtrlPoint, end, points, distanceTolerance);
            points.Add(end);

            return points;
        }
    }
}