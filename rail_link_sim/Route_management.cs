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
    public partial class Route_management : Form
    {
        public Route_management()
        {
            InitializeComponent();
        }

        class track_shortest_pass
        {
            public string track;
            public int cost;

            public track_shortest_pass(string _t, int _c)
            {
                track = _t;
                cost = _c;
            }
        }

        Physical_Train_Network fullscale = new Physical_Train_Network();
        List<PictureBox> track_icon;
        List<string> selected_tracks = new List<string>();
        List<string> suggested_tracks = new List<string>();
        List<track_shortest_pass> rec_chk = new List<track_shortest_pass>();
        List<string> suggested_run = new List<string>();
        List<string> suggested_sub = new List<string>();
        List<string> stop_track = new List<string>();
        List<string> tmp_stop_track = new List<string>();

        int command_flag = 0; // 0=nothing , 1=add, 2 =edit
        int plan_mode = 0; // 1 = plan route 2 = plan stop point
        int edit_item = 0;
        int suggest_min = 0;

        List<Physical_route> routes = new List<Physical_route>();

        private void paint_event(object sender, PaintEventArgs e)
        {

            Pen blackPen = new Pen(Color.LightGray, 7);
            Pen PointPen = new Pen(Color.White, 7);
            Pen big = new Pen(Color.Pink, 7);

            e.Graphics.TranslateTransform(((Panel)sender).AutoScrollPosition.X, ((Panel)sender).AutoScrollPosition.Y);


            /*foreach (KeyValuePair<string, physical_track> _t in fullscale.tracks)
            {
                e.Graphics.DrawLine(PointPen, _t.Value.location_end_start, _t.Value.location_start_end);
            }*/
            /*foreach (visual_point _vp in fullscale.visual_points)
            {
                e.Graphics.DrawEllipse(blackPen, _vp.x - 5, _vp.y - 5, 10, 10);
            }*/


            foreach (KeyValuePair<string, physical_point> _p in fullscale.points)
            {
                e.Graphics.DrawEllipse(big, _p.Value.visual.Value.x - 7, _p.Value.visual.Value.y - 7, 14, 14);
            }


        }

        public void draw_physical_track(Panel _p)
        {



            foreach (visual_point _vp in fullscale.visual_points)
            {
                PictureBox pictureBox1 = new PictureBox();
                pictureBox1.BackColor = Color.Transparent;
                pictureBox1.Location = new System.Drawing.Point(_vp.x - 8, _vp.y - 8);
                pictureBox1.Name = "pictureBox1";
                pictureBox1.Size = new System.Drawing.Size(16, 16);
                pictureBox1.Image = rail_link_sim.Properties.Resources.vertex_icon;
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                splitContainer1.Panel1.Controls.Add(pictureBox1);
                //e.Graphics.DrawEllipse(blackPen, _vp.x - 5, _vp.y - 5, 10, 10);
            }

            foreach (KeyValuePair<string, physical_track> _t in fullscale.tracks)
            {
                PictureBox pictureBox1 = new PictureBox();
                Pen mypen = new Pen(Color.White, 7);
                //e.Graphics.DrawLine(PointPen, _t.Value.location_end_start, _t.Value.location_start_end);
                if (_t.Value.start_end_high == 0)
                {
                    Bitmap DrawArea;
                    pictureBox1.BackColor = Color.White;
                    pictureBox1.Location = new System.Drawing.Point(_t.Value.location_end_start.X + 3, _t.Value.location_end_start.Y - 3);
                    pictureBox1.Name = _t.Value.name;
                    pictureBox1.Size = new System.Drawing.Size(_t.Value.start_end_width - 6, 6);
                    DrawArea = new Bitmap(pictureBox1.Size.Width, pictureBox1.Size.Height);
                    Graphics g;
                    g = Graphics.FromImage(DrawArea);
                    g.DrawRectangle(mypen, 0, 0, pictureBox1.Size.Width, pictureBox1.Size.Height);
                    pictureBox1.Image = DrawArea;
                }
                else if (_t.Value.start_end_high > 0)
                {
                    Bitmap DrawArea;
                    pictureBox1.BackColor = Color.Transparent;
                    pictureBox1.Location = new System.Drawing.Point(_t.Value.location_end_start.X + 3, _t.Value.location_end_start.Y+3);
                    pictureBox1.Name = _t.Value.name;
                    pictureBox1.Size = new System.Drawing.Size(_t.Value.start_end_width - 6, (_t.Value.start_end_high) - 6);
                    DrawArea = new Bitmap(pictureBox1.Size.Width, pictureBox1.Size.Height);
                    Graphics g;
                    g = Graphics.FromImage(DrawArea);
                    if (_t.Value.location_start_end.X > _t.Value.location_end_start.X)
                    {
                        g.DrawLine(mypen, 0, 0, pictureBox1.Size.Width, pictureBox1.Size.Height);
                        //g.Dispose();
                    }
                    else
                    {
                        g.DrawLine(mypen, 0, 0, pictureBox1.Size.Width, pictureBox1.Size.Height);
                        //g.Dispose();
                    }
                    pictureBox1.Image = DrawArea;

                }
                else if (_t.Value.start_end_high < 0)
                {
                    Bitmap DrawArea;
                    pictureBox1.BackColor = Color.Transparent;
                    pictureBox1.Location = new System.Drawing.Point(_t.Value.location_end_start.X + 3, _t.Value.location_end_start.Y + _t.Value.start_end_high + 3);
                    pictureBox1.Name = _t.Value.name;
                    pictureBox1.Size = new System.Drawing.Size(_t.Value.start_end_width - 6, (-_t.Value.start_end_high) - 6);
                    DrawArea = new Bitmap(pictureBox1.Size.Width, pictureBox1.Size.Height);
                    Graphics g;
                    g = Graphics.FromImage(DrawArea);
                    if (_t.Value.location_start_end.X > _t.Value.location_end_start.X)
                    {
                        g.DrawLine(mypen, 0, pictureBox1.Size.Height, pictureBox1.Size.Width, 0);
                        //g.Dispose();
                    }
                    else
                    {
                        g.DrawLine(mypen, 0, 0, pictureBox1.Size.Width, pictureBox1.Size.Height);
                        //g.Dispose();
                    }
                    pictureBox1.Image = DrawArea;

                }

                

                pictureBox1.MouseDown += new MouseEventHandler(track_down_event);
                pictureBox1.MouseMove += new MouseEventHandler(track_on_event);
                pictureBox1.MouseLeave += new EventHandler(track_out_event);

                splitContainer1.Panel1.Controls.Add(pictureBox1);
                _t.Value._o = pictureBox1;
                //fullscale.tracks[_t.Value.name] = tmp_t;
                track_icon.Add(pictureBox1);

            }


            _p.Paint += new System.Windows.Forms.PaintEventHandler(paint_event);
            _p.Refresh();

        }

        int find_route(List<track_shortest_pass> _tracks, string _target)
        {
            int ret = -1; // if there no track
            for (int i = 0; i < _tracks.Count; i++)
            {
                if (_tracks[i].track.Equals(_target))
                {
                    ret = i;
                    break;
                }
            }
            return ret;
        }

        void suggest_route(string _current, string _goal, int _pass)
        {
            int _f = find_route(rec_chk, _current);

            if (_f == -1)
            {
                rec_chk.Add(new track_shortest_pass(_current, _pass));
                if (_current.Equals(_goal))
                {
                    suggested_sub.Clear();
                    suggested_sub = new List<string>(suggested_run);
                }
            }
            else if (_pass >= rec_chk[_f].cost)
            {
                return;
            }
            else
            {
                rec_chk[_f].cost = _pass;
                if (_current.Equals(_goal))
                {
                    suggested_sub.Clear();
                    suggested_sub = new List<string>(suggested_run);
                }
            }


            foreach (string _t in fullscale.tracks[_current].next_start_end)
            {
                suggested_run.Add(_t);
                suggest_route(_t, _goal, _pass + 1);
                suggested_run.RemoveAt(suggested_run.Count - 1);
            }
            foreach (string _t in fullscale.tracks[_current].next_end_start)
            {
                suggested_run.Add(_t);
                suggest_route(_t, _goal, _pass + 1);
                suggested_run.RemoveAt(suggested_run.Count - 1);
            }
        }

        void run_suggest(string _current, string _goal, int _pass, int _min)
        {
            int _f = find_route(rec_chk, _current);
            if (_f == -1)
            {
                rec_chk.Add(new track_shortest_pass(_current, _pass));
            }
            else if (_pass >= rec_chk[_f].cost)
            {
                return;
            }
            else
            {
                rec_chk[_f].cost = _pass;
            }

            foreach (string _t in fullscale.tracks[_current].next_start_end)
            {
                suggest_route(_t, _goal, _pass + 1);
            }
            foreach (string _t in fullscale.tracks[_current].next_end_start)
            {
                suggest_route(_t, _goal, _pass + 1);
            }
        }

        void track_out_event(object sender, EventArgs e)
        {
            PictureBox o = (PictureBox)sender;

            if (true)
            {
                if (!selected_tracks.Contains(o.Name) && !suggested_tracks.Contains(o.Name))
                {
                    Pen mypen = new Pen(Color.White, 7);
                    draw_track(mypen, o, 0);
                }
            }


        }
        void track_on_event(object sender, MouseEventArgs e)
        {
            Color pixelColour;
            PictureBox o = (PictureBox)sender;

            int px = e.X * o.Image.Width / o.ClientSize.Width;
            int py = e.Y * o.Image.Height / o.ClientSize.Height;
            pixelColour = ((Bitmap)o.Image).GetPixel(px, py);
            Console.Write(o.Name + " ");
            Console.Write(px + " " + py);
            Console.WriteLine(pixelColour.ToString());
            if (pixelColour.A != 0)
            {
                if (!selected_tracks.Contains(o.Name) && !suggested_tracks.Contains(o.Name))
                {
                    Pen mypen = new Pen(Color.Orange, 7);
                    draw_track(mypen, o, 0);
                }
            }
            else
            {
                if (!selected_tracks.Contains(o.Name) && !suggested_tracks.Contains(o.Name))
                {
                    Pen mypen = new Pen(Color.White, 7);
                    draw_track(mypen, o, 0);
                }
            }
        }

        void track_down_event(object sender, MouseEventArgs e)
        {
            Color pixelColour;
            PictureBox o = (PictureBox)sender;

            int px = e.X * o.Image.Width / o.ClientSize.Width;
            int py = e.Y * o.Image.Height / o.ClientSize.Height;
            pixelColour = ((Bitmap)o.Image).GetPixel(px, py);

            Console.Write(o.Name + " ");
            Console.Write(px + " " + py);
            Console.WriteLine(pixelColour.ToString());
            if (pixelColour.A != 0 && command_flag!=0)
            {
                if (plan_mode == 1)
                {
                    if (!selected_tracks.Contains(o.Name))
                    {
                        selected_tracks.Add(o.Name);
                        Pen mypen = new Pen(Color.Red, 7);
                        draw_track(mypen, o, 0);
                    }
                    else
                    {
                        Pen mypen = new Pen(Color.White, 7);
                        selected_tracks.Remove(o.Name);

                        foreach (string _track_name in suggested_tracks)
                        {
                            if (!selected_tracks.Contains(_track_name))
                            {
                                draw_track(mypen, (PictureBox)fullscale.tracks[_track_name]._o, 0);
                            }
                        }
                        
                    }

                    suggested_tracks.Clear();

                    if (selected_tracks.Count > 0)
                    {

                        suggested_tracks.Add(selected_tracks[0]);
                        if (selected_tracks.Count >= 2) // start suggest
                        {
                            for (int j = 0; j < selected_tracks.Count - 1; j++)
                            {
                                rec_chk.Clear();
                                suggest_route(selected_tracks[j], selected_tracks[j + 1], 0);
                                suggest_min = find_route(rec_chk, selected_tracks[j + 1]);
                                for (int i = 0; i < suggested_sub.Count - 1; i++)
                                {
                                    Pen pen2 = new Pen(Color.LightPink, 7);
                                    draw_track(pen2, (PictureBox)fullscale.tracks[suggested_sub[i]]._o, 0);
                                    suggested_tracks.Add(suggested_sub[i]);
                                }
                                suggested_tracks.Add(selected_tracks[j + 1]);
                            }

                        }
                    }

                    // clear stop point if the suggest track wa clear
                    tmp_stop_track = new List<string>(stop_track);
                    foreach (string _track_name in tmp_stop_track)
                    {
                        if (!suggested_tracks.Contains(_track_name))
                        {
                            stop_track.Remove(_track_name);
                        }
                    }

                    stop_point_view.Rows.Clear();
                    foreach (string _track_name in stop_track)
                    {
                        stop_point_view.Rows.Add(_track_name);
                    }

                    selected_gridview.Rows.Clear();
                    foreach (string _trach_name in suggested_tracks)
                    {
                        selected_gridview.Rows.Add(_trach_name, 0);
                    }
                }
                else if(plan_mode == 2)
                {
                    if (stop_track.Contains(o.Name))
                    {
                        stop_track.Remove(o.Name);
                        //Refresh_track();
                        stop_point_view.Rows.Clear();
                        foreach (string _track_name in stop_track)
                        {
                            stop_point_view.Rows.Add(_track_name);
                        }
                    }
                    else if (suggested_tracks.Contains(o.Name))
                    {
                        stop_track.Add(o.Name);
                        stop_point_view.Rows.Clear();
                        foreach (string _track_name in stop_track)
                        {
                            stop_point_view.Rows.Add(_track_name);
                        }
                    }
                }
                Refresh_track();

            }

        }

        public void draw_track(Pen _p, PictureBox _o,int _mark)
        {
            if (fullscale.tracks[_o.Name].start_end_high == 0)
            {
                Bitmap DrawArea;
                DrawArea = new Bitmap(_o.Size.Width, _o.Size.Height);
                Graphics g;
                g = Graphics.FromImage(DrawArea);
                g.DrawRectangle(_p, 0, 0, _o.Size.Width, _o.Size.Height);
                if (_mark == 1)
                {
 
                    g.FillRectangle(Brushes.Yellow, (float)0.30 * (float)_o.Size.Width, 0, (float)0.30 * (float)_o.Size.Width, _o.Size.Height);
                }
                //_o.BackColor = Color.LightPink;
                _o.Image = DrawArea;
            }
            else if (fullscale.tracks[_o.Name].start_end_high > 0)
            {
                Bitmap DrawArea;
                DrawArea = new Bitmap(_o.Size.Width, _o.Size.Height);
                Graphics g;
                g = Graphics.FromImage(DrawArea);
                if (fullscale.tracks[_o.Name].location_start_end.X > fullscale.tracks[_o.Name].location_end_start.X)
                {
                    g.DrawLine(_p, 0, 0, _o.Size.Width, _o.Size.Height);
                    if (_mark == 1)
                    {
                        Pen mypen = new Pen(Color.Yellow, 7);
                        g.DrawLine(mypen, (float)0.30 * _o.Size.Width, (float)0.30 * _o.Size.Height, (float)0.70 * _o.Size.Width, (float)0.70*_o.Size.Height);
                    }
                    //g.Dispose();
                }
                else
                {
                    g.DrawLine(_p, 0, 0, _o.Size.Width, _o.Size.Height);
                    if (_mark == 1)
                    {
                        Pen mypen = new Pen(Color.Yellow, 7);
                        g.DrawLine(mypen, (float)0.30 * _o.Size.Width, (float)0.30 * _o.Size.Height, (float)0.70 * _o.Size.Width, (float)0.70 * _o.Size.Height);
                    }
                }
                _o.Image = DrawArea;

            }
            else if (fullscale.tracks[_o.Name].start_end_high < 0)
            {
                Bitmap DrawArea;
                DrawArea = new Bitmap(_o.Size.Width, _o.Size.Height);
                Graphics g;
                g = Graphics.FromImage(DrawArea);
                if (fullscale.tracks[_o.Name].location_start_end.X > fullscale.tracks[_o.Name].location_end_start.X)
                {
                    g.DrawLine(_p, 0, _o.Size.Height, _o.Size.Width, 0);
                    if (_mark == 1)
                    {
                        Pen mypen = new Pen(Color.Yellow, 7);
                        g.DrawLine(mypen, (float)0.30 * _o.Size.Width, (float)0.70 * _o.Size.Height, (float)0.70 * _o.Size.Width, (float)0.30 * _o.Size.Height);
                    }
                }
                else
                {
                    g.DrawLine(_p, 0, 0, _o.Size.Width, _o.Size.Height);
                    if (_mark == 1)
                    {
                        Pen mypen = new Pen(Color.Yellow, 7);
                        g.DrawLine(mypen, (float)0.30 * _o.Size.Width, (float)0.30 * _o.Size.Height, (float)0.70 * _o.Size.Width, (float)0.70 * _o.Size.Height);
                    }
                    //g.Dispose();
                }
                _o.Image = DrawArea;

            }
        }


        private void Route_management_Load(object sender, EventArgs e)
        {
            track_icon = new List<PictureBox>();

            Deserialize(routes, "somsin.xml");

            /*

            fullscale.addTrack("ST11", "ST11", 100000, 0, 1000, 20, 100, 0);
            fullscale.addTrack("T0000", "T0000", 50000, 1000, 1500, 40, 50, 0);
            fullscale.addTrack("T0001", "T0001", 50000, 1500, 2000, 40, 50, 0);
            fullscale.addTrack("T0002", "T0002", 50000, 2000, 2500, 60, 50, 0);
            fullscale.addTrack("T0003", "T0003", 600000, 2500, 8500, 150, 50, 0);
            fullscale.addTrack("T0004", "T0004", 600000, 8500, 14500, 90, 50, 0);
            fullscale.addTrack("T0005", "T0005", 50000, 14500, 15000, 90, 50, 0);
            fullscale.addTrack("T0006", "T0006", 300000, 15000, 18000, 150, 50, 0);
            fullscale.addTrack("T0007", "T0007", 50000, 18000, 18500, 90, 50, 0);
            fullscale.addTrack("T0008", "T0008", 300000, 18500, 21500, 60, 50, 0);
            fullscale.addTrack("T0009", "T0009", 600000, 21500, 27500, 90, 50, 0);
            fullscale.addTrack("T0010", "T0010", 600000, 27500, 33500, 150, 50, 0);
            fullscale.addTrack("T0011", "T0011", 900000, 33500, 42500, 200, 50, 0);
            fullscale.addTrack("T0012", "T0012", 600000, 42500, 48500, 150, 50, 0);
            fullscale.addTrack("T0013", "T0013", 50000, 48500, 49000, 80, 50, 0);
            fullscale.addTrack("T0014", "T0014", 300000, 49000, 52000, 80, 50, 0);
            fullscale.addTrack("T0015", "T0015", 50000, 52000, 52500, 80, 50, 0);
            fullscale.addTrack("T0016", "T0016", 600000, 52500, 58500, 150, 50, 0);
            fullscale.addTrack("T0017", "T0017", 300000, 58500, 61500, 80, 50, 0);
            fullscale.addTrack("T0018", "T0018", 50000, 61500, 62000, 60, 50, -50);
            fullscale.addTrack("ST21", "ST21", 100000, 62000, 63000, 20, 100, 0);

            fullscale.addTrack("T0019", "T0019", 50000, 0, 62000, 60, 50, 0);
            fullscale.addTrack("ST22", "ST22", 100000, 62000, 63000, 20, 100, 0);

            fullscale.addTrack("ST12", "ST12", 100000, 0, 1000, 20, 100, 0);
            fullscale.addTrack("T0020", "T0020", 50000, 1000, 1500, 40, 50, 0);
            fullscale.addTrack("T0021", "T0021", 50000, 1500, 2000, 40, 50, -50);

            fullscale.addTrack("ST13", "ST13", 100000, 0, 1000, 20, 100, 0);
            fullscale.addTrack("T0040", "T0040", 50000, 1000, 1500, 40, 50, -50);

            fullscale.addTrack("T0022", "T0022", 50000, 2000, 2500, 60, 50, -100);
            fullscale.addTrack("T0023", "T0023", 600000, 2500, 8500, 150, 50, 0);
            fullscale.addTrack("T0024", "T0024", 600000, 8500, 14500, 90, 50, 0);
            fullscale.addTrack("T0025", "T0025", 50000, 14500, 15000, 90, 50, 0);
            fullscale.addTrack("T0026", "T0026", 300000, 15000, 18000, 150, 50, 0);
            fullscale.addTrack("T0027", "T0027", 50000, 18000, 18500, 90, 50, 0);
            fullscale.addTrack("T0028", "T0028", 300000, 18500, 21500, 60, 50, 0);
            fullscale.addTrack("T0029", "T0029", 600000, 21500, 27500, 90, 50, 0);
            fullscale.addTrack("T0030", "T0030", 600000, 27500, 33500, 150, 50, 0);
            fullscale.addTrack("T0031", "T0031", 900000, 33500, 42500, 200, 50, 0);
            fullscale.addTrack("T0032", "T0032", 600000, 42500, 48500, 150, 50, 0);
            fullscale.addTrack("T0033", "T0033", 50000, 48500, 49000, 80, 50, 0);
            fullscale.addTrack("T0034", "T0034", 300000, 49000, 52000, 80, 50, 0);
            fullscale.addTrack("T0035", "T0035", 50000, 52000, 52500, 80, 50, 0);
            fullscale.addTrack("T0036", "T0036", 600000, 52500, 58500, 150, 50, 0);
            fullscale.addTrack("T0037", "T0037", 300000, 58500, 61500, 80, 50, 0);
            fullscale.addTrack("T0038", "T0038", 50000, 61500, 62000, 60, 50, 0);
            fullscale.addTrack("ST23", "ST23", 100000, 62000, 63000, 20, 100, 0);

            fullscale.addTrack("T0039", "T0039", 50000, 0, 62000, 60, 50, 50);
            fullscale.addTrack("ST24", "ST24", 100000, 62000, 63000, 20, 100, 0);

            fullscale.addTrack("ST14", "ST14", 100000, 0, 1000, 20, 100, 0);
            fullscale.addTrack("T0041", "T0041", 50000, 1000, 1500, 40, 50, 0);
            fullscale.addTrack("T0042", "T0042", 50000, 1500, 2000, 40, 50, 0);

            fullscale.addTrack("ST15", "ST15", 100000, 0, 1000, 20, 100, 0);
            fullscale.addTrack("T0043", "T0043", 50000, 1000, 1500, 40, 50, 0);
            fullscale.addTrack("T0044", "T0044", 50000, 1500, 2000, 40, 50, -50);

            fullscale.addTrack("ST16", "ST16", 100000, 0, 1000, 20, 100, 0);
            fullscale.addTrack("T0045", "T0045", 50000, 1000, 1500, 40, 50, -50);

            fullscale.addTrack("T0046", "T0046", 50000, 0, 15000, 90, 50, 50);
            fullscale.addTrack("T0047", "T0047", 50000, 15000, 18500, 90, 50, -50);
            fullscale.addTrack("T0048", "T0048", 50000, 48500, 49000, 80, 50, 50);
            fullscale.addTrack("T0049", "T0049", 50000, 52000, 52500, 80, 50, -50);

            fullscale.addLink("ST11", "T0000");
            fullscale.addLink("T0000", "T0001");
            fullscale.addLink("T0001", "T0002");
            fullscale.addLink("T0002", "T0003");
            fullscale.addLink("T0003", "T0004");
            fullscale.addLink("T0004", "T0005");
            fullscale.addLink("T0005", "T0006");
            fullscale.addLink("T0006", "T0007");
            fullscale.addLink("T0007", "T0008");
            fullscale.addLink("T0008", "T0009");
            fullscale.addLink("T0009", "T0010");
            fullscale.addLink("T0010", "T0011");
            fullscale.addLink("T0011", "T0012");
            fullscale.addLink("T0012", "T0013");
            fullscale.addLink("T0013", "T0014");
            fullscale.addLink("T0014", "T0015");
            fullscale.addLink("T0015", "T0016");
            fullscale.addLink("T0016", "T0017");
            fullscale.addLink("T0017", "T0018");
            fullscale.addLink("T0018", "ST21");

            fullscale.addLink("T0017", "T0019");
            fullscale.addLink("T0019", "ST22");

            fullscale.addLink("ST12", "T0020");
            fullscale.addLink("T0020", "T0021");
            fullscale.addLink("T0021", "T0002");

            fullscale.addLink("ST13", "T0040");
            fullscale.addLink("T0040", "T0021");

            fullscale.addLink("ST14", "T0041");
            fullscale.addLink("T0041", "T0042");
            fullscale.addLink("T0042", "T0022");
            fullscale.addLink("T0022", "T0023");
            fullscale.addLink("T0023", "T0024");
            fullscale.addLink("T0024", "T0025");
            fullscale.addLink("T0025", "T0026");
            fullscale.addLink("T0026", "T0027");
            fullscale.addLink("T0027", "T0028");
            fullscale.addLink("T0028", "T0029");
            fullscale.addLink("T0029", "T0030");
            fullscale.addLink("T0030", "T0031");
            fullscale.addLink("T0031", "T0032");
            fullscale.addLink("T0032", "T0033");
            fullscale.addLink("T0033", "T0034");
            fullscale.addLink("T0034", "T0035");
            fullscale.addLink("T0035", "T0036");
            fullscale.addLink("T0036", "T0037");
            fullscale.addLink("T0037", "T0038");
            fullscale.addLink("T0038", "ST23");

            fullscale.addLink("T0037", "T0039");
            fullscale.addLink("T0039", "ST24");

            fullscale.addLink("T0004", "T0046");
            fullscale.addLink("T0046", "T0026");

            fullscale.addLink("T0026", "T0047");
            fullscale.addLink("T0047", "T0008");

            fullscale.addLink("T0012", "T0048");
            fullscale.addLink("T0048", "T0034");

            fullscale.addLink("T0034", "T0049");
            fullscale.addLink("T0049", "T0016");

            fullscale.addLink("ST15", "T0043");
            fullscale.addLink("T0043", "T0044");
            fullscale.addLink("T0044", "T0022");

            fullscale.addLink("ST16", "T0045");
            fullscale.addLink("T0045", "T0044");

            //fullscale.addTrack(name,  display_name,   length,     start_kilo,     end_kilo,   max_speed,  width   ,   high)
            // ATU to SRB
            fullscale.addTrack("T1000", "T1000", 100000, 63, 64, 50, 50, 50);
            fullscale.addTrack("T1001", "T1001", 100000, 63, 64, 50, 50, 0);
            fullscale.addTrack("T1002", "T1002", 200000, 64, 66, 80, 50, 0);
            fullscale.addTrack("T1003", "T1003", 200000, 66, 68, 150, 50, 0);
            fullscale.addTrack("T1026", "T1026", 200000, 66, 68, 50, 50, 50);
            fullscale.addTrack("T1004", "T1004", 200000, 68, 70, 150, 50, 0);
            fullscale.addTrack("T1005", "T1005", 200000, 70, 72, 150, 50, 0);
            fullscale.addTrack("T1027", "T1027", 200000, 70, 72, 50, 50, -50);
            fullscale.addTrack("T1006", "T1006", 200000, 72, 74, 150, 50, 0);
            fullscale.addTrack("T1007", "T1007", 200000, 74, 76, 150, 50, 0);
            fullscale.addTrack("T1028", "T1028", 200000, 74, 76, 50, 50, 50);
            fullscale.addTrack("T1008", "T1008", 200000, 76, 78, 150, 50, 0);
            fullscale.addTrack("T1009", "T1009", 200000, 78, 80, 150, 50, 0);
            fullscale.addTrack("T1029", "T1029", 200000, 78, 80, 50, 50, -50);
            fullscale.addTrack("T1010", "T1010", 2000000, 80, 100, 250, 150, 0);
            fullscale.addTrack("T1011", "T1011", 400000, 100, 104, 120, 150, 0);
            fullscale.addTrack("T1012", "T1012", 200000, 104, 106, 60, 50, -50);
            fullscale.addTrack("T1030", "T1030", 200000, 104, 106, 60, 50, 0);
            fullscale.addTrack("T1014", "T1014", 100000, 63, 64, 50, 50, 0);
            fullscale.addTrack("T1013", "T1013", 100000, 63, 64, 50, 50, -50);
            fullscale.addTrack("T1015", "T1015", 200000, 64, 66, 80, 50, 0);
            fullscale.addTrack("T1016", "T1016", 200000, 66, 68, 150, 50, 0);
            fullscale.addTrack("T1017", "T1017", 200000, 68, 70, 150, 50, 0);
            fullscale.addTrack("T1018", "T1018", 200000, 70, 72, 150, 50, 0);
            fullscale.addTrack("T1019", "T1019", 200000, 72, 74, 150, 50, 0);
            fullscale.addTrack("T1020", "T1020", 200000, 74, 76, 150, 50, 0);
            fullscale.addTrack("T1021", "T1021", 200000, 76, 78, 150, 50, 0);
            fullscale.addTrack("T1022", "T1022", 200000, 78, 80, 150, 50, 0);

            fullscale.addTrack("T1023", "T1023", 200000, 80, 100, 250, 150, 0);
            fullscale.addTrack("T1024", "T1024", 400000, 100, 104, 120, 150, 0);
            fullscale.addTrack("T1025", "T1025", 200000, 104, 106, 60, 50, 50);
            fullscale.addTrack("T1031", "T1031", 200000, 104, 106, 60, 50, 0);

            fullscale.addTrack("ST31", "ST31", 100000, 106, 107, 60, 100, 0);
            fullscale.addTrack("ST32", "ST32", 100000, 106, 107, 60, 100, 0);
            fullscale.addTrack("ST33", "ST33", 100000, 106, 107, 60, 100, 0);
            fullscale.addTrack("ST34", "ST34", 100000, 106, 107, 60, 100, 0);

            fullscale.addLink("ST21", "T1000");
            fullscale.addLink("ST22", "T1001");
            fullscale.addLink("T1001", "T1002");
            fullscale.addLink("T1000", "T1002");
            fullscale.addLink("T1002", "T1003");
            fullscale.addLink("T1003", "T1004");
            fullscale.addLink("T1004", "T1005");
            fullscale.addLink("T1005", "T1006");
            fullscale.addLink("T1006", "T1007");
            fullscale.addLink("T1007", "T1008");
            fullscale.addLink("T1008", "T1009");
            fullscale.addLink("T1009", "T1010");
            fullscale.addLink("T1010", "T1011");
            fullscale.addLink("T1011", "T1012");
            fullscale.addLink("T1011", "T1030");
            fullscale.addLink("T1012", "ST31");
            fullscale.addLink("T1030", "ST32");

            fullscale.addLink("ST23", "T1014");
            fullscale.addLink("ST24", "T1013");
            fullscale.addLink("T1014", "T1015");
            fullscale.addLink("T1013", "T1015");
            fullscale.addLink("T1015", "T1016");
            fullscale.addLink("T1016", "T1017");
            fullscale.addLink("T1017", "T1018");
            fullscale.addLink("T1018", "T1019");
            fullscale.addLink("T1019", "T1020");
            fullscale.addLink("T1020", "T1021");
            fullscale.addLink("T1021", "T1022");
            fullscale.addLink("T1022", "T1023");
            fullscale.addLink("T1023", "T1024");
            fullscale.addLink("T1024", "T1025");
            fullscale.addLink("T1024", "T1031");
            fullscale.addLink("T1031", "ST33");
            fullscale.addLink("T1025", "ST34");

            fullscale.addLink("T1002", "T1026");
            fullscale.addLink("T1026", "T1017");

            fullscale.addLink("T1017", "T1027");
            fullscale.addLink("T1027", "T1006");

            fullscale.addLink("T1006", "T1028");
            fullscale.addLink("T1028", "T1021");

            fullscale.addLink("T1021", "T1029");
            fullscale.addLink("T1029", "T1010");

            // SRB 107 to PKC 174
            fullscale.addTrack("T2001", "T2001", 100000, 107, 108, 50, 50, 50);
            fullscale.addTrack("T2002", "T2002", 100000, 107, 108, 50, 50, 0);
            fullscale.addTrack("T2003", "T2003", 200000, 108, 110, 80, 50, 0);
            fullscale.addTrack("T2004", "T2004", 200000, 110, 112, 150, 50, 0);
            fullscale.addTrack("T2025", "T2025", 200000, 110, 112, 50, 50, 50);
            fullscale.addTrack("T2005", "T2005", 200000, 112, 114, 150, 50, 0);
            fullscale.addTrack("T2006", "T2006", 200000, 114, 116, 150, 50, 0);
            fullscale.addTrack("T2026", "T2026", 200000, 114, 116, 50, 50, -50);
            fullscale.addTrack("T2007", "T2007", 200000, 116, 118, 150, 50, 0);
            fullscale.addTrack("T2008", "T2008", 200000, 118, 120, 150, 50, 0);
            fullscale.addTrack("T2027", "T2027", 200000, 118, 120, 50, 50, 50);
            fullscale.addTrack("T2009", "T2009", 2600000, 120, 146, 200, 50, 0);
            fullscale.addTrack("T2010", "T2010", 2600000, 146, 172, 150, 50, 0);
            fullscale.addTrack("T2011", "T2011", 100000, 172, 173, 50, 50, -50);
            fullscale.addTrack("T2012", "T2012", 100000, 172, 173, 50, 50, 0);

            fullscale.addTrack("T2014", "T2014", 100000, 107, 108, 50, 50, -50);
            fullscale.addTrack("T2013", "T2013", 100000, 107, 108, 50, 50, 0);
            fullscale.addTrack("T2015", "T2015", 200000, 108, 110, 80, 50, 0);
            fullscale.addTrack("T2016", "T2016", 200000, 110, 112, 150, 50, 0);
            fullscale.addTrack("T2017", "T2017", 200000, 112, 114, 150, 50, 0);
            fullscale.addTrack("T2018", "T2018", 200000, 114, 116, 150, 50, 0);
            fullscale.addTrack("T2019", "T2019", 200000, 116, 118, 150, 50, 0);
            fullscale.addTrack("T2020", "T2020", 200000, 118, 120, 150, 50, 0);
            fullscale.addTrack("T2021", "T2021", 2600000, 120, 146, 200, 50, 0);
            fullscale.addTrack("T2022", "T2022", 2600000, 146, 172, 150, 50, 0);
            fullscale.addTrack("T2024", "T2024", 100000, 172, 173, 50, 50, 50);
            fullscale.addTrack("T2023", "T2112", 100000, 172, 173, 50, 50, 0);

            fullscale.addTrack("ST41", "ST41", 100000, 173, 174, 60, 100, 0);
            fullscale.addTrack("ST42", "ST42", 100000, 173, 174, 60, 100, 0);
            fullscale.addTrack("ST43", "ST43", 100000, 173, 174, 60, 100, 0);
            fullscale.addTrack("ST44", "ST44", 100000, 173, 174, 60, 100, 0);

            fullscale.addLink("ST31", "T2001");
            fullscale.addLink("ST32", "T2002");
            fullscale.addLink("T2001", "T2003");
            fullscale.addLink("T2002", "T2003");
            fullscale.addLink("T2003", "T2004");
            fullscale.addLink("T2004", "T2005");
            fullscale.addLink("T2005", "T2006");
            fullscale.addLink("T2006", "T2007");
            fullscale.addLink("T2007", "T2008");
            fullscale.addLink("T2008", "T2009");
            fullscale.addLink("T2009", "T2010");
            fullscale.addLink("T2010", "T2011");
            fullscale.addLink("T2010", "T2012");
            fullscale.addLink("T2011", "ST41");
            fullscale.addLink("T2012", "ST42");

            fullscale.addLink("ST33", "T2013");
            fullscale.addLink("ST34", "T2014");
            fullscale.addLink("T2013", "T2015");
            fullscale.addLink("T2014", "T2015");
            fullscale.addLink("T2015", "T2016");
            fullscale.addLink("T2016", "T2017");
            fullscale.addLink("T2017", "T2018");
            fullscale.addLink("T2018", "T2019");
            fullscale.addLink("T2019", "T2020");
            fullscale.addLink("T2020", "T2021");
            fullscale.addLink("T2021", "T2022");
            fullscale.addLink("T2022", "T2023");
            fullscale.addLink("T2022", "T2024");
            fullscale.addLink("T2023", "ST43");
            fullscale.addLink("T2024", "ST44");

            fullscale.addLink("T2003", "T2025");
            fullscale.addLink("T2025", "T2017");

            fullscale.addLink("T2017", "T2026");
            fullscale.addLink("T2026", "T2007");

            fullscale.addLink("T2007", "T2027");
            fullscale.addLink("T2027", "T2021");

            // PKC 174 to NRS 252
            fullscale.addTrack("T3000", "T3000", 100000, 174, 175, 50, 50, 50);
            fullscale.addTrack("T3001", "T3001", 100000, 174, 175, 50, 50, 0);
            fullscale.addTrack("T3002", "T3002", 200000, 175, 177, 80, 50, 0);
            fullscale.addTrack("T3003", "T3003", 200000, 177, 179, 150, 50, 0);
            fullscale.addTrack("T3036", "T3036", 200000, 177, 179, 50, 50, 50);
            fullscale.addTrack("T3004", "T3004", 200000, 179, 181, 150, 50, 0);
            fullscale.addTrack("T3005", "T3005", 200000, 181, 183, 150, 50, 0);
            fullscale.addTrack("T3037", "T3037", 200000, 181, 183, 50, 50, -50);
            fullscale.addTrack("T3006", "T3006", 200000, 183, 185, 150, 50, 0);
            fullscale.addTrack("T3007", "T3007", 200000, 185, 187, 150, 50, 0);
            fullscale.addTrack("T3038", "T3038", 200000, 185, 187, 50, 50, 50);
            fullscale.addTrack("T3008", "T3008", 200000, 187, 189, 150, 50, 0);
            fullscale.addTrack("T3009", "T3009", 200000, 187, 189, 150, 50, 0);
            fullscale.addTrack("T3039", "T3039", 200000, 187, 189, 150, 50, -50);
            fullscale.addTrack("T3010", "T3010", 400000, 189, 192, 150, 50, 0);
            fullscale.addTrack("T3011", "T3011", 3200000, 192, 224, 150, 200, 0);
            fullscale.addTrack("T3012", "T3012", 200000, 224, 226, 250, 50, 0);
            fullscale.addTrack("T3013", "T3013", 2000000, 226, 246, 150, 150, 0);
            fullscale.addTrack("T3014", "T3014", 200000, 226, 246, 60, 50, 0);
            fullscale.addTrack("T3040", "T3040", 200000, 226, 246, 60, 50, 50);
            fullscale.addTrack("T3015", "T3015", 400000, 246, 250, 120, 50, 0);
            fullscale.addTrack("T3016", "T3016", 200000, 250, 252, 60, 50, -50);
            fullscale.addTrack("T3017", "T3017", 200000, 250, 252, 60, 50, 0);

            fullscale.addTrack("T3019", "T3019", 100000, 174, 175, 50, 50, -50);
            fullscale.addTrack("T3018", "T3018", 100000, 174, 175, 50, 50, 0);
            fullscale.addTrack("T3020", "T3020", 200000, 175, 177, 80, 50, 0);
            fullscale.addTrack("T3021", "T3021", 200000, 177, 179, 150, 50, 0);
            fullscale.addTrack("T3022", "T3022", 200000, 179, 181, 150, 50, 0);
            fullscale.addTrack("T3023", "T3023", 200000, 181, 183, 150, 50, 0);
            fullscale.addTrack("T3024", "T3024", 200000, 183, 185, 150, 50, 0);
            fullscale.addTrack("T3025", "T3025", 200000, 185, 187, 150, 50, 0);
            fullscale.addTrack("T3026", "T3026", 200000, 187, 189, 150, 50, 0);
            fullscale.addTrack("T3027", "T3027", 200000, 187, 189, 150, 50, 0);
            fullscale.addTrack("T3028", "T3028", 400000, 189, 192, 150, 50, 0);
            fullscale.addTrack("T3029", "T3029", 3200000, 192, 224, 150, 200, 0);
            fullscale.addTrack("T3030", "T3030", 200000, 224, 226, 250, 50, 0);
            fullscale.addTrack("T3031", "T3031", 2000000, 226, 246, 150, 150, 0);
            fullscale.addTrack("T3032", "T3032", 200000, 226, 246, 60, 50, 0);
            fullscale.addTrack("T3033", "T3033", 400000, 246, 250, 120, 50, 0);
            fullscale.addTrack("T3034", "T3034", 200000, 250, 252, 60, 50, 50);
            fullscale.addTrack("T3035", "T3035", 200000, 250, 252, 60, 50, 0);

            fullscale.addTrack("ST51", "ST51", 100000, 251, 252, 60, 100, 0);
            fullscale.addTrack("ST52", "ST52", 100000, 251, 252, 60, 100, 0);
            fullscale.addTrack("ST53", "ST53", 100000, 251, 252, 60, 100, 0);
            fullscale.addTrack("ST54", "ST54", 100000, 251, 252, 60, 100, 0);

            fullscale.addLink("ST41", "T3000");
            fullscale.addLink("ST42", "T3001");
            fullscale.addLink("T3000", "T3002");
            fullscale.addLink("T3001", "T3002");
            fullscale.addLink("T3002", "T3003");
            fullscale.addLink("T3003", "T3004");
            fullscale.addLink("T3004", "T3005");
            fullscale.addLink("T3005", "T3006");
            fullscale.addLink("T3006", "T3007");
            fullscale.addLink("T3007", "T3008");
            fullscale.addLink("T3008", "T3009");
            fullscale.addLink("T3009", "T3010");
            fullscale.addLink("T3010", "T3011");
            fullscale.addLink("T3011", "T3012");
            fullscale.addLink("T3012", "T3013");
            fullscale.addLink("T3013", "T3014");
            fullscale.addLink("T3014", "T3015");
            fullscale.addLink("T3015", "T3016");
            fullscale.addLink("T3015", "T3017");
            fullscale.addLink("T3016", "ST51");
            fullscale.addLink("T3017", "ST52");

            fullscale.addLink("ST43", "T3018");
            fullscale.addLink("ST44", "T3019");
            fullscale.addLink("T3018", "T3020");
            fullscale.addLink("T3019", "T3020");
            fullscale.addLink("T3020", "T3021");
            fullscale.addLink("T3021", "T3022");
            fullscale.addLink("T3022", "T3023");
            fullscale.addLink("T3023", "T3024");
            fullscale.addLink("T3024", "T3025");
            fullscale.addLink("T3025", "T3026");
            fullscale.addLink("T3026", "T3027");
            fullscale.addLink("T3027", "T3028");
            fullscale.addLink("T3028", "T3029");
            fullscale.addLink("T3029", "T3030");
            fullscale.addLink("T3030", "T3031");
            fullscale.addLink("T3031", "T3032");
            fullscale.addLink("T3032", "T3033");
            fullscale.addLink("T3033", "T3034");
            fullscale.addLink("T3033", "T3035");
            fullscale.addLink("T3034", "ST53");
            fullscale.addLink("T3035", "ST54");

            fullscale.addLink("T3002", "T3036");
            fullscale.addLink("T3036", "T3022");

            fullscale.addLink("T3022", "T3037");
            fullscale.addLink("T3037", "T3006");

            fullscale.addLink("T3006", "T3038");
            fullscale.addLink("T3038", "T3026");

            fullscale.addLink("T3026", "T3039");
            fullscale.addLink("T3039", "T3010");

            fullscale.addLink("T3011", "T3040");
            fullscale.addLink("T3040", "T3031");

            fullscale.addLink("T3031", "T3050");
            fullscale.addLink("T3050", "T3015");
            */

            fullscale.addTrack("physical_tracks.xml");
            fullscale.addLink("track_links.xml");

            fullscale.verify_track("ST11", 50, 100, 0);
                     

            //fullscale.chk("T0001",5111);
            draw_physical_track(splitContainer1.Panel1);
            Refresh_route_view();

            //fullscale.print_file();

        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {

        }

        private void route_save_box_Click(object sender, EventArgs e)
        {
            if (command_flag != 0)
            { // fucking add remove all color!!!!
                if (route_name_box.Text == "")
                {
                    MessageBox.Show("input name!!");
                    return;
                }
                {

                    if (command_flag == 1) // add
                    {
                        Physical_route temp_route = new Physical_route();
                        temp_route.name = route_name_box.Text;
                        temp_route.tracks = new List<string>(suggested_tracks);
                        temp_route.selected_track = new List<string>(selected_tracks);
                        temp_route.stop_point = new List<string>(stop_track);
                        routes.Add(temp_route);
                    }
                    else if(command_flag == 2) //edit
                    {
                        routes[edit_item].selected_track = new List<string>(selected_tracks);
                        routes[edit_item].tracks = new List<string>(suggested_tracks);
                        routes[edit_item].stop_point = new List<string>(stop_track);
                    }

                    var serializer = new XmlSerializer(typeof(List<Physical_route>));
                    using (var stream = File.Open("somsin.xml",FileMode.Create))
                    {
                        serializer.Serialize(stream, routes);
                        stream.Close();
                    }


                    splitContainer2.Panel2.Enabled = false;

                    foreach(string _tt in suggested_tracks)
                    {
                        Pen mypen = new Pen(Color.White, 7);
                        draw_track(mypen, (PictureBox)fullscale.tracks[_tt]._o, 0);
                    }
                    selected_tracks.Clear();
                    suggested_tracks.Clear();
                    
                }
                command_flag = 0;
                Refresh_route_view();
                splitContainer2.Panel1.Enabled = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            splitContainer2.Panel2.Enabled = true;
            route_name_box.Text = "";
            selected_gridview.Rows.Clear();
            stop_point_view.Rows.Clear();

            suggested_tracks = new List<string>();
            selected_tracks = new List<string>();
            stop_track = new List<string>();

            command_flag = 1;
            plan_mode = 1;
            route_but.Enabled = false;
            stop_but.Enabled = true;
            splitContainer2.Panel1.Enabled = false;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            splitContainer2.Panel2.Enabled = false;
            selected_gridview.Rows.Clear();

            suggested_tracks = new List<string>();
            selected_tracks = new List<string>();
            stop_track = new List<string>();

            stop_point_view.Rows.Clear();
            Refresh_track();

            command_flag = 0;
            splitContainer2.Panel1.Enabled = true;
            Refresh_route_view();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(route_view.SelectedCells.Count == 1)
            {
                command_flag = 2; // edit
                splitContainer2.Panel2.Enabled = true;
                suggested_tracks = new List<string>(routes[route_view.SelectedCells[0].RowIndex].tracks);
                selected_tracks = new List<string>(routes[route_view.SelectedCells[0].RowIndex].selected_track);
                stop_track = new List<string>(routes[route_view.SelectedCells[0].RowIndex].stop_point);
                edit_item = route_view.SelectedCells[0].RowIndex;

                Refresh_track();

                selected_gridview.Rows.Clear();
                foreach (string _trach_name in suggested_tracks)
                {
                    selected_gridview.Rows.Add(_trach_name, 0);
                }

                stop_point_view.Rows.Clear();
                foreach (string _trach_name in stop_track)
                {
                    stop_point_view.Rows.Add(_trach_name, 0);
                }

                plan_mode = 1;
                route_but.Enabled = false;
                stop_but.Enabled = true;

                route_name_box.Text = routes[route_view.SelectedCells[0].RowIndex].name;

                splitContainer2.Panel1.Enabled = false;

            }

        }

        public class Physical_route
        {
            public string name;
            public List<string> tracks;
            public List<string> selected_track;
            public List<string> stop_point;
        }
        private void button5_Click(object sender, EventArgs e)
        {


        }

        public void Deserialize(List<Physical_route> list, string fileName)
        {
            if (File.Exists("somsin.xml"))
            {
                var serializer = new XmlSerializer(typeof(List<Physical_route>));
                using (var stream = File.OpenRead("somsin.xml"))
                {
                    var other = (List<Physical_route>)(serializer.Deserialize(stream));
                    list.Clear();
                    list.AddRange(other);
                }
            }
        }

        public void Refresh_track()
        {
            foreach (KeyValuePair<string, physical_track> _t in fullscale.tracks)
            {
                Pen mypen = new Pen(Color.White, 7);
                if(stop_track.Contains(_t.Value.name)) draw_track(mypen, (PictureBox)_t.Value._o, 1);
                else draw_track(mypen, (PictureBox)_t.Value._o, 0);
            }
            foreach (string _t in suggested_tracks)
            {
                Pen mypen = new Pen(Color.Pink, 7);
                if (stop_track.Contains(_t)) draw_track(mypen, (PictureBox)fullscale.tracks[_t]._o, 1);
                else draw_track(mypen, (PictureBox)fullscale.tracks[_t]._o, 0);
            }
            foreach (string _t in selected_tracks)
            {
                Pen mypen = new Pen(Color.Red, 7);
                if (stop_track.Contains(_t)) draw_track(mypen, (PictureBox)fullscale.tracks[_t]._o, 1);
                else draw_track(mypen, (PictureBox)fullscale.tracks[_t]._o, 0);
            }
            /*
            foreach (string _t in stop_track)
            {
                Pen mypen = new Pen(Color.Orange, 7);
                draw_track(mypen, (PictureBox)fullscale.tracks[_t]._o, 1);
            }*/
        }
        
        public void Refresh_route_view()
        {
            route_view.Rows.Clear();
            route_view.Rows.Clear();
            foreach (Physical_route _pr in routes)
            {
                route_view.Rows.Add(_pr.name, _pr.tracks.Count, _pr.tracks.ToString());

            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (route_view.SelectedCells.Count == 1)
            {
                routes.RemoveAt(route_view.SelectedCells[0].RowIndex);
                var serializer = new XmlSerializer(typeof(List<Physical_route>));
                using (var stream = File.Open("somsin.xml", FileMode.Create))
                {
                    serializer.Serialize(stream, routes);

                    stream.Close();
                }
                Refresh_route_view();
            }
        }

        private void route_but_Click(object sender, EventArgs e)
        {
            plan_mode = 1;
            route_but.Enabled = false;
            stop_but.Enabled = true;
        }

        private void stop_but_Click(object sender, EventArgs e)
        {
            plan_mode = 2;
            stop_but.Enabled = false;
            route_but.Enabled = true;
        }

        private void Route_management_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(command_flag != 0)
            {
                if (MessageBox.Show("You didn't save the route now\nAre you sure to exit?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    e.Cancel = true;
                }
            }
        }
    }
}