using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace rail_link_sim
{
    public class Physical_Train_Network
    {
        public enum PointAction : int { reverse = 2, normal = 1 , error = 0};

        public class physical_track_input
        {
            public String name;
            public String display_name;
            public int legnth;
            public int distance_start;
            public int distance_end;
            public int max_speed;
            public float radius;
            public float gradient;

            public int start_end_width;
            public int start_end_high;

            public physical_track_input(string _n, string _dn, int _l, int _ds, int _de, int _ms, int _w, int _h)
            {
                name = _n;
                display_name = _dn;
                legnth = _l;
                distance_start = _ds;
                distance_end = _de;
                max_speed = _ms;

                start_end_width = _w;
                start_end_high = _h;

                radius = 0;
                gradient = 0;
            }

            public physical_track_input()
            {
            }
        }
        public class physical_track : physical_track_input
        {
            /*
            public String name;
            public String display_name;
            public int legnth;
            public int distance_start;
            public int distance_end;
            public int max_speed;
            public float radius;
            public float gradient;

            public int start_end_width;
            public int start_end_high;
            */

            public string signal_start_end;
            public string signal_end_start;

            public Point location_start_end;
            public Point location_end_start;

            public List<string> next_start_end;
            public List<string> next_end_start;
            public List<string> point_start_end;
            public List<string> point_end_start;         

            public Object _o;

            public List<Point> drawing;
            public float drawing_width;

            public physical_track(string _n, string _dn, int _l, int _ds, int _de, int _ms, int _w, int _h) : base(_n, _dn, _l, _ds, _de, _ms, _w, _h)
            {

                name = _n;
                display_name = _dn;
                legnth = _l;
                distance_start = _ds;
                distance_end = _de;
                max_speed = _ms;

                start_end_width = _w;
                start_end_high = _h;

                radius = 0;
                gradient = 0;

                location_start_end = new Point();
                location_end_start = new Point();

                next_start_end = new List<string>();
                next_end_start = new List<string>();
                point_start_end = new List<string>();
                point_end_start = new List<string>();

                drawing = new List<Point>();
                drawing_width = -1;

                _o = null;

            }
            public physical_track() : base()
            {

            }


        }

        public class physical_point
        {
            public String name;
            public String display_name;
            public int action; // 1 = normal , 2 = revers
            public string track_in;
            public string track_normal;
            public string track_reverse;
            public Nullable<visual_point> visual;

            public int track_point_dir; // 0 = start_end up , 1 = start_end_down , 2 = end_start_up , 3 = end_start_down

            public class interlock
            {
                public string point;
                public int type; // -1 = default 0 = same 1 = diff

                public interlock()
                {

                }

                public interlock(string _point, int _type)
                {
                    point = _point;
                    type = _type;
                }

            }

            public List<interlock> interlocks = new List<interlock>();

            public physical_point(string _n, string _dn, string _in, string _nor, string _rev, int _track_dir)
            {
                name = _n;
                display_name = _dn;
                action = (int)PointAction.normal;
                track_in = _in;
                track_normal = _nor;
                track_reverse = _rev;
                visual = null;
                track_point_dir = _track_dir;
                interlocks = new List<interlock>();
            }

            public void set_action(int _action)
            {
                action = _action;
            }
        }

        public class Physical_signal
        {
            public string name;
            public string display_name;
            public string track;

            public int dir_type; // 0 = start_end , 1 = end_start
            public int signal_type;

            public Physical_signal(string _n, string _dn, string _track, int _dir, int _type)
            {
                name = _n;
                display_name = _dn;
                track = _track;

                dir_type = _dir;
                signal_type = _type;
            }
         
        }

        public struct visual_point
        {
            //static int vpoint_cnt = 0;
            //public string name;
            public int x;
            public int y;
            public int distance;
            public List<string> linked_track;

            public visual_point(int _x, int _y, int _d, List<string> _l)
            {
                //name = vpoint_cnt.ToString();
                //vpoint_cnt++;
                x = _x;
                y = _y;
                distance = _d;
                linked_track = _l;
            }
        }

        public class track_link
        {
            public string l1;
            public string l2;

            public track_link(string _l1, string _l2)
            {
                l1 = _l1;
                l2 = _l2;
            }

            public track_link()
            {
                l1 = "";
                l2 = "";
            }
        }
        int track_cnt = 0;
        int point_cnt = 0;
        int vpoint_cnt = 1;

        public Dictionary<String, physical_track> tracks = new Dictionary<string, physical_track>();
        public Dictionary<String, physical_point> points = new Dictionary<string, physical_point>();
        public Dictionary<String, Physical_signal> signals = new Dictionary<string, Physical_signal>();
        public List<track_link> links;
        public List<string> draw_list;
        public List<visual_point> visual_points;

        public Physical_Train_Network()
        {
            links = new List<track_link>();
            visual_points = new List<visual_point>();
        }

        public bool addTrack(string _input_file_name)
        {
            List<physical_track_input> list = new List<physical_track_input>();

            if (File.Exists(_input_file_name))
            {
                var serializer = new XmlSerializer(typeof(List<physical_track_input>));
                using (var stream = File.OpenRead(_input_file_name))
                {
                    var other = (List<physical_track_input>)(serializer.Deserialize(stream));
                    //list.Clear();
                    list.AddRange(other);
                }
            }

            foreach(physical_track_input _input_track in list)
            {
                if (!tracks.ContainsKey(_input_track.name))
                {
                    tracks.Add(_input_track.name, new physical_track(_input_track.name, _input_track.display_name, _input_track.legnth, _input_track.distance_start, _input_track.distance_end, _input_track.max_speed, _input_track.start_end_width, _input_track.start_end_high));
                    track_cnt++;
                }
            }

            return true;
        }
    
        public bool addTrack(string _n, string _dn, int _l, int _ds, int _de, int _ms, int _w, int _h)
        {
            if (!tracks.ContainsKey(_n))
            {
                tracks.Add(_n, new physical_track(_n, _dn, _l, _ds, _de, _ms, _w, _h));
                track_cnt++;
                return true;
            }
            else
            {
                return false;
            }
        }
        

        public bool addLink(string _scr, string _dest)
        {
            
            if (tracks.ContainsKey(_scr) && tracks.ContainsKey(_dest))
            {
                foreach (track_link _t in links) // check redun
                {
                    if ((_t.l1 == _scr && _t.l2 == _dest) || (_t.l2 == _scr && _t.l1 == _dest)) return false;
                }
                links.Add(new track_link(_scr, _dest));
                tracks[_scr].next_start_end.Add(_dest);
                tracks[_dest].next_end_start.Add(_scr);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool addLink(string _input_file_name)
        {

            List<track_link> list = new List<track_link>();

            if (File.Exists(_input_file_name))
            {
                var serializer = new XmlSerializer(typeof(List<track_link>));
                using (var stream = File.OpenRead(_input_file_name))
                {
                    var other = (List<track_link>)(serializer.Deserialize(stream));
                    //list.Clear();
                    list.AddRange(other);
                }
            }

            foreach (track_link _l in list)
            {
                string _scr = _l.l1;
                string _dest = _l.l2;

                if (tracks.ContainsKey(_scr) && tracks.ContainsKey(_dest))
                {
                    foreach (track_link _t in links) // check redun
                    {
                        if ((_t.l1 == _scr && _t.l2 == _dest) || (_t.l2 == _scr && _t.l1 == _dest)) continue;
                    }
                    links.Add(new track_link(_scr, _dest));
                    tracks[_scr].next_start_end.Add(_dest);
                    tracks[_dest].next_end_start.Add(_scr);
                    //return true;
                }
                else
                {
                    //return false;
                }
            }

            return true;
        }

        public bool addSignal(string _n, string _dn, string _track, int _dir, int _type)
        {
            if (!signals.ContainsKey(_n) && tracks.ContainsKey(_track))
            {
                signals.Add(_n, new Physical_signal(_n, _dn, _track, _dir, _type));
                if(_dir == 0)
                {
                    tracks[_track].signal_start_end = _n;
                }
                else
                {
                    tracks[_track].signal_end_start = _n;
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool addPoint(string _n, string _in, string _nor, string _rev)
        {
            if (tracks.ContainsKey(_in) && tracks.ContainsKey(_nor) && tracks.ContainsKey(_rev))
            {
                if (tracks[_in].next_start_end.Contains(_nor))
                {
                    if (tracks[_rev].start_end_high < 0)
                    {
                        points.Add(_n, new physical_point(_n, _n, _in, _nor, _rev, 0));
                    }
                    else
                    {
                        points.Add(_n, new physical_point(_n, _n, _in, _nor, _rev, 1));
                    }
                    tracks[_in].point_start_end.Add(_n);
                    foreach (visual_point v_point in visual_points)
                    {
                        if (v_point.linked_track.Contains(_in) && v_point.linked_track.Contains(_nor) && v_point.linked_track.Contains(_rev))
                        {
                            physical_point _p = points[_n];
                            _p.visual = v_point;
                            points[_n] = _p;
                        }
                    }

                    point_cnt++;

                    return true;
                }
                else if (tracks[_in].next_end_start.Contains(_nor))
                {
                    if (tracks[_rev].start_end_high < 0)
                    {
                        points.Add(_n, new physical_point(_n, _n, _in, _nor, _rev, 2));
                    }
                    else
                    {
                        points.Add(_n, new physical_point(_n, _n, _in, _nor, _rev, 3));
                    }
                    tracks[_in].point_end_start.Add(_n);
                    point_cnt++;
                    foreach (visual_point v_point in visual_points)
                    {
                        if (v_point.linked_track.Contains(_in) && v_point.linked_track.Contains(_nor) && v_point.linked_track.Contains(_rev))
                        {
                            physical_point _p = points[_n];
                            _p.visual = v_point;
                            points[_n] = _p;
                        }
                    }

                    return true;
                }
                else
                {
                    return false;
                }

            }
            else
            {
                return false;
            }
        }

        public bool add_point_interlock(string p1, string p2, int type)
        {
            bool ret = true;

            if(points.ContainsKey(p1) && points.ContainsKey(p2))
            {
                points[p1].interlocks.Add(new physical_point.interlock(p2,type));
                points[p2].interlocks.Add(new physical_point.interlock(p1, type));
            }

            return ret;
        }

        private void compile_track(string _n, int _x, int _y, int _r)
        {
            physical_track _t = tracks[_n];
            Pen blackPen = new Pen(Color.LightGray, 3);
            if (!draw_list.Contains(tracks[_n].name))
            {
                draw_list.Add(tracks[_n].name);
                if (_r == 0)
                { // normal draw

                    _t.location_end_start.X = _x;
                    _t.location_end_start.Y = _y;
                    _t.location_start_end.X = _x + _t.start_end_width;
                    _t.location_start_end.Y = _y + _t.start_end_high;
                    tracks[_n] = _t;
                    // add point

                    List<string> _point_link = new List<string>(_t.next_start_end);

                    foreach (string _link_name in _t.next_start_end)
                    {
                        foreach (string _return_track in tracks[_link_name].next_end_start)
                        {
                            if (!_point_link.Contains(_return_track)) _point_link.Add(_return_track);
                        }
                    }

                    visual_point _visual_p = new visual_point(_t.location_start_end.X, _t.location_start_end.Y, _t.distance_end, _point_link);
                    visual_points.Add(_visual_p);

                    foreach (string next_t in _t.next_start_end)
                    {
                        compile_track(tracks[next_t].name, _x + _t.start_end_width, _y + _t.start_end_high, 0);
                    }
                    foreach (string next_t in _t.next_end_start)
                    {
                        compile_track(tracks[next_t].name, _x, _y, 1);
                    }
                }
                else
                { // revers
                  //e.Graphics.DrawLine(blackPen, _x, _y, _x - _t.start_end_width, _y - _t.start_end_high);
                    _t.location_start_end.X = _x;
                    _t.location_start_end.Y = _y;
                    _t.location_end_start.X = _x - _t.start_end_width;
                    _t.location_end_start.Y = _y - _t.start_end_high;
                    tracks[_n] = _t;

                    List<string> _point_link = new List<string>(_t.next_end_start);

                    foreach (string _link_name in _t.next_end_start)
                    {
                        foreach (string _return_track in tracks[_link_name].next_start_end)
                        {
                            if (!_point_link.Contains(_return_track)) _point_link.Add(_return_track);
                        }
                    }

                    visual_point _visual_p = new visual_point(_t.location_end_start.X, _t.location_end_start.Y, _t.distance_start, _point_link);
                    visual_points.Add(_visual_p);

                    foreach (string next_t in _t.next_start_end)
                    {
                        compile_track(tracks[next_t].name, _x, _y, 0);
                    }
                    foreach (string next_t in _t.next_end_start)
                    {
                        compile_track(tracks[next_t].name, _x - _t.start_end_width, _y - _t.start_end_high, 1);
                    }
                }
            }

        }

        public void draw_graphic_signal()
        {

        }

        public void draw_graphic_track()
        {
            foreach (KeyValuePair<string, physical_track> _t in tracks)
            {
                List<Point> _m_point = new List<Point>();
                float drawing_width = 0;
                if (_t.Value.start_end_high == 0)
                {
                    drawing_width = 8;

                    _m_point.Add(new Point(_t.Value.location_end_start.X + 1 , _t.Value.location_end_start.Y - 3));
                    _m_point.Add(new Point(_t.Value.location_end_start.X + 1 + _t.Value.start_end_width - 2, _t.Value.location_end_start.Y - 3));
                    _m_point.Add(new Point(_t.Value.location_end_start.X + 1 + _t.Value.start_end_width - 2, _t.Value.location_end_start.Y - 3 + 6));
                    _m_point.Add(new Point(_t.Value.location_end_start.X + 1 , _t.Value.location_end_start.Y - 3 + 6));

                }
                else if (_t.Value.start_end_high > 0)
                {
                    // check add draw

                    bool draw_s_e_add = true;
                    bool draw_e_s_add = true;

                    foreach (string _1_link in _t.Value.next_start_end)
                    {
                        if (tracks[_1_link].next_end_start.Count > 1) draw_s_e_add = false;
                    }
                    foreach (string _1_link in _t.Value.next_end_start)
                    {
                        if (tracks[_1_link].next_start_end.Count > 1) draw_e_s_add = false;
                    }

                    drawing_width = (draw_s_e_add ^ draw_e_s_add) ? (float)11 : 8;

                    // strat draw track

                    if (_t.Value.location_start_end.X > _t.Value.location_end_start.X)
                    {
                        if (draw_s_e_add == false)
                        {
                            _m_point.Add(new Point(_t.Value.location_start_end.X - 8 - 1, _t.Value.location_start_end.Y - 3 - 2));
                            _m_point.Add(new Point(_t.Value.location_start_end.X - 1, _t.Value.location_start_end.Y - 3 - 2));
                        }
                        else
                        {
                            _m_point.Add(new Point(_t.Value.location_start_end.X - 8 - 2, _t.Value.location_start_end.Y + 3));
                            _m_point.Add(new Point(_t.Value.location_start_end.X - 1, _t.Value.location_start_end.Y + 3));
                            _m_point.Add(new Point(_t.Value.location_start_end.X - 1, _t.Value.location_start_end.Y - 3));
                            _m_point.Add(new Point(_t.Value.location_start_end.X - 2 - 4, _t.Value.location_start_end.Y - 3));
                        }

                        if (draw_e_s_add == false)
                        {
                            _m_point.Add(new Point(_t.Value.location_end_start.X + 8 + 1, _t.Value.location_end_start.Y + 3 + 2));
                            _m_point.Add(new Point(_t.Value.location_end_start.X + 1, _t.Value.location_end_start.Y + 3 + 2));
                        }
                        else
                        {
                            _m_point.Add(new Point(_t.Value.location_end_start.X + 8 + 2, _t.Value.location_end_start.Y - 3));
                            _m_point.Add(new Point(_t.Value.location_end_start.X + 1, _t.Value.location_end_start.Y - 3));
                            _m_point.Add(new Point(_t.Value.location_end_start.X + 1, _t.Value.location_end_start.Y + 3));
                            _m_point.Add(new Point(_t.Value.location_end_start.X + 2 + 4, _t.Value.location_end_start.Y + 3));
                        }
                        
                    }
                    else
                    {
 
                        if (draw_e_s_add == false)
                        {
                            _m_point.Add(new Point(_t.Value.location_end_start.X - 8 - 0, _t.Value.location_end_start.Y - 3 - 2));
                            _m_point.Add(new Point(_t.Value.location_end_start.X - 0, _t.Value.location_end_start.Y - 3 - 2));
                        }
                        else
                        {
                            _m_point.Add(new Point(_t.Value.location_end_start.X - 8 - 2, _t.Value.location_end_start.Y + 3));
                            _m_point.Add(new Point(_t.Value.location_end_start.X - 1, _t.Value.location_end_start.Y + 3));
                            _m_point.Add(new Point(_t.Value.location_end_start.X - 1, _t.Value.location_end_start.Y - 3));
                            _m_point.Add(new Point(_t.Value.location_end_start.X - 2 - 4, _t.Value.location_end_start.Y - 3));
                        }

                        if (draw_s_e_add == false)
                        {
                            _m_point.Add(new Point(_t.Value.location_start_end.X + 8, _t.Value.location_start_end.Y + 3 + 2));
                            _m_point.Add(new Point(_t.Value.location_start_end.X , _t.Value.location_start_end.Y + 3 + 2));
                        }
                        else
                        {
                            _m_point.Add(new Point(_t.Value.location_start_end.X + 8 + 2, _t.Value.location_start_end.Y - 3));
                            _m_point.Add(new Point(_t.Value.location_start_end.X + 1, _t.Value.location_start_end.Y - 3));
                            _m_point.Add(new Point(_t.Value.location_start_end.X + 1, _t.Value.location_start_end.Y + 3));
                            _m_point.Add(new Point(_t.Value.location_start_end.X + 2 + 4, _t.Value.location_start_end.Y + 3));
                        }

                    }

                }
                else if (_t.Value.start_end_high < 0)
                {
                    // check add draw

                    bool draw_s_e_add = true;
                    bool draw_e_s_add = true;

                    foreach (string _1_link in _t.Value.next_start_end)
                    {
                        if (tracks[_1_link].next_end_start.Count > 1) draw_s_e_add = false;
                    }
                    foreach (string _1_link in _t.Value.next_end_start)
                    {
                        if (tracks[_1_link].next_start_end.Count > 1) draw_e_s_add = false;
                    }

                    drawing_width = (draw_s_e_add ^ draw_e_s_add) ? 11 : 8;
                    // strat draw

                    if (_t.Value.location_start_end.X > _t.Value.location_end_start.X)
                    {

                        if (draw_s_e_add == false)
                        {
                            _m_point.Add(new Point(_t.Value.location_start_end.X - 8 - 1, _t.Value.location_start_end.Y + 3 + 2));
                            _m_point.Add(new Point(_t.Value.location_start_end.X - 1, _t.Value.location_start_end.Y + 3 + 2));
                        }
                        else
                        {
                            _m_point.Add(new Point(_t.Value.location_start_end.X - 8 - 2, _t.Value.location_start_end.Y - 3));
                            _m_point.Add(new Point(_t.Value.location_start_end.X - 1, _t.Value.location_start_end.Y - 3));
                            _m_point.Add(new Point(_t.Value.location_start_end.X - 1, _t.Value.location_start_end.Y + 3));
                            _m_point.Add(new Point(_t.Value.location_start_end.X - 2 - 4, _t.Value.location_start_end.Y + 3));
                        }

                        if (draw_e_s_add == false)
                        {
                            _m_point.Add(new Point(_t.Value.location_end_start.X + 8 + 1, _t.Value.location_end_start.Y - 3 - 2));
                            _m_point.Add(new Point(_t.Value.location_end_start.X + 1, _t.Value.location_end_start.Y - 3 - 2));
                        }
                        else
                        {
                            _m_point.Add(new Point(_t.Value.location_end_start.X + 8 + 2, _t.Value.location_end_start.Y + 3));
                            _m_point.Add(new Point(_t.Value.location_end_start.X + 1, _t.Value.location_end_start.Y + 3));
                            _m_point.Add(new Point(_t.Value.location_end_start.X + 1, _t.Value.location_end_start.Y - 3));
                            _m_point.Add(new Point(_t.Value.location_end_start.X + 2 + 4, _t.Value.location_end_start.Y - 3));
                        }
                    }
                    else
                    {

                        if (draw_e_s_add == false)
                        {
                            _m_point.Add(new Point(_t.Value.location_end_start.X - 8 - 2, _t.Value.location_end_start.Y + 3 + 2));
                            _m_point.Add(new Point(_t.Value.location_end_start.X - 2, _t.Value.location_end_start.Y + 3 + 2));
                        }
                        else
                        {
                            _m_point.Add(new Point(_t.Value.location_end_start.X - 8 - 2, _t.Value.location_end_start.Y - 3));
                            _m_point.Add(new Point(_t.Value.location_end_start.X - 1, _t.Value.location_end_start.Y - 3));
                            _m_point.Add(new Point(_t.Value.location_end_start.X - 1, _t.Value.location_end_start.Y + 3));
                            _m_point.Add(new Point(_t.Value.location_end_start.X - 2 - 4, _t.Value.location_end_start.Y + 3));
                        }

                        if (draw_s_e_add == false)
                        {
                            _m_point.Add(new Point(_t.Value.location_start_end.X + 8 + 2, _t.Value.location_start_end.Y - 3 - 2));
                            _m_point.Add(new Point(_t.Value.location_start_end.X + 2, _t.Value.location_start_end.Y - 3 - 2));
                        }
                        else
                        {
                            _m_point.Add(new Point(_t.Value.location_start_end.X + 8 + 2, _t.Value.location_start_end.Y + 3));
                            _m_point.Add(new Point(_t.Value.location_start_end.X + 1, _t.Value.location_start_end.Y + 3));
                            _m_point.Add(new Point(_t.Value.location_start_end.X + 1, _t.Value.location_start_end.Y - 3));
                            _m_point.Add(new Point(_t.Value.location_start_end.X + 2 + 4, _t.Value.location_start_end.Y - 3));
                        }

                    }

                }

                _t.Value.drawing_width = drawing_width;
                _t.Value.drawing = new List<Point>(_m_point);
               
            }// end foreach draw track
        }
        
        public void verify_track(string _first, int _x, int _y, int _dir)
        {
            draw_list = new List<string>();
            draw_list.Clear();

            List<string> _point_link = new List<string>(tracks[_first].next_start_end);

            foreach (string _link_name in tracks[_first].next_start_end)
            {
                foreach (string _return_track in tracks[_link_name].next_end_start)
                {
                    if (!_point_link.Contains(_return_track)) _point_link.Add(_return_track);
                }
            }

            visual_point _visual_p = new visual_point(_x, _y, 0, _point_link);
            visual_points.Add(_visual_p);

            compile_track(_first, _x, _y, _dir);
            draw_graphic_track();
            draw_graphic_signal();

        }

        private void paint_event(object sender, PaintEventArgs e)
        {

            Pen blackPen = new Pen(Color.Gray, 4);
            Pen PointPen = new Pen(Color.White, 2);
            Pen big = new Pen(Color.Pink, 5);

            e.Graphics.TranslateTransform(((Panel)sender).AutoScrollPosition.X, ((Panel)sender).AutoScrollPosition.Y);


            foreach (KeyValuePair<string, physical_track> _t in tracks)
            {
                //DrawnRectangle dr = new DrawnRectangle(new RectangleF(24, 24, 200, 200), _index, 0, 0, new Pen(Color.Blue, 4));
                e.Graphics.DrawLine(PointPen, _t.Value.location_end_start, _t.Value.location_start_end);
            }
            foreach (visual_point _vp in visual_points)
            {
                e.Graphics.DrawEllipse(blackPen, _vp.x - 5, _vp.y - 5, 10, 10);
            }
            foreach (KeyValuePair<string, physical_point> _p in points)
            {
                e.Graphics.DrawEllipse(big, _p.Value.visual.Value.x - 7, _p.Value.visual.Value.y - 7, 14, 14);
            }


        }

        public void draw_physical_track(Panel _p)
        {

            _p.Paint += new System.Windows.Forms.PaintEventHandler(paint_event);
            _p.Refresh();

        }

        public class Planning_sch
        {
            public string station_name;
            public List<string> tracks;
            public int width;
            public int size;
            public int position_x;

            public Planning_sch(string _st, List<string> _tn, int _w, int _s, int _x)
            {
                station_name = _st;
                tracks = _tn;
                width = _w;
                size = _s;
                position_x = _x;
            }
        }

        public class Physical_route
        {
            public string name;
            public List<string> tracks;
            public List<string> selected_track;
            public List<string> stop_point;

            public override string ToString()
            {
                return name;
            }

        }

        public class maintenance_routine
        {
            public string name;
            public int used_distance; // meter
            public int used_time; // day
            public int maintain_time; // day

            public maintenance_routine(string _name, int _used_distance, int _used_time, int _maintain_time)
            {
                name = _name;
                used_distance = _used_distance;
                used_time = _used_time;
                maintain_time = _maintain_time;
            }
        }
        public class maintenance_plan
        {
            public List<maintenance_routine> level = new List<maintenance_routine>();
        }

        public class Physical_Train
        {
            public string name;
            public double acc;
            public int length;
            public int max_speed;
            public int type;
            public maintenance_plan maintenamce_plan;

            public Physical_Train(Physical_Train _train)
            {
                name = _train.name;
                acc = _train.acc;
                length = _train.length;
                max_speed = _train.max_speed;
                type = _train.type;
                maintenamce_plan = _train.maintenamce_plan;
            }

            public Physical_Train()
            {

            }

            public override string ToString()
            {
                return name;
            }
        }

        public class Service_Plan
        {
            public string name;
            public List<Service_Course> courses;

            public Service_Plan()
            {

            }
            public Service_Plan(Service_Plan service_Plan)
            {
                name = service_Plan.name;
                courses = service_Plan.courses;
            }

            public override string ToString()
            {
                return name;
            }
        }

        public class Service_Course
        {
            public string name;
            public string train;
            public Physical_route route;
            public DateTime start_time;

            public Service_Course()
            {

            }

            public Service_Course(string _n,string _t,Physical_route _r,DateTime _st)
            {
                name = _n;
                train = _t;
                start_time = _st;
                route = _r;

                //start_time = new DateTime();
            }

            public override string ToString()
            {
                return name;
            }
        }



        //public class timed_paln

        public class speed_point
        {
            public float speed;
            public int track;
            public float track_travel;
            public float time_travel;
            public float length_travel;
            public float graphic_x;
            public float graphic_y;

            public speed_point()
            {

            }
            public speed_point(float _s,int _t,float _p)
            {
                speed = _s;
                track = _t;
                track_travel = _p;
                time_travel = 0;
                length_travel = 0;
                graphic_x = 0;
                graphic_y = 0;
            }
            public speed_point(speed_point _np)
            {
                speed = _np.speed;
                track = _np.track;
                track_travel = _np.track_travel;
                time_travel = _np.time_travel;
                length_travel = _np.length_travel;
                graphic_x = _np.graphic_x;
                graphic_y = _np.graphic_y;
            }
        }

        public class timed_course : Service_Course
        {

            public List<speed_point> max_speed_points;
            public List<speed_point> operate_speed_points;

            public List<float> max_speeds = new List<float>();
            public List<float> operate_speeds = new List<float>();

            public List<float> dewell_time = new List<float>();
            public List<int> dewell_pos = new List<int>();

            public int next_course = -1;
            public int previous_course = -1;

            public int start_sec = 0;
            public timed_course()
            {

            }

            public timed_course(Service_Course s)
            {
                name = s.name;
                train = s.train;
                start_time = s.start_time;
                route = s.route;
                max_speed_points = new List<speed_point>();
                operate_speed_points = new List<speed_point>();
                dewell_time = new List<float>();
            }
        }

        public class conflict_trip
        {
            public int[] trip_index = new int[2];
            public int[] start_point_index = new int[2];
            public int[] end_point_index = new int[2];
        }

        public class train_schdule
        {
            public List<timed_course> trips;
        
        }

    }

}
