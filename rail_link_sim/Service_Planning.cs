using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Xml.Serialization;
using static rail_link_sim.Physical_Train_Network;

namespace rail_link_sim
{
    public partial class Service_Planning : Form
    {
        Physical_Train_Network fullscale = new Physical_Train_Network();


        List<Physical_route> routes = new List<Physical_route>();
        List<Service_Course> courses_draw = new List<Service_Course>();
        List<Physical_route> draw_routes = new List<Physical_route>();
        List<Service_Plan> plans = new List<Service_Plan>();

        Service_Plan current_plan = new Service_Plan();

        List<Planning_sch> planning_sch = new List<Planning_sch>();

       
        int command_flag = 0; //1=add 2=edit
        int edit_index = 0;
        public Service_Planning()
        {
            InitializeComponent();
        }

        int test_train_cap = 400;
        double test_train_cost_per_kilo = 1.8;

        List<List<int>> est_use_time = new List<List<int>>();

        int[,] ticket_price = new int[5, 5] {
        { 0,    278,    320,    393,    535},
        { 278,  0,      278,    320,    393},
        { 320,  278,    0,      278,    320},
        { 393,  320,    278,    0,      278},
        { 535,  393,    320,    278,    0}
        };

        class passenger_data {
            public DateTime time;
            public int count;

            public passenger_data(int hour,int min,int _count)
            {
                time = new DateTime(1, 1, 1, hour,min,0);
                count = _count;              
            }

            public passenger_data(DateTime _time, int _count)
            {
                time = _time;
                count = _count;
            }
        }

        List<passenger_data>[,] served_supply = new List<passenger_data>[5, 5];
        List<passenger_data>[,] passenger_supply = new List<passenger_data>[5, 5];
        List<passenger_data>[,] passenger_demand = new List<passenger_data>[5, 5];
        List<passenger_data>[,] cal_pass_demand = new List<passenger_data>[5, 5];
        List<List<int>> cal_pass_supply = new List<List<int>>();

        // just for debug
        int[] random_normal = new int[] { 0, 0, 0, 0, 0, 20, 50, 120, 30, 30, 30, 50, 100, 50, 50, 50, 50, 80, 80, 170, 80, 80, 0, 0 ,0};
        int[] normal_peaktime = new int[] { 0, 0, 0, 0, 80, 180, 300, 220, 160, 150, 100, 180, 210, 200, 100, 200, 200, 400, 240, 80, 60, 0, 0, 0, 0, 0 };
        int[] revers_peaktime = new int[25];


        private void Service_Planning_Load(object sender, EventArgs e)
        {

            Deserialize(plans, "plans.xml");
            Deserialize2(routes, "somsin.xml");

            fullscale.addTrack("physical_tracks.xml");
            fullscale.addLink("track_links.xml");
            fullscale.verify_track("ST11", 50, 200, 0);

            plan_select_box.Items.Clear();
            foreach (Service_Plan _p in plans)
            {
                plan_select_box.Items.Add(_p.name);               
            }
            if(plan_select_box.Items.Count > 0) plan_select_box.SelectedIndex = 0;

            current_plan.name = "new";
            current_plan.courses = new List<Service_Course>();

            foreach (Physical_route _r in routes)
            {
                routes_gridview.Rows.Add(_r.name, _r.tracks.Count,_r.stop_point[0],_r.stop_point[_r.stop_point.Count-1],_r.stop_point.Count);
            }

            planning_sch.Add(new Planning_sch("BKK", new List<string>() { "ST11", "ST12", "ST13", "ST14", "ST15", "ST16" }, 0, 50, 0));
            planning_sch.Add(new Planning_sch("AYU", new List<string>() { "ST21", "ST22", "ST23", "ST24", "ST25", "ST26" }, 100, 25 , 100));
            planning_sch.Add(new Planning_sch("SRB", new List<string>() { "ST31", "ST32", "ST33", "ST34", "ST35", "ST36" }, 80, 25 , 180));
            planning_sch.Add(new Planning_sch("PKC", new List<string>() { "ST41", "ST42", "ST43", "ST44", "ST45", "ST46" }, 80, 25 , 260));
            planning_sch.Add(new Planning_sch("NRS", new List<string>() { "ST51", "ST52", "ST53", "ST54", "ST55", "ST56" }, 100, 25 , 360));

            int plot_x = 10;
            foreach(Planning_sch _p in planning_sch)
            {

            }

            from_demand_box.Items.Add("ALL station");
            to_demand_box.Items.Add("ALL station");
            foreach (Planning_sch _P in planning_sch) {
                from_demand_box.Items.Add(_P.station_name);
                to_demand_box.Items.Add(_P.station_name);
            }
            from_demand_box.SelectedIndex = 0;
            to_demand_box.SelectedIndex = 0;

            for (int i = 0; i <= 24; i++)
            {
                revers_peaktime[i] = normal_peaktime[24 - i];
            }

            // just for test
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    passenger_demand[i, j] = new List<passenger_data>();
                    passenger_supply[i, j] = new List<passenger_data>();
                    for (int k = 0; k < 24; k++)
                    {
                        passenger_demand[i, j].Add(new passenger_data(k, 0, random_normal[k]));
                    }
                }
            }

            passenger_demand[0, 4] = new List<passenger_data>();
            passenger_demand[4, 0] = new List<passenger_data>();
            for (int k = 0; k < 24; k++)
            {
                passenger_demand[0, 4].Add(new passenger_data(k, 0, normal_peaktime[k]));
                passenger_demand[4, 0].Add(new passenger_data(k, 0, revers_peaktime[k]));
            }

        }

        public void Deserialize2(List<Physical_route> list, string fileName)
        {
            if (File.Exists(fileName))
            {
                var serializer = new XmlSerializer(typeof(List<Physical_route>));
                using (var stream = File.OpenRead(fileName))
                {
                    var other = (List<Physical_route>)(serializer.Deserialize(stream));
                    list.Clear();
                    list.AddRange(other);
                }
            }
        }

        public void Deserialize(List<Service_Plan> list, string fileName)
        {
            if (File.Exists(fileName))
            {
                var serializer = new XmlSerializer(typeof(List<Service_Plan>));
                using (var stream = File.OpenRead(fileName))
                {
                    var other = (List<Service_Plan>)(serializer.Deserialize(stream));
                    list.Clear();
                    list.AddRange(other);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // check name
            if (current_plan.courses.Any(Service_Course => Service_Course.name == course_name.Text))
            {
                MessageBox.Show("duplicate name");
            }
            else
            {
                DateTime time = new DateTime(1, 1, 1, (int)num_hou.Value, (int)num_min.Value, 0);               
                current_plan.courses.Add(new Service_Course(course_name.Text, train_box.Text, routes[routes_gridview.SelectedCells[0].RowIndex], time));
                current_plan.courses.Sort((Service_Course a, Service_Course b) => (a.start_time.CompareTo(b.start_time)));

                plan_gridview.Rows.Clear();
                foreach (Service_Course _c in current_plan.courses)
                {
                    plan_gridview.Rows.Add(_c.start_time.ToString("hh : mm"), _c.name, _c.route.tracks.Count, _c.route.tracks[0], _c.route.tracks[_c.route.tracks.Count - 1], _c.route.stop_point.Count);
                }
                refresh_est_time();
                splitContainer2.Panel1.Refresh();
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if(plan_gridview.SelectedCells.Count >= 1)
            {
                int _rm_index = plan_gridview.SelectedCells[0].RowIndex;
                current_plan.courses.RemoveAt(_rm_index);
                plan_gridview.Rows.RemoveAt(_rm_index);

                refresh_est_time();
                splitContainer1.Panel2.Refresh();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(command_flag == 1)
            {
                if (plans.Any(Service_Plan => Service_Plan.name == plan_name.Text))
                {
                    MessageBox.Show("duplicate name");
                }
                else
                {
                    current_plan.name = plan_name.Text;
                    plans.Add(current_plan);
                    // refresh selector
                    plan_select_box.Items.Clear();
                    foreach (Service_Plan _p in plans)
                    {
                        plan_select_box.Items.Add(_p.name);
                    }
                    if (plan_select_box.Items.Count > 0) plan_select_box.SelectedIndex = 0;
                }
            }
            else if(command_flag == 2)
            {
                plans[edit_index] = new Service_Plan(current_plan);
            }

            XmlSerializer serializer = new XmlSerializer(typeof(List<Service_Plan>));
            using (var stream = File.Open("plans.xml", FileMode.Create))
            {
                serializer.Serialize(stream, plans);
                stream.Close();
            }

            plan_select_group.Enabled = true;
            plan_view_group.Enabled = false;
            course_view_group.Enabled = false;
            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            command_flag = 1;
            plan_select_group.Enabled = false;
            plan_view_group.Enabled = true;
            course_view_group.Enabled = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {

            current_plan = new Service_Plan(plans[plan_select_box.SelectedIndex]);
            edit_index = plan_select_box.SelectedIndex;

            //current_plan.courses.Sort((Service_Course a, Service_Course b) => (a.start_time.CompareTo(b.start_time)));

            plan_gridview.Rows.Clear();
            foreach (Service_Course _c in current_plan.courses)
            {
                plan_gridview.Rows.Add(_c.start_time.ToString("hh : mm"), _c.name, _c.route.tracks.Count, _c.route.tracks[0], _c.route.tracks[_c.route.tracks.Count - 1], _c.route.stop_point.Count);
            }

            plan_select_group.Enabled = false;
            plan_view_group.Enabled = true;
            course_view_group.Enabled = true;

            refresh_est_time();

            splitContainer2.Panel1.Refresh();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            plan_view_group.Enabled = false;
            course_view_group.Enabled = false;
            plan_select_group.Enabled = true;
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        { }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            panel1.AutoScroll = true;

            int draw_start_x = 150;

            Pen yellowpen = new Pen(Color.Yellow, 5);
            yellowpen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
            yellowpen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
            Font drawFont = new Font("Arial", 16, FontStyle.Bold);
            StringFormat sf = new StringFormat();
            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Center;

            StringFormat sf_L = new StringFormat();
            sf_L.LineAlignment = StringAlignment.Center;
            sf_L.Alignment = StringAlignment.Far;
            // draw head
            foreach (Planning_sch _p in planning_sch)
            {
                if (_p.width != 0)
                {
                    e.Graphics.DrawLine(yellowpen, draw_start_x + _p.position_x, 100, draw_start_x + _p.position_x - _p.width, 100);
                }
            }
            foreach (Planning_sch _p in planning_sch)
            {
                e.Graphics.FillEllipse(Brushes.Yellow, draw_start_x + _p.position_x - _p.size / 2, 100 - _p.size / 2, _p.size, _p.size);
                e.Graphics.FillEllipse(Brushes.Orange, draw_start_x + _p.position_x - ((float)0.7 * _p.size) / 2, 100 - ((float)0.7 * _p.size) / 2, (float)0.7 * _p.size, (float)0.7 * _p.size);
                e.Graphics.DrawString(_p.station_name, drawFont, Brushes.Orange, draw_start_x + _p.position_x, 50, sf);
            }

            Pen gridpen = new Pen(Color.DarkGray, 2);

            int draw_y = 180;
            int draw_x_last;

            Pen track_pen = new Pen(Color.Yellow, 4);
            Font track_font = new Font("Arial", 14, FontStyle.Bold);
            Font time_font = new Font("Arial", 12, FontStyle.Regular);

            int route_cnt = 0;
            foreach (Service_Course _c in current_plan.courses)
            {
                draw_x_last = draw_start_x + planning_sch.Find(Planning_sch => Planning_sch.tracks.Contains(_c.route.stop_point[0])).position_x;

                e.Graphics.DrawLine(gridpen, draw_start_x + planning_sch[0].position_x - 40, draw_y, draw_start_x + planning_sch[planning_sch.Count - 1].position_x + 40, draw_y);
                // draw time
                e.Graphics.DrawString(_c.start_time.ToString("HH:mm"), track_font, Brushes.Yellow, draw_x_last, draw_y - 20, sf);
                // draw name
                e.Graphics.DrawString(_c.name, track_font, Brushes.Yellow, draw_start_x - 50, draw_y, sf_L);
                bool first_d = true;

                draw_x_last = draw_start_x + planning_sch.Find(Planning_sch => Planning_sch.tracks.Contains(_c.route.stop_point[0])).position_x;
                
                foreach (string _t in _c.route.stop_point)
                {
                    Planning_sch _sch = planning_sch.Find(Planning_sch => Planning_sch.tracks.Contains(_t));
                    e.Graphics.DrawLine(track_pen, draw_x_last, draw_y, draw_start_x + _sch.position_x, draw_y);
                    draw_x_last = _sch.position_x + draw_start_x;
                }

                draw_x_last = draw_start_x + planning_sch.Find(Planning_sch => Planning_sch.tracks.Contains(_c.route.stop_point[0])).position_x;
                int stop_cnt = 0;

                //for(int stop_cnt = 0; stop_cnt < _c.route.stop_point.Count; stop_cnt)
                foreach (string _t in _c.route.stop_point)
                {
                    Planning_sch _sch = planning_sch.Find(Planning_sch => Planning_sch.tracks.Contains(_t));
                    if (_sch != null)
                    {
                        if (first_d)
                        {
                            e.Graphics.FillEllipse(Brushes.Orange, draw_start_x + _sch.position_x - 8, draw_y - 8, 16, 16);
                            first_d = false;
                        }
                        else if (stop_cnt + 1 == _c.route.stop_point.Count)
                        { // last
                            e.Graphics.DrawString(_c.start_time.AddMinutes(est_use_time[route_cnt][stop_cnt]).ToString("HH:mm"), time_font, Brushes.Aqua, draw_start_x + _sch.position_x, draw_y - 18, sf);
                            if (_sch.position_x > draw_x_last) // dir check
                            {
                                //e.Graphics.FillEllipse(Brushes.Red, draw_start_x + _sch.position_x - 8, draw_y - 8, 16, 16);
                                e.Graphics.DrawLine(yellowpen, draw_start_x + _sch.position_x, draw_y, draw_start_x + _sch.position_x - 8, draw_y - 8);
                                e.Graphics.DrawLine(yellowpen, draw_start_x + _sch.position_x, draw_y, draw_start_x + _sch.position_x - 8, draw_y + 8);
                            }
                            else
                            {
                                //e.Graphics.FillEllipse(Brushes.Purple, draw_start_x + _sch.position_x - 8, draw_y - 8, 16, 16);
                                e.Graphics.DrawLine(yellowpen, draw_start_x + _sch.position_x, draw_y, draw_start_x + _sch.position_x + 8, draw_y - 8);
                                e.Graphics.DrawLine(yellowpen, draw_start_x + _sch.position_x, draw_y, draw_start_x + _sch.position_x + 8, draw_y + 8);
                            }
                        }
                        else
                        {
                            e.Graphics.FillEllipse(Brushes.Yellow, draw_start_x + _sch.position_x - 8, draw_y - 8, 16, 16);
                            e.Graphics.DrawString(_c.start_time.AddMinutes(est_use_time[route_cnt][stop_cnt]).ToString("HH:mm"), time_font, Brushes.Aqua, draw_start_x + _sch.position_x, draw_y - 18, sf);
                        }

                        

                        //draw_x_last = _sch.position_x + draw_start_x;
                        stop_cnt++;
                    }
                }
                draw_y += 50;
                route_cnt++;
            }


        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            if(from_demand_box.SelectedIndex < 0 || to_demand_box.SelectedIndex < 0)
            {
                MessageBox.Show("please select station");
                return;
            }

            if (from_demand_box.SelectedIndex != 0 && from_demand_box.SelectedIndex == to_demand_box.SelectedIndex )
            {
                MessageBox.Show("invalid station : same station");
                return;
            }

            // init cal demand
            
            for(int i = 0; i < 5; i++)
            {
                for(int j = 0; j < 5; j++)
                {
                    cal_pass_demand[i, j] = new List<passenger_data>();
                    for (int k = 0; k < passenger_demand[i,j].Count; k++)
                    {
                        cal_pass_demand[i, j].Add(new passenger_data(passenger_demand[i,j][k].time, passenger_demand[i, j][k].count));
                    }
                }
            }

            // init supply

            cal_pass_supply = new List<List<int>>();
            for (int i=0;i< current_plan.courses.Count; i++)
            {
                cal_pass_supply.Add(new List<int>());
                for(int j=0;j< current_plan.courses[i].route.stop_point.Count; j++)
                {
                    cal_pass_supply[i].Add(400);
                }
            }


            int period_gap = 30;
            for(int period = period_gap; period < 120; period += period_gap)
            {
                int pass_cnt = 0;
                for(int t = 0; t < current_plan.courses.Count ; t++)
                {                                        
                    for(int i = 0; i < current_plan.courses[t].route.stop_point.Count; i++)
                    {
                        // suming possible served demand
                        List<List<int>> pass_demand = new List<List<int>>();
                        List<List<int>> pass_index = new List<List<int>>();
                        List<int> sum_demand = new List<int>();
                        
                        int total_sum_demand = 0;
                        int f_station = find_station(current_plan.courses[t].route.stop_point[i]);

                        for (int j = i + 1; j < current_plan.courses[t].route.stop_point.Count; j++)
                        {
                            pass_index.Add(new List<int>());
                            pass_demand.Add(new List<int>());
                            sum_demand.Add(0);
                            int t_station = find_station(current_plan.courses[t].route.stop_point[j]);
                            // search inrange passenger
                            for (int _d = 0; _d < cal_pass_demand[i, j].Count; _d++)
                            {
                                DateTime cmp_date = new DateTime(1,1,1,0,0,0);
                                int demand_min = (cal_pass_demand[f_station, t_station][_d].time.Hour * 60) + cal_pass_demand[f_station, t_station][_d].time.Minute;
                                int supply_min = (current_plan.courses[t].start_time.AddMinutes(est_use_time[t][j - i - 1]).Hour * 60) + current_plan.courses[t].start_time.AddMinutes(est_use_time[t][j - i - 1]).Hour;
                                if (demand_min > supply_min - period && demand_min <= supply_min + period) {
                                    pass_index[j - i - 1].Add(_d);
                                    pass_demand[j - i - 1].Add(cal_pass_demand[f_station, t_station][_d].count);
                                    total_sum_demand += cal_pass_demand[f_station, t_station][_d].count;
                                    sum_demand[j - i - 1] += cal_pass_demand[f_station, t_station][_d].count;
                                }                                                                  
                            }
                        }

                        // process served demand (avg method)
                        double served_percent = (double)cal_pass_supply[t][i] / total_sum_demand;
                        if (served_percent > 1.00) served_percent = 1.00;

                        // delete supply
                        if (cal_pass_supply[t][i] > total_sum_demand)
                        {
                            cal_pass_supply[t][i] -= total_sum_demand;
                        }
                        else
                        {
                            cal_pass_supply[t][i] = 0;
                        }

                        // delete demand
                        for (int j = i+1;j< current_plan.courses[t].route.stop_point.Count; j++)
                        {
                            //double each_demand_percent = sum_demand[j - i - 1];
                            int t_station = find_station(current_plan.courses[t].route.stop_point[j]); 
                            for(int k=0;k<pass_index[j - i - 1].Count; k++)
                            {
                                cal_pass_demand[f_station, t_station][pass_index[j - i - 1][k]].count = (int)(served_percent * cal_pass_demand[f_station, t_station][pass_index[j - i - 1][k]].count);
                            }
                            
                        }
                        
                    }
                }
            }

            deman_panel.Controls.Clear();
            deman_panel.AutoScroll = false;

            List<int> _origin = new List<int>();
            List<int> _destination = new List<int>();

            if (from_demand_box.SelectedIndex == 0) _origin = new int[] { 0, 1, 2, 3, 4 }.ToList();
            else _origin.Add(from_demand_box.SelectedIndex - 1);
            if (to_demand_box.SelectedIndex == 0) _destination = new int[] { 0, 1, 2, 3, 4 }.ToList();
            else _destination.Add(to_demand_box.SelectedIndex - 1);

            int chart_cnt = 0;
            int _x_size = deman_panel.Size.Width - 33;

            foreach (int _o in _origin)
            {
                foreach (int _d in _destination)
                {

                    if (_o != _d)
                    {
                        Chart _chart = new Chart();

                        deman_panel.Controls.Add(_chart);
                        _chart.Location = new Point(3, 10 + 160 * chart_cnt);
                        _chart.Size = new Size(_x_size, 150);
                        _chart.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
                        _chart.BackColor = SystemColors.WindowFrame;
                        _chart.ChartAreas.Add("area");
                        _chart.ChartAreas[0].BackColor = Color.DimGray;
                        _chart.ChartAreas[0].AxisX.LineColor = Color.White;
                        _chart.ChartAreas[0].AxisX.LabelStyle.Format = "HH:00";
                        _chart.ChartAreas[0].AxisX.MajorGrid.LineColor = SystemColors.ControlDark;
                        _chart.ChartAreas[0].AxisX.MajorTickMark.LineColor = Color.White;
                        _chart.ChartAreas[0].AxisX.LabelStyle.ForeColor = Color.White;
                        //_chart.ChartAreas[0].AxisX.Interval = 1;
                        _chart.ChartAreas[0].AxisX.IntervalAutoMode = IntervalAutoMode.VariableCount;
                        _chart.ChartAreas[0].AxisX.IntervalType = DateTimeIntervalType.Hours;
                        _chart.ChartAreas[0].AxisY.LineColor = Color.White;
                        _chart.ChartAreas[0].AxisY.MajorGrid.LineColor = SystemColors.ControlDark;
                        _chart.ChartAreas[0].AxisY.MajorTickMark.LineColor = Color.White;
                        _chart.ChartAreas[0].AxisY.LabelStyle.ForeColor = Color.White;

                        _chart.Series.Add("demand");
                        _chart.Series[0].ChartType = SeriesChartType.SplineArea;

                        foreach(passenger_data _p in passenger_demand[_o, _d])
                        {
                            _chart.Series[0].Points.AddXY(_p.time,_p.count);
                        }

                        _chart.Series.Add("supply");
                        _chart.Series[1].ChartType = SeriesChartType.BoxPlot;
                        _chart.Series[1].IsValueShownAsLabel = true;
                        _chart.Series[1].LabelForeColor = Color.Orange;

                        foreach (passenger_data _p in passenger_supply[_o, _d])
                        {
                            _chart.Series[1].Points.AddXY(_p.time, _p.count);
                        }

                        _chart.Series.Add("remaining demand");
                        _chart.Series[2].ChartType = SeriesChartType.SplineArea;

                        foreach (passenger_data _p in cal_pass_demand[_o, _d])
                        {
                            _chart.Series[2].Points.AddXY(_p.time, _p.count);
                        }

                        _chart.Titles.Add("name");
                        _chart.Titles[0].ForeColor = Color.White;
                        _chart.Titles[0].Text = planning_sch[_o].station_name + " -> " + planning_sch[_d].station_name;
                        _chart.Titles[0].Alignment = ContentAlignment.TopLeft;
                        _chart.Legends.Add("legend");
                        _chart.Legends[0].BackColor = SystemColors.WindowFrame;
                        _chart.Legends[0].ForeColor = Color.White;

                        chart_cnt++;
                    }
                }
            }

            deman_panel.AutoScroll = true;

        }

        private void button8_Click(object sender, EventArgs e)
        {
            refresh_demand_view();
        }

        public void refresh_demand_view()
        {          



        }

        public int find_station(string track_name)
        {
            int ans = -1;

            for(int i = 0; i < planning_sch.Count; i++)
            {
                foreach(string _s in planning_sch[i].tracks)
                {
                    if (track_name == _s) ans = i;
                }
            }

            return ans;
        }

        public void refresh_est_time()
        {
            est_use_time = new List<List<int>>();
            foreach(Service_Course _C in current_plan.courses)
            {
                est_use_time.Add(new List<int>());
                double _min_use = 0.00;
                int _stop_index = 1;
                est_use_time.Last().Add((int)_min_use);
                for (int i = 1; i < _C.route.tracks.Count; i++)
                {
                    _min_use += ((double)fullscale.tracks[_C.route.tracks[i]].legnth / (0.9 * fullscale.tracks[_C.route.tracks[i]].max_speed)) / 100000 * 60;
                    if (_C.route.stop_point.Contains(_C.route.tracks[i])) {
                        _min_use += 20;
                        est_use_time.Last().Add((int)_min_use);
                        _stop_index++;
                    }
                }
            }

            for (int i = 0; i < planning_sch.Count -1; i++)
            {
                for (int j = 0; j < planning_sch.Count -1; j++)
                {
                    passenger_supply[i, j] = new List<passenger_data>();
                }
            }

            int _c_cnt = 0;
            foreach (Service_Course _C in current_plan.courses)
            {
                for (int i = 0; i < _C.route.stop_point.Count; i++)
                {
                    int f_station = find_station(_C.route.stop_point[i]);
                    for (int j = i + 1; j < _C.route.stop_point.Count; j++)
                    {
                        int t_station = find_station(_C.route.stop_point[j]);
                        //passenger_supply[i, j, k, 0] += 400; // for test train only!!!!!
                        passenger_supply[f_station, t_station].Add(new passenger_data(_C.start_time.AddMinutes(est_use_time[_c_cnt][j - i - 1]), 400));
                    }
                }
                _c_cnt++;
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            refresh_est_time();
        }
    }
}

