using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.Serialization;
using static rail_link_sim.Physical_Train_Network;

namespace rail_link_sim
{
    public partial class simmulation_engine : Form
    {
        Physical_Train_Network fullscale = new Physical_Train_Network();
        float _size = 1;

        int _h_location = 0;
        int _v_location = 0;

        int test_dir = 0;

        string selected_signal = null;
        string hover_signal = null;
        string last_hover_signal = null;

        int mouse_mode = 1; // 1 = reserved 2 = release

        int network_mode = -1; // -1 = don't select  0 = server 1 = client

        public static List<operate_route> operate_routes = new List<operate_route>();
        public static List<operate_train> operate_trains = new List<operate_train>();

        static bool deforming_packet = false;

        public static List<string[]> request_route = new List<string[]>();


        public simmulation_engine()
        {
            InitializeComponent();

            sim_panel.MouseWheel += Sim_panel_MouseWheel;
        }

        private void Sim_panel_MouseWheel(object sender, MouseEventArgs e)
        {
            _size += (float)(0.002 * e.Delta);
            //Console.WriteLine(_size.ToString());
            if (deforming_packet == false)
            {
                sim_panel.Invalidate();
            }
        }

        private void simmulation_engine_Load(object sender, EventArgs e)
        {

            fullscale.addTrack("physical_tracks.xml");
            fullscale.addLink("track_links.xml");
            
            fullscale.addPoint("SW101", "T0004", "T0005", "T0046");
            fullscale.addPoint("SW102", "T0008", "T0007", "T0047");
            fullscale.addPoint("SW103", "T0012", "T0013", "T0048");
            fullscale.addPoint("SW104", "T0016", "T0015", "T0049");

            fullscale.addPoint("SW151", "T0002", "T0001", "T0021");
            fullscale.addPoint("SW152", "T0021", "T0020", "T0040");
            fullscale.addPoint("SW153", "T0022", "T0042", "T0044");
            fullscale.addPoint("SW154", "T0044", "T0043", "T0045");

            fullscale.addPoint("SW111", "T0026", "T0025", "T0046");
            fullscale.addPoint("SW112", "T0026", "T0027", "T0047");
            fullscale.addPoint("SW113", "T0034", "T0033", "T0048");
            fullscale.addPoint("SW114", "T0034", "T0035", "T0049");

            fullscale.addPoint("SW201", "T0017", "T0019", "T0018");
            fullscale.addPoint("SW202", "T1002", "T1001", "T1000");
            fullscale.addPoint("SW211", "T0037", "T0038", "T0039");
            fullscale.addPoint("SW212", "T1015", "T1014", "T1013");

            fullscale.add_point_interlock("SW101", "SW111", 0);
            fullscale.add_point_interlock("SW102", "SW112", 0);
            fullscale.add_point_interlock("SW103", "SW113", 0);
            fullscale.add_point_interlock("SW104", "SW114", 0);

            fullscale.addSignal("S001", "S001", "ST11", 0, 0);
            fullscale.addSignal("S101", "S101", "ST11", 1, 0);
            fullscale.addSignal("S002", "S002", "ST12", 0, 0);
            fullscale.addSignal("S102", "S102", "ST12", 1, 0);
            fullscale.addSignal("S003", "S003", "ST13", 0, 0);
            fullscale.addSignal("S103", "S103", "ST13", 1, 0);
            fullscale.addSignal("S004", "S004", "ST14", 0, 0);
            fullscale.addSignal("S104", "S104", "ST14", 1, 0);
            fullscale.addSignal("S005", "S005", "ST15", 0, 0);
            fullscale.addSignal("S105", "S105", "ST15", 1, 0);
            fullscale.addSignal("S006", "S006", "ST16", 0, 0);
            fullscale.addSignal("S106", "S106", "ST16", 1, 0);

            fullscale.addSignal("S011", "S011", "T0002", 0, 0);
            fullscale.addSignal("S111", "S111", "T0002", 1, 0);

            fullscale.addSignal("S012", "S012", "T0023", 0, 0);
            fullscale.addSignal("S112", "S112", "T0023", 1, 0);

            fullscale.addSignal("S013", "S013", "T0004", 0, 0);
            fullscale.addSignal("S014", "S014", "T0006", 0, 0);
            fullscale.addSignal("S015", "S015", "T0009", 0, 0);
            fullscale.addSignal("S016", "S016", "T0012", 0, 0);
            fullscale.addSignal("S017", "S017", "T0014", 0, 0);
            fullscale.addSignal("S018", "S018", "T0017", 0, 0);

            fullscale.addSignal("S113", "S113", "T0026", 1, 0);
            fullscale.addSignal("S114", "S114", "T0028", 1, 0);
            fullscale.addSignal("S115", "S115", "T0030", 1, 0);
            fullscale.addSignal("S116", "S116", "T0034", 1, 0);
            fullscale.addSignal("S117", "S117", "T0036", 1, 0);
            fullscale.addSignal("S118", "S118", "T0037", 1, 0);

            fullscale.addSignal("S021", "S021", "ST21", 0, 0);
            fullscale.addSignal("S121", "S121", "ST21", 1, 0);
            fullscale.addSignal("S022", "S022", "ST22", 0, 0);
            fullscale.addSignal("S122", "S122", "ST22", 1, 0);
            fullscale.addSignal("S023", "S023", "ST23", 0, 0);
            fullscale.addSignal("S123", "S123", "ST23", 1, 0);
            fullscale.addSignal("S024", "S024", "ST24", 0, 0);
            fullscale.addSignal("S124", "S124", "ST24", 1, 0);

            fullscale.verify_track("ST11", 50, 200, 0);

            fullscale.points["SW102"].action = (int)PointAction.reverse;
            //fullscale.points["SW103"].action = (int)PointAction.reverse;
            //fullscale.points["SW113"].action = (int)PointAction.reverse;
            fullscale.points["SW104"].action = (int)PointAction.normal;

            fullscale.points["SW112"].action = (int)PointAction.reverse;
            // in it simmulator

            // test train
            operate_trains.Add(new operate_train());
            operate_trains[0].name = "BTS001";
            operate_trains[0].length = 85;
            operate_trains[0].track = "ST11";
            operate_trains[0].travel_percent = 50.00;
            operate_trains[0].speed = 0;
            operate_trains[0].route = -1;

            operate_trains.Add(new operate_train());
            operate_trains[1].name = "BTS002";
            operate_trains[1].length = 85;
            operate_trains[1].track = "T0028";
            operate_trains[1].travel_percent = 50.00;
            operate_trains[1].speed = 0;
            operate_trains[1].route = -1;

            if (deforming_packet == false)
            {
                sim_panel.Invalidate();
            }
        }

    
        private void sim_timer_Tick(object sender, EventArgs e)
        {
            int time_divider = 100;
            testbox.Text = "";

            if (network_mode != 1)
            {

                while(request_route.Count > 0) { 
                    reserve_route(request_route[0][0], request_route[0][1]);
                    request_route.RemoveAt(0);
                }

                // calculate speed for each train
                foreach (operate_train _train in operate_trains)
                {
                    double max_acc = 1.9;
                    double max_dcc = 2.0;
                    double dest_distance = -1;
                    double break_distance = _train.speed * _train.speed / (2 * max_dcc);

                    if (_train.route == -1)
                    {
                        _train.operate_speed = 0;
                        //_train.dir = -1; // stop
                        dest_distance = -1;
                    }
                    else
                    {
                        operate_route _route = operate_routes[_train.route];
                        _train.dir = _route.dir;

                        if (_route.route_queue_track.Count > 1)
                        {
                            dest_distance = fullscale.tracks[_train.track].legnth * (100.00 - _train.travel_percent) / 100;
                            dest_distance += ((double)fullscale.tracks[_route.route_queue_track[_route.route_queue_track.Count - 1]].legnth / 2);
                        }
                        else
                        {
                            dest_distance = fullscale.tracks[_train.track].legnth * (50.00 - _train.travel_percent) / 100;
                        }
                        for (int i = 1; i < _route.route_queue_track.Count - 1; i++)
                        {
                            dest_distance += fullscale.tracks[_route.route_queue_track[i]].legnth;
                        }
                        dest_distance /= 100;

                        if (dest_distance > break_distance)
                        {
                            // non timetable (use 80% of civil speed)
                            _train.operate_speed = ((double)fullscale.tracks[_train.track].max_speed * 1000 / 3600.00) * 80 / 100.00;// 300% now
                        }
                        else
                        {
                            _train.operate_speed = 0;
                        }

                    }

                    testbox.Text += dest_distance + ":" + break_distance + "\r\n";
                }

                // calculate speed for each train
                foreach (operate_train _train in operate_trains)
                {
                    double max_acc = 1.9;
                    double max_dcc = -2.0;

                    double remain_track_distance;
                    double travel_distance;

                    double max_new_speed = 0.00;

                    if (_train.dir > -1)
                    {

                        if (_train.operate_speed > _train.speed)
                        {
                            max_new_speed = _train.speed + max_acc * ((double)sim_timer.Interval / time_divider);
                            if (max_new_speed > _train.operate_speed) max_new_speed = _train.operate_speed;
                        }
                        else
                        {
                            max_new_speed = _train.speed + max_dcc * ((double)sim_timer.Interval / time_divider);
                            if (max_new_speed < _train.operate_speed) max_new_speed = _train.operate_speed;
                        }

                        travel_distance = ((max_new_speed + _train.speed) / 2.00) * ((double)sim_timer.Interval / time_divider);
                        remain_track_distance = ((double)fullscale.tracks[_train.track].legnth / 100) * ((100.00 - _train.travel_percent) / 100);

                        while (travel_distance > remain_track_distance)
                        {
                            // move to next track
                            travel_distance -= remain_track_distance;

                            //release resource
                            if (_train.route != -1)
                            {
                                operate_route _route = operate_routes[_train.route];
                                //release point interlock

                                for (int i = 0; i < _route.interlocking_point.Count; i++)
                                {
                                    if (fullscale.points[_route.interlocking_point[i].point].track_in == _train.track)
                                    {
                                        _route.interlocking_point.RemoveAt(i);
                                        i = -1;
                                    }
                                }


                                //release signal

                                if (_route.route_queue_signal.Count > 0)
                                {
                                    if (_route.route_queue_signal[0] == fullscale.tracks[_train.track].signal_end_start || _route.route_queue_signal[0] == fullscale.tracks[_train.track].signal_start_end)
                                    {
                                        _route.route_queue_signal.RemoveAt(0);
                                    }
                                }

                                //release track
                                _route.route_queue_track.RemoveAt(0);
                            }

                            if (_train.dir == 0)
                            {
                                // if there point in front of train
                                if (fullscale.tracks[_train.track].next_start_end.Count > 1)
                                {
                                    string _point = fullscale.tracks[_train.track].point_start_end[0];
                                    if (fullscale.points[_point].action == (int)PointAction.normal) _train.track = fullscale.points[_point].track_normal;
                                    else _train.track = fullscale.points[_point].track_reverse;
                                }
                                else if (fullscale.tracks[_train.track].next_start_end.Count == 1)
                                {
                                    _train.track = fullscale.tracks[_train.track].next_start_end[0];
                                }
                                else
                                {
                                    // error wtf!
                                }
                            }
                            else if (_train.dir == 1)
                            {
                                // if there point in front of train
                                if (fullscale.tracks[_train.track].next_end_start.Count > 1)
                                {
                                    string _point = fullscale.tracks[_train.track].point_end_start[0];
                                    if (fullscale.points[_point].action == (int)PointAction.normal) _train.track = fullscale.points[_point].track_normal;
                                    else _train.track = fullscale.points[_point].track_reverse;
                                }
                                else if (fullscale.tracks[_train.track].next_end_start.Count == 1)
                                {
                                    _train.track = fullscale.tracks[_train.track].next_end_start[0];
                                }
                                else
                                {
                                    // error wtf!
                                }
                            }

                            remain_track_distance = fullscale.tracks[_train.track].legnth / 100;
                            _train.travel_percent = 0.00;

                        }

                        //release route
                        if (_train.route > -1 && operate_routes[_train.route].route_queue_track.Count <= 1 && _train.speed == 0.00)
                        {
                            operate_routes.RemoveAt(_train.route);

                            foreach (operate_train _tmp_train in operate_trains)
                            {
                                if (_tmp_train.route > _train.route) _tmp_train.route--;
                            }
                            Console.WriteLine("release route " + _train.route.ToString() + "(route count : )" + operate_routes.Count.ToString() + ")");
                            _train.route = -1;
                        }

                        _train.travel_percent += (((double)travel_distance / (fullscale.tracks[_train.track].legnth / 100)) * 100);


                    }

                    _train.speed = max_new_speed;

                    // calculate occupied_tracks
                    double front_remain_distance;
                    double back_remain_distance;
                    double half_train_length = _train.length / 2;
                    _train.occupied_tracks.Clear();
                    _train.occupied_tracks.Add(_train.track);

                    if (_train.dir == 0)
                    {
                        front_remain_distance = ((double)fullscale.tracks[_train.track].legnth / 100) * ((100.00 - _train.travel_percent) / 100);
                        back_remain_distance = ((double)fullscale.tracks[_train.track].legnth / 100) * ((_train.travel_percent) / 100);
                    }
                    else
                    {
                        back_remain_distance = ((double)fullscale.tracks[_train.track].legnth / 100) * ((100.00 - _train.travel_percent) / 100);
                        front_remain_distance = ((double)fullscale.tracks[_train.track].legnth / 100) * ((_train.travel_percent) / 100);
                    }

                    {
                        testbox.Text += "(" + front_remain_distance.ToString() + ")";
                        string _cur_track = _train.track;

                        while (half_train_length > front_remain_distance)
                        {
                            string _next_track = "";
                            half_train_length -= front_remain_distance;

                            if (fullscale.tracks[_cur_track].next_start_end.Count > 1)
                            {
                                string _point = fullscale.tracks[_cur_track].point_start_end[0];
                                if (fullscale.points[_point].action == (int)PointAction.normal) _next_track = fullscale.points[_point].track_normal;
                                else _next_track = fullscale.points[_point].track_reverse;
                            }
                            else if (fullscale.tracks[_cur_track].next_start_end.Count == 1)
                            {
                                _next_track = fullscale.tracks[_cur_track].next_start_end[0];
                            }
                            else
                            {
                                break;
                            }

                            _train.occupied_tracks.Add(_next_track);
                            front_remain_distance = fullscale.tracks[_next_track].legnth / 100;
                            _cur_track = _next_track;
                        }

                        _cur_track = _train.track;
                        half_train_length = _train.length / 2;

                        while (half_train_length > back_remain_distance)
                        {
                            string _next_track = "";
                            half_train_length -= back_remain_distance;

                            if (fullscale.tracks[_cur_track].next_end_start.Count > 1)
                            {
                                string _point = fullscale.tracks[_cur_track].point_end_start[0];
                                if (fullscale.points[_point].action == (int)PointAction.normal) _next_track = fullscale.points[_point].track_normal;
                                else _next_track = fullscale.points[_point].track_reverse;
                            }
                            else if (fullscale.tracks[_cur_track].next_end_start.Count == 1)
                            {
                                _next_track = fullscale.tracks[_cur_track].next_end_start[0];
                            }
                            else
                            {
                                break;
                            }

                            _train.occupied_tracks.Add(_next_track);
                            back_remain_distance = fullscale.tracks[_next_track].legnth;
                            _cur_track = _next_track;
                        }
                    }

                    //release route from train
                    testbox.Text += _train.name + " " + _train.track + " " + _train.speed + " " + _train.travel_percent + " " + +_train.dir + "\r\n";
                }
            }

            // form packet and send to client
            if (network_mode == 0)
            {
                //Console.Write(clients.Count);
                for (int i = 0; i < clients.Count; i++)
                {
                    Socket _client = clients[i];
                    if (_client.Connected)
                    {
                        if (clients_recived_ready[i] == 1)
                        {
                            Server_Send(_client, packet_former(operate_routes) + packet_former(operate_trains) + "&");
                            clients_recived_ready[i] = 0;
                        }
                        //Server_Send(_client, packet_former(operate_trains));
                    }
                    else
                    {
                        clients.RemoveAt(i);
                        clients_recived_ready.RemoveAt(i);
                        i--;
                    }
                }
            }

            //Console.WriteLine(packet_former(operate_routes));
            //Console.WriteLine(packet_former(operate_trains));

            if (deforming_packet == false)
            {
                sim_panel.Invalidate();
            }
        }

        public string XMLToString<T>(T _data)
        {
            var stringwriter = new System.IO.StringWriter();
            var serializer = new XmlSerializer(typeof(T));
            serializer.Serialize(stringwriter, _data);
            return stringwriter.ToString();
        }

        public static string packet_former(List<operate_route> _data)
        {
            string packet = "";

            foreach (operate_route _r in _data)
            {
                packet += "R," + _r.route_queue_track.Count.ToString() + ",";
                foreach (string _track in _r.route_queue_track)
                {
                    packet += _track + ",";
                }
                packet += _r.route_queue_signal.Count.ToString() + ",";
                foreach (string _signal in _r.route_queue_signal)
                {
                    packet += _signal + ",";
                }
                packet += _r.interlocking_point.Count.ToString() + ",";
                foreach (point_interlock _interlock in _r.interlocking_point)
                {
                    packet += _interlock.point + "," + _interlock.dir.ToString() + ",";
                }
                packet += _r.dir.ToString() + ",";
                packet += _r.train_id.ToString() + ",";
                packet += "\n";

            }

            return packet;
        }

        public static string packet_former(List<operate_train> _data)
        {
            string packet = "";

            foreach (operate_train _t in _data)
            {
                packet += "T," + _t.name + ",";
                packet += _t.speed.ToString() + ",";
                packet += _t.operate_speed.ToString() + ",";
                packet += _t.track + ",";
                packet += _t.travel_percent + ",";
                packet += _t.length.ToString() + ",";
                packet += _t.route + ",";
                packet += _t.dir.ToString() + ",";
                packet += _t.position.X.ToString() + ",";
                packet += _t.position.Y.ToString() + ",";
                packet += _t.occupied_tracks.Count + ",";
                foreach (string _oc in _t.occupied_tracks)
                {
                    packet += _oc + ",";
                }
                packet += "\n";
            }

            return packet;

        }

        public static bool packet_deformer(string _data,out List<operate_route> _route, out List<operate_train> _train)
        {
            bool packet_valid = true;
            int track_count;
            int signal_count;
            int point_count;
            int element_count;

            _route = new List<operate_route>();
            _train = new List<operate_train>();

            try
            {
               
                foreach (string _r in _data.Split('\n'))
                {
                    if (_r[0] == 'R') // route packet
                    {
                        _route.Add(new operate_route());
                        string[] _element = _r.Split(',');

                        // track
                        track_count = int.Parse(_element[1]);
                        element_count = 2;
                        for (int i = 0; i < track_count; i++)
                        {
                            _route.Last().route_queue_track.Add(_element[element_count]);
                            element_count++;
                        }

                        // signal
                        signal_count = int.Parse(_element[element_count]);
                        element_count++;
                        for (int i = 0; i < signal_count; i++)
                        {
                            _route.Last().route_queue_signal.Add(_element[element_count]);
                            element_count++;
                        }

                        // point
                        point_count = int.Parse(_element[element_count]);
                        element_count++;
                        for (int i = 0; i < point_count; i++)
                        {
                            _route.Last().interlocking_point.Add(new point_interlock(_element[element_count], int.Parse(_element[element_count + 1])));
                            element_count += 2;
                        }

                        element_count -= 1;
                        _route.Last().dir = int.Parse(_element[element_count]);
                        element_count++;
                        _route.Last().train_id = int.Parse(_element[element_count]);
                    }
                    else if(_r[0] == 'T')
                    {
                        _train.Add(new operate_train());
                        string[] _element = _r.Split(',');

                        element_count = 1;
                        _train.Last().name = _element[element_count]; element_count++;
                        _train.Last().speed = double.Parse(_element[element_count]); element_count++;
                        _train.Last().operate_speed = double.Parse(_element[element_count]); element_count++;
                        _train.Last().track = _element[element_count]; element_count++;
                        _train.Last().travel_percent = double.Parse(_element[element_count]); element_count++;
                        _train.Last().length = int.Parse(_element[element_count]); element_count++;
                        _train.Last().route = int.Parse(_element[element_count]); element_count++;
                        _train.Last().dir = int.Parse(_element[element_count]); element_count++;
                        _train.Last().position.X = int.Parse(_element[element_count]); element_count++;
                        _train.Last().position.Y = int.Parse(_element[element_count]); element_count++;

                        // occupied_tracks
                        track_count = int.Parse(_element[element_count]); element_count++;
                        for (int i = 0; i < track_count; i++)
                        {
                            _train.Last().occupied_tracks.Add(_element[element_count]);
                            element_count++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                packet_valid = false;
            }

            return packet_valid;
        }

        private void sim_panel_Paint(object sender, PaintEventArgs e)
        {                       
            
                        
            e.Graphics.TranslateTransform(_h_location, _v_location);
            e.Graphics.ScaleTransform(_size, _size);

            // draw track
            foreach (KeyValuePair<string, physical_track> _t in fullscale.tracks)
            {               
                e.Graphics.FillPolygon(Brushes.Pink, _t.Value.drawing.ToArray());
            }

            // draw interlocking point
            Brush interlock_brush = new SolidBrush(Color.Orange);
            foreach (operate_route _o in operate_routes)
            {
                foreach (point_interlock _point in _o.interlocking_point)
                {
                    e.Graphics.FillPolygon(interlock_brush, fullscale.tracks[fullscale.points[_point.point].track_in].drawing.ToArray());
                    e.Graphics.FillPolygon(interlock_brush, fullscale.tracks[fullscale.points[_point.point].track_normal].drawing.ToArray());
                    e.Graphics.FillPolygon(interlock_brush, fullscale.tracks[fullscale.points[_point.point].track_reverse].drawing.ToArray());

                    foreach (physical_point.interlock _relate_point in fullscale.points[_point.point].interlocks)
                    {
                        e.Graphics.FillPolygon(interlock_brush, fullscale.tracks[fullscale.points[_relate_point.point].track_in].drawing.ToArray());
                        e.Graphics.FillPolygon(interlock_brush, fullscale.tracks[fullscale.points[_relate_point.point].track_normal].drawing.ToArray());
                        e.Graphics.FillPolygon(interlock_brush, fullscale.tracks[fullscale.points[_relate_point.point].track_reverse].drawing.ToArray());

                    }
                }
            }

            // draw suggest queue route track
            Brush suggest_brush = new SolidBrush(Color.FromArgb(128, 0, 0, 255));
            foreach (string _t in suggest_routing_track)
            {
                e.Graphics.FillPolygon(suggest_brush, fullscale.tracks[_t].drawing.ToArray());
            }

            // draw queue route track
            Brush queue_brush = new SolidBrush(Color.FromArgb(128, 0, 255, 0));
            foreach (operate_route _o in operate_routes) {
                foreach (string _t in _o.route_queue_track)
                {
                    e.Graphics.FillPolygon(queue_brush, fullscale.tracks[_t].drawing.ToArray());
                }
            }

            //draw point
            SolidBrush bg_colour = new SolidBrush(sim_panel.BackColor);
            foreach (KeyValuePair<string, physical_point> _p in fullscale.points)
            {
                List<Point> _point_rev_drawing = new List<Point>();
                List<Point> _point_nor_drawing = new List<Point>();

                //MessageBox.Show(fullscale.tracks[_p.Value.track_reverse].drawing_width.ToString());

                if (_p.Value.track_point_dir == 1)
                {
                    foreach (Point _sp in fullscale.tracks[_p.Value.track_reverse].drawing)
                    {
                        _point_rev_drawing.Add(new Point(_sp.X, _sp.Y - (int)fullscale.tracks[_p.Value.track_reverse].drawing_width));
                    }

                    foreach (Point _sp in fullscale.tracks[_p.Value.track_normal].drawing)
                    {
                        _point_nor_drawing.Add(new Point(_sp.X, _sp.Y + (int)fullscale.tracks[_p.Value.track_normal].drawing_width));
                    }

                    if (_p.Value.action == (int)PointAction.normal)
                    {
                        e.Graphics.FillPolygon(bg_colour, _point_nor_drawing.ToArray());

                        //e.Graphics.FillPolygon(Brushes.LightPink, fullscale.tracks[_p.Value.track_normal].drawing.ToArray());
                        //e.Graphics.FillPolygon(Brushes.Aquamarine, _point_rev_drawing.ToArray());
                    }
                    else if(_p.Value.action == (int)PointAction.reverse)
                    {
                        e.Graphics.FillPolygon(bg_colour, _point_rev_drawing.ToArray());

                        //e.Graphics.FillPolygon(Brushes.LightPink, fullscale.tracks[_p.Value.track_reverse].drawing.ToArray());
                        //e.Graphics.FillPolygon(Brushes.Aquamarine, _point_nor_drawing.ToArray());
                    }

                    // draw name
                    Font _f = new Font("Arial", 9);
                    StringFormat _sf = new StringFormat();
                    _sf.LineAlignment = StringAlignment.Center;
                    _sf.Alignment = StringAlignment.Center;
                  
                    e.Graphics.DrawString(_p.Value.name, _f, Brushes.HotPink, new Point(fullscale.tracks[_p.Value.track_in].location_start_end.X, fullscale.tracks[_p.Value.track_in].location_start_end.Y - 20), _sf);
                }
                else if(_p.Value.track_point_dir == 0)
                {
                    foreach (Point _sp in fullscale.tracks[_p.Value.track_reverse].drawing)
                    {
                        _point_rev_drawing.Add(new Point(_sp.X, _sp.Y + (int)fullscale.tracks[_p.Value.track_reverse].drawing_width));
                    }

                    foreach (Point _sp in fullscale.tracks[_p.Value.track_normal].drawing)
                    {
                        _point_nor_drawing.Add(new Point(_sp.X, _sp.Y - (int)fullscale.tracks[_p.Value.track_normal].drawing_width));
                    }

                    if (_p.Value.action == (int)PointAction.reverse)
                    {
                        e.Graphics.FillPolygon(bg_colour, _point_rev_drawing.ToArray());
                        //e.Graphics.FillPolygon(Brushes.LightPink, fullscale.tracks[_p.Value.track_reverse].drawing.ToArray());
                        //e.Graphics.FillPolygon(Brushes.Aquamarine, _point_rev_drawing.ToArray());
                    }
                    else if (_p.Value.action == (int)PointAction.normal)
                    {
                        e.Graphics.FillPolygon(bg_colour, _point_nor_drawing.ToArray());
                        //e.Graphics.FillPolygon(Brushes.LightPink, fullscale.tracks[_p.Value.track_normal].drawing.ToArray());
                        //e.Graphics.FillPolygon(Brushes.Aquamarine, _point_nor_drawing.ToArray());
                    }

                    // draw name
                    Font _f = new Font("Arial", 9);
                    StringFormat _sf = new StringFormat();
                    _sf.LineAlignment = StringAlignment.Center;
                    _sf.Alignment = StringAlignment.Center;

                    e.Graphics.DrawString(_p.Value.name, _f, Brushes.AliceBlue, new Point(fullscale.tracks[_p.Value.track_in].location_start_end.X, fullscale.tracks[_p.Value.track_in].location_start_end.Y + 20), _sf);
                }
                else if (_p.Value.track_point_dir == 3)
                {
                    foreach (Point _sp in fullscale.tracks[_p.Value.track_reverse].drawing)
                    {
                        _point_rev_drawing.Add(new Point(_sp.X, _sp.Y + (int)fullscale.tracks[_p.Value.track_reverse].drawing_width));
                    }

                    foreach (Point _sp in fullscale.tracks[_p.Value.track_normal].drawing)
                    {
                        _point_nor_drawing.Add(new Point(_sp.X, _sp.Y - (int)fullscale.tracks[_p.Value.track_normal].drawing_width));
                    }

                    if (_p.Value.action == (int)PointAction.reverse)
                    {
                        e.Graphics.FillPolygon(bg_colour, _point_rev_drawing.ToArray());
                        //e.Graphics.FillPolygon(Brushes.LightPink, fullscale.tracks[_p.Value.track_reverse].drawing.ToArray());
                        //e.Graphics.FillPolygon(Brushes.DarkGoldenrod, _point_rev_drawing.ToArray());
                    }
                    else if (_p.Value.action == (int)PointAction.normal)
                    {
                        e.Graphics.FillPolygon(bg_colour, _point_nor_drawing.ToArray());
                        //e.Graphics.FillPolygon(Brushes.LightPink, fullscale.tracks[_p.Value.track_normal].drawing.ToArray());
                        //e.Graphics.FillPolygon(Brushes.DarkGoldenrod, _point_nor_drawing.ToArray());
                    }

                    // draw name
                    Font _f = new Font("Arial", 9);
                    StringFormat _sf = new StringFormat();
                    _sf.LineAlignment = StringAlignment.Center;
                    _sf.Alignment = StringAlignment.Center;

                    e.Graphics.DrawString(_p.Value.name, _f, Brushes.BurlyWood, new Point(fullscale.tracks[_p.Value.track_in].location_end_start.X, fullscale.tracks[_p.Value.track_in].location_end_start.Y + 20), _sf);
                }
                else if (_p.Value.track_point_dir == 2)
                {
                    foreach (Point _sp in fullscale.tracks[_p.Value.track_reverse].drawing)
                    {
                        _point_rev_drawing.Add(new Point(_sp.X, _sp.Y - (int)fullscale.tracks[_p.Value.track_reverse].drawing_width));
                    }

                    foreach (Point _sp in fullscale.tracks[_p.Value.track_normal].drawing)
                    {
                        _point_nor_drawing.Add(new Point(_sp.X, _sp.Y + (int)fullscale.tracks[_p.Value.track_normal].drawing_width));
                    }

                    if (_p.Value.action == (int)PointAction.reverse)
                    {
                        e.Graphics.FillPolygon(bg_colour, _point_rev_drawing.ToArray());
                        //e.Graphics.FillPolygon(Brushes.LightPink, fullscale.tracks[_p.Value.track_reverse].drawing.ToArray());
                        //e.Graphics.FillPolygon(Brushes.Crimson, _point_rev_drawing.ToArray());
                    }
                    else if (_p.Value.action == (int)PointAction.normal)
                    {
                        e.Graphics.FillPolygon(bg_colour, _point_nor_drawing.ToArray());
                        //e.Graphics.FillPolygon(Brushes.LightPink, fullscale.tracks[_p.Value.track_normal].drawing.ToArray());
                        //e.Graphics.FillPolygon(Brushes.Crimson, _point_nor_drawing.ToArray());
                    }

                    // draw name
                    Font _f = new Font("Arial", 9);
                    StringFormat _sf = new StringFormat();
                    _sf.LineAlignment = StringAlignment.Center;
                    _sf.Alignment = StringAlignment.Center;

                    e.Graphics.DrawString(_p.Value.name, _f, Brushes.DarkBlue, new Point(fullscale.tracks[_p.Value.track_in].location_end_start.X, fullscale.tracks[_p.Value.track_in].location_end_start.Y - 20), _sf);
                }

            }

            // draw signal
            foreach (KeyValuePair<string, Physical_signal> _s in fullscale.signals)
            {
                if (_s.Value.dir_type == 0)
                {
                                        //e.Graphics.FillEllipse(Brushes.Red, fullscale.tracks[_s.Value.track].location_start_end.X - 13, fullscale.tracks[_s.Value.track].location_start_end.Y - 20, 12, 12);
                    e.Graphics.FillRectangle(Brushes.WhiteSmoke, fullscale.tracks[_s.Value.track].location_start_end.X - 13 - 10, fullscale.tracks[_s.Value.track].location_start_end.Y - 20 + 5, 18, 3);
                    e.Graphics.FillRectangle(Brushes.WhiteSmoke, fullscale.tracks[_s.Value.track].location_start_end.X - 13 - 10, fullscale.tracks[_s.Value.track].location_start_end.Y - 20 + 5, 3, 8);

                    if (selected_signal == _s.Value.name)
                    {
                        e.Graphics.FillEllipse(Brushes.White, fullscale.tracks[_s.Value.track].location_start_end.X - 13, fullscale.tracks[_s.Value.track].location_start_end.Y - 20, 12, 12);
                    }
                    else if (hover_signal == _s.Value.name)
                    {
                        e.Graphics.FillEllipse(Brushes.Orange, fullscale.tracks[_s.Value.track].location_start_end.X - 13, fullscale.tracks[_s.Value.track].location_start_end.Y - 20, 12, 12);
                    }
                    else
                    {
                        e.Graphics.FillEllipse(Brushes.Red, fullscale.tracks[_s.Value.track].location_start_end.X - 13, fullscale.tracks[_s.Value.track].location_start_end.Y - 20, 12, 12);
                    }

                    Font _f = new Font("Arial", 7);
                    StringFormat _sf = new StringFormat();
                    _sf.LineAlignment = StringAlignment.Center;
                    _sf.Alignment = StringAlignment.Center;

                    e.Graphics.DrawString(_s.Value.display_name, _f, Brushes.Orange, new Point(fullscale.tracks[_s.Value.track].location_start_end.X - 15 , fullscale.tracks[_s.Value.track].location_start_end.Y - 28), _sf);

                }
                else
                {
                    //e.Graphics.FillEllipse(Brushes.Red, fullscale.tracks[_s.Value.track].location_start_end.X - 13, fullscale.tracks[_s.Value.track].location_start_end.Y - 20, 12, 12);
                    e.Graphics.FillRectangle(Brushes.WhiteSmoke, fullscale.tracks[_s.Value.track].location_end_start.X + 13 - 8, fullscale.tracks[_s.Value.track].location_end_start.Y + 7 + 5, 18, 3);
                    e.Graphics.FillRectangle(Brushes.WhiteSmoke, fullscale.tracks[_s.Value.track].location_end_start.X + 13 + 8, fullscale.tracks[_s.Value.track].location_end_start.Y + 7, 3, 8);


                    if (selected_signal == _s.Value.name)
                    {
                        e.Graphics.FillEllipse(Brushes.White, fullscale.tracks[_s.Value.track].location_end_start.X + 1, fullscale.tracks[_s.Value.track].location_end_start.Y + 7, 12, 12);
                    }
                    else if(hover_signal == _s.Value.name)
                    {
                        e.Graphics.FillEllipse(Brushes.Orange, fullscale.tracks[_s.Value.track].location_end_start.X + 1, fullscale.tracks[_s.Value.track].location_end_start.Y + 7, 12, 12);
                    }
                    else
                    {
                        e.Graphics.FillEllipse(Brushes.Red, fullscale.tracks[_s.Value.track].location_end_start.X + 1, fullscale.tracks[_s.Value.track].location_end_start.Y + 7, 12, 12);
                    }

                    Font _f = new Font("Arial", 7);
                    StringFormat _sf = new StringFormat();
                    _sf.LineAlignment = StringAlignment.Center;
                    _sf.Alignment = StringAlignment.Center;

                    e.Graphics.DrawString(_s.Value.display_name, _f, Brushes.Orange, new Point(fullscale.tracks[_s.Value.track].location_end_start.X + 15, fullscale.tracks[_s.Value.track].location_end_start.Y + 27), _sf);


                }
            }

            // reserved signal
            foreach (operate_route _route in operate_routes)
            {
                for(int i=0;i<_route.route_queue_signal.Count - 1; i++)
                {
                    if(fullscale.signals[_route.route_queue_signal[i]].dir_type == 1)
                    {
                        e.Graphics.FillEllipse(Brushes.LightGreen, fullscale.tracks[fullscale.signals[_route.route_queue_signal[i]].track].location_end_start.X + 1, fullscale.tracks[fullscale.signals[_route.route_queue_signal[i]].track].location_end_start.Y + 7, 12, 12);
                    }
                    else
                    {
                        e.Graphics.FillEllipse(Brushes.LightGreen, fullscale.tracks[fullscale.signals[_route.route_queue_signal[i]].track].location_start_end.X - 13, fullscale.tracks[fullscale.signals[_route.route_queue_signal[i]].track].location_start_end.Y - 20, 12, 12);
                    }
                }
            }

            foreach (KeyValuePair<string, physical_track> _t in fullscale.tracks)
            {
                // draw name
                Font _f = new Font("Arial", 9);
                StringFormat _sf = new StringFormat();
                _sf.LineAlignment = StringAlignment.Center;
                _sf.Alignment = StringAlignment.Center;

                e.Graphics.DrawString(_t.Value.display_name, _f, Brushes.Orange, new Point((_t.Value.location_end_start.X + _t.Value.location_start_end.X) / 2, (_t.Value.location_end_start.Y + _t.Value.location_start_end.Y) / 2 - 10), _sf);
            }

            //draw train
            foreach(operate_train _train in operate_trains)
            {
                if (_train.name != null)
                {
                    Font _f = new Font("Arial", 12);
                    StringFormat _sf = new StringFormat();
                    _sf.LineAlignment = StringAlignment.Center;
                    _sf.Alignment = StringAlignment.Center;
                    Brush train_brush = new SolidBrush(Color.FromArgb(255, 255, 0, 0));

                    e.Graphics.FillPolygon(train_brush, fullscale.tracks[_train.track].drawing.ToArray());

                    //draw occupied track
                    foreach(string _occ_track in _train.occupied_tracks)
                    {
                        e.Graphics.FillPolygon(Brushes.Red, fullscale.tracks[_occ_track].drawing.ToArray());
                    }

                    e.Graphics.DrawString(_train.name, _f, Brushes.Wheat, new Point((fullscale.tracks[_train.track].location_end_start.X + fullscale.tracks[_train.track].location_start_end.X) / 2, (fullscale.tracks[_train.track].location_end_start.Y + fullscale.tracks[_train.track].location_start_end.Y) / 2), _sf);


                    //just for debug
                    if (_train.dir == 0)
                    {
                        e.Graphics.FillEllipse(Brushes.Black, (float)(fullscale.tracks[_train.track].location_end_start.X + (Math.Abs(fullscale.tracks[_train.track].location_start_end.X - fullscale.tracks[_train.track].location_end_start.X) * _train.travel_percent / 100.00) - 3), (float)(fullscale.tracks[_train.track].location_end_start.Y + ((fullscale.tracks[_train.track].location_start_end.Y - fullscale.tracks[_train.track].location_end_start.Y) * _train.travel_percent / 100.00) - 3), 6, 6);
                    }
                    else
                    {
                        e.Graphics.FillEllipse(Brushes.Black, (float)(fullscale.tracks[_train.track].location_start_end.X - (Math.Abs(fullscale.tracks[_train.track].location_start_end.X - fullscale.tracks[_train.track].location_end_start.X) * _train.travel_percent / 100.00) - 3), (float)(fullscale.tracks[_train.track].location_start_end.Y - ((fullscale.tracks[_train.track].location_start_end.Y - fullscale.tracks[_train.track].location_end_start.Y) * _train.travel_percent / 100.00) - 3), 6, 6);
                    }
                }        
            }

        }

        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            _h_location = -hScrollBar1.Value;

            if (deforming_packet == false)
            {
                sim_panel.Invalidate();
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            fullscale.points["SW101"].action = (int)PointAction.normal;
            fullscale.points["SW111"].action = (int)PointAction.normal;

            fullscale.points["SW152"].action = (int)PointAction.normal;
            sim_panel.Invalidate();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            fullscale.points["SW101"].action = (int)PointAction.reverse;
            fullscale.points["SW111"].action = (int)PointAction.reverse;

            fullscale.points["SW152"].action = (int)PointAction.reverse;
            sim_panel.Invalidate();
        }

        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            _v_location = -vScrollBar1.Value;
            sim_panel.Invalidate();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            fullscale.points["SW102"].action = (int)PointAction.normal;
            fullscale.points["SW112"].action = (int)PointAction.normal;
            sim_panel.Invalidate();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            fullscale.points["SW102"].action = (int)PointAction.reverse;
            fullscale.points["SW112"].action = (int)PointAction.reverse;
            sim_panel.Invalidate();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            fullscale.points["SW103"].action = (int)PointAction.normal;
            fullscale.points["SW113"].action = (int)PointAction.normal;
            sim_panel.Invalidate();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            fullscale.points["SW103"].action = (int)PointAction.reverse;
            fullscale.points["SW113"].action = (int)PointAction.reverse;
            sim_panel.Invalidate();
        }

        private void button13_Click(object sender, EventArgs e)
        {
            fullscale.points["SW104"].action = (int)PointAction.normal;
            fullscale.points["SW114"].action = (int)PointAction.normal;
            sim_panel.Invalidate();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            fullscale.points["SW104"].action = (int)PointAction.reverse;
            fullscale.points["SW114"].action = (int)PointAction.reverse;
            sim_panel.Invalidate();
        }

        private void sim_panel_MouseMove(object sender, MouseEventArgs e)
        {
            // grab view ?
            /*
            if (movine_cell != null)
            {
                movine_cell.start_point.X = movine_offset_x + e.X;
                movine_cell.start_point.Y = movine_offset_y + e.Y;
                //Console.WriteLine("H");
                test_draw.Invalidate();
            }*/

            // detect signal
            var size = 20;
            var buffer = new Bitmap(size * 2, size * 2);
            string signal_found = null;
            //hover_signal = null;

            foreach (KeyValuePair<string, Physical_signal> _s in fullscale.signals)
            {
                Graphics g = Graphics.FromImage(buffer);

                g.TranslateTransform(_h_location, _v_location);
                //g.ScaleTransform(_size, _size);

                //using (var g = Graphics.FromImage(buffer))
                //{
                //g.ScaleTransform(_size,_size);
                g.Clear(Color.Black);
                if (fullscale.signals[_s.Value.name].dir_type == 1)
                {
                    g.FillEllipse(Brushes.Green, (fullscale.tracks[_s.Value.track].location_end_start.X + 1 - 4) * _size - e.X + size, (fullscale.tracks[_s.Value.track].location_end_start.Y + 7  - 4 ) * _size - e.Y + size, 20 * _size, 20 * _size);
                }
                else if (fullscale.signals[_s.Value.name].dir_type == 0)
                {
                    //g.DrawLine(new Pen(Color.Green, 10), _last_x - e.X + size, _last_y - e.Y + size, _last_x + _n.X - e.X + size, _last_y + _n.Y - e.Y + size);
                    g.FillEllipse(Brushes.Green, (fullscale.tracks[_s.Value.track].location_start_end.X + -13 - 4) * _size - e.X + size, (fullscale.tracks[_s.Value.track].location_start_end.Y - 20  - 4 ) * _size - e.Y + size, 20 * _size, 20 * _size);
                }
                //}
                if (buffer.GetPixel(size, size).ToArgb() != Color.Black.ToArgb())
                {
                    signal_found = _s.Value.name;
                    hover_signal = _s.Value.name;
                }
                else
                {
                    hover_signal = null;
                }

                g.Dispose();
                
            }
            buffer.Dispose();

            hover_signal = signal_found;

            if (hover_signal != null)
            {
                //Console.WriteLine(hover_signal);
            }

            if (last_hover_signal != hover_signal)
            {
                suggest_routing_signal.Clear();
                suggest_routing_track.Clear();
                

                if (selected_signal != null)
                {
                    if (hover_signal != null && fullscale.signals[selected_signal].dir_type == fullscale.signals[hover_signal].dir_type)
                    {
                        routing_q_com = false;
                        routing_s_distance = -1.00;
                        ans_rev_sig_cnt = -1;
                        ans_routing_q_track.Clear();
                        ans_routing_q_signal.Clear();
                        ans_interlock.Clear();
                        route_q(fullscale.signals[selected_signal].track, fullscale.signals[hover_signal].track, 0, 0, 0, fullscale.signals[selected_signal].dir_type);
                        Console.WriteLine("rev = " + ans_rev_sig_cnt.ToString());
                        suggest_routing_signal = new List<string>(ans_routing_q_signal);
                        suggest_routing_track = new List<string>(ans_routing_q_track);
                    }

                }

                sim_panel.Invalidate();
                last_hover_signal = hover_signal;
            }


        }

        private void sim_panel_MouseDown(object sender, MouseEventArgs e)
        {

            // start reserve
            if (selected_signal != null || (network_mode == 0 && request_route.Count > 1))
            {
                if (hover_signal != null && fullscale.signals[selected_signal].dir_type == fullscale.signals[hover_signal].dir_type)
                {
                    request_route.Add(new string[2] { selected_signal, hover_signal });
                }
                else
                {
                    Console.WriteLine("error : must be same direction");
                }
                selected_signal = null;
                hover_signal = null;
            }
            // first select
            else if (hover_signal != null)
            {
                selected_signal = hover_signal;
            }

            if (deforming_packet == false)
            {
                sim_panel.Invalidate();
            }
        }

        public void reserve_route(string dest_track, string ori_track)
        {


            routing_q_com = false;
            routing_s_distance = -1.00;
            ans_rev_sig_cnt = -1;
            ans_routing_q_track.Clear();
            ans_routing_q_signal.Clear();
            //ans_interlock.Clear(); // re calculate all time

            route_q(fullscale.signals[dest_track].track, fullscale.signals[ori_track].track, 0, 0, 0, fullscale.signals[dest_track].dir_type);
            Console.WriteLine("rev = " + ans_rev_sig_cnt.ToString());

            bool contain_train = false;
            for (int i = 0; i < operate_trains.Count; i++)
            {
                if (ans_routing_q_track.Contains(operate_trains[i].track))
                {
                    contain_train = true;
                }
            }

            bool conflict_check = true;

            if (ans_routing_q_track.Count > 1)
            {

                // check conflic (track)

                foreach (operate_route _o in operate_routes)
                {

                    // check direction of first and last track      
                    if (ans_routing_q_track[0] == _o.route_queue_track[0] || ans_routing_q_track[ans_routing_q_track.Count - 1] == _o.route_queue_track[_o.route_queue_track.Count - 1])
                    {
                        conflict_check = false;
                        Console.WriteLine("track conflict (first and first or last and last)");
                    }

                    int ans_dir = fullscale.signals[dest_track].dir_type;
                    if ((ans_routing_q_track[0] == _o.route_queue_track[_o.route_queue_track.Count - 1] && contain_train && _o.train_id != -1) || (_o.route_queue_track[0] == ans_routing_q_track[ans_routing_q_track.Count - 1] && contain_train && _o.train_id != -1))
                    {
                        conflict_check = false;
                        Console.WriteLine("cannot join (both route contain train)");
                    }

                    if ((ans_routing_q_track[0] == _o.route_queue_track[_o.route_queue_track.Count - 1] && ans_dir != _o.dir) || (_o.route_queue_track[0] == ans_routing_q_track[ans_routing_q_track.Count - 1] && ans_dir != _o.dir))
                    {
                        conflict_check = false;
                        Console.WriteLine("track conflict (difference direction)");
                    }

                    // check all track                  
                    for (int i = 1; i < _o.route_queue_track.Count - 1; i++)
                    {
                        string _operate_track = _o.route_queue_track[i];
                        foreach (string _in_track in ans_routing_q_track)
                        {
                            if (_in_track == _operate_track)
                            {
                                conflict_check = false;
                                Console.WriteLine("track conflict (track already reserved)");
                            }
                        }
                    }
                }

                // check conflic (train)

                for (int i = 1; i < ans_routing_q_track.Count; i++)
                {
                    string _operate_track = ans_routing_q_track[i];
                    foreach (operate_train _train in operate_trains)
                    {
                        if (_train.track == _operate_track)
                        {
                            conflict_check = false;
                            Console.WriteLine("train conflict");
                        }
                    }
                }
            }

            // add to queue route
            if (conflict_check && ans_routing_q_track.Count > 1)
            {
                operate_routes.Add(new operate_route());
                operate_routes[operate_routes.Count - 1].route_queue_signal = new List<string>(ans_routing_q_signal);
                operate_routes[operate_routes.Count - 1].route_queue_track = new List<string>(ans_routing_q_track);
                operate_routes[operate_routes.Count - 1].dir = fullscale.signals[dest_track].dir_type;
                operate_routes[operate_routes.Count - 1].train_id = -1;

                // check if there is train on track

                for (int i = 0; i < operate_trains.Count; i++)
                {
                    if (ans_routing_q_track.Contains(operate_trains[i].track))
                    {
                        operate_routes[operate_routes.Count - 1].train_id = i;
                        operate_trains[i].route = operate_routes.Count - 1;
                    }
                }

                // re alignment route
                int added_index = operate_routes.Count - 1;
                for (int i = 0; i < operate_routes.Count - 1; i++)
                {
                    if (operate_routes[i].route_queue_track[operate_routes[i].route_queue_track.Count - 1] == operate_routes[operate_routes.Count - 1].route_queue_track[0])
                    {
                        added_index = i;
                        operate_routes[i].route_queue_track.RemoveAt(operate_routes[i].route_queue_track.Count - 1);
                        operate_routes[i].route_queue_track.AddRange(operate_routes[operate_routes.Count - 1].route_queue_track);

                        operate_routes[i].route_queue_signal.RemoveAt(operate_routes[i].route_queue_signal.Count - 1);
                        operate_routes[i].route_queue_signal.AddRange(operate_routes[operate_routes.Count - 1].route_queue_signal);

                        if (operate_routes[operate_routes.Count - 1].train_id != -1)
                        {
                            operate_routes[i].train_id = operate_routes[operate_routes.Count - 1].train_id;
                            operate_trains[operate_routes[i].train_id].route = i;
                        }

                        operate_routes.RemoveAt(operate_routes.Count - 1);

                        Console.WriteLine("join route :" + added_index.ToString() + " and " + i);
                    }
                }

                for (int i = 0; i < operate_routes.Count; i++)
                {
                    if (added_index != i && operate_routes[i].route_queue_track[0] == operate_routes[added_index].route_queue_track[operate_routes[added_index].route_queue_track.Count - 1])
                    {
                        operate_routes[added_index].route_queue_track.RemoveAt(operate_routes[added_index].route_queue_track.Count - 1);
                        operate_routes[added_index].route_queue_track.AddRange(operate_routes[i].route_queue_track);

                        operate_routes[added_index].route_queue_signal.RemoveAt(operate_routes[added_index].route_queue_signal.Count - 1);
                        operate_routes[added_index].route_queue_signal.AddRange(operate_routes[i].route_queue_signal);

                        operate_routes.RemoveAt(i);

                        if (i < added_index)
                        {
                            added_index--;
                            foreach (operate_train _train in operate_trains)
                            {
                                if (_train.route > i)
                                {
                                    _train.route--;
                                }
                                else if (_train.route == i)
                                {
                                    _train.route = added_index;
                                }
                            }
                        }

                        Console.WriteLine("join route :" + added_index.ToString() + " and " + i);
                    }
                }

                //re calculate point
                operate_routes[added_index].interlocking_point.Clear();
                if (operate_routes[added_index].dir == 0)
                {
                    for (int i = 0; i < operate_routes[added_index].route_queue_track.Count - 1; i++)
                    {
                        string _track = operate_routes[added_index].route_queue_track[i];
                        string _next_track = operate_routes[added_index].route_queue_track[i + 1];
                        foreach (string _point in fullscale.tracks[_next_track].point_end_start)
                        {
                            if (fullscale.points[_point].track_normal == _track)
                            {
                                operate_routes[added_index].interlocking_point.Add(new point_interlock(_point, 0));
                            }
                            else
                            {
                                operate_routes[added_index].interlocking_point.Add(new point_interlock(_point, 1));
                            }
                        }
                        foreach (string _point in fullscale.tracks[_track].point_start_end)
                        {
                            if (fullscale.points[_point].track_normal == _next_track)
                            {
                                operate_routes[added_index].interlocking_point.Add(new point_interlock(_point, 0));
                            }
                            else
                            {
                                operate_routes[added_index].interlocking_point.Add(new point_interlock(_point, 1));
                            }
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < operate_routes[added_index].route_queue_track.Count - 1; i++)
                    {
                        string _track = operate_routes[added_index].route_queue_track[i];
                        string _next_track = operate_routes[added_index].route_queue_track[i + 1];
                        foreach (string _point in fullscale.tracks[_next_track].point_start_end)
                        {
                            if (fullscale.points[_point].track_normal == _track)
                            {
                                operate_routes[added_index].interlocking_point.Add(new point_interlock(_point, 0));
                            }
                            else
                            {
                                operate_routes[added_index].interlocking_point.Add(new point_interlock(_point, 1));
                            }
                        }
                        foreach (string _point in fullscale.tracks[_track].point_end_start)
                        {
                            if (fullscale.points[_point].track_normal == _next_track)
                            {
                                operate_routes[added_index].interlocking_point.Add(new point_interlock(_point, 0));
                            }
                            else
                            {
                                operate_routes[added_index].interlocking_point.Add(new point_interlock(_point, 1));
                            }
                        }
                    }
                }

                // change point action
                foreach (point_interlock _point in operate_routes[added_index].interlocking_point)
                {
                    fullscale.points[_point.point].action = (_point.dir + 1);
                    foreach (physical_point.interlock _relate_point in fullscale.points[_point.point].interlocks)
                    {
                        if (_relate_point.type == 0)
                        {
                            fullscale.points[_relate_point.point].action = _point.dir + 1;
                        }
                        else
                        {
                            fullscale.points[_relate_point.point].action = (_point.dir % 2) + 1; // reverse direction
                        }
                    }
                }

                Console.WriteLine("route count :" + operate_routes.Count.ToString());
            }



        }

        public class point_interlock
        {
            public string point;
            public int dir; // 0 = normal 1 = reverse

            public point_interlock(string _p,int _dir)
            {
                point = _p;
                dir = _dir;
            }

            public point_interlock()
            {

            }
        }

        public List<string> routing_q_signal = new List<string>();
        public List<string> routing_q_track = new List<string>();
        public List<string> ans_routing_q_signal = new List<string>();
        public List<point_interlock> ans_interlock = new List<point_interlock>();
        public List<string> ans_routing_q_track = new List<string>();

        public List<string> suggest_routing_signal = new List<string>();
        public List<string> suggest_routing_track = new List<string>();

        bool routing_q_com;
        double routing_s_distance;
        int ans_rev_sig_cnt;

        public class operate_route
        {
            public List<string> route_queue_track = new List<string>();
            public List<string> route_queue_signal = new List<string>();
            //public List<point_interlock> interlock_point = new List<point_interlock>(); not nessassary

            public List<string> routing_track = new List<string>();
            public List<string> routing_signal = new List<string>();
            public List<point_interlock> interlocking_point = new List<point_interlock>();

            public int dir = -1;
            public int train_id = -1;

            public operate_route()
            {

            }
        }

        public class operate_train
        {
            public string name;
            public double speed;
            public double operate_speed;
            public string track;
            public double travel_percent;
            public int length;
            public int route;
            public int dir;
            public Point position = new Point();
            public List<string> occupied_tracks = new List<string>();

            public operate_train()
            {

            }
        }

        public void route_q(string cur_t, string des_t, int n, double distance,int rev_sig_cnt, int dir) {

            if (routing_s_distance > 0 && distance > routing_s_distance)
            {
                return;
            }

            // add track
            routing_q_track.Add(cur_t);
            if ((dir == 1 && fullscale.tracks[cur_t].signal_start_end != null) || (dir == 0 && fullscale.tracks[cur_t].signal_end_start != null))
            {
                rev_sig_cnt++;
            }

            // add signal
            if(dir == 0 && fullscale.tracks[cur_t].signal_start_end != null)
            {
                routing_q_signal.Add(fullscale.tracks[cur_t].signal_start_end);
            }
            if (dir == 1 && fullscale.tracks[cur_t].signal_end_start != null)
            {
                routing_q_signal.Add(fullscale.tracks[cur_t].signal_end_start);
            }

            // find next
            if (cur_t != des_t)
            {
                
                if (dir == 0)
                {
                    foreach (string next_track in fullscale.tracks[cur_t].next_start_end)
                    {
                        route_q(next_track, des_t, n + 1, distance + fullscale.tracks[cur_t].legnth, rev_sig_cnt, 0);
                    }
                }
                else
                {
                    foreach (string next_track in fullscale.tracks[cur_t].next_end_start)
                    {
                        route_q(next_track, des_t, n + 1, distance + fullscale.tracks[cur_t].legnth, rev_sig_cnt, 1);
                    }
                }
                
            }
            // found!!
            else
            {
                if (ans_rev_sig_cnt < 0 || rev_sig_cnt < ans_rev_sig_cnt)
                {
                    routing_s_distance = distance;
                    ans_routing_q_signal = new List<string>(routing_q_signal);
                    ans_routing_q_track = new List<string>(routing_q_track);
                    ans_rev_sig_cnt = rev_sig_cnt;
                }
            }
            routing_q_track.RemoveAt(routing_q_track.Count - 1);

            if ((dir == 0 && fullscale.tracks[cur_t].signal_start_end != null) || (dir == 1 && fullscale.tracks[cur_t].signal_end_start != null))
            {
                routing_q_signal.RemoveAt(routing_q_signal.Count - 1);
            }

        }

        public static List<Socket> clients = new List<Socket>();
        public static List<int> clients_recived_ready = new List<int>();

        public class StateObject
        {
            public Socket workSocket = null;
            public const int BufferSize = 1024 * 8;
            public byte[] buffer = new byte[BufferSize];
            public StringBuilder sb = new StringBuilder();
        }

        private void tcplistener_DoWork(object sender, DoWorkEventArgs e)
        {
            IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
            IPEndPoint localEP = new IPEndPoint(ipHostInfo.AddressList[0], 31130);

            Console.WriteLine("Local address and port : {0}", localEP.ToString());

            Socket listener = new Socket(localEP.Address.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            try
            {
                listener.Bind(localEP);
                listener.Listen(100);

                    //Console.WriteLine("Waiting for a connection...");
                    listener.BeginAccept(
                        new AsyncCallback(acceptCallback),
                        listener);
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            Console.WriteLine("Closing the listener...");
        }

        public static void acceptCallback(IAsyncResult ar)
        {
            // Get the socket that handles the client request.  
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);
            clients.Add(handler);
            clients_recived_ready.Add(1); // ready

            // Create the state object.  
            StateObject state = new StateObject();
            state.workSocket = handler;
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(Server_ReadCallback), state);

            listener.BeginAccept(
                new AsyncCallback(acceptCallback),
                listener);
        }


        public static void Server_ReadCallback(IAsyncResult ar)
        {
            String content = String.Empty;

            // Retrieve the state object and the handler socket  
            // from the asynchronous state object.  
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;

            // find client number
            int ack_number = -1;

            for (int i=0;i<clients.Count;i++) {
                if (handler == clients[i]) ack_number = i;
            }

            try
            {
                // Read data from the client socket.   
                int bytesRead = handler.EndReceive(ar);

                if (bytesRead > 0)
                {
                    // There  might be more data, so store the data received so far.  
                    state.sb.Append(Encoding.ASCII.GetString(
                        state.buffer, 0, bytesRead));

                    content = state.sb.ToString();
                    if (content.IndexOf("$") > -1)
                    {
                        // ack pecket
                        if (ack_number >= 0 && ack_number < clients_recived_ready.Count)
                        {
                            clients_recived_ready[ack_number] = 1;
                        }
                        if(content.Length > 1) // ack with reserve route
                        {
                            string[] _splited_ack = content.Split(',');
                            if (_splited_ack[0] == "C")
                            {
                                request_route.Add(new string[] { _splited_ack[1], _splited_ack[2] });
                            }
                            
                        }
                        //Console.WriteLine("got ack from " + ack_number.ToString());

                    }
                    /*
                    else if (content.IndexOf("&") > -1)
                    {
                        // cmd pecket
                        if (ack_number >= 0 && ack_number < clients_recived_ready.Count)
                        {
                            clients_recived_ready[ack_number] = 1;
                        }
                        //Console.WriteLine("got cmd from" + ack_number.ToString());
                    }*/
                    else
                    {
                        // Not all data received. Get more.  

                        
                        handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                        new AsyncCallback(Server_ReadCallback), state);
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private static void Server_Send(Socket handler, String data)
        {
            try { 
                // Convert the string data to byte data using ASCII encoding.  
                byte[] byteData = Encoding.ASCII.GetBytes(data);
                

                // Begin sending the data to the remote device.  
                handler.BeginSend(byteData, 0, byteData.Length, 0,
                    new AsyncCallback(Server_SendCallback), handler);

                StateObject state = new StateObject();
                state.workSocket = handler;

                handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(Server_ReadCallback), state);

            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private static void Server_SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket handler = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = handler.EndSend(ar);
                //Console.WriteLine("Sent {0} bytes to client.", bytesSent);

                /*
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
                */
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

        }

        private static void StartClient()
        {
            // Connect to a remote device.  
            try
            {
                // Establish the remote endpoint for the socket.  
                // The name of the   
                // remote device is "host.contoso.com".  
                //IPHostEntry ipHostInfo = Dns.GetHostEntry("host.contoso.com");
                IPAddress ipAddress = IPAddress.Parse("10.26.5.178");
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 31130);

                // Create a TCP/IP socket.  
                Socket client = new Socket(ipAddress.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);

                // Connect to the remote endpoint.  
                client.BeginConnect(remoteEP,
                    new AsyncCallback(ConnectCallback), client);


                // Send test data to the remote device.  
                //Send(client, "This is a test<EOF>");

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket client = (Socket)ar.AsyncState;

                // Complete the connection.  
                client.EndConnect(ar);

                Console.WriteLine("Socket connected to {0}",
                    client.RemoteEndPoint.ToString());

                Receive(client);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void Receive(Socket client)
        {
            try
            {
                // Create the state object.  
                StateObject state = new StateObject();
                state.workSocket = client;
              
                // Begin receiving the data from the remote device.  
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReceiveCallback), state);
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the state object and the client socket   
                // from the asynchronous state object.  
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.workSocket;

                // Read data from the remote device.  
                int bytesRead = client.EndReceive(ar);

                if (bytesRead > 0)
                {
                    // There might be more data, so store the data received so far.  
                    state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                    if (state.sb[state.sb.Length - 1] == '&')
                    {
                        Console.WriteLine("got " + state.sb.Length.ToString());
                        deforming_packet = true;
                        packet_deformer(state.sb.ToString(), out operate_routes, out operate_trains);
                        deforming_packet = false;
                        state.sb.Clear();

                        if (request_route.Count > 0)
                        {
                            Send(client, "C," + request_route[0][0] + "," + request_route[0][1] + ",$");
                            request_route.RemoveAt(0);
                        }
                        else
                        {
                            Send(client, "$");
                        }
                        //sim_panel.Invalidate();
                    }

                    // Get the rest of the data.  
                    client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                        new AsyncCallback(ReceiveCallback), state);
                }
                else
                {
                    // All the data has arrived; put it in response.  
                    if (state.sb.Length > 1)
                    {

                    }
                    // Signal that all bytes have been received.  
                    //receiveDone.Set();

                    //sim_panel.Invalidate();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void Send(Socket client, String data)
        {
            // Convert the string data to byte data using ASCII encoding.  
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.  
            client.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), client);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket client = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = client.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to server.", bytesSent);

                // Signal that all bytes have been sent.  
                //sendDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            network_mode = 0;
            tcplistener.RunWorkerAsync();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            network_mode = 1;
            StartClient();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Release the socket.  
            //client.Shutdown(SocketShutdown.Both);
            //client.Close();
        }
    }
}
