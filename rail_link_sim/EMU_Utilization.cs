using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using rail_link_sim;
using static rail_link_sim.Physical_Train_Network;
using System.IO;
using System.Xml.Serialization;

namespace rail_link_sim
{
    public partial class EMU_Utilization : Form
    {

        List<Service_Plan> plans = new List<Service_Plan>();
        List<Physical_route> routes = new List<Physical_route>();
        public Physical_Train_Network fullscale = new Physical_Train_Network();

        List<timed_course> time_table_data = new List<timed_course>();
        List<Planning_sch> planning_sch = new List<Planning_sch>();

        int main_tain_threshold = 500;
        public EMU_Utilization()
        {
            InitializeComponent();
        }

        public class emu_calculate_data : Physical_Train_Network.Physical_Train
        {
            public int used_distance;
            public int used_time;
            public int last_maintain;
            public int next_maintain;
            public int location;
            public bool in_use;
            public bool fix_in_depot;

            public emu_calculate_data(Physical_Train_Network.Physical_Train _train, int _distance, int _time, int next, int last, int _location,bool _in_use,bool _fix_in_depot) : base(_train)
            {
                name = _train.name;
                acc = _train.acc;
                length = _train.length;
                max_speed = _train.max_speed;
                type = _train.type;
                maintenamce_plan = _train.maintenamce_plan;

                used_distance = _distance;
                used_time = _time;
                last_maintain = last;
                next_maintain = next;
                location = _location;
                in_use = _in_use;
                fix_in_depot = _fix_in_depot;
            }

            public emu_calculate_data(emu_calculate_data _emu) : base()
            {
                name = _emu.name;
                acc = _emu.acc;
                length = _emu.length;
                max_speed = _emu.max_speed;
                type = _emu.type;
                maintenamce_plan = _emu.maintenamce_plan;

                used_distance = _emu.used_distance;
                used_time = _emu.used_time;
                last_maintain = _emu.last_maintain;
                next_maintain = _emu.next_maintain;
                location = _emu.location;
                in_use = _emu.in_use;
                fix_in_depot = _emu.fix_in_depot;
            }

            public emu_calculate_data(string _name,int _type,maintenance_plan _plan,int _location) : base()
            {
                name = _name;
                type = _type;
                maintenamce_plan = _plan;
                last_maintain = 0;
                location = _location;

            }
        
        }

        public class trip_calculate_data
        {
            public string name;
            public int origin_station;
            public int destination_station;
            public int used_distance;
            public int used_time;
            public int depart_time;
            public int arrival_time;

            public trip_calculate_data(trip_calculate_data _trip)
            {
                name = _trip.name;
                origin_station = _trip.origin_station;
                destination_station = _trip.destination_station;
                used_distance = _trip.used_distance;
                used_time = _trip.used_time;
                depart_time = _trip.depart_time;
                arrival_time = _trip.arrival_time;
            }

            public trip_calculate_data()
            {

            }
        }

        public class maintenance_period
        {
            public int used_time;
            public int used_distance;
            public maintenance_routine routine;
        }
        public class emu_utilization_ans
        {
            public emu_calculate_data emu;
            public trip_calculate_data trip;
            public maintenance_routine maintain;

            public emu_utilization_ans(emu_utilization_ans _ans)
            {
                if (_ans.emu != null) emu = new emu_calculate_data(_ans.emu);
                else emu = null;
                if (_ans.trip != null) trip = new trip_calculate_data(_ans.trip);
                else trip = null;
                maintain = _ans.maintain;
            }

            public emu_utilization_ans(emu_calculate_data _emu, trip_calculate_data _trip,maintenance_routine _maintain)
            {
                if (_emu != null) emu = new emu_calculate_data(_emu);
                else emu = null;

                if (_trip != null) trip = new trip_calculate_data(_trip);
                else trip = null;

                maintain = _maintain;
            }
        }

        public class queuing_train
        {
            public emu_calculate_data emu;
            public int queue_time;

            public queuing_train(emu_calculate_data _emu, int _queue_time)
            {
                emu = new emu_calculate_data(_emu);
                queue_time = _queue_time;
            }
        }

        public List<emu_calculate_data> trains = new List<emu_calculate_data>();
        public maintenance_plan test_maintain_plan = new maintenance_plan();

        public List<List<emu_calculate_data>> train_in_station = new List<List<emu_calculate_data>>();

        public List<emu_utilization_ans> utilization_ans = new List<emu_utilization_ans>();
        public List<emu_utilization_ans> utilization_output = new List<emu_utilization_ans>();
        public List<trip_calculate_data> trip_data = new List<trip_calculate_data>();

        bool utilize_complete = false;

        private void EMU_Utilization_Load(object sender, EventArgs e)
        {
            //Deserialize(plans, "plans.xml");
            //Deserialize2(routes, "somsin.xml");
            fullscale.addTrack("physical_tracks.xml");
            fullscale.addLink("track_links.xml");
            // just for test
            test_maintain_plan.level.Add(new maintenance_routine("Maintain LV1", 5000, 1000000, 1));
            test_maintain_plan.level.Add(new maintenance_routine("Maintain LV2", 20000, 1000000, 1));
            test_maintain_plan.level.Add(new maintenance_routine("Maintain LV3", 40000, 1000000, 1));
            test_maintain_plan.level.Add(new maintenance_routine("Maintain LV4", 2400000, 1000000, 1));
            test_maintain_plan.level.Add(new maintenance_routine("Maintain LV5", 4800000, 1000000, 1));

            planning_sch.Add(new Planning_sch("BKK", new List<string>() { "ST11", "ST12", "ST13", "ST14", "ST15", "ST16" }, 0, 50, 0));
            planning_sch.Add(new Planning_sch("AYU", new List<string>() { "ST21", "ST22", "ST23", "ST24", "ST25", "ST26" }, 100, 25, 100));
            planning_sch.Add(new Planning_sch("SRB", new List<string>() { "ST31", "ST32", "ST33", "ST34", "ST35", "ST36" }, 80, 25, 180));
            planning_sch.Add(new Planning_sch("PKC", new List<string>() { "ST41", "ST42", "ST43", "ST44", "ST45", "ST46" }, 80, 25, 260));
            planning_sch.Add(new Planning_sch("NRS", new List<string>() { "ST51", "ST52", "ST53", "ST54", "ST55", "ST56" }, 100, 25, 360));

            train_in_station.Add(new List<emu_calculate_data>());
            train_in_station.Add(new List<emu_calculate_data>());
            train_in_station.Add(new List<emu_calculate_data>());
            train_in_station.Add(new List<emu_calculate_data>());
            train_in_station.Add(new List<emu_calculate_data>());


            trains.Add(new emu_calculate_data("EMU001", 1, test_maintain_plan, 0));
            trains.Add(new emu_calculate_data("EMU002", 1, test_maintain_plan, 0));
            trains.Add(new emu_calculate_data("EMU003", 1, test_maintain_plan, 0));
            trains.Add(new emu_calculate_data("EMU004", 1, test_maintain_plan, 0));
            trains.Add(new emu_calculate_data("EMU005", 1, test_maintain_plan, 4));
            trains.Add(new emu_calculate_data("EMU006", 1, test_maintain_plan, 4));
            trains.Add(new emu_calculate_data("EMU007", 1, test_maintain_plan, 4));
            trains.Add(new emu_calculate_data("EMU008", 1, test_maintain_plan, 4));
            trains.Add(new emu_calculate_data("EMU009", 1, test_maintain_plan, 0));
            trains.Add(new emu_calculate_data("EMU010", 1, test_maintain_plan, 4));
            trains.Add(new emu_calculate_data("EMU011", 1, test_maintain_plan, 0));
            trains.Add(new emu_calculate_data("EMU012", 1, test_maintain_plan, 4));
            trains.Add(new emu_calculate_data("EMU013", 1, test_maintain_plan, 0));
            trains.Add(new emu_calculate_data("EMU014", 1, test_maintain_plan, 4));
            trains.Add(new emu_calculate_data("EMU015", 1, test_maintain_plan, 0));
            trains.Add(new emu_calculate_data("EMU016", 1, test_maintain_plan, 4));
            trains.Add(new emu_calculate_data("EMU017", 1, test_maintain_plan, 0));
            trains.Add(new emu_calculate_data("EMU018", 1, test_maintain_plan, 4));

            item_view.Rows.Add("brake Pad",6546);
            item_view.Rows.Add("Gearbox", 125456);

            // refresh emu view
            emu_view.Rows.Clear();
            int i = 0;
            foreach (emu_calculate_data _emu in trains)
            {
                emu_view.Rows.Add(i,true , _emu.ToString() , _emu.type, 0, planning_sch[_emu.location].station_name);
                i++;
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            

            if (File.Exists("schdule_ex.xml"))
            {
                var serializer = new XmlSerializer(typeof(List<timed_course>));
                using (var stream = File.OpenRead("schdule_ex.xml"))
                {
                    var other = (List<timed_course>)(serializer.Deserialize(stream));
                    //list.Clear();
                    time_table_data.AddRange(other);
                }
            }

            // add train to station array
            for (int i = 0; i < emu_view.RowCount; i++)
            {
                if ((bool)emu_view.Rows[i].Cells[1].Value == true)
                {
                    emu_calculate_data _emu = trains[(int)emu_view.Rows[i].Cells[0].Value];
                    _emu.used_distance = int.Parse(emu_view.Rows[i].Cells[4].Value.ToString());
                    _emu.used_time = 0;
                    _emu.next_maintain = _emu.maintenamce_plan.level.First().used_distance;
                    train_in_station[_emu.location].Add(_emu);
                }
            }

            if (to_picker.Value < from_picker.Value) {
                MessageBox.Show("invalid date");
                return;
            }

            int cal_date = (to_picker.Value - from_picker.Value).Days;
            String s = cal_date.ToString();
            MessageBox.Show(s);
            // convet timetable to calculate data for 1 course
            trip_data.Clear();
            for (int j = 0; j < cal_date; j++)
            {              
                foreach (timed_course trip in time_table_data)
                {
                    //Console.WriteLine(trip.name);
                    trip_data.Add(new trip_calculate_data());
                    trip_data.Last().name = trip.name;
                    trip_data.Last().origin_station = find_station_index(trip.route.tracks.First());
                    trip_data.Last().destination_station = find_station_index(trip.route.tracks.Last());
                    trip_data.Last().used_distance = (int)trip.operate_speed_points.Last().length_travel;
                    trip_data.Last().used_time = (int)trip.operate_speed_points.Last().time_travel;
                    trip_data.Last().depart_time = trip.start_sec + (j * (60 * 60 * 24));
                    trip_data.Last().arrival_time = trip_data.Last().depart_time + (int)trip.operate_speed_points.Last().time_travel;
                }
                
            }
            trip_data.Sort((x, y) => x.depart_time.CompareTo(y.depart_time));
            Console.WriteLine("start search " + trip_data.Count.ToString() + " trips!");
            // calculation

            //init data 
            utilization_ans.Clear();
            utilization_output.Clear();
            utilize_complete = false;

            counter_debug = "";
            search_utilize(0,train_in_station,new List<queuing_train>(),new List<queuing_train>());
           
            // print output

            Console.WriteLine("\r\n\r\n");
            Console.WriteLine("search complete! ()" + utilization_output.Count.ToString() + "output");
            textBox2.Text = "";
            foreach (emu_utilization_ans _ans in utilization_output)
            {
                if(_ans.maintain == null)// trip
                {
                    if(_ans.trip.used_distance <= 0) // arrival
                    {
                        //Console.WriteLine((_ans.trip.arrival_time /*% (24 * 60 * 60)*/).ToString() + ":\t " +_ans.emu.name + " has arrived at " + planning_sch[_ans.emu.location].station_name);
                        textBox2.Text += ((_ans.trip.arrival_time /*% (24 * 60 * 60)*/).ToString() + ":\t " + _ans.emu.name + " has arrived at " + planning_sch[_ans.emu.location].station_name);
                        textBox2.Text += "\r\n";
                    }
                    else
                    {
                        //Console.WriteLine((_ans.trip.depart_time /*% (24*60*60)*/).ToString() + ":\t use " + _ans.emu.name + " as " + _ans.trip.name + " from " + planning_sch[_ans.trip.origin_station].station_name + " to " + planning_sch[_ans.trip.destination_station].station_name);
                        textBox2.Text += ((_ans.trip.depart_time /*% (24*60*60)*/).ToString() + ":\t use " + _ans.emu.name + " as " + _ans.trip.name + " from " + planning_sch[_ans.trip.origin_station].station_name + " to " + planning_sch[_ans.trip.destination_station].station_name);
                        textBox2.Text += "\r\n";
                    }
                }
                else
                {
                    if(_ans.trip.used_distance <= 0) // arrival
                    {
                        //Console.WriteLine((_ans.trip.arrival_time /*% (24 * 60 * 60)*/).ToString() + ":\t " + _ans.emu.name + " has finish "+ _ans.maintain.name + " and arrived " + planning_sch[_ans.emu.location].station_name);
                        textBox2.Text += ((_ans.trip.arrival_time /*% (24 * 60 * 60)*/).ToString() + ":\t " + _ans.emu.name + " has finish " + _ans.maintain.name + " and arrived " + planning_sch[_ans.emu.location].station_name);
                        textBox2.Text += "\r\n";
                    }
                    else
                    {
                        //Console.WriteLine((_ans.trip.depart_time /*% (24*60*60)*/).ToString() + ":\t "+ _ans.emu.name + " go to depot for " + _ans.maintain.name);
                        textBox2.Text += ((_ans.trip.depart_time /*% (24*60*60)*/).ToString() + ":\t " + _ans.emu.name + " go to depot for " + _ans.maintain.name);
                        textBox2.Text += "\r\n";
                        maintain_view.Rows.Add(_ans.emu.name, _ans.emu.used_distance, _ans.maintain.name, _ans.trip.depart_time);
                    }
                }
            }


        }

        string counter_debug;

        public void search_utilize(int _n,List<List<emu_calculate_data>> train_in_station,List<queuing_train> in_depot,List<queuing_train> transfering)
        {
            if(utilize_complete == true)
            {
                return;
            }
            if (_n < trip_data.Count)
            {

                // deep copy
                Queue<List<emu_calculate_data>> _trains = new Queue<List<emu_calculate_data>>(train_in_station);
                for (int k = 0; k < _trains.Count; k++)
                {
                    _trains= new Queue<emu_calculate_data>(train_in_station[k]);
                    for (int l = 0; l < _trains[k].Count; l++)
                    {
                        _trains[k][l] = new emu_calculate_data(train_in_station[k][l]);
                    }
                }

                Queue<queuing_train> _transfering = new Queue<queuing_train>();
                for(int k=0;k < transfering.Count; k++)
                {
                    _transfering.Add(new queuing_train(transfering[k].emu, transfering[k].queue_time));
                }

                Queue<queuing_train> _in_depot = new Queue<queuing_train>();
                for (int k = 0; k < in_depot.Count; k++)
                {
                    _in_depot.Enqueue(new queuing_train(in_depot[k].emu, in_depot[k].queue_time));
                }

                trip_calculate_data _trip = trip_data[_n];
                int current_time = _trip.depart_time;
                int arrival_cnt = 0;

                // check if train arrived and depot necessary
                for (int j = 0; j < _transfering.Count; j++)
                {
                    // train arrived
                    if (_transfering[j].queue_time < current_time)
                    {
                        arrival_cnt++;
                        emu_calculate_data _arrived_train = _transfering[j].emu;
                        utilization_ans.Add(new emu_utilization_ans(_transfering[j].emu, _trip, null));
                        utilization_ans.Last().trip.used_distance = 0;
                        utilization_ans.Last().trip.arrival_time = _transfering[j].queue_time;

                        //Console.WriteLine(transfering[j].emu.name + " arrived at " + planning_sch[transfering[j].emu.location].station_name);

                        // go to depot
                        if (_arrived_train.next_maintain < main_tain_threshold)
                        {
                            _arrived_train.last_maintain += _arrived_train.maintenamce_plan.level[0].used_distance;
                            _arrived_train.next_maintain = _arrived_train.maintenamce_plan.level[0].used_distance;
                            int maintain_index = 0;
                            for (int k = 0; k < _arrived_train.maintenamce_plan.level.Count; k++)
                            {
                                maintenance_routine _routine = _arrived_train.maintenamce_plan.level[k];
                                if (_arrived_train.last_maintain % _routine.used_distance == 0)
                                {
                                    maintain_index = k;
                                }
                            }

                            //Console.WriteLine(transfering[j].emu.name + " go to depot for " + _arrived_train.maintenamce_plan.level[maintain_index].name);
                            arrival_cnt++;
                            utilization_ans.Add(new emu_utilization_ans(_arrived_train,_trip, _arrived_train.maintenamce_plan.level[maintain_index]));
                            utilization_ans.Last().trip.depart_time = _transfering[j].queue_time;
                            _in_depot.Add(new queuing_train(_transfering[j].emu, _transfering[j].queue_time + ((_arrived_train.maintenamce_plan.level[maintain_index].maintain_time + 1) * 24 * 60 * 60)));
                        }
                        // go to station
                        else
                        {
                            _trains[_transfering[j].emu.location].Add(_transfering[j].emu);
                        }

                        _transfering.RemoveAt(j);
                        j = -1;
                    }
                }

                // check if train arrival from depot
                for(int j = 0; j < _in_depot.Count; j++)
                {
                    if(_in_depot[j].queue_time < current_time)
                    {
                        arrival_cnt++;
                        emu_calculate_data _arrived_train = _in_depot[j].emu;
                        utilization_ans.Add(new emu_utilization_ans(_in_depot[j].emu, _trip, _in_depot[j].emu.maintenamce_plan.level[0]));
                        utilization_ans.Last().trip.used_distance = 0;
                        utilization_ans.Last().trip.arrival_time = _in_depot[j].queue_time;

                        //Console.WriteLine("hi");

                        _trains[_in_depot[j].emu.location].Add(_in_depot[j].emu);
                        _in_depot.RemoveAt(j);
                        j = -1;
                    }
                }

                // select the lowest distance train to use

                if (_trains[_trip.origin_station].Count > 0)
                {
                    _trains[_trip.origin_station].Sort((x, y) => (y.used_distance.CompareTo(x.used_distance)));
                   
                    for (int j = 0; j < _trains[_trip.origin_station].Count; j++)
                    {
                        int last_location = _trains[_trip.origin_station][j].location;
                        utilization_ans.Add(new emu_utilization_ans(_trains[_trip.origin_station][0], _trip,null));
                        
                        //select emu
                        _trains[_trip.origin_station][j].location = _trip.destination_station;
                        _trains[_trip.origin_station][j].used_distance += _trip.used_distance / 1000;
                        _trains[_trip.origin_station][j].used_time += _trip.used_time / 1000;
                        _trains[_trip.origin_station][j].next_maintain -= _trip.used_distance / 1000;
                        _trains[_trip.origin_station][j].in_use = true;

                        ///Console.WriteLine("use " + _trains[_trip.origin_station][j].name + " as " + _trip.name + " from " + planning_sch[_trip.origin_station].station_name + " to " + planning_sch[_trip.destination_station].station_name);

                        _transfering.Add(new queuing_train(_trains[_trip.origin_station][j], _trip.arrival_time));
                        _trains[_trip.origin_station].RemoveAt(j);

                        //Console.WriteLine(_n.ToString() + ":" + j.ToString());

                        search_utilize(_n + 1, _trains, _in_depot, _transfering);

                        _trains[_trip.origin_station].Insert(j, new emu_calculate_data(_transfering.Last().emu));
                        _trains[_trip.origin_station][j].used_distance -= _trip.used_distance / 1000;
                        _trains[_trip.origin_station][j].used_time -= _trip.used_time / 1000;
                        _trains[_trip.origin_station][j].next_maintain += _trip.used_distance / 1000;
                        _trains[_trip.origin_station][j].location = last_location;
                        _transfering.RemoveAt(_transfering.Count - 1);


                        utilization_ans.RemoveAt(utilization_ans.Count - 1);
                    }
                    //Console.WriteLine("use " + train_in_station[_trip.origin_station].First().name + " as " + _trip.name + " from " + planning_sch[_trip.origin_station].station_name + " to " + planning_sch[_trip.destination_station].station_name);

                }
                else
                {
                    //Console.WriteLine("no train in station wtf can i do???");
                    //return;
                }

                // remove arrival report
                for(int i=0;i< arrival_cnt; i++)
                {
                    utilization_ans.RemoveAt(utilization_ans.Count - 1);
                }
            }
            else
            {
                // found solution fucking deep copy!!!

                utilization_output.Clear();
                foreach(emu_utilization_ans _ans in utilization_ans)
                {
                    utilization_output.Add(new emu_utilization_ans(_ans));
                }
                foreach(queuing_train _ans in transfering)
                {
                    utilization_output.Add(new emu_utilization_ans(_ans.emu,trip_data.Last(),null));
                    utilization_output.Last().trip.used_distance = 0;
                    utilization_output.Last().trip.arrival_time = _ans.queue_time;
                }
                utilize_complete = true;
            }
        }

        public int find_station_index(string _track)
        {
            for(int i = 0; i < planning_sch.Count; i++)
            {
                if (planning_sch[i].tracks.Contains(_track))
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
