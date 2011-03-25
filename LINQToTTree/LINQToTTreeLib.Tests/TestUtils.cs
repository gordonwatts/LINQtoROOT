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
        public static void DumpCodeToConsole(this IGeneratedCode code)
        {
            Console.WriteLine("Code:");
            foreach (var var in code.CodeBody.DeclaredVariables)
            {
                Console.WriteLine(var.Type.Name + " " + var.VariableName + " = " + var.InitialValue.RawValue + ";");
            }
            foreach (var l in code.CodeBody.CodeItUp())
            {
                Console.WriteLine("  " + l);
            }
        }
    }
}
