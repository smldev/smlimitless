//-----------------------------------------------------------------------
// <copyright file="Program.cs" company="Chris Akridge">
//     Copyrighted unter the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMLimitless
{
    #if WINDOWS
        /// <summary>
        /// Contains the entry point to the program.
        /// </summary>
        public static class Program
        {
            /// <summary>
            /// The main entry point for the application.
            /// </summary>
            /// <param name="args">Command-line arguments.</param>
            public static void Main(string[] args)
            {
                using (SmlProgram game = new SmlProgram())
                {
                    game.Run();
                }
            }
        }
    #endif
}
