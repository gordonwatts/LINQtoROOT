
using System;
namespace LINQToTreeHelpers
{
    /// <summary>
    /// A class to make it easy to setup a plotting style.
    /// </summary>
    public static class PlotStyleSetup
    {
        ///
        /// Use this style as the default plotting style
        /// 
        public static void SetAsStyle()
        {
            plotStyle.Value.cd();
            //ROOTNET.NTROOT.gROOT.SetStyle("Plain");
        }

        private static Lazy<ROOTNET.Interface.NTStyle> plotStyle = new Lazy<ROOTNET.Interface.NTStyle>(() => CreateDefaultStyle());

        private static ROOTNET.Interface.NTStyle CreateDefaultStyle()
        {
            ROOTNET.NTROOT.gROOT.SetStyle("Plain");
            ROOTNET.NTStyle style = new ROOTNET.NTStyle(ROOTNET.NTStyle.gStyle);
            style.Name = "NicePlots";

            //
            // The background fill should be white!
            // 

            short backgroundFill = (short)ROOTNET.EColor.kWhite;
            style.FillColor = backgroundFill;
            style.FrameFillColor = backgroundFill;
            style.HistFillColor = backgroundFill;
            style.TitleFillColor = backgroundFill;

            //
            // The histogram line size
            // 

            style.HistLineWidth = 2;

            return style;
        }
    }
}
