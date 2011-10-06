using System;
using System.Collections.Generic;

namespace LINQToTreeHelpers
{
    /// <summary>
    /// A loop of color! This is an internal class.
    /// </summary>
    class ColorLoop
    {
        static Lazy<short[]> Colors = new Lazy<short[]>(() => CreateColors());
        public ColorLoop()
        {
        }

        /// <summary>
        /// Returns a list of colors we can use for plots. These are "optimized" to work well on beamers.
        /// </summary>
        /// <returns></returns>
        private static short[] CreateColors()
        {
            List<short> c = new List<short>();
            c.Add((short)ROOTNET.EColor.kBlack);
            c.Add((short)ROOTNET.EColor.kBlue);
            c.Add((short)ROOTNET.NTColor.GetColorDark((short)ROOTNET.EColor.kGreen));
            c.Add((short)ROOTNET.EColor.kRed);

            c.Add((short)ROOTNET.EColor.kViolet);
            c.Add((short)ROOTNET.EColor.kOrange);
            c.Add((short)ROOTNET.EColor.kMagenta);
            return c.ToArray();
        }

        private IEnumerator<short> _colorLoop = Colors.Value.ContinuousIterator().GetEnumerator();

        public short NextColor()
        {
            _colorLoop.MoveNext();
            return _colorLoop.Current;
        }
    }
}
