
using System;
using LINQToTTreeLib.CodeAttributes;
namespace LINQToTreeHelpers
{
    /// <summary>
    /// Some helper routines for dealing with root.
    /// </summary>
    [CPPHelperClass]
    public static class ROOTUtils
    {
        /// <summary>
        /// Add a histogram to a ROOT directory.
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="h"></param>
        public static void Add(this ROOTNET.Interface.NTDirectory dir, ROOTNET.Interface.NTH1 h)
        {
            h.SetDirectory(dir);
        }

        /// <summary>
        /// Save a plot to a TDirectory. Return the plot so it can also be used in other
        /// places.
        /// 
        /// Temp Fix: We need to set the object owner so that this object won't be cleaned up during
        /// GC, however, ROOT.NET doesn't support it yet. So instead we will just set it to null and
        /// return null for now. To be fixed when we update ROOT.NET.
        /// </summary>
        /// <param name="hist"></param>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static ROOTNET.Interface.NTH1 SaveToROOTDirectory(this ROOTNET.Interface.NTH1 hist, ROOTNET.Interface.NTDirectory dir)
        {
            hist.InternalWriteObject(dir);
            return null;
        }

        /// <summary>
        /// Do an immediate write to a root directory
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="dir"></param>
        public static void WriteToROOTDirectory<T>(this T obj, ROOTNET.Interface.NTDirectory dir)
            where T : ROOTNET.Interface.NTObject
        {
            obj.InternalWriteObject(dir);
        }

        #region CPP ROOT Helpers
        /// <summary>
        /// Create a TLorentzVector from pt, eta, phi, and E.
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="eta"></param>
        /// <param name="phi"></param>
        /// <param name="E"></param>
        /// <returns></returns>
        [CPPCode(IncludeFiles = new string[] { "TLorentzVector.h" },
            Code = new string[]{
                "TLorentzVector tlzUnique;",
                "tlzUnique.SetPtEtaPhiE(pt, eta, phi, E);",
                "CreateTLZ = &tlzUnique;"
            })]
        public static ROOTNET.NTLorentzVector CreateTLZ(double pt, double eta, double phi, double E)
        {
            throw new NotImplementedException("This should never get called!");
#if false
            var tlz = new ROOTNET.NTLorentzVector();
            tlz.SetPtEtaPhiE(pt, eta, phi, E);
            return tlz;
#endif
        }

        /// <summary>
        /// Create a TLZ, assuming a pion mass.
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="eta"></param>
        /// <param name="phi"></param>
        /// <returns></returns>
        [CPPCode(IncludeFiles = new string[] { "TLorentzVector.h" },
            Code = new string[]{
                "TLorentzVector tlzUnique;",
                "tlzUnique.SetPtEtaPhiM(pt, eta, phi, 139.6);",
                "CreateTLZ = &tlzUnique;"
            })]
        public static ROOTNET.NTLorentzVector CreateTLZ(double pt, double eta, double phi)
        {
            throw new NotImplementedException("This should never get called!");
#if false
            var tlz = new ROOTNET.NTLorentzVector();
            tlz.SetPtEtaPhiM(pt, eta, phi, 139.6);
            return tlz;
#endif
        }

        /// <summary>
        /// Calculate DR^2 rather than DR. The object is to get a sqrt out of the center of a very
        /// hot loop. Otherwise, use the standard DeltaR. Since this is an extension method, you
        /// can just use tlx.DeltaR2(other) to get the DR2 between tlx and other.
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        [CPPCode(IncludeFiles = new[] { "TLorentzVector.h", "TVector2.h" },
            Code = new[] {
                "double detaUnique = v1->Eta() - v2->Eta();",
                "double dphiUnique = TVector2::Phi_mpi_pi(v1->Phi() - v2->Phi());",
                "DeltaR2 = detaUnique*detaUnique + dphiUnique*dphiUnique;"
            })]
        public static double DeltaR2(this ROOTNET.NTLorentzVector v1, ROOTNET.NTLorentzVector v2)
        {
            throw new NotImplementedException("this should never get called");
#if false
            double deta = v1.Eta() - v2.Eta();
            double deltaphi = ROOTNET.NTVector2.Phi_mpi_pi(v1.Phi() - v2.Phi());
            return deta * deta + deltaphi * deltaphi;
#endif
        }

        /// <summary>
        /// Create a TLorentzVector from px, py, pz, and E.
        /// </summary>
        /// <param name="px"></param>
        /// <param name="py"></param>
        /// <param name="pz"></param>
        /// <param name="E"></param>
        /// <returns></returns>
        [CPPCode(IncludeFiles = new string[] { "TLorentzVector.h" },
            Code = new string[]{
                "TLorentzVector tlzUnique;",
                "tlzUnique.SetPtEtaPhiE(px, py, pz, E);",
                "CreateTLZPxPyPzE = &tlzUnique;"
            })]
        public static ROOTNET.NTLorentzVector CreateTLZPxPyPzE(double px, double py, double pz, double E)
        {
            throw new NotImplementedException("This should never get called!");
#if false
            var tlz = new ROOTNET.NTLorentzVector();
            tlz.SetPxPyPzE(px, py, pz, E);
            return tlz;
#endif
        }

        /// <summary>
        /// Create a TLorentzVector from px, py, pz, and M.
        /// </summary>
        /// <param name="px"></param>
        /// <param name="py"></param>
        /// <param name="pz"></param>
        /// <param name="M"></param>
        /// <returns></returns>
        [CPPCode(IncludeFiles = new string[] { "TLorentzVector.h" },
            Code = new string[]{
                "TLorentzVector tlzUnique;",
                "tlzUnique.SetXYZM(px, py, pz, M);",
                "CreateTLZPxPyPzM = &tlzUnique;"
            })]
        public static ROOTNET.NTLorentzVector CreateTLZPxPyPzM(double px, double py, double pz, double M)
        {
            throw new NotImplementedException("This should never get called!");
#if false
            var tlz = new ROOTNET.NTLorentzVector();
            tlz.SetXYZM(px, py, pz, M);
            return tlz;
#endif
        }
        #endregion

    }
}
