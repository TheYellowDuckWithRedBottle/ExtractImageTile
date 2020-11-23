using System;
using System.Collections.Generic;
using System.Text;

namespace Test
{
    public class VectorKits
    {
        public static float Cross(PointF p1, PointF p2, PointF p3)
        {
            return (p2.X - p1.X) * (p3.Y - p1.Y) - (p2.Y - p1.Y) * (p3.X - p1.X);
        }

        /// <summary>
        /// 叉乘.
        /// V1(x1, y1) X V2(x2, y2) = x1y2 – y1x2
        /// </summary>
        /// <param name="vector1"></param>
        /// <param name="vector2"></param>
        /// <returns></returns>
        public static float Cross(PointF vector1, PointF vector2)
        {
            return Cross(vector1.X, vector1.Y, vector2.X, vector2.Y);
        }

        /// <summary>
        /// 叉乘
        /// </summary>
        /// <param name="v1X">v1向量的X分量</param>
        /// <param name="v1Y">v1向量的Y分量</param>
        /// <param name="v2X">v2向量的X分量</param>
        /// <param name="v2Y">v2向量的Y分量</param>
        /// <returns></returns>
        public static float Cross(float v1X, float v1Y, float v2X, float v2Y)
        {
            return v1X * v2Y - v1Y * v2X;
        }


        /// <summary>
        /// 点乘(P1P2 * P1P3)
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <returns></returns>
        public static float Dot(PointF p1, PointF p2, PointF p3)
        {
            return (p2.X - p1.X) * (p3.X - p1.X) + (p3.Y - p1.Y) * (p2.Y - p1.Y);
        }


        /// <summary>
        /// 点乘.
        /// V1( x1, y1) x V2(x2, y2) = x1*x2 + y1*y2
        /// </summary>
        /// <returns></returns>
        public static float Dot(PointF vector1, PointF vector2)
        {
            return Dot(vector1.X, vector1.Y, vector2.X, vector2.Y);
        }

        /// <summary>
        /// 点乘.
        /// V1( x1, y1) x V2(x2, y2) = x1*x2 + y1*y2
        /// </summary>
        /// <param name="v1X">v1向量的X分量</param>
        /// <param name="v1Y">v1向量的Y分量</param>
        /// <param name="v2X">v2向量的X分量</param>
        /// <param name="v2Y">v2向量的Y分量</param>
        /// <returns></returns>
        public static float Dot(float v1X, float v1Y, float v2X, float v2Y)
        {
            return v1X * v2X + v1Y * v2Y;
        }

    } }

