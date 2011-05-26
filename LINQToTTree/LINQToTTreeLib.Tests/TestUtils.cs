using System;
using LinqToTTreeInterfacesLib;

namespace LINQToTTreeLib.Tests
{
    /// <summary>
    /// Helpers
    /// </summary>
    public static class TestUtils
    {
        /// <summary>
        /// Dump the code to the console - for debugging a test...
        /// </summary>
        /// <param name="code"></param>
        public static void DumpCodeToConsole(this IGeneratedQueryCode code)
        {
            Console.WriteLine("Code:");
            foreach (var var in code.CodeBody.DeclaredVariables)
            {
                string initalValue = "default()";
                if (var.InitialValue != null && var.InitialValue.RawValue != null)
                    initalValue = var.InitialValue.RawValue;

                Console.WriteLine(var.Type.Name + " " + var.VariableName + " = " + initalValue + ";");
            }
            foreach (var l in code.CodeBody.CodeItUp())
            {
                Console.WriteLine("  " + l);
            }
        }
    }
}
