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
            }

            Console.WriteLine("Creating dataset {0}.", testDatasetName);
        }
    }
}
