using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Test
{
   public class Program
    {
        static void Main(string[] args)
        {
        }
        public static bool IsIntersect(PointF p1, PointF p2, PointF q1, PointF q2)
        {
            //排斥试验，判断p1p2在q1q2为对角线的矩形区之外
            if (Math.Max(p1.X, p2.X) < Math.Min(q1.X, q2.X))
            {//P1P2中最大的X比Q1Q2中的最小X还要小，说明P1P2在Q1Q2的最左点的左侧，不可能相交。
                return false;
            }

            if (Math.Min(p1.X, p2.X) > Math.Max(q1.X, q2.X))
            {//P1P2中最小的X比Q1Q2中的最大X还要大，说明P1P2在Q1Q2的最右点的右侧，不可能相交。
                return false;
            }

            if (Math.Max(p1.Y, p2.Y) < Math.Min(q1.Y, q2.Y))
            {//P1P2中最大的Y比Q1Q2中的最小Y还要小，说明P1P2在Q1Q2的最低点的下方，不可能相交。
                return false;
            }

            if (Math.Min(p1.Y, p2.Y) > Math.Max(q1.Y, q2.Y))
            {//P1P2中最小的Y比Q1Q2中的最大Y还要大，说明P1P2在Q1Q2的最高点的上方，不可能相交。
                return false;
            }

            //跨立试验
            var crossP1P2Q1 = VectorKits.Cross(p1, p2, q1);
            var crossP1Q2P2 = VectorKits.Cross(p1, q2, p2);
            var crossQ1Q2P1 = VectorKits.Cross(q1, q2, p1);
            var crossQ1P2Q2 = VectorKits.Cross(q1, p2, q2);

            bool isIntersect = (crossP1P2Q1 * crossP1Q2P2 >= 0) && (crossQ1Q2P1 * crossQ1P2Q2 >= 0);
            return isIntersect;
        }
    }
}
