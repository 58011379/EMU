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
    public partial class physical_simulator : Form
    {
        public class T_point
        {
            public int X;
            public int Y;
            public bool highlight;

            public T_point(int _x, int _y,bool _h)
            {
                X = _x;
                Y = _y;
                highlight = _h;
            }
        }
        public class cell
        {
            public List<T_point> points;
            public T_point start_point;
            public string name;
            public List<int> hightlight_points;

            public cell(string _n, int _x, int _y ,bool _h, List<T_point> _p)
            {
                start_point = new T_point(_x, _y , _h);
                points = new List<T_point>(_p);
                name = _n;
            }
        }
        class T_line
        {
            public List<cell> cells;

            public T_line()
            {
                cells = new List<cell>();
            }

            public void addPoint(string name, int _x, int _y, bool _h,List<T_point> _lp)
            {
                cells.Add(new cell(name, _x, _y, _h, _lp));
            }

        }
        
        public int mouse_build_state = 0;
        public Point mouse_build_start_point = new Point(20, 20);
        public Point mouse_build_end_point = new Point(20,20);
        T_line lines = new T_line();
        cell last_selected_cell = null;
        cell selected_cell = null;

        cell movine_cell = null;
        int movine_offset_x = 0;
        int movine_offset_y = 0;

        cell selected_point;
        int selected_point_segment;

        cell movine_point = null;
        //int movine_offset_x = 0;
        //int movine_offset_y = 0;

        Physical_Train_Network fullscale = new Physical_Train_Network();

        public physical_simulator()
        {
            InitializeComponent();

            this.DoubleBuffered = true;

            List<T_point> _p = new List<T_point>();

            _p.Clear();
            _p.Add(new T_point(3 * 40, 3 * 40, false));
            _p.Add(new T_point(3 * 30, 3 * 40, true));
            _p.Add(new T_point(3 * 40, 3 * 40, false));
            _p.Add(new T_point(3 * 20, 3 * 50, false));
                        
            lines.addPoint("testa", 15 , 30, false, new List<T_point>(_p));

            _p.Clear();
            _p.Add(new T_point(3 * 100, 3 * 40, false));
            _p.Add(new T_point(3 * 30, 3 * 40, true));
            _p.Add(new T_point(3 * 40, 3 * 40, false));
            _p.Add(new T_point(3 * 20, 3 * 50, false));

            lines.addPoint("testb", 60, 10 , true, new List<T_point>(_p));
            lines.addPoint("testc", 80, 10,  true, new List<T_point>(_p));
            lines.addPoint("testd", 100, 10, true, new List<T_point>(_p));
            lines.addPoint("teste", 120, 10, true, new List<T_point>(_p));
            lines.addPoint("testf", 140, 10, true, new List<T_point>(_p));
            lines.addPoint("testg", 160, 10, true, new List<T_point>(_p));
            lines.addPoint("teste", 180, 10, true, new List<T_point>(_p));
            lines.addPoint("testi", 200, 10, true, new List<T_point>(_p));
            lines.addPoint("testj", 220, 10, true, new List<T_point>(_p));
            lines.addPoint("testk", 240, 10, true, new List<T_point>(_p));
            lines.addPoint("testl", 260, 10, true, new List<T_point>(_p));
            lines.addPoint("testm", 280, 10, true, new List<T_point>(_p));


        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            mouse_build_state = 1;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            Color pixelColour;
            PictureBox o = pictureBox1;
            
            int px = e.X * pictureBox1.Image.Width / pictureBox1.ClientSize.Width;
            int py = e.Y * pictureBox1.Image.Height / pictureBox1.ClientSize.Height;
            pixelColour = ((Bitmap)pictureBox1.Image).GetPixel(px, py);
            Console.Write(px + " " + py);
            Console.WriteLine(pixelColour.ToString());
            if (pixelColour.A != 0)
            {
                MessageBox.Show("fuck!");
            }
        }

        private void physical_panel_Resize(object sender, EventArgs e)
        {
            this.Refresh();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            physical_panel.Refresh();
        }

        private void test_draw_Paint(object sender, PaintEventArgs e)
        {

            Pen line_pen = new Pen(Color.Aqua, 3);
            Pen line_pen2 = new Pen(Color.Orange, 10);

            int _last_x, _last_y;

            // draw lines
            foreach (cell _cell in lines.cells)
            {
                if (selected_cell != _cell)
                {
                    _last_x = _cell.start_point.X;
                    _last_y = _cell.start_point.Y;
                    if (_cell.start_point.highlight == true)
                    {
                        e.Graphics.FillEllipse(Brushes.Aqua, _last_x - 5, _last_y - 5, 10, 10);
                    }
                    foreach (T_point _n in _cell.points)
                    {
                        e.Graphics.DrawLine(line_pen, _last_x, _last_y, _last_x + _n.X, _last_y + _n.Y);
                        _last_x += _n.X;
                        _last_y += _n.Y;
                        if (_n.highlight == true)
                        {
                            e.Graphics.FillEllipse(Brushes.Aqua, _last_x - 5, _last_y - 5, 10, 10);
                        }
                    }
                }
                //e.Graphics.DrawLines(line_pen,_cell.points.ToArray());
            }

            if (selected_cell != null)
            {
                _last_x = selected_cell.start_point.X;
                _last_y = selected_cell.start_point.Y;
                foreach (T_point _n in selected_cell.points)
                {
                    e.Graphics.DrawLine(line_pen2, _last_x, _last_y, _last_x + _n.X, _last_y + _n.Y);
                    _last_x += _n.X;
                    _last_y += _n.Y;
                }
                // display data on mouse
                Font drawFont = new Font("Arial", 14);
                e.Graphics.DrawString(selected_cell.name, drawFont,Brushes.Black,test_draw.PointToClient(Cursor.Position).X + 10, test_draw.PointToClient(Cursor.Position).Y - 20);
            }
        }

        private void test_draw_MouseMove(object sender, MouseEventArgs e)
        {
            // move line
            if (movine_cell != null)
            {
                movine_cell.start_point.X = movine_offset_x + e.X;
                movine_cell.start_point.Y = movine_offset_y + e.Y;
                //Console.WriteLine("H");
                test_draw.Invalidate();
            }
            // detect line
            var size = 10;
            var buffer = new Bitmap(size * 2, size * 2);
            selected_cell = null;

            int _last_x, _last_y;
            foreach (cell _cell in lines.cells)
            {
                _last_x = _cell.start_point.X;
                _last_y = _cell.start_point.Y;
                foreach (T_point _n in _cell.points)
                {
                    using (var g = Graphics.FromImage(buffer))
                    {
                        g.Clear(Color.Black);
                        g.DrawLine(new Pen(Color.Green, 10), _last_x - e.X + size, _last_y - e.Y + size, _last_x + _n.X - e.X + size, _last_y + _n.Y - e.Y + size);
                    }

                    if (buffer.GetPixel(size, size).ToArgb() != Color.Black.ToArgb())
                    {
                        selected_cell = _cell;
                    }

                    _last_x += _n.X;
                    _last_y += _n.Y;
                }
                //e.Graphics.DrawLines(line_pen,_cell.points.ToArray());
            }

            // dot detect
            foreach (cell _cell in lines.cells)
            {
                _last_x = _cell.start_point.X;
                _last_y = _cell.start_point.Y;
                foreach (T_point _n in _cell.points)
                {
                    using (var g = Graphics.FromImage(buffer))
                    {
                        g.Clear(Color.Black);
                        g.DrawLine(new Pen(Color.Green, 10), _last_x - e.X + size, _last_y - e.Y + size, _last_x + _n.X - e.X + size, _last_y + _n.Y - e.Y + size);
                    }

                    if (buffer.GetPixel(size, size).ToArgb() != Color.Black.ToArgb())
                    {
                        selected_cell = _cell;
                    }

                    _last_x += _n.X;
                    _last_y += _n.Y;
                }
                //e.Graphics.DrawLines(line_pen,_cell.points.ToArray());


            }



            if (selected_cell != null)
            {
                Console.WriteLine(selected_cell.name);
            }

            if (last_selected_cell != selected_cell)
            {
                test_draw.Invalidate();
                last_selected_cell = selected_cell;
            }
        }

        private void test_draw_MouseDown(object sender, MouseEventArgs e)
        {
            if (selected_cell != null)
            {
                movine_cell = selected_cell;
                movine_offset_x = movine_cell.start_point.X - e.X;
                movine_offset_y = movine_cell.start_point.Y - e.Y;
            }
        }

        private void test_draw_MouseUp(object sender, MouseEventArgs e)
        {
            movine_cell = null;
        }
    }
}
