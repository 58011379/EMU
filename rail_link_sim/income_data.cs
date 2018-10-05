using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace rail_link_sim
{
    public partial class income_data : Form
    {
        public income_data()
        {
            InitializeComponent();
        }

        private void income_data_Load(object sender, EventArgs e)
        {
            
            this.comboBox1.Items.AddRange(new object[] {"BKK->AYU", "BKK->SRB", "BKK->PKC", "BKK->NRS", "AYU->SRB", "AYU->PKC", "AYU->NRS" });
            this.comboBox1.SelectedIndex = 0;

            this.comboBox2.Items.AddRange(new object[] { "January", "February" , "March", "April", "May", "June", "July", "August", "August", "September", "October", "November", "December" });
            this.comboBox2.SelectedIndex = 0;

            this.comboBox3.Items.AddRange(new object[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" });
            this.comboBox3.SelectedIndex = 0;
            
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            /*
            int number = this.comboBox1.SelectedIndex;
            MessageBox.Show(number.ToString);
            */

            if (comboBox1.SelectedIndex == 0) {
                
                this.chart1.Series["People"].Points.AddXY("BKK", 13652);
                this.chart1.Series["People"].Points.AddXY("AYU", 7852);
                this.chart1.Series["People"].Points.AddXY("SRB", 13145);
                this.chart1.Series["People"].Points.AddXY("PKC", 9541);
                this.chart1.Series["People"].Points.AddXY("NRS", 7621);
                
            }


        }
    }
}
