using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace rail_link_sim
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new train_management()); 
            //Application.Run(new Route_management());
            //Application.Run(new simmulation_engine());
            //Application.Run(new physical_simulator());
            //Application.Run(new Service_Planning()); 
            //Application.Run(new infarstructure_file_builder());
            //Application.Run(new train_time_table());
            //Application.Run(new graph_editor_panel());
            Application.Run(new EMU_Utilization());
        }
    }
}
