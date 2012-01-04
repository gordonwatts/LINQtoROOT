using System;

namespace SetupPROOFServer
{
    class Program
    {
        /// <summary>
        /// Dataset name used by our tests... Contains only a single file.
        /// </summary>
        const string testDatasetName = "LINQTest";

        /// <summary>
        /// List of file paths that can be found on the PROOF server.
        /// </summary>
        static string[] testDataSetFileList = {
                                                 "/phys/groups/tev/scratch3/users/btag/mc11/group.perf-flavtag.105016.J7_pythia_jetjet.BTAG_D3PD.PCTF.r2725.0.111203145120/group.perf-flavtag.50938_004742.EXT0._00003.BTAG_D3PD.pool.root",
                                       };

        /// <summary>
        /// Simple setup for a PROOF server - so we can get it configured so all of our tests
        /// will run correctly.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            //
            // Make sure we know what we are doing
            //

            if (args.Length == 0)
            {
                Console.WriteLine("Usage: SetupPROOFServer <proof-server>");
                return;
            }

            string proofURI = string.Format("proof://{0}", args[0]);

            //
            // Open the PROOF server up, really don't need workers for this! :-)
            //

            var connection = new ROOTNET.NTProof(proofURI);

            //
            // Check to see if the dataset exists or not
            //

            if (connection.ExistsDataSet(testDatasetName))
            {
                Console.WriteLine("The test dataset exists");
                return;
            }

            Console.WriteLine("Creating dataset {0}.", testDatasetName);

            //
            // Create & define the dataset.
            //

            var files = new ROOTNET.NTFileCollection(testDatasetName, string.Format("File list for {0}", testDatasetName));
            foreach (var f in testDataSetFileList)
            {
                var finfo = new ROOTNET.NTFileInfo(f);
                files.Add(finfo);
            }
            Console.WriteLine("Dataset {0} will contain {1} files.", testDatasetName, testDataSetFileList.Length);

            if (!connection.RegisterDataSet(testDatasetName, files))
            {
                Console.WriteLine("Registration of the dataset unsuccessful");
                return;
            }

            Console.WriteLine("Dataset registered");

            connection.Close();
        }
    }
}
