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
using System.Xml.Serialization;
using static rail_link_sim.Physical_Train_Network;

namespace rail_link_sim
{
    public partial class train_time_table : Form
    {
        public train_time_table()
        {
            InitializeComponent();
        }

        List<Service_Plan> plans = new List<Service_Plan>();
        List<Physical_route> routes = new List<Physical_route>();
        public Physical_Train_Network fullscale = new Physical_Train_Network();

        List<Planning_sch> planning_sch = new List<Planning_sch>();
        public Service_Course selected_course;

        Service_Plan selected_plan;
        bool speed_draw_req = false;
        int speed_h_location = 0;
        int speed_v_location = 0;

        bool editing_flag = false;
        bool adding_flag = false;
        int editing_index = -1;

        // connect mode
        bool connect_mode = true;
        int connect_point_index = -1;
        int connect_trip_index = -1;



        int position_h_location = 0;
        int position_v_location = 0;
        List<int> station_distance = new List<int>();

        List<float> selected_accum_length = new List<float>();

        // speed graph editor
        public int speed_graph_edit_hover = -1;
        public int speed_graph_edit_selected = -1;
        graph_editor_panel edit_form;
        public List<float> selected_max_speeds = new List<float>();
        public List<float> selected_operate_speeds = new List<float>();

        // position graph interact
        speed_point pos_graph_point_hover = null;
        Service_Course pos_graph_course_hover = null;
        int pos_graph_track_cnt = -1;
        float pos_graph_point_distance;
        int pos_graph_selected_index = -2;

        float train_acc_up = (float)0.40;
        float train_acc_down = (float)-0.50;

        int graphic_scale = 27;

        // conflict route
        List<conflict_trip> conflict_trips = new List<conflict_trip>();

        // speed finder
        List<timed_course> timed_data = new List<timed_course>();

        public List<float> selected_speeds = new List<float>();
        List<speed_point> operate_speed_graph_tmp = new List<speed_point>();
        List<speed_point> max_speed_graph_tmp = new List<speed_point>();

        List<speed_point> speed_graph_tmp = new List<speed_point>();
        List<speed_point> make_speed_tmp = new List<speed_point>();
        public List<float> dewell_tmp = new List<float>();
        public List<int> dewell_pos = new List<int>();
        public int dewell_edit_index = -1;

        bool make_speed_complete;

        private void train_time_table_Load(object sender, EventArgs e)
        {
            Deserialize(plans, "plans.xml");
            Deserialize2(routes, "somsin.xml");
            fullscale.addTrack("physical_tracks.xml");
            fullscale.addLink("track_links.xml");

            fullscale.verify_track("ST11", 50, 100, 0);

            foreach (Physical_route r in routes)
            {
                route_box.Items.Add(r);
            }

            train_box.Items.Add("1");

            planning_sch.Add(new Planning_sch("BKK", new List<string>() { "ST11", "ST12", "ST13", "ST14", "ST15", "ST16" }, 0, 50, 0));
            planning_sch.Add(new Planning_sch("AYU", new List<string>() { "ST21", "ST22", "ST23", "ST24", "ST25", "ST26" }, 100, 25, 100));
            planning_sch.Add(new Planning_sch("SRB", new List<string>() { "ST31", "ST32", "ST33", "ST34", "ST35", "ST36" }, 80, 25, 180));
            planning_sch.Add(new Planning_sch("PKC", new List<string>() { "ST41", "ST42", "ST43", "ST44", "ST45", "ST46" }, 80, 25, 260));
            planning_sch.Add(new Planning_sch("NRS", new List<string>() { "ST51", "ST52", "ST53", "ST54", "ST55", "ST56" }, 100, 25, 360));

            station_distance.Add(500);
            station_distance.Add(62500);
            station_distance.Add(106500);
            station_distance.Add(173500);
            station_distance.Add(251500);

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

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // for test only select "0" plan
            selected_plan = plans[0];
            course_box.Items.Clear();
            foreach (Service_Course c in selected_plan.courses)
            {
                course_box.Items.Add(c);
            }



        }

        private void course_box_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (course_box.SelectedIndex != -1)
            {
                selected_course = selected_plan.courses[course_box.SelectedIndex];

                speed_graph_tmp.Clear(); // for prevent event from timebox
                //train_box.SelectedItem = selected_course.train;
                //route_box.SelectedItem
                train_box.SelectedIndex = 0;
                route_box.SelectedIndex = route_box.FindString(selected_course.route.ToString());
                start_h.Value = selected_course.start_time.Hour;
                start_m.Value = selected_course.start_time.Minute;
                start_s.Value = selected_course.start_time.Second;

                Console.WriteLine(selected_course.route.ToString());

                /*
                speed_graph_tmp.Clear();
                make_speed_tmp.Clear();
                speed_graph_tmp.Add(new speed_point(0, 0, 0));
                make_speed_complete = false;
                make_speed_graph(0, 0, selected_course.route.tracks.Count);
                */

                // init speed sch
                selected_max_speeds.Clear();
                selected_operate_speeds.Clear();
                for (int i = 0; i < selected_course.route.tracks.Count; i++)
                {
                    selected_max_speeds.Add((float)m_speed_percent.Value);
                    selected_operate_speeds.Add((float)o_speed_percent.Value);
                }

                // init dewell sch
                dewell_tmp.Clear();
                dewell_tmp.Add(0);
                for (int i = 1; i < selected_course.route.stop_point.Count - 1; i++)
                {
                    dewell_tmp.Add((float)dewell_box.Value);
                }
                dewell_tmp.Add(0);

                dewell_pos.Clear();
                int tmp_pos = 0;
                for (int i = 0; i < selected_course.route.tracks.Count; i++)
                {
                    if (selected_course.route.tracks[i] == selected_course.route.stop_point[tmp_pos])
                    {
                        dewell_pos.Add(i);
                        tmp_pos++;
                    }
                }

                // find max speed

                speed_graph_tmp = max_speed_graph_tmp;
                selected_speeds = selected_max_speeds;

                speed_graph_tmp.Clear();
                make_speed_tmp.Clear();
                speed_graph_tmp.Add(new speed_point(0, 0, 0));
                make_speed_complete = false;
                make_speed_graph_opt(0, 0, 0, selected_course.route.tracks.Count);
                calculate_point();

                // find operate speed

                speed_graph_tmp = operate_speed_graph_tmp;
                selected_speeds = selected_operate_speeds;

                speed_graph_tmp.Clear();
                make_speed_tmp.Clear();
                speed_graph_tmp.Add(new speed_point(0, 0, 0));
                make_speed_complete = false;
                make_speed_graph_opt(0, 0, 0, selected_course.route.tracks.Count);
                calculate_point();

                pos_graph_hscroll.Value = (int)operate_speed_graph_tmp[0].graphic_x - 20;
                position_h_location = -pos_graph_hscroll.Value;



                speed_draw_req = true;
                speed_graph.Invalidate();
                position_graph.Invalidate();
            }
        }

        public void calculate_point()
        {
            //dewell_gird.Rows.Clear();

            int start_sec = (int)start_s.Value + (int)(start_m.Value * 60) + (int)(start_h.Value * 3600);
            double time_scale = 0.08;
            double position_scale = 0.0014;
            double py;

            selected_accum_length = new List<float>();
            int v_line_loc = 0;
            selected_accum_length.Add(v_line_loc);
            foreach (string t in selected_course.route.tracks)
            {
                int graphic_length;
                graphic_length = (fullscale.tracks[t].legnth / 100);
                v_line_loc += graphic_length;
                selected_accum_length.Add(v_line_loc);
            }

            float acc_time = start_sec;
            float last_time = start_sec;

            speed_graph_tmp[0].length_travel = selected_accum_length[speed_graph_tmp[0].track] + speed_graph_tmp[0].track_travel;
            speed_graph_tmp[0].time_travel = start_sec;

            if (fullscale.tracks[selected_course.route.tracks[speed_graph_tmp[0].track]].distance_start < fullscale.tracks[selected_course.route.tracks[speed_graph_tmp[speed_graph_tmp.Count - 1].track]].distance_start)
            {
                py = fullscale.tracks[selected_course.route.tracks[speed_graph_tmp[0].track]].distance_start + ((speed_graph_tmp[0].track_travel / ((double)fullscale.tracks[selected_course.route.tracks[speed_graph_tmp[0].track]].legnth / 100)) * Math.Abs((double)fullscale.tracks[selected_course.route.tracks[speed_graph_tmp[0].track]].distance_end - (double)fullscale.tracks[selected_course.route.tracks[speed_graph_tmp[0].track]].distance_start));
            }
            else
            {
                py = fullscale.tracks[selected_course.route.tracks[speed_graph_tmp[0].track]].distance_end - ((speed_graph_tmp[0].track_travel / ((double)fullscale.tracks[selected_course.route.tracks[speed_graph_tmp[0].track]].legnth / 100)) * Math.Abs((double)fullscale.tracks[selected_course.route.tracks[speed_graph_tmp[0].track]].distance_end - (double)fullscale.tracks[selected_course.route.tracks[speed_graph_tmp[0].track]].distance_start));
            }

            speed_graph_tmp[0].graphic_x = (1500 + speed_graph_tmp[0].time_travel) * (float)time_scale;
            speed_graph_tmp[0].graphic_y = 80 + (float)py * (float)position_scale;

            int stop_point_cnt = 1;

            for (int i = 1; i < speed_graph_tmp.Count; i++)
            {

                float use_time;

                // add dewell time
                if (stop_point_cnt < selected_course.route.stop_point.Count -1 && selected_course.route.tracks[speed_graph_tmp[i].track] == selected_course.route.stop_point[stop_point_cnt] && speed_graph_tmp[i].speed < 0.01)
                {
                    speed_graph_tmp[i].length_travel = selected_accum_length[speed_graph_tmp[i].track] + speed_graph_tmp[i].track_travel;
                    use_time = (float)(speed_graph_tmp[i].length_travel - speed_graph_tmp[i - 1].length_travel) / ((speed_graph_tmp[i - 1].speed + speed_graph_tmp[i].speed) / 2);
                    acc_time += use_time;
                    speed_graph_tmp[i].time_travel = acc_time;

                    if (fullscale.tracks[selected_course.route.tracks[speed_graph_tmp[0].track]].distance_start < fullscale.tracks[selected_course.route.tracks[speed_graph_tmp[speed_graph_tmp.Count - 1].track]].distance_start)
                    {
                        py = fullscale.tracks[selected_course.route.tracks[speed_graph_tmp[i].track]].distance_start + ((speed_graph_tmp[i].track_travel / ((double)fullscale.tracks[selected_course.route.tracks[speed_graph_tmp[i].track]].legnth / 100)) * Math.Abs((double)fullscale.tracks[selected_course.route.tracks[speed_graph_tmp[i].track]].distance_end - (double)fullscale.tracks[selected_course.route.tracks[speed_graph_tmp[i].track]].distance_start));
                    }
                    else
                    {
                        py = fullscale.tracks[selected_course.route.tracks[speed_graph_tmp[i].track]].distance_end - ((speed_graph_tmp[i].track_travel / ((double)fullscale.tracks[selected_course.route.tracks[speed_graph_tmp[i].track]].legnth / 100)) * Math.Abs((double)fullscale.tracks[selected_course.route.tracks[speed_graph_tmp[i].track]].distance_end - (double)fullscale.tracks[selected_course.route.tracks[speed_graph_tmp[i].track]].distance_start));
                    }

                    speed_graph_tmp[i].graphic_x = (1500 + speed_graph_tmp[i].time_travel) * (float)time_scale;
                    speed_graph_tmp[i].graphic_y = 80 + (float)py * (float)position_scale;

                    speed_graph_tmp.Insert(i + 1, new speed_point(speed_graph_tmp[i].speed, speed_graph_tmp[i].track, speed_graph_tmp[i].track_travel));
                    i++;
                    speed_graph_tmp[i].length_travel = selected_accum_length[speed_graph_tmp[i].track] + speed_graph_tmp[i].track_travel;
                    //acc_time += (float)dewell_box.Value;
                    acc_time += dewell_tmp[stop_point_cnt];
                    speed_graph_tmp[i].time_travel = acc_time;

                    if (fullscale.tracks[selected_course.route.tracks[speed_graph_tmp[0].track]].distance_start < fullscale.tracks[selected_course.route.tracks[speed_graph_tmp[speed_graph_tmp.Count - 1].track]].distance_start)
                    {
                        py = fullscale.tracks[selected_course.route.tracks[speed_graph_tmp[i].track]].distance_start + ((speed_graph_tmp[i].track_travel / ((double)fullscale.tracks[selected_course.route.tracks[speed_graph_tmp[i].track]].legnth / 100)) * Math.Abs((double)fullscale.tracks[selected_course.route.tracks[speed_graph_tmp[i].track]].distance_end - (double)fullscale.tracks[selected_course.route.tracks[speed_graph_tmp[i].track]].distance_start));
                    }
                    else
                    {
                        py = fullscale.tracks[selected_course.route.tracks[speed_graph_tmp[i].track]].distance_end - ((speed_graph_tmp[i].track_travel / ((double)fullscale.tracks[selected_course.route.tracks[speed_graph_tmp[i].track]].legnth / 100)) * Math.Abs((double)fullscale.tracks[selected_course.route.tracks[speed_graph_tmp[i].track]].distance_end - (double)fullscale.tracks[selected_course.route.tracks[speed_graph_tmp[i].track]].distance_start));
                    }

                    speed_graph_tmp[i].graphic_x = (1500 + speed_graph_tmp[i].time_travel) * (float)time_scale;
                    speed_graph_tmp[i].graphic_y = 80 + (float)py * (float)position_scale;
                    // add arr dpt to grid
                    //dewell_gird.Rows.Add(selected_course.route.tracks[speed_graph_tmp[i].track], dewell_box.Value.ToString(), dewell_tmp[stop_point_cnt].ToString(), dewell_tmp[stop_point_cnt].ToString());

                    stop_point_cnt++;
                }
                else
                {
                    speed_graph_tmp[i].length_travel = selected_accum_length[speed_graph_tmp[i].track] + speed_graph_tmp[i].track_travel;
                    use_time = (float)(speed_graph_tmp[i].length_travel - speed_graph_tmp[i - 1].length_travel) / ((speed_graph_tmp[i - 1].speed + speed_graph_tmp[i].speed) / 2);
                    acc_time += use_time;
                    speed_graph_tmp[i].time_travel = acc_time;

                    if (fullscale.tracks[selected_course.route.tracks[speed_graph_tmp[0].track]].distance_start < fullscale.tracks[selected_course.route.tracks[speed_graph_tmp[speed_graph_tmp.Count - 1].track]].distance_start)
                    {
                        py = fullscale.tracks[selected_course.route.tracks[speed_graph_tmp[i].track]].distance_start + ((speed_graph_tmp[i].track_travel / ((double)fullscale.tracks[selected_course.route.tracks[speed_graph_tmp[i].track]].legnth / 100)) * Math.Abs((double)fullscale.tracks[selected_course.route.tracks[speed_graph_tmp[i].track]].distance_end - (double)fullscale.tracks[selected_course.route.tracks[speed_graph_tmp[i].track]].distance_start));
                    }
                    else
                    {
                        py = fullscale.tracks[selected_course.route.tracks[speed_graph_tmp[i].track]].distance_end - ((speed_graph_tmp[i].track_travel / ((double)fullscale.tracks[selected_course.route.tracks[speed_graph_tmp[i].track]].legnth / 100)) * Math.Abs((double)fullscale.tracks[selected_course.route.tracks[speed_graph_tmp[i].track]].distance_end - (double)fullscale.tracks[selected_course.route.tracks[speed_graph_tmp[i].track]].distance_start));
                    }

                    speed_graph_tmp[i].graphic_x = (1500 + speed_graph_tmp[i].time_travel) * (float)time_scale;
                    speed_graph_tmp[i].graphic_y = 80 + (float)py * (float)position_scale;

                }

                last_time = acc_time;
            }


        }

        public void make_speed_graph(int current, float last_speed, int dest)
        {

            if (make_speed_complete == false)
            {
                if (current < dest)
                {
                    string c_track = selected_course.route.tracks[current];
                    int track_cnt = selected_course.route.tracks.Count;
                    float next_speed;
                    float lim_speed = (float)fullscale.tracks[c_track].max_speed * 1000 / 3600 * selected_max_speeds[current] / (float)100.00; // km/h to m/s 
                    float last_lim_speed = (float)fullscale.tracks[c_track].max_speed * 1000 / 3600 * selected_max_speeds[current] / (float)100.00; // km/h to m/s 

                    if (current > 0)
                    {
                        last_lim_speed = (float)fullscale.tracks[selected_course.route.tracks[current - 1]].max_speed * 1000 / 3600 * selected_max_speeds[current] / (float)100.00; // km/h to m/s 
                    }

                    if (last_speed > lim_speed || last_speed > last_lim_speed)
                    {
                        return;
                    }

                    Planning_sch _sch = planning_sch.Find(Planning_sch => Planning_sch.tracks.Contains(c_track));
                    if (_sch == null) // if it's not a station
                    {
                        float remain_length = (float)fullscale.tracks[c_track].legnth / 100; // centemeter to meter                                              
                        float travel_length;

                        //up
                        for (int i = current; i < track_cnt; i++)
                        {
                            next_speed = (float)fullscale.tracks[selected_course.route.tracks[i]].max_speed * 1000 / 3600 * selected_max_speeds[current] / (float)100.00;
                            if (next_speed > last_speed)
                            {
                                travel_length = (next_speed * next_speed - last_speed * last_speed) / (2 * train_acc_up);
                                if (travel_length < remain_length)
                                {
                                    make_speed_tmp.Add(new speed_point(next_speed, current, travel_length));
                                    make_speed_tmp.Add(new speed_point(next_speed, current + 1, 0));
                                    make_speed_graph(current + 1, next_speed, dest);
                                    make_speed_tmp.RemoveAt(make_speed_tmp.Count - 1);
                                    make_speed_tmp.RemoveAt(make_speed_tmp.Count - 1);
                                }
                            }
                        }

                        //no change
                        make_speed_tmp.Add(new speed_point(last_speed, current + 1, 0));
                        make_speed_graph(current + 1, last_speed, dest);
                        make_speed_tmp.RemoveAt(make_speed_tmp.Count - 1);


                        //down
                        for (int i = current; i < track_cnt; i++)
                        {
                            next_speed = (float)fullscale.tracks[selected_course.route.tracks[i]].max_speed * 1000 / 3600 * selected_max_speeds[current] / (float)100.00;
                            if (next_speed < last_speed)
                            {
                                travel_length = (next_speed * next_speed - last_speed * last_speed) / (2 * train_acc_down);
                                if (travel_length < remain_length)
                                {
                                    make_speed_tmp.Add(new speed_point(next_speed, current, travel_length));
                                    make_speed_tmp.Add(new speed_point(next_speed, current + 1, 0));
                                    make_speed_graph(current + 1, next_speed, dest);
                                    make_speed_tmp.RemoveAt(make_speed_tmp.Count - 1);
                                    make_speed_tmp.RemoveAt(make_speed_tmp.Count - 1);
                                }
                            }
                        }

                    }
                    else
                    {
                        make_speed_tmp.Add(new speed_point(0, current, 0));
                        make_speed_tmp.Add(new speed_point(0, current + 1, 0));
                        make_speed_graph(current + 1, 0, dest);
                    }
                }
                else
                {
                    make_speed_complete = true;
                    speed_graph_tmp.Clear();
                    foreach (speed_point _s in make_speed_tmp)
                    {
                        speed_graph_tmp.Add(new speed_point(_s.speed, _s.track, _s.track_travel));
                    }
                }
            }
        }

        public void make_speed_graph_opt(int current, float last_speed, int dir, int dest)
        {

            if (make_speed_complete == false)
            {
                if (current < dest)
                {
                    string c_track = selected_course.route.tracks[current];
                    int track_cnt = selected_course.route.tracks.Count;
                    float max_speed = (float)fullscale.tracks[c_track].max_speed * 1000 / 3600;
                    float lim_speed = (float)fullscale.tracks[c_track].max_speed * 1000 / 3600 * selected_speeds[current] / (float)100.00; // km/h to m/s 
                    float last_lim_speed = (float)fullscale.tracks[c_track].max_speed * 1000 / 3600 * selected_speeds[current] / (float)100.00; // km/h to m/s 
                    float next_lim_speed = (float)fullscale.tracks[selected_course.route.tracks[current]].max_speed * 1000 / 3600; // km/h to m/s 

                    float next_speed;
                    float next_speed_rec = (float)0.00;
                    float hold_speed;
                    float between_speed = (float)0.00;
                    float between_speed_rec = (float)0.00;

                    if (current > 0)
                    {
                        last_lim_speed = (float)fullscale.tracks[selected_course.route.tracks[current - 1]].max_speed * 1000 / 3600; // km/h to m/s 
                    }
                    if (current + 1 < selected_course.route.tracks.Count)
                    {
                        next_lim_speed = (float)fullscale.tracks[selected_course.route.tracks[current + 1]].max_speed * 1000 / 3600; // km/h to m/s 
                    }
                    float remain_length = (float)fullscale.tracks[c_track].legnth / 100; // centemeter to meter   

                    //Planning_sch _sch = planning_sch.Find(Planning_sch => Planning_sch.tracks.Contains(c_track));
                    //if (_sch == null) // if it's not a station
                    if (!selected_course.route.stop_point.Contains(c_track))
                    {

                        if ((dir == 1 && last_speed == max_speed) || last_speed > max_speed)
                        {
                            return;
                        }

                        if (c_track == "T3012")
                        {
                            Console.Write(" ");
                        }


                        float travel_length;
                        float hold_length;
                        float hold_length_rec = (float)0.00;
                        float between_length = (float)0.00; ;
                        float between_length_rec = (float)0.00;
                        //float acum_length = (float)0.00;

                        int i = current;
                        //for (int i = current; i < track_cnt; i++)
                        {
                            //Console.Write((i-current).ToString()+" ");

                            hold_speed = (float)fullscale.tracks[selected_course.route.tracks[i]].max_speed * 1000 / 3600 * selected_speeds[current] / (float)100.00;

                            if (/*lim_speed > last_lim_speed*//*lim_speed > last_speed*/hold_speed > last_speed)
                            {   // up
                                travel_length = (hold_speed * hold_speed - last_speed * last_speed) / (2 * train_acc_up);
                            }
                            else
                            {   // down
                                travel_length = (hold_speed * hold_speed - last_speed * last_speed) / (2 * train_acc_down);
                            }

                            // next track is station or end ?
                            if (i + 1 >= track_cnt || selected_course.route.stop_point.Contains(selected_course.route.tracks[i + 1]))
                            {
                                next_speed = 0;
                                hold_length = remain_length - ((next_speed * next_speed - hold_speed * hold_speed) / (2 * train_acc_up));
                                if (last_speed - 0.0001 > lim_speed)
                                {
                                    return;
                                }
                                else
                                {
                                    make_speed_tmp.Add(new speed_point(last_speed, current + 1, 0));
                                    make_speed_graph_opt(current + 1, last_speed, 0, dest);
                                    make_speed_tmp.RemoveAt(make_speed_tmp.Count - 1);
                                }
                            }
                            else if (next_lim_speed > lim_speed)
                            {
                                next_speed = (float)fullscale.tracks[selected_course.route.tracks[i]].max_speed * 1000 / 3600;
                                next_speed_rec = next_speed * selected_speeds[current] / (float)100.00;
                                hold_length = remain_length - ((next_speed * next_speed - hold_speed * hold_speed) / (2 * train_acc_up));
                                hold_length_rec = remain_length - ((next_speed_rec * next_speed_rec - hold_speed * hold_speed) / (2 * train_acc_up));

                            }
                            else
                            {
                                next_speed = (float)fullscale.tracks[selected_course.route.tracks[i + 1]].max_speed * 1000 / 3600;
                                next_speed_rec = next_speed * selected_speeds[current] / (float)100.00;
                                hold_length = remain_length - ((next_speed * next_speed - hold_speed * hold_speed) / (2 * train_acc_down));
                                hold_length_rec = remain_length - ((next_speed_rec * next_speed_rec - hold_speed * hold_speed) / (2 * train_acc_down));
                            }

                            if ((Math.Abs(lim_speed - last_speed) < 0.001) && hold_length < remain_length && next_lim_speed > lim_speed)
                            {
                                //hold and up
                                make_speed_tmp.Add(new speed_point(hold_speed, current, hold_length));
                                make_speed_tmp.Add(new speed_point(next_speed, current + 1, 0));
                                make_speed_graph_opt(current + 1, next_speed, 1, dest);
                                make_speed_tmp.RemoveAt(make_speed_tmp.Count - 1);
                                make_speed_tmp.RemoveAt(make_speed_tmp.Count - 1);
                            }

                            // up and up
                            if (next_lim_speed > lim_speed && travel_length < hold_length && dir != -1)
                            {
                                make_speed_tmp.Add(new speed_point(hold_speed, current, travel_length));
                                make_speed_tmp.Add(new speed_point(hold_speed, current, hold_length));
                                make_speed_tmp.Add(new speed_point(next_speed, current + 1, 0));
                                make_speed_graph_opt(current + 1, next_speed, 1, dest);
                                make_speed_tmp.RemoveAt(make_speed_tmp.Count - 1);
                                make_speed_tmp.RemoveAt(make_speed_tmp.Count - 1);
                                make_speed_tmp.RemoveAt(make_speed_tmp.Count - 1);
                            }

                            //up and hold
                            if (travel_length < remain_length && dir != -1)
                            {
                                make_speed_tmp.Add(new speed_point(hold_speed, current, travel_length));
                                make_speed_tmp.Add(new speed_point(hold_speed, current + 1, 0));
                                make_speed_graph_opt(current + 1, hold_speed, 0, dest);
                                make_speed_tmp.RemoveAt(make_speed_tmp.Count - 1);
                                make_speed_tmp.RemoveAt(make_speed_tmp.Count - 1);
                            }

                            // up and down to top                    
                            if (next_lim_speed < lim_speed && travel_length < hold_length && dir != -1)
                            {
                                make_speed_tmp.Add(new speed_point(hold_speed, current, travel_length));
                                make_speed_tmp.Add(new speed_point(hold_speed, current, hold_length));
                                make_speed_tmp.Add(new speed_point(next_speed, current + 1, 0));
                                make_speed_graph_opt(current + 1, next_speed, -1, dest);
                                make_speed_tmp.RemoveAt(make_speed_tmp.Count - 1);
                                make_speed_tmp.RemoveAt(make_speed_tmp.Count - 1);
                                make_speed_tmp.RemoveAt(make_speed_tmp.Count - 1);
                            }

                            // up and down to top imp  

                            if (travel_length >= hold_length && dir != -1)
                            {
                                between_speed = (float)Math.Sqrt((((2 * train_acc_up * train_acc_down * remain_length) + (train_acc_down * last_speed * last_speed) - (train_acc_up * next_speed * next_speed)) / (train_acc_down - train_acc_up)));
                                between_length = ((between_speed * between_speed) - (last_speed * last_speed)) / (2 * train_acc_up);

                                make_speed_tmp.Add(new speed_point(between_speed, current, between_length));
                                make_speed_tmp.Add(new speed_point(next_speed, current + 1, 0));
                                make_speed_graph_opt(current + 1, next_speed, -1, dest);
                                make_speed_tmp.RemoveAt(make_speed_tmp.Count - 1);
                                make_speed_tmp.RemoveAt(make_speed_tmp.Count - 1);
                            }


                            // up and down to rec
                            if (next_speed_rec < lim_speed && travel_length < hold_length_rec && dir != -1)
                            {
                                make_speed_tmp.Add(new speed_point(hold_speed, current, travel_length));
                                make_speed_tmp.Add(new speed_point(hold_speed, current, hold_length_rec));
                                make_speed_tmp.Add(new speed_point(next_speed_rec, current + 1, 0));
                                make_speed_graph_opt(current + 1, next_speed_rec, 0, dest);
                                make_speed_tmp.RemoveAt(make_speed_tmp.Count - 1);
                                make_speed_tmp.RemoveAt(make_speed_tmp.Count - 1);
                                make_speed_tmp.RemoveAt(make_speed_tmp.Count - 1);
                            }

                            // up and down to rec imp
                            if (travel_length >= hold_length_rec && dir != -1)
                            {

                                between_speed_rec = (float)Math.Sqrt((((2 * train_acc_up * train_acc_down * remain_length) + (train_acc_down * last_speed * last_speed) - (train_acc_up * next_speed_rec * next_speed_rec)) / (train_acc_down - train_acc_up)));
                                between_length_rec = ((between_speed_rec * between_speed_rec) - (last_speed * last_speed)) / (2 * train_acc_up);

                                make_speed_tmp.Add(new speed_point(between_speed_rec, current, between_length_rec));
                                make_speed_tmp.Add(new speed_point(next_speed_rec, current + 1, 0));
                                make_speed_graph_opt(current + 1, next_speed_rec, 0, dest);
                                make_speed_tmp.RemoveAt(make_speed_tmp.Count - 1);
                                make_speed_tmp.RemoveAt(make_speed_tmp.Count - 1);
                            }

                            // hold and hold
                            if (last_speed <= lim_speed)
                            {
                                make_speed_tmp.Add(new speed_point(last_speed, current + 1, 0));
                                make_speed_graph_opt(current + 1, last_speed, 0, dest);
                                make_speed_tmp.RemoveAt(make_speed_tmp.Count - 1);
                            }

                            if ((Math.Abs(lim_speed - last_speed) < 0.001) && hold_length < remain_length && next_speed < lim_speed)
                            {
                                //hold and down to top
                                make_speed_tmp.Add(new speed_point(hold_speed, current, hold_length));
                                make_speed_tmp.Add(new speed_point(next_speed, current + 1, 0));
                                make_speed_graph_opt(current + 1, next_speed, -1, dest);
                                make_speed_tmp.RemoveAt(make_speed_tmp.Count - 1);
                                make_speed_tmp.RemoveAt(make_speed_tmp.Count - 1);
                            }

                            if ((Math.Abs(lim_speed - last_speed) < 0.001) && hold_length_rec < remain_length && next_speed_rec < lim_speed)
                            {
                                //hold and down to rec
                                make_speed_tmp.Add(new speed_point(hold_speed, current, hold_length_rec));
                                make_speed_tmp.Add(new speed_point(next_speed_rec, current + 1, 0));
                                make_speed_graph_opt(current + 1, next_speed_rec, -1, dest);
                                make_speed_tmp.RemoveAt(make_speed_tmp.Count - 1);
                                make_speed_tmp.RemoveAt(make_speed_tmp.Count - 1);
                            }

                            if (max_speed >= last_speed)
                            {
                                // down and up to top

                                if (next_lim_speed > lim_speed && travel_length < hold_length && dir != 1)
                                {
                                    make_speed_tmp.Add(new speed_point(hold_speed, current, travel_length));
                                    make_speed_tmp.Add(new speed_point(hold_speed, current, hold_length));
                                    make_speed_tmp.Add(new speed_point(next_speed, current + 1, 0));
                                    make_speed_graph_opt(current + 1, next_speed, 1, dest);
                                    make_speed_tmp.RemoveAt(make_speed_tmp.Count - 1);
                                    make_speed_tmp.RemoveAt(make_speed_tmp.Count - 1);
                                    make_speed_tmp.RemoveAt(make_speed_tmp.Count - 1);
                                }

                                // down and up to rec

                                if (next_lim_speed > lim_speed && travel_length < hold_length_rec && dir != 1)
                                {
                                    make_speed_tmp.Add(new speed_point(hold_speed, current, travel_length));
                                    make_speed_tmp.Add(new speed_point(hold_speed, current, hold_length_rec));
                                    make_speed_tmp.Add(new speed_point(next_speed_rec, current + 1, 0));
                                    make_speed_graph_opt(current + 1, next_speed_rec, 0, dest);
                                    make_speed_tmp.RemoveAt(make_speed_tmp.Count - 1);
                                    make_speed_tmp.RemoveAt(make_speed_tmp.Count - 1);
                                    make_speed_tmp.RemoveAt(make_speed_tmp.Count - 1);
                                }

                                //down and hold
                                if (travel_length < remain_length && dir != 1)
                                {
                                    make_speed_tmp.Add(new speed_point(hold_speed, current, travel_length));
                                    make_speed_tmp.Add(new speed_point(hold_speed, current + 1, 0));
                                    make_speed_graph_opt(current + 1, hold_speed, 0, dest);
                                    make_speed_tmp.RemoveAt(make_speed_tmp.Count - 1);
                                    make_speed_tmp.RemoveAt(make_speed_tmp.Count - 1);
                                }

                                // down and down to top
                                if (next_lim_speed < lim_speed && travel_length < hold_length && dir != 1)
                                {
                                    make_speed_tmp.Add(new speed_point(hold_speed, current, travel_length));
                                    make_speed_tmp.Add(new speed_point(hold_speed, current, hold_length));
                                    make_speed_tmp.Add(new speed_point(next_speed, current + 1, 0));
                                    make_speed_graph_opt(current + 1, next_speed, -1, dest);
                                    make_speed_tmp.RemoveAt(make_speed_tmp.Count - 1);
                                    make_speed_tmp.RemoveAt(make_speed_tmp.Count - 1);
                                    make_speed_tmp.RemoveAt(make_speed_tmp.Count - 1);
                                }

                                // down and down to rec
                                if (next_lim_speed < lim_speed && travel_length < hold_length_rec && dir != 1)
                                {
                                    make_speed_tmp.Add(new speed_point(hold_speed, current, travel_length));
                                    make_speed_tmp.Add(new speed_point(hold_speed, current, hold_length_rec));
                                    make_speed_tmp.Add(new speed_point(next_speed_rec, current + 1, 0));
                                    make_speed_graph_opt(current + 1, next_speed_rec, 0, dest);
                                    make_speed_tmp.RemoveAt(make_speed_tmp.Count - 1);
                                    make_speed_tmp.RemoveAt(make_speed_tmp.Count - 1);
                                    make_speed_tmp.RemoveAt(make_speed_tmp.Count - 1);
                                }

                            }

                        }
                    }
                    else
                    {
                        next_speed = (float)fullscale.tracks[selected_course.route.tracks[current]].max_speed * 1000 / 3600;
                        next_speed_rec = next_speed * selected_speeds[current] / (float)100.00;

                        make_speed_tmp.Add(new speed_point(0, current, remain_length / 2));
                        //make_speed_tmp.Add(new speed_point(0, current, remain_length / 2));
                        make_speed_tmp.Add(new speed_point(next_speed_rec, current + 1, 0));



                        make_speed_graph_opt(current + 1, 0, 0, dest);

                        //make_speed_tmp.RemoveAt(make_speed_tmp.Count - 1);
                        make_speed_tmp.RemoveAt(make_speed_tmp.Count - 1);
                        make_speed_tmp.RemoveAt(make_speed_tmp.Count - 1);
                    }
                }
                else
                {
                    make_speed_complete = true;
                    speed_graph_tmp.Clear();
                    foreach (speed_point _s in make_speed_tmp)
                    {
                        speed_graph_tmp.Add(new speed_point(_s.speed, _s.track, _s.track_travel));
                    }
                    speed_graph_tmp.RemoveAt(speed_graph_tmp.Count - 1);
                }
            }
        }

        private void speed_graph_Paint(object sender, PaintEventArgs e)
        {
            if (speed_draw_req == true && (course_box.SelectedIndex >= 0 || editing_flag == true))
            {

                e.Graphics.TranslateTransform(speed_h_location, 0);
                e.Graphics.ScaleTransform((float)1.0, (float)1.0);

                Graphics g = e.Graphics;

                Pen bg_pen = new Pen(Color.Gray, 2);
                Pen m_pen = new Pen(Color.Pink, 2);
                Pen b_pen = new Pen(Color.LightBlue, 1);
                Pen r_pen = new Pen(Color.LightGray, 1);
                Pen g_pen = new Pen(Color.LightGreen, (float)1.5);
                Pen o_pen = new Pen(Color.LightBlue, (float)1.5);
                r_pen.DashPattern = new float[] { 3.0F, 1.0F };
                b_pen.DashPattern = new float[] { 3.0F, 1.0F };

                Font _f = new Font("Arial", 10);
                Font _lf = new Font("Arial", 7);
                StringFormat _sf = new StringFormat();
                _sf.LineAlignment = StringAlignment.Center;
                _sf.Alignment = StringAlignment.Center;

                StringFormat _sr = new StringFormat();
                _sr.LineAlignment = StringAlignment.Center;
                _sr.Alignment = StringAlignment.Far;

                List<float> accum_length = new List<float>();
                float v_line_loc = 0;
                accum_length.Add(v_line_loc);
                int track_cnt = 0;

                g.DrawLine(bg_pen, 30, 170, 45, 170);
                g.DrawLine(bg_pen, 30, 170 - 50, 45, 170 - 50);
                g.DrawLine(bg_pen, 30, 170 - 100, 45, 170 - 100);
                g.DrawLine(bg_pen, 30, 170 - 150, 45, 170 - 150);
                e.Graphics.DrawString("0", _f, Brushes.Orange, new Point(25, 170), _sr);
                e.Graphics.DrawString("100", _f, Brushes.Orange, new Point(25, 170 - 50), _sr);
                e.Graphics.DrawString("200", _f, Brushes.Orange, new Point(25, 170 - 100), _sr);
                e.Graphics.DrawString("300", _f, Brushes.Orange, new Point(25, 170 - 150), _sr);

                g.DrawLine(bg_pen, 50, 10, 50, 170);

                Service_Course selected_course;

                if (editing_flag == false)
                {
                    selected_course = selected_plan.courses[course_box.SelectedIndex];
                }
                else
                {
                    selected_course = timed_data[editing_index];
                }

                // draw selected v-s

                foreach (string t in selected_course.route.tracks)
                {
                    Planning_sch _sch = planning_sch.Find(Planning_sch => Planning_sch.tracks.Contains(t));
                    float graphic_length;

                    if (_sch == null)
                    { // normal track
                        graphic_length = (fullscale.tracks[t].legnth / 100);
                        bg_pen = new Pen(Color.Gray, 2);
                        //Console.WriteLine(graphic_length.ToString() + "," + t);
                    }
                    else
                    { // if it is station use dewll time
                        graphic_length = (fullscale.tracks[t].legnth / 100);
                        //bg_pen = new Pen(Color.LightGray, 2);
                    }

                    v_line_loc += graphic_length;
                    accum_length.Add(v_line_loc);

                    // draw selected segment
                    if (speed_graph_edit_hover == track_cnt)
                    {
                        g.FillRectangle(new SolidBrush(Color.FromArgb(32, 255, 255, 255)), 50 + (v_line_loc - graphic_length) / graphic_scale, 170 - 160, graphic_length / graphic_scale - 1, 150);
                    }

                    g.DrawLine(bg_pen, 50 + v_line_loc / graphic_scale, 10, 50 + v_line_loc / graphic_scale, 170);
                    g.DrawLine(bg_pen, 50 + ((v_line_loc - graphic_length) / graphic_scale) + 2, 170, 50 + (v_line_loc / graphic_scale) - 2, 170);

                    if (track_cnt % 2 == 0)
                    {
                        e.Graphics.DrawString(t, _lf, Brushes.Orange, new PointF(50 + (v_line_loc - (graphic_length / 2)) / graphic_scale, 180), _sf);
                    }
                    else
                    {
                        e.Graphics.DrawString(t, _lf, Brushes.Orange, new PointF(50 + (v_line_loc - (graphic_length / 2)) / graphic_scale, 190), _sf);
                    }

                    // text speed
                    e.Graphics.DrawString(fullscale.tracks[t].max_speed.ToString(), _lf, Brushes.LightPink, new PointF(50 + (v_line_loc - graphic_length / 2) / graphic_scale, 170 - (fullscale.tracks[t].max_speed / 2) - 32), _sf);
                    e.Graphics.DrawString((fullscale.tracks[t].max_speed * selected_max_speeds[track_cnt] / (float)100.00).ToString(), _lf, Brushes.LightGray, new PointF(50 + (v_line_loc - graphic_length / 2) / graphic_scale, 170 - (fullscale.tracks[t].max_speed / 2) - 21), _sf);
                    e.Graphics.DrawString((fullscale.tracks[t].max_speed * selected_operate_speeds[track_cnt] / (float)100.00).ToString(), _lf, Brushes.LightBlue, new PointF(50 + (v_line_loc - graphic_length / 2) / graphic_scale, 170 - (fullscale.tracks[t].max_speed / 2) - 10), _sf);

                    // max civil speed
                    g.DrawLine(m_pen, 50 + (v_line_loc - graphic_length) / graphic_scale, 170 - (fullscale.tracks[t].max_speed / 2), 50 + v_line_loc / graphic_scale, 170 - (fullscale.tracks[t].max_speed / 2));
                    // max speed
                    g.DrawLine(r_pen, 50 + (v_line_loc - graphic_length) / graphic_scale, 170 - ((fullscale.tracks[t].max_speed * selected_max_speeds[track_cnt] / (float)100.00) / 2), 50 + v_line_loc / graphic_scale, 170 - ((fullscale.tracks[t].max_speed * (selected_max_speeds[track_cnt] / 100)) / 2));
                    // operate speed
                    g.DrawLine(b_pen, 50 + (v_line_loc - graphic_length) / graphic_scale, 170 - ((fullscale.tracks[t].max_speed * selected_operate_speeds[track_cnt] / (float)100.00) / 2), 50 + v_line_loc / graphic_scale, 170 - ((fullscale.tracks[t].max_speed * (selected_operate_speeds[track_cnt] / 100)) / 2));


                    track_cnt++;
                }

                // draw max & operate speed line
                for (int i = 1; i < max_speed_graph_tmp.Count; i++)
                {
                    g.DrawLine(g_pen, 50 + max_speed_graph_tmp[i - 1].length_travel / graphic_scale, 170 - (max_speed_graph_tmp[i - 1].speed / 2000 * 3600), 50 + max_speed_graph_tmp[i].length_travel / graphic_scale, 170 - (max_speed_graph_tmp[i].speed / 2000 * 3600));
                }

                for (int i = 1; i < operate_speed_graph_tmp.Count; i++)
                {
                    g.DrawLine(o_pen, 50 + operate_speed_graph_tmp[i - 1].length_travel / graphic_scale, 170 - (operate_speed_graph_tmp[i - 1].speed / 2000 * 3600), 50 + operate_speed_graph_tmp[i].length_travel / graphic_scale, 170 - (operate_speed_graph_tmp[i].speed / 2000 * 3600));
                }

                // v=s/t dafaq
                /*
                float remain_length = (float)fullscale.tracks["T0001"].legnth / 1000; // milimeter to meter
                float next_speed = (float)fullscale.tracks["T0001"].max_speed * 1000 / 3600 * (float)o_speed_percent.Value / (float)100.00; // km/h to m/s 
                float train_acc = (float)2.00;
                float travel_length;
                float current_speed = (float)0.00;

                travel_length = (next_speed * next_speed - current_speed * current_speed) / (2 * train_acc);

                g.DrawLine(g_pen, 150, 170,150 + travel_length/graphic_scale, 170 - (next_speed / 2000 * 3600));
                */
            }


        }

        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            speed_h_location = -hScrollBar1.Value;
            speed_graph.Invalidate();
        }

        private void position_graph_Paint(object sender, PaintEventArgs e)
        {
            int i;
            Graphics g = e.Graphics;

            e.Graphics.TranslateTransform(position_h_location, 0);

            double time_scale = 0.08;
            double position_scale = 0.0014;

            Pen bg_pen = new Pen(Color.Gray, 2);
            Pen m_pen = new Pen(Color.Pink, 2);
            Pen r_pen = new Pen(Color.LightGray, 1);
            Pen g_pen = new Pen(Color.LightGreen, 1);

            Pen editing_pen = new Pen(Color.LightBlue, 1);
            editing_pen.DashPattern = new float[] { 3.0F, 1.0F };

            Font _f = new Font("Arial", 10);
            StringFormat _sf = new StringFormat();
            _sf.LineAlignment = StringAlignment.Center;
            _sf.Alignment = StringAlignment.Center;

            StringFormat _sr = new StringFormat();
            _sr.LineAlignment = StringAlignment.Center;
            _sr.Alignment = StringAlignment.Far;

            // draw sch (y)
            i = 0;
            foreach (Planning_sch p in planning_sch)
            {
                float tmp_y = 80 + station_distance[i] * (float)position_scale;
                g.DrawLine(bg_pen, 100, tmp_y, 6950, tmp_y);
                e.Graphics.DrawString(p.station_name, _f, Brushes.Orange, new PointF(80, tmp_y), _sf);
                i++;
            }

            // draw time (x)
            i = 0;
            for (float t = 0; t < 86400.00; t += (float)1800.00)
            {
                float tmp_x = (float)120.00 + t * (float)time_scale;
                string tmp_time = ((int)t / 3600).ToString("00") + ":" + (((int)t % 3600) / 60).ToString("00");
                e.Graphics.DrawString(tmp_time, _f, Brushes.Orange, new PointF(tmp_x, 60), _sf);
                g.DrawLine(bg_pen, tmp_x, 60, tmp_x, 470);
            }

            //draw selected s-t
            if (operate_speed_graph_tmp.Count > 0)
            {
                if (fullscale.tracks[selected_course.route.tracks[operate_speed_graph_tmp[0].track]].distance_start < fullscale.tracks[selected_course.route.tracks[operate_speed_graph_tmp[operate_speed_graph_tmp.Count - 1].track]].distance_start)
                {
                    g.DrawString(selected_course.name, _f, Brushes.LightGreen, operate_speed_graph_tmp[0].graphic_x - 20, operate_speed_graph_tmp[0].graphic_y - 20);
                }
                else
                {
                    g.DrawString(selected_course.name, _f, Brushes.LightGreen, operate_speed_graph_tmp[0].graphic_x - 20, operate_speed_graph_tmp[0].graphic_y + 10);
                }
                for (i = 1; i < operate_speed_graph_tmp.Count; i++)
                {

                    g.DrawLine(g_pen, operate_speed_graph_tmp[i - 1].graphic_x, operate_speed_graph_tmp[i - 1].graphic_y, operate_speed_graph_tmp[i].graphic_x, operate_speed_graph_tmp[i].graphic_y);

                }
            }

            //draw all operate s-t
            g_pen = new Pen(Color.Orange, 1);

            int j = 0;
            foreach (timed_course draw_course in timed_data)
            {
                // draw course name
                if (fullscale.tracks[draw_course.route.tracks[draw_course.operate_speed_points[0].track]].distance_start < fullscale.tracks[draw_course.route.tracks[draw_course.operate_speed_points[draw_course.operate_speed_points.Count - 1].track]].distance_start)
                {
                    g.DrawString(draw_course.name, _f, Brushes.Orange, draw_course.operate_speed_points[0].graphic_x - 20, draw_course.operate_speed_points[0].graphic_y - 20);
                }
                else
                {
                    g.DrawString(draw_course.name, _f, Brushes.Orange, draw_course.operate_speed_points[0].graphic_x - 20, draw_course.operate_speed_points[0].graphic_y + 20);
                }
                // draw graph
                for (i = 1; i < draw_course.operate_speed_points.Count; i++)
                {
                    if (j == editing_index && editing_flag == true)
                    {
                        g.DrawLine(editing_pen, draw_course.operate_speed_points[i - 1].graphic_x, draw_course.operate_speed_points[i - 1].graphic_y, draw_course.operate_speed_points[i].graphic_x, draw_course.operate_speed_points[i].graphic_y);
                    }
                    else
                    {
                        g.DrawLine(g_pen, draw_course.operate_speed_points[i - 1].graphic_x, draw_course.operate_speed_points[i - 1].graphic_y, draw_course.operate_speed_points[i].graphic_x, draw_course.operate_speed_points[i].graphic_y);
                    }

                }
                // draw connect line (if have)
                if (draw_course.next_course != -1)
                {
                    if (draw_course.operate_speed_points[0].length_travel < draw_course.operate_speed_points[draw_course.operate_speed_points.Count - 1].length_travel)
                    {
                        //g.DrawLine(g_pen, draw_course.operate_speed_points[draw_course.operate_speed_points.Count - 1].graphic_x, draw_course.operate_speed_points[draw_course.operate_speed_points.Count - 1].graphic_y, draw_course.operate_speed_points[draw_course.operate_speed_points.Count - 1].graphic_x, draw_course.operate_speed_points[draw_course.operate_speed_points.Count - 1].graphic_y + 20);
                        g.DrawLine(g_pen, draw_course.operate_speed_points[draw_course.operate_speed_points.Count - 1].graphic_x, draw_course.operate_speed_points[draw_course.operate_speed_points.Count - 1].graphic_y, timed_data[draw_course.next_course].operate_speed_points[0].graphic_x, draw_course.operate_speed_points[draw_course.operate_speed_points.Count - 1].graphic_y);
                        //g.DrawLine(g_pen, timed_data[draw_course.next_course].operate_speed_points[0].graphic_x, draw_course.operate_speed_points[draw_course.operate_speed_points.Count - 1].graphic_y , timed_data[draw_course.next_course].operate_speed_points[0].graphic_x, timed_data[draw_course.next_course].operate_speed_points[0].graphic_y);
                    }
                }

                j++;
            }

            //draw mouse interect
            // draw selected point
            if (pos_graph_point_hover != null)
            {
                if (pos_graph_point_distance < 2000)
                {
                    g.FillEllipse(Brushes.Aqua, pos_graph_point_hover.graphic_x - 3, pos_graph_point_hover.graphic_y - 3, 6, 6);
                    g.DrawString("time = " + ((int)pos_graph_point_hover.time_travel / 3600).ToString("00") + ":" + (((int)pos_graph_point_hover.time_travel % 3600) / 60).ToString("00") + ":" + ((int)pos_graph_point_hover.time_travel % 60).ToString("00"), _f, Brushes.Yellow, pos_graph_point_hover.graphic_x + 10, pos_graph_point_hover.graphic_y - 10);
                    g.DrawString("speed = " + (int)pos_graph_point_hover.speed * 3600 / 1000 + " K/h", _f, Brushes.Yellow, pos_graph_point_hover.graphic_x + 10, pos_graph_point_hover.graphic_y - 24);
                    g.DrawString("track = " + pos_graph_course_hover.route.tracks[pos_graph_point_hover.track], _f, Brushes.Yellow, pos_graph_point_hover.graphic_x + 10, pos_graph_point_hover.graphic_y - 38);
                    g.DrawString("trip = " + pos_graph_course_hover.name, _f, Brushes.Yellow, pos_graph_point_hover.graphic_x + 10, pos_graph_point_hover.graphic_y - 52);
                }
            }

            // draw connect mode
            if (connect_mode == true)
            {
                // draw connect point (first and last)



                foreach (timed_course trip in timed_data)
                {
                    g.FillEllipse(Brushes.Yellow, trip.operate_speed_points[0].graphic_x - 2, trip.operate_speed_points[0].graphic_y - 2, 4, 4);
                    g.FillEllipse(Brushes.Yellow, trip.operate_speed_points[trip.operate_speed_points.Count - 1].graphic_x - 2, trip.operate_speed_points[trip.operate_speed_points.Count - 1].graphic_y - 2, 4, 4);
                    g.DrawEllipse(Pens.Yellow, trip.operate_speed_points[0].graphic_x - 6, trip.operate_speed_points[0].graphic_y - 6, 12, 12);
                    g.DrawEllipse(Pens.Yellow, trip.operate_speed_points[trip.operate_speed_points.Count - 1].graphic_x - 6, trip.operate_speed_points[trip.operate_speed_points.Count - 1].graphic_y - 6, 12, 12);
                }

                if (connect_trip_index > -1)
                {
                    g.FillEllipse(Brushes.OrangeRed, timed_data[connect_trip_index].operate_speed_points[connect_point_index].graphic_x - 2, timed_data[connect_trip_index].operate_speed_points[connect_point_index].graphic_y - 2, 4, 4);
                    g.DrawEllipse(Pens.OrangeRed, timed_data[connect_trip_index].operate_speed_points[connect_point_index].graphic_x - 6, timed_data[connect_trip_index].operate_speed_points[connect_point_index].graphic_y - 6, 12, 12);

                }

            }

            // draw conflict trip
            if (timed_data.Count > 1)
            {
                timed_data.Add(new timed_course(selected_course));
                timed_data[timed_data.Count - 1].start_sec = (int)start_s.Value + (int)(start_m.Value * 60) + (int)(start_h.Value * 3600);
                timed_data[timed_data.Count - 1].operate_speed_points.Clear();
                timed_data[timed_data.Count - 1].operate_speed_points = new List<speed_point>(operate_speed_graph_tmp);
                timed_data[timed_data.Count - 1].max_speed_points.Clear();
                timed_data[timed_data.Count - 1].max_speed_points = new List<speed_point>(max_speed_graph_tmp);

                timed_data[timed_data.Count - 1].max_speeds = new List<float>(selected_max_speeds);
                timed_data[timed_data.Count - 1].operate_speeds = new List<float>(selected_operate_speeds);

                timed_data[timed_data.Count - 1].dewell_pos = new List<int>(dewell_pos);
                timed_data[timed_data.Count - 1].dewell_time = new List<float>(dewell_tmp);
                foreach (conflict_trip c in conflict_trips)
                {
                    // draw rectangle
                    //if()
                    float x1 = (timed_data[c.trip_index[0]].operate_speed_points[c.start_point_index[0]].graphic_x < timed_data[c.trip_index[1]].operate_speed_points[c.start_point_index[1]].graphic_x) ? timed_data[c.trip_index[0]].operate_speed_points[c.start_point_index[0]].graphic_x : timed_data[c.trip_index[1]].operate_speed_points[c.start_point_index[1]].graphic_x;
                    float x2 = (timed_data[c.trip_index[0]].operate_speed_points[c.end_point_index[0]].graphic_x > timed_data[c.trip_index[1]].operate_speed_points[c.end_point_index[1]].graphic_x) ? timed_data[c.trip_index[0]].operate_speed_points[c.end_point_index[0]].graphic_x : timed_data[c.trip_index[1]].operate_speed_points[c.end_point_index[1]].graphic_x;
                    float y1 = (timed_data[c.trip_index[0]].operate_speed_points[c.start_point_index[0]].graphic_y < timed_data[c.trip_index[1]].operate_speed_points[c.start_point_index[1]].graphic_y) ? timed_data[c.trip_index[0]].operate_speed_points[c.start_point_index[0]].graphic_y : timed_data[c.trip_index[1]].operate_speed_points[c.start_point_index[1]].graphic_y;
                    float y2 = (timed_data[c.trip_index[0]].operate_speed_points[c.end_point_index[0]].graphic_y > timed_data[c.trip_index[1]].operate_speed_points[c.end_point_index[1]].graphic_y) ? timed_data[c.trip_index[0]].operate_speed_points[c.end_point_index[0]].graphic_y : timed_data[c.trip_index[1]].operate_speed_points[c.end_point_index[1]].graphic_y;

                    g.DrawRectangle(Pens.OrangeRed, x1, y1, x2 - x1, y2 - y1);

                }
                timed_data.RemoveAt(timed_data.Count - 1);
            }

        }

        private void hScrollBar2_Scroll(object sender, ScrollEventArgs e)
        {
            position_h_location = -pos_graph_hscroll.Value;
            position_graph.Invalidate();
        }

        private void o_speed_percent_ValueChanged(object sender, EventArgs e)
        {
            speed_graph.Invalidate();
        }

        private void start_h_ValueChanged(object sender, EventArgs e)
        {
            if ((adding_flag == true && speed_graph_tmp.Count > 1) || editing_flag == true)
            {
                refresh_selected_speed();
            }
        }

        private void start_m_ValueChanged(object sender, EventArgs e)
        {
            if ((adding_flag == true && speed_graph_tmp.Count > 1) || editing_flag == true)
            {
                refresh_selected_speed();
            }
        }

        private void start_s_ValueChanged(object sender, EventArgs e)
        {
            if ((adding_flag == true && speed_graph_tmp.Count > 1) || editing_flag == true)
            {
                refresh_selected_speed();
            }
        }

        public void refresh_selected_speed()
        {

            // find max speed

            speed_graph_tmp = max_speed_graph_tmp;
            selected_speeds = selected_max_speeds;

            speed_graph_tmp.Clear();
            make_speed_tmp.Clear();
            speed_graph_tmp.Add(new speed_point(0, 0, 0));
            make_speed_complete = false;
            make_speed_graph_opt(0, 0, 0, selected_course.route.tracks.Count);
            calculate_point();

            // find operate speed

            speed_graph_tmp = operate_speed_graph_tmp;
            selected_speeds = selected_operate_speeds;

            speed_graph_tmp.Clear();
            make_speed_tmp.Clear();
            speed_graph_tmp.Add(new speed_point(0, 0, 0));
            make_speed_complete = false;
            make_speed_graph_opt(0, 0, 0, selected_course.route.tracks.Count);
            calculate_point();

            check_route_conflict();
            speed_draw_req = true;
            speed_graph.Invalidate();
            position_graph.Invalidate();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (adding_flag == true && course_box.SelectedIndex > -1)
            {
                timed_data.Add(new timed_course(selected_course));
                timed_data[timed_data.Count - 1].start_sec = (int)start_s.Value + (int)(start_m.Value * 60) + (int)(start_h.Value * 3600);
                timed_data[timed_data.Count - 1].operate_speed_points.Clear();
                timed_data[timed_data.Count - 1].operate_speed_points = new List<speed_point>(operate_speed_graph_tmp);
                timed_data[timed_data.Count - 1].max_speed_points.Clear();
                timed_data[timed_data.Count - 1].max_speed_points = new List<speed_point>(max_speed_graph_tmp);

                timed_data[timed_data.Count - 1].max_speeds = new List<float>(selected_max_speeds);
                timed_data[timed_data.Count - 1].operate_speeds = new List<float>(selected_operate_speeds);

                timed_data[timed_data.Count - 1].dewell_pos = new List<int>(dewell_pos);
                timed_data[timed_data.Count - 1].dewell_time = new List<float>(dewell_tmp);

                graphs_view.Rows.Add(timed_data[timed_data.Count - 1].name, timed_data[timed_data.Count - 1].route, start_h.Value * 3600 + start_m.Value * 60 + start_s.Value);
                //speed_graphs.Add(new List<speed_point>(speed_graph_tmp));

            }
            else if (editing_flag == true)
            {
                timed_data[editing_index].start_sec = (int)start_s.Value + (int)(start_m.Value * 60) + (int)(start_h.Value * 3600);
                timed_data[editing_index].operate_speed_points.Clear();
                timed_data[editing_index].operate_speed_points = new List<speed_point>(operate_speed_graph_tmp);
                timed_data[editing_index].max_speed_points.Clear();
                timed_data[editing_index].max_speed_points = new List<speed_point>(max_speed_graph_tmp);

                timed_data[editing_index].max_speeds = new List<float>(selected_max_speeds);
                timed_data[editing_index].operate_speeds = new List<float>(selected_operate_speeds);

                timed_data[editing_index].dewell_pos = new List<int>(dewell_pos);
                timed_data[editing_index].dewell_time = new List<float>(dewell_tmp);

                //chang name ???

                //graphs_view.Rows.Add(timed_data[timed_data.Count - 1].name, timed_data[timed_data.Count - 1].route, start_h.Value * 3600 + start_m.Value * 60 + start_s.Value);

            }

            speed_graph_tmp.Clear();
            operate_speed_graph_tmp.Clear();

            course_box.SelectedIndex = -1;

            speed_graph.Invalidate();
            position_graph.Invalidate();

            selected_accum_length.Clear();

            add_course_group.Enabled = false;
            editing_flag = false;
            adding_flag = false;

            course_add_but.Enabled = true;
            course_edit_but.Enabled = true;
            course_delete_but.Enabled = true;
        }

        private void speed_graph_MouseMove(object sender, MouseEventArgs e)
        {
            var size = 20;
            var buffer = new Bitmap(size * 2, size * 2);

            int mouse_x = e.X;
            int mouse_y = e.Y;
            int selected = -1;

            //Console.WriteLine(mouse_x);

            if (selected_accum_length.Count > 0 && mouse_y > 15 && mouse_y < 170 && mouse_x > 50)
            {
                for (int i = 1; i < selected_accum_length.Count; i++)
                {
                    if (mouse_x < 50 + selected_accum_length[i] / graphic_scale + speed_h_location)
                    {
                        selected = i;
                        //Console.WriteLine(i);
                        break;
                    }
                }
            }

            if (selected != -1)
            {
                speed_graph_edit_hover = selected - 1;
                speed_graph.Invalidate();
            }
            else
            {
                speed_graph_edit_hover = -1;
                speed_graph.Invalidate();
            }

        }

        private void speed_graph_MouseDown(object sender, MouseEventArgs e)
        {
            if (speed_graph_edit_hover != -1)
            {
                speed_graph_edit_selected = speed_graph_edit_hover;

                if (edit_form == null || edit_form.IsDisposed)
                {
                    edit_form = new graph_editor_panel(this);
                }

                edit_form.speed_graph_event_enable = false;

                // dewell editor
                dewell_edit_index = -1;
                //edit_form.dewell_box.Value = 0;
                edit_form.dewell_box.Enabled = false;

                for (int i = 1; i < selected_course.route.stop_point.Count - 1; i++)
                {
                    if (speed_graph_edit_selected == dewell_pos[i])
                    {
                        dewell_edit_index = i;
                        edit_form.dewell_box.Enabled = true;
                        edit_form.dewell_box.Value = (int)dewell_tmp[i];
                    }
                }

                // speed editor
                edit_form.Text = selected_course.route.tracks[speed_graph_edit_selected];

                edit_form.civil_text.Text = "Max Civil Speed : " + fullscale.tracks[selected_course.route.tracks[speed_graph_edit_selected]].max_speed.ToString();
                edit_form.max_speed_box.Value = (int)selected_max_speeds[speed_graph_edit_selected];
                edit_form.operate_speed_box.Value = (int)selected_operate_speeds[speed_graph_edit_selected];

                edit_form.max_speed_text.Text = "(" + (fullscale.tracks[selected_course.route.tracks[speed_graph_edit_selected]].max_speed * selected_max_speeds[speed_graph_edit_selected] / (float)100.00).ToString() + ")";
                edit_form.operate_speed_text.Text = "(" + (fullscale.tracks[selected_course.route.tracks[speed_graph_edit_selected]].max_speed * selected_operate_speeds[speed_graph_edit_selected] / (float)100.00).ToString() + ")";

                edit_form.speed_graph_event_enable = true;

                edit_form.Show();
                edit_form.Location = new Point(Cursor.Position.X - edit_form.Width, Cursor.Position.Y - edit_form.Height);

                edit_form.BringToFront();
            }
        }

        private void position_graph_MouseMove(object sender, MouseEventArgs e)
        {
            Nullable<float> nearest_distance = null;
            //Nullable<float> nearest_pos_value = null;
            Service_Course nearest_course = new Service_Course();
            speed_point nearest_point = new speed_point(0, 0, 0);
            int nearest_trip_index = 0;

            int tmp_track_cnt = -1;
            int track_cnt = 0;
            int trip_cnt = 0;

            // all line
            trip_cnt = 0;
            foreach (timed_course trip in timed_data)
            {
                track_cnt = 0;
                foreach (speed_point t_point in trip.operate_speed_points)
                {
                    //Console.WriteLine((1500 + t_point.time_travel) * (float)time_scale - position_h_location);

                    // find nearest point
                    if (nearest_distance == null || Math.Abs(((e.X - position_h_location - t_point.graphic_x) * (e.X - position_h_location - t_point.graphic_x)) + ((e.Y - position_v_location - t_point.graphic_y) * (e.Y - position_v_location - t_point.graphic_y))) < Math.Abs((float)nearest_distance))
                    {
                        nearest_distance = ((e.X - position_h_location - t_point.graphic_x) * (e.X - position_h_location - t_point.graphic_x)) + ((e.Y - position_v_location - t_point.graphic_y) * (e.Y - position_v_location - t_point.graphic_y));
                        nearest_point = t_point;
                        nearest_course = trip;
                        nearest_trip_index = trip_cnt;
                        tmp_track_cnt = track_cnt;

                    }
                    track_cnt++;
                }
                trip_cnt++;
            }

            // building line
            track_cnt = 0;
            if (operate_speed_graph_tmp.Count > 0)
            {
                foreach (speed_point t_point in operate_speed_graph_tmp)
                {
                    // find nearest point
                    if (nearest_distance == null || Math.Abs(((e.X - position_h_location - t_point.graphic_x) * (e.X - position_h_location - t_point.graphic_x)) + ((e.Y - position_v_location - t_point.graphic_y) * (e.Y - position_v_location - t_point.graphic_y))) < Math.Abs((float)nearest_distance))
                    {
                        nearest_distance = ((e.X - position_h_location - t_point.graphic_x) * (e.X - position_h_location - t_point.graphic_x)) + ((e.Y - position_v_location - t_point.graphic_y) * (e.Y - position_v_location - t_point.graphic_y));
                        nearest_point = t_point;
                        nearest_course = selected_course;
                        nearest_trip_index = -1;
                        tmp_track_cnt = track_cnt;

                    }
                    track_cnt++;
                }
            }

            if (nearest_distance != null)
            {
                pos_graph_point_hover = nearest_point;
                pos_graph_track_cnt = tmp_track_cnt;
                pos_graph_course_hover = nearest_course;
                pos_graph_point_distance = (float)nearest_distance;
                pos_graph_selected_index = nearest_trip_index;
                position_graph.Invalidate();
            }
            //Console.WriteLine(pos_graph_track_cnt);
            //Console.WriteLine(e.X.ToString());
            //for(int i=0;i< timed_data.Count)
        }

        private void button1_Click(object sender, EventArgs e)
        {
            course_add_but.Enabled = false;
            course_edit_but.Enabled = false;
            course_delete_but.Enabled = false;

            course_box.Enabled = true;
            route_box.Enabled = true;
            train_box.Enabled = true;

            m_speed_percent.Enabled = true;
            o_speed_percent.Enabled = true;
            dewell_box.Enabled = true;

            adding_flag = true;
            add_course_group.Enabled = true;
        }

        private void course_edit_but_Click(object sender, EventArgs e)
        {
            if (graphs_view.SelectedCells.Count > 0 && editing_flag == false)
            {
                add_course_group.Enabled = true;

                

                editing_index = graphs_view.SelectedCells[0].RowIndex;
                start_h.Value = (int)timed_data[editing_index].start_sec / 3600;
                start_m.Value = (int)(timed_data[editing_index].start_sec % 3600) / 60;
                start_s.Value = (int)(timed_data[editing_index].start_sec % 60);
                // protect event

                selected_course = timed_data[editing_index];

                editing_flag = true;

                operate_speed_graph_tmp.Clear();
                operate_speed_graph_tmp = new List<speed_point>(timed_data[editing_index].operate_speed_points);
                max_speed_graph_tmp.Clear();
                max_speed_graph_tmp = new List<speed_point>(timed_data[editing_index].max_speed_points);

                selected_max_speeds = new List<float>(timed_data[editing_index].max_speeds);
                selected_operate_speeds = new List<float>(timed_data[editing_index].operate_speeds);

                dewell_pos = new List<int>(timed_data[editing_index].dewell_pos);
                dewell_tmp = new List<float>(timed_data[editing_index].dewell_time);

                pos_graph_hscroll.Value = (int)operate_speed_graph_tmp[0].graphic_x - 20;
                position_h_location = -pos_graph_hscroll.Value;

                selected_accum_length = new List<float>();
                int v_line_loc = 0;
                selected_accum_length.Add(v_line_loc);
                foreach (string t in selected_course.route.tracks)
                {
                    int graphic_length;
                    graphic_length = (fullscale.tracks[t].legnth / 100);
                    v_line_loc += graphic_length;
                    selected_accum_length.Add(v_line_loc);
                }

                speed_draw_req = true;
                speed_graph.Invalidate();
                position_graph.Invalidate();

                m_speed_percent.Enabled = false;
                o_speed_percent.Enabled = false;
                dewell_box.Enabled = false;

                course_box.Enabled = false;
                route_box.Enabled = false;
                train_box.Enabled = false;

                course_add_but.Enabled = false;
                course_edit_but.Enabled = false;
                course_delete_but.Enabled = false;

            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            speed_graph_tmp.Clear();

            editing_flag = false;
            adding_flag = false;

            course_box.SelectedIndex = -1;

            speed_graph.Invalidate();
            position_graph.Invalidate();

            selected_accum_length.Clear();

            add_course_group.Enabled = false;

            course_add_but.Enabled = true;
            course_edit_but.Enabled = true;
            course_delete_but.Enabled = true;
        }

        private void position_graph_MouseDown(object sender, MouseEventArgs e)
        {
            // click for connect mode
            if (connect_mode == true && pos_graph_point_distance < 2000 && pos_graph_selected_index >= 0)
            {
                if (pos_graph_track_cnt == 0 || pos_graph_track_cnt == timed_data[pos_graph_selected_index].operate_speed_points.Count - 1)
                {
                    // for first click
                    if (connect_trip_index == -1)
                    {
                        connect_trip_index = pos_graph_selected_index;
                        connect_point_index = pos_graph_track_cnt;
                        position_graph.Invalidate();
                        MessageBox.Show(connect_point_index.ToString());
                    }
                    // for second click
                    else
                    {
                        // click same point => cancle
                        if (connect_trip_index == pos_graph_selected_index && connect_point_index == pos_graph_track_cnt)
                        {
                            connect_trip_index = -1;
                            connect_point_index = -1;
                            position_graph.Invalidate();
                        }
                        // connect
                        else
                        {
                            // if same dir (inbound and inbpos_graph_track_cntound or outbound and outbound)
                            if ((connect_point_index == 0 && pos_graph_track_cnt == 0) || (connect_point_index != 0 && pos_graph_track_cnt != 0))
                            {
                                MessageBox.Show("cannot connect same direction trip!");
                            }
                            // if not same track
                            //else if (timed_data[connect_trip_index].route.tracks[timed_data[connect_trip_index].operate_speed_points[connect_point_index].track] != timed_data[pos_graph_selected_index].route.tracks[timed_data[pos_graph_selected_index].operate_speed_points[pos_graph_track_cnt].track])
                            //{
                            //    MessageBox.Show("distance must be same! ( [" + timed_data[connect_trip_index].route.tracks[timed_data[connect_trip_index].operate_speed_points[connect_point_index].track] + "] and [" + timed_data[pos_graph_selected_index].route.tracks[timed_data[pos_graph_selected_index].operate_speed_points[pos_graph_track_cnt].track] + "] )");
                            //}
                            else
                            {
                                MessageBox.Show("connect trip " + timed_data[connect_trip_index].name + " to trip " + timed_data[pos_graph_selected_index].name);
                                if (timed_data[connect_trip_index].operate_speed_points[connect_point_index].time_travel < timed_data[pos_graph_selected_index].operate_speed_points[pos_graph_track_cnt].time_travel)
                                {
                                    timed_data[connect_trip_index].next_course = pos_graph_selected_index;
                                    timed_data[pos_graph_selected_index].previous_course = connect_trip_index;
                                }
                                else
                                {
                                    timed_data[connect_trip_index].previous_course = pos_graph_selected_index;
                                    timed_data[pos_graph_selected_index].next_course = connect_trip_index;
                                }
                            }

                            connect_trip_index = -1;
                            connect_point_index = -1;
                            position_graph.Invalidate();

                        }
                    }
                }
            }
        }

        public void check_route_conflict()
        {
            //Console.WriteLine("start check");
            conflict_trips.Clear();

            // add selected trip
            if (adding_flag == true || editing_flag == true)
            {
                timed_data.Add(new timed_course(selected_course));
                timed_data[timed_data.Count - 1].start_sec = (int)start_s.Value + (int)(start_m.Value * 60) + (int)(start_h.Value * 3600);
                timed_data[timed_data.Count - 1].operate_speed_points.Clear();
                timed_data[timed_data.Count - 1].operate_speed_points = new List<speed_point>(operate_speed_graph_tmp);
                timed_data[timed_data.Count - 1].max_speed_points.Clear();
                timed_data[timed_data.Count - 1].max_speed_points = new List<speed_point>(max_speed_graph_tmp);

                timed_data[timed_data.Count - 1].max_speeds = new List<float>(selected_max_speeds);
                timed_data[timed_data.Count - 1].operate_speeds = new List<float>(selected_operate_speeds);

                timed_data[timed_data.Count - 1].dewell_pos = new List<int>(dewell_pos);
                timed_data[timed_data.Count - 1].dewell_time = new List<float>(dewell_tmp);
            }

            for (int i = 0; i < timed_data.Count; i++)
            {
                for(int j = i+1 ; j< timed_data.Count; j++)
                {
                    if (i != j)
                    {
                        // start compair trip
                        
                        List<int> trip_1_point = new List<int>();
                        List<int> trip_2_point = new List<int>();

                        //calculate point for each segment
                       
                        trip_1_point.Add(0);
                        for (int p = 1; p < timed_data[i].operate_speed_points.Count; p ++)
                        {
                            if(timed_data[i].operate_speed_points[p].track != timed_data[i].operate_speed_points[p - 1].track)
                            {
                                trip_1_point.Add(p);
                            }
                        }
                        trip_1_point.Add(timed_data[i].operate_speed_points.Count-1);

                        trip_2_point.Add(0);
                        for (int p = 1; p < timed_data[j].operate_speed_points.Count; p++)
                        {
                            if (timed_data[j].operate_speed_points[p].track != timed_data[j].operate_speed_points[p - 1].track)
                            {
                                trip_2_point.Add(p);
                            }
                        }
                        trip_2_point.Add(timed_data[j].operate_speed_points.Count-1);

                        int point_1_index = 0, point_2_index = 0;
                        while (point_1_index < timed_data[i].route.tracks.Count && point_2_index < timed_data[j].route.tracks.Count)
                        {
                            // if same track
                            if(timed_data[i].route.tracks[point_1_index] == timed_data[j].route.tracks[point_2_index])
                            {
                                Console.Write("con : " + timed_data[i].route.tracks[point_1_index]);
                                // if have some overlap time
                                //float s1 = timed_data[i].operate_speed_points[trip_1_point[point_1_index + 1]].time_travel;
                                if (!(timed_data[i].operate_speed_points[trip_1_point[point_1_index + 1]].time_travel < timed_data[j].operate_speed_points[trip_2_point[point_2_index]].time_travel || timed_data[j].operate_speed_points[trip_2_point[point_2_index + 1]].time_travel < timed_data[i].operate_speed_points[trip_1_point[point_1_index]].time_travel))
                                {
                                    Console.WriteLine(i.ToString() + " : " + point_1_index.ToString() + " , " + j.ToString() + " : " + point_2_index.ToString());
                                    conflict_trips.Add(new conflict_trip());
                                    conflict_trips[conflict_trips.Count - 1].trip_index[0] = i;
                                    conflict_trips[conflict_trips.Count - 1].trip_index[1] = j;
                                    conflict_trips[conflict_trips.Count - 1].start_point_index[0] = trip_1_point[point_1_index];
                                    conflict_trips[conflict_trips.Count - 1].start_point_index[1] = trip_2_point[point_2_index];
                                    conflict_trips[conflict_trips.Count - 1].end_point_index[0] = trip_1_point[point_1_index + 1];
                                    conflict_trips[conflict_trips.Count - 1].end_point_index[1] = trip_2_point[point_2_index + 1];
                                }
                            }

                            // increase counter
                            if (point_2_index >= timed_data[j].route.tracks.Count - 1 || timed_data[i].operate_speed_points[trip_1_point[point_1_index + 1]].time_travel < timed_data[j].operate_speed_points[trip_2_point[point_2_index + 1]].time_travel)
                            {
                                point_1_index++;
                            }
                            else
                            {
                                point_2_index++;
                            }
                        }

                        //Console.WriteLine("end" + i.ToString() + "," + j.ToString());
                    }
                }
            }
            //Console.WriteLine("end check");
            timed_data.RemoveAt(timed_data.Count-1);
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            check_route_conflict();
            position_graph.Invalidate();
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var serializer = new XmlSerializer(typeof(List<timed_course>));
            using (var stream = File.Open("schdule_ex.xml", FileMode.Create))
            {
                serializer.Serialize(stream, timed_data);
                stream.Close();
            }
        }
    }
}
