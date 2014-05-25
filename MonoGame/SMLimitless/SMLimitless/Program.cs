//-----------------------------------------------------------------------
// <copyright file="Program.cs" company="The Limitless Development Team">
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
                try
                {
                    using (SmlProgram game = new SmlProgram())
                    {
                        game.Run();
                    }
                }
                catch (Exception ex)
                {
                    // TODO: get rid of this catch block whenever we get UIs working
                    string message = string.Format("An unhandled exception has occurred in Super Mario Limitless.{0}Exception: {1}{0}{0}Message: {2}", Environment.NewLine, ex.GetType().FullName, ex.Message);
                    System.Windows.Forms.MessageBox.Show(message, "Unhandled Exception", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                }
            }
        }
    #endif
}
