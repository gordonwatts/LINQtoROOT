
using System;
namespace CleanupPROOFServer
{
    class Program
    {
        /// <summary>
        /// Dataset name used by our tests... Contains only a single file.
        /// </summary>
        const string testDatasetName = "LINQTest";

        /// <summary>
        /// Simple cleanup for a PROOF server - remove everything we've configured.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            //
            // Make sure we know what we are doing
            //

            if (args.Length == 0)
            {
                Console.WriteLine("Usage: CleanupPROOFServer <proof-server>");
                return;
            }

            string proofURI = string.Format("proof://{0}", args[0]);

            //
            // Open the PROOF server up, really don't need workers for this! :-)
            //

            var connection = new ROOTNET.NTProof(proofURI);

            try
            {
                //
                // Check to see if the dataset exists or not
                //

                if (!connection.ExistsDataSet(testDatasetName))
                {
                    Console.WriteLine("The test dataset does not exists");
                    return;
                }

                Console.WriteLine("Removing the dataset {0}.", testDatasetName);

                //
                // kill off the dataset.
                //

                var r = connection.RemoveDataSet(testDatasetName);
                if (r != 0)
                {
                    Console.WriteLine("Failed to remove dataset {0}.", testDatasetName);
                    return;
                }

                Console.WriteLine("Dataset removed");

            }
            finally
            {
                connection.Close();
            }
        }
    }
}
