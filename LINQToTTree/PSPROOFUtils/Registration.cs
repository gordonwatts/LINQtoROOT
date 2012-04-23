using System.ComponentModel;
using System.Management.Automation;

namespace PSPROOFUtils
{
    /// <summary>
    /// Register all the providers, etc., in this assembly
    /// </summary>
    [RunInstaller(true)]
    public class Registration : PSSnapIn
    {
        public override string Description
        {
            get { return "Provider to help with ROOT PROOF datasets"; }
        }

        public override string Name
        {
            get { return "PROOFSnapIn"; }
        }

        public override string Vendor
        {
            get { return "Gordon Watts"; }
        }
    }
}
