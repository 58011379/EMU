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
    public partial class graph_editor_panel : Form
    {
        private train_time_table parent;
        public bool speed_graph_event_enable = false;
        public graph_editor_panel()
        {
            InitializeComponent();
        }
        public graph_editor_panel(train_time_table _p)
        {
            parent = _p;
            InitializeComponent();
        }

        private void max_speed_box_ValueChanged(object sender, EventArgs e)
        {
            if (speed_graph_event_enable)
            {
                parent.selected_max_speeds[parent.speed_graph_edit_selected] = (float)max_speed_box.Value;
                parent.refresh_selected_speed();
                parent.speed_graph.Invalidate();

                max_speed_text.Text = "(" + (parent.fullscale.tracks[parent.selected_course.route.tracks[parent.speed_graph_edit_selected]].max_speed * parent.selected_max_speeds[parent.speed_graph_edit_selected] / (float)100.00).ToString() + ")";

            }
        }

        private void operate_speed_box_ValueChanged(object sender, EventArgs e)
        {
            if (speed_graph_event_enable)
            {               
                parent.selected_operate_speeds[parent.speed_graph_edit_selected] = (float)operate_speed_box.Value;
                parent.refresh_selected_speed();
                parent.speed_graph.Invalidate();

                operate_speed_text.Text = "(" + (parent.fullscale.tracks[parent.selected_course.route.tracks[parent.speed_graph_edit_selected]].max_speed * parent.selected_operate_speeds[parent.speed_graph_edit_selected] / (float)100.00).ToString() + ")";

            }
        }

        private void dewell_box_ValueChanged(object sender, EventArgs e)
        {
            if (speed_graph_event_enable)
            {
                parent.dewell_tmp[parent.dewell_edit_index] = (float)dewell_box.Value;
                parent.refresh_selected_speed();               

            }
        }
    }
}
