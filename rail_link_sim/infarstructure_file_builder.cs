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
    public partial class infarstructure_file_builder : Form
    {
        public List<physical_track_input> track_input = new List<physical_track_input>();
        public List<track_link> links = new List<track_link>();
        public infarstructure_file_builder()
        {
            InitializeComponent();
        }
        //Physical_Train_Network fullscale = new Physical_Train_Network();


        private void infarstructure_file_builder_Load(object sender, EventArgs e)
        {
            //track_input.Add(new physical_track_input());
            track_input.Add(new physical_track_input("ST11", "ST11", 100000, 0, 1000, 20, 100, 0));
            track_input.Add(new physical_track_input("T0000", "T0000", 50000, 1000, 1500, 40, 50, 0));
            track_input.Add(new physical_track_input("T0001", "T0001", 50000, 1500, 2000, 40, 50, 0));
            track_input.Add(new physical_track_input("T0002", "T0002", 50000, 2000, 2500, 60, 50, 0));
            track_input.Add(new physical_track_input("T0003", "T0003", 600000, 2500, 8500, 150, 50, 0));
            track_input.Add(new physical_track_input("T0004", "T0004", 600000, 8500, 14500, 90, 50, 0));
            track_input.Add(new physical_track_input("T0005", "T0005", 50000, 14500, 15000, 90, 50, 0));
            track_input.Add(new physical_track_input("T0006", "T0006", 300000, 15000, 18000, 150, 50, 0));
            track_input.Add(new physical_track_input("T0007", "T0007", 50000, 18000, 18500, 90, 50, 0));
            track_input.Add(new physical_track_input("T0008", "T0008", 300000, 18500, 21500, 60, 50, 0));
            track_input.Add(new physical_track_input("T0009", "T0009", 600000, 21500, 27500, 90, 50, 0));
            track_input.Add(new physical_track_input("T0010", "T0010", 600000, 27500, 33500, 150, 50, 0));
            track_input.Add(new physical_track_input("T0011", "T0011", 900000, 33500, 42500, 200, 50, 0));
            track_input.Add(new physical_track_input("T0012", "T0012", 600000, 42500, 48500, 150, 50, 0));
            track_input.Add(new physical_track_input("T0013", "T0013", 50000, 48500, 49000, 80, 50, 0));
            track_input.Add(new physical_track_input("T0014", "T0014", 300000, 49000, 52000, 80, 50, 0));
            track_input.Add(new physical_track_input("T0015", "T0015", 50000, 52000, 52500, 80, 50, 0));
            track_input.Add(new physical_track_input("T0016", "T0016", 600000, 52500, 58500, 150, 50, 0));
            track_input.Add(new physical_track_input("T0017", "T0017", 300000, 58500, 61500, 80, 50, 0));
            track_input.Add(new physical_track_input("T0018", "T0018", 50000, 61500, 62000, 60, 50, -50));
            track_input.Add(new physical_track_input("ST21", "ST21", 100000, 62000, 63000, 20, 100, 0));

            track_input.Add(new physical_track_input("T0019", "T0019", 50000, 0, 62000, 60, 50, 0));
            track_input.Add(new physical_track_input("ST22", "ST22", 100000, 62000, 63000, 20, 100, 0));

            track_input.Add(new physical_track_input("ST12", "ST12", 100000, 0, 1000, 20, 100, 0));
            track_input.Add(new physical_track_input("T0020", "T0020", 50000, 1000, 1500, 40, 50, 0));
            track_input.Add(new physical_track_input("T0021", "T0021", 50000, 1500, 2000, 40, 50, -50));

            track_input.Add(new physical_track_input("ST13", "ST13", 100000, 0, 1000, 20, 100, 0));
            track_input.Add(new physical_track_input("T0040", "T0040", 50000, 1000, 1500, 40, 50, -50));

            track_input.Add(new physical_track_input("T0022", "T0022", 50000, 2000, 2500, 60, 50, -100));
            track_input.Add(new physical_track_input("T0023", "T0023", 600000, 2500, 8500, 150, 50, 0));
            track_input.Add(new physical_track_input("T0024", "T0024", 600000, 8500, 14500, 90, 50, 0));
            track_input.Add(new physical_track_input("T0025", "T0025", 50000, 14500, 15000, 90, 50, 0));
            track_input.Add(new physical_track_input("T0026", "T0026", 300000, 15000, 18000, 150, 50, 0));
            track_input.Add(new physical_track_input("T0027", "T0027", 50000, 18000, 18500, 90, 50, 0));
            track_input.Add(new physical_track_input("T0028", "T0028", 300000, 18500, 21500, 60, 50, 0));
            track_input.Add(new physical_track_input("T0029", "T0029", 600000, 21500, 27500, 90, 50, 0));
            track_input.Add(new physical_track_input("T0030", "T0030", 600000, 27500, 33500, 150, 50, 0));
            track_input.Add(new physical_track_input("T0031", "T0031", 900000, 33500, 42500, 200, 50, 0));
            track_input.Add(new physical_track_input("T0032", "T0032", 600000, 42500, 48500, 150, 50, 0));
            track_input.Add(new physical_track_input("T0033", "T0033", 50000, 48500, 49000, 80, 50, 0));
            track_input.Add(new physical_track_input("T0034", "T0034", 300000, 49000, 52000, 80, 50, 0));
            track_input.Add(new physical_track_input("T0035", "T0035", 50000, 52000, 52500, 80, 50, 0));
            track_input.Add(new physical_track_input("T0036", "T0036", 600000, 52500, 58500, 150, 50, 0));
            track_input.Add(new physical_track_input("T0037", "T0037", 300000, 58500, 61500, 80, 50, 0));
            track_input.Add(new physical_track_input("T0038", "T0038", 50000, 61500, 62000, 60, 50, 0));
            track_input.Add(new physical_track_input("ST23", "ST23", 100000, 62000, 63000, 20, 100, 0));

            track_input.Add(new physical_track_input("T0039", "T0039", 50000, 0, 62000, 60, 50, 50));
            track_input.Add(new physical_track_input("ST24", "ST24", 100000, 62000, 63000, 20, 100, 0));

            track_input.Add(new physical_track_input("ST14", "ST14", 100000, 0, 1000, 20, 100, 0));
            track_input.Add(new physical_track_input("T0041", "T0041", 50000, 1000, 1500, 40, 50, 0));
            track_input.Add(new physical_track_input("T0042", "T0042", 50000, 1500, 2000, 40, 50, 0));

            track_input.Add(new physical_track_input("ST15", "ST15", 100000, 0, 1000, 20, 100, 0));
            track_input.Add(new physical_track_input("T0043", "T0043", 50000, 1000, 1500, 40, 50, 0));
            track_input.Add(new physical_track_input("T0044", "T0044", 50000, 1500, 2000, 40, 50, -50));

            track_input.Add(new physical_track_input("ST16", "ST16", 100000, 0, 1000, 20, 100, 0));
            track_input.Add(new physical_track_input("T0045", "T0045", 50000, 1000, 1500, 40, 50, -50));

            track_input.Add(new physical_track_input("T0046", "T0046", 50000, 0, 15000, 90, 50, 50));
            track_input.Add(new physical_track_input("T0047", "T0047", 50000, 15000, 18500, 90, 50, -50));
            track_input.Add(new physical_track_input("T0048", "T0048", 50000, 48500, 49000, 80, 50, 50));
            track_input.Add(new physical_track_input("T0049", "T0049", 50000, 52000, 52500, 80, 50, -50));

            links.Add(new track_link("ST11", "T0000"));
            links.Add(new track_link("T0000", "T0001"));
            links.Add(new track_link("T0001", "T0002"));
            links.Add(new track_link("T0002", "T0003"));
            links.Add(new track_link("T0003", "T0004"));
            links.Add(new track_link("T0004", "T0005"));
            links.Add(new track_link("T0005", "T0006"));
            links.Add(new track_link("T0006", "T0007"));
            links.Add(new track_link("T0007", "T0008"));
            links.Add(new track_link("T0008", "T0009"));
            links.Add(new track_link("T0009", "T0010"));
            links.Add(new track_link("T0010", "T0011"));
            links.Add(new track_link("T0011", "T0012"));
            links.Add(new track_link("T0012", "T0013"));
            links.Add(new track_link("T0013", "T0014"));
            links.Add(new track_link("T0014", "T0015"));
            links.Add(new track_link("T0015", "T0016"));
            links.Add(new track_link("T0016", "T0017"));
            links.Add(new track_link("T0017", "T0018"));
            links.Add(new track_link("T0018", "ST21"));

            links.Add(new track_link("T0017", "T0019"));
            links.Add(new track_link("T0019", "ST22"));

            links.Add(new track_link("ST12", "T0020"));
            links.Add(new track_link("T0020", "T0021"));
            links.Add(new track_link("T0021", "T0002"));

            links.Add(new track_link("ST13", "T0040"));
            links.Add(new track_link("T0040", "T0021"));

            links.Add(new track_link("ST14", "T0041"));
            links.Add(new track_link("T0041", "T0042"));
            links.Add(new track_link("T0042", "T0022"));
            links.Add(new track_link("T0022", "T0023"));
            links.Add(new track_link("T0023", "T0024"));
            links.Add(new track_link("T0024", "T0025"));
            links.Add(new track_link("T0025", "T0026"));
            links.Add(new track_link("T0026", "T0027"));
            links.Add(new track_link("T0027", "T0028"));
            links.Add(new track_link("T0028", "T0029"));
            links.Add(new track_link("T0029", "T0030"));
            links.Add(new track_link("T0030", "T0031"));
            links.Add(new track_link("T0031", "T0032"));
            links.Add(new track_link("T0032", "T0033"));
            links.Add(new track_link("T0033", "T0034"));
            links.Add(new track_link("T0034", "T0035"));
            links.Add(new track_link("T0035", "T0036"));
            links.Add(new track_link("T0036", "T0037"));
            links.Add(new track_link("T0037", "T0038"));
            links.Add(new track_link("T0038", "ST23"));

            links.Add(new track_link("T0037", "T0039"));
            links.Add(new track_link("T0039", "ST24"));

            links.Add(new track_link("T0004", "T0046"));
            links.Add(new track_link("T0046", "T0026"));

            links.Add(new track_link("T0026", "T0047"));
            links.Add(new track_link("T0047", "T0008"));

            links.Add(new track_link("T0012", "T0048"));
            links.Add(new track_link("T0048", "T0034"));

            links.Add(new track_link("T0034", "T0049"));
            links.Add(new track_link("T0049", "T0016"));

            links.Add(new track_link("ST15", "T0043"));
            links.Add(new track_link("T0043", "T0044"));
            links.Add(new track_link("T0044", "T0022"));

            links.Add(new track_link("ST16", "T0045"));
            links.Add(new track_link("T0045", "T0044"));

            //track_input.Add(new physical_track_input(name,  display_name,   length,     start_kilo,     end_kilo,   max_speed,  width   ,   high)
            // ATU to SRB
            track_input.Add(new physical_track_input("T1000", "T1000", 100000, 63000, 64000, 50, 50, 50));
            track_input.Add(new physical_track_input("T1001", "T1001", 100000, 63000, 64000, 50, 50, 0));
            track_input.Add(new physical_track_input("T1002", "T1002", 200000, 64000, 66000, 80, 50, 0));
            track_input.Add(new physical_track_input("T1003", "T1003", 200000, 66000, 68000, 150, 50, 0));
            track_input.Add(new physical_track_input("T1026", "T1026", 200000, 66000, 68000, 50, 50, 50));
            track_input.Add(new physical_track_input("T1004", "T1004", 200000, 68000, 70000, 150, 50, 0));
            track_input.Add(new physical_track_input("T1005", "T1005", 200000, 70000, 72000, 150, 50, 0));
            track_input.Add(new physical_track_input("T1027", "T1027", 200000, 70000, 72000, 50, 50, -50));
            track_input.Add(new physical_track_input("T1006", "T1006", 200000, 72000, 74000, 150, 50, 0));
            track_input.Add(new physical_track_input("T1007", "T1007", 200000, 74000, 76000, 150, 50, 0));
            track_input.Add(new physical_track_input("T1028", "T1028", 200000, 74000, 76000, 50, 50, 50));
            track_input.Add(new physical_track_input("T1008", "T1008", 200000, 76000, 78000, 150, 50, 0));
            track_input.Add(new physical_track_input("T1009", "T1009", 200000, 78000, 80000, 150, 50, 0));
            track_input.Add(new physical_track_input("T1029", "T1029", 200000, 78000, 80000, 50, 50, -50));
            track_input.Add(new physical_track_input("T1010", "T1010", 2000000, 80000, 100000, 250, 150, 0));
            track_input.Add(new physical_track_input("T1011", "T1011", 400000, 100000, 104000, 120, 150, 0));
            track_input.Add(new physical_track_input("T1012", "T1012", 200000, 104000, 106000, 60, 50, -50));
            track_input.Add(new physical_track_input("T1030", "T1030", 200000, 104000, 106000, 60, 50, 0));
            track_input.Add(new physical_track_input("T1014", "T1014", 100000, 63000, 64000, 50, 50, 0));
            track_input.Add(new physical_track_input("T1013", "T1013", 100000, 63000, 64000, 50, 50, -50));
            track_input.Add(new physical_track_input("T1015", "T1015", 200000, 64000, 66000, 80, 50, 0));
            track_input.Add(new physical_track_input("T1016", "T1016", 200000, 66000, 68000, 150, 50, 0));
            track_input.Add(new physical_track_input("T1017", "T1017", 200000, 68000, 70000, 150, 50, 0));
            track_input.Add(new physical_track_input("T1018", "T1018", 200000, 70000, 72000, 150, 50, 0));
            track_input.Add(new physical_track_input("T1019", "T1019", 200000, 72000, 74000, 150, 50, 0));
            track_input.Add(new physical_track_input("T1020", "T1020", 200000, 74000, 76000, 150, 50, 0));
            track_input.Add(new physical_track_input("T1021", "T1021", 200000, 76000, 78000, 150, 50, 0));
            track_input.Add(new physical_track_input("T1022", "T1022", 200000, 78000, 80000, 150, 50, 0));

            track_input.Add(new physical_track_input("T1023", "T1023", 2000000, 80000, 100000, 150, 150, 0));
            track_input.Add(new physical_track_input("T1024", "T1024", 400000, 100000, 104000, 120, 150, 0));
            track_input.Add(new physical_track_input("T1025", "T1025", 200000, 104000, 106000, 60, 50, 50));
            track_input.Add(new physical_track_input("T1031", "T1031", 200000, 104000, 106000, 60, 50, 0));

            track_input.Add(new physical_track_input("ST31", "ST31", 100000, 106000, 107000, 60, 100, 0));
            track_input.Add(new physical_track_input("ST32", "ST32", 100000, 106000, 107000, 60, 100, 0));
            track_input.Add(new physical_track_input("ST33", "ST33", 100000, 106000, 107000, 60, 100, 0));
            track_input.Add(new physical_track_input("ST34", "ST34", 100000, 106000, 107000, 60, 100, 0));

            links.Add(new track_link("ST21", "T1000"));
            links.Add(new track_link("ST22", "T1001"));
            links.Add(new track_link("T1001", "T1002"));
            links.Add(new track_link("T1000", "T1002"));
            links.Add(new track_link("T1002", "T1003"));
            links.Add(new track_link("T1003", "T1004"));
            links.Add(new track_link("T1004", "T1005"));
            links.Add(new track_link("T1005", "T1006"));
            links.Add(new track_link("T1006", "T1007"));
            links.Add(new track_link("T1007", "T1008"));
            links.Add(new track_link("T1008", "T1009"));
            links.Add(new track_link("T1009", "T1010"));
            links.Add(new track_link("T1010", "T1011"));
            links.Add(new track_link("T1011", "T1012"));
            links.Add(new track_link("T1011", "T1030"));
            links.Add(new track_link("T1012", "ST31"));
            links.Add(new track_link("T1030", "ST32"));

            links.Add(new track_link("ST23", "T1014"));
            links.Add(new track_link("ST24", "T1013"));
            links.Add(new track_link("T1014", "T1015"));
            links.Add(new track_link("T1013", "T1015"));
            links.Add(new track_link("T1015", "T1016"));
            links.Add(new track_link("T1016", "T1017"));
            links.Add(new track_link("T1017", "T1018"));
            links.Add(new track_link("T1018", "T1019"));
            links.Add(new track_link("T1019", "T1020"));
            links.Add(new track_link("T1020", "T1021"));
            links.Add(new track_link("T1021", "T1022"));
            links.Add(new track_link("T1022", "T1023"));
            links.Add(new track_link("T1023", "T1024"));
            links.Add(new track_link("T1024", "T1025"));
            links.Add(new track_link("T1024", "T1031"));
            links.Add(new track_link("T1031", "ST33"));
            links.Add(new track_link("T1025", "ST34"));

            links.Add(new track_link("T1002", "T1026"));
            links.Add(new track_link("T1026", "T1017"));

            links.Add(new track_link("T1017", "T1027"));
            links.Add(new track_link("T1027", "T1006"));

            links.Add(new track_link("T1006", "T1028"));
            links.Add(new track_link("T1028", "T1021"));

            links.Add(new track_link("T1021", "T1029"));
            links.Add(new track_link("T1029", "T1010"));

            // SRB 107 to PKC 174
            track_input.Add(new physical_track_input("T2001", "T2001", 100000, 107000, 108000, 50, 50, 50));
            track_input.Add(new physical_track_input("T2002", "T2002", 100000, 107000, 108000, 50, 50, 0));
            track_input.Add(new physical_track_input("T2003", "T2003", 200000, 108000, 110000, 80, 50, 0));
            track_input.Add(new physical_track_input("T2004", "T2004", 200000, 110000, 112000, 150, 50, 0));
            track_input.Add(new physical_track_input("T2025", "T2025", 200000, 110000, 112000, 50, 50, 50));
            track_input.Add(new physical_track_input("T2005", "T2005", 200000, 112000, 114000, 150, 50, 0));
            track_input.Add(new physical_track_input("T2006", "T2006", 200000, 114000, 116000, 150, 50, 0));
            track_input.Add(new physical_track_input("T2026", "T2026", 200000, 114000, 116000, 50, 50, -50));
            track_input.Add(new physical_track_input("T2007", "T2007", 200000, 116000, 118000, 150, 50, 0));
            track_input.Add(new physical_track_input("T2008", "T2008", 200000, 118000, 120000, 150, 50, 0));
            track_input.Add(new physical_track_input("T2027", "T2027", 200000, 118000, 120000, 50, 50, 50));
            track_input.Add(new physical_track_input("T2009", "T2009", 2600000, 120000, 146000, 200, 50, 0));
            track_input.Add(new physical_track_input("T2010", "T2010", 2600000, 146000, 172000, 150, 50, 0));
            track_input.Add(new physical_track_input("T2011", "T2011", 100000, 172000, 173000, 50, 50, -50));
            track_input.Add(new physical_track_input("T2012", "T2012", 100000, 172000, 173000, 50, 50, 0));

            track_input.Add(new physical_track_input("T2014", "T2014", 100000, 107000, 108000, 50, 50, -50));
            track_input.Add(new physical_track_input("T2013", "T2013", 100000, 107000, 108000, 50, 50, 0));
            track_input.Add(new physical_track_input("T2015", "T2015", 200000, 108000, 110000, 80, 50, 0));
            track_input.Add(new physical_track_input("T2016", "T2016", 200000, 110000, 112000, 150, 50, 0));
            track_input.Add(new physical_track_input("T2017", "T2017", 200000, 112000, 114000, 150, 50, 0));
            track_input.Add(new physical_track_input("T2018", "T2018", 200000, 114000, 116000, 150, 50, 0));
            track_input.Add(new physical_track_input("T2019", "T2019", 200000, 116000, 118000, 150, 50, 0));
            track_input.Add(new physical_track_input("T2020", "T2020", 200000, 118000, 120000, 150, 50, 0));
            track_input.Add(new physical_track_input("T2021", "T2021", 2600000, 120000, 146000, 200, 50, 0));
            track_input.Add(new physical_track_input("T2022", "T2022", 2600000, 146000, 172000, 150, 50, 0));
            track_input.Add(new physical_track_input("T2024", "T2024", 100000, 172000, 173000, 50, 50, 50));
            track_input.Add(new physical_track_input("T2023", "T2112", 100000, 172000, 173000, 50, 50, 0));

            track_input.Add(new physical_track_input("ST41", "ST41", 100000, 173000, 174000, 60, 100, 0));
            track_input.Add(new physical_track_input("ST42", "ST42", 100000, 173000, 174000, 60, 100, 0));
            track_input.Add(new physical_track_input("ST43", "ST43", 100000, 173000, 174000, 60, 100, 0));
            track_input.Add(new physical_track_input("ST44", "ST44", 100000, 173000, 174000, 60, 100, 0));

            links.Add(new track_link("ST31", "T2001"));
            links.Add(new track_link("ST32", "T2002"));
            links.Add(new track_link("T2001", "T2003"));
            links.Add(new track_link("T2002", "T2003"));
            links.Add(new track_link("T2003", "T2004"));
            links.Add(new track_link("T2004", "T2005"));
            links.Add(new track_link("T2005", "T2006"));
            links.Add(new track_link("T2006", "T2007"));
            links.Add(new track_link("T2007", "T2008"));
            links.Add(new track_link("T2008", "T2009"));
            links.Add(new track_link("T2009", "T2010"));
            links.Add(new track_link("T2010", "T2011"));
            links.Add(new track_link("T2010", "T2012"));
            links.Add(new track_link("T2011", "ST41"));
            links.Add(new track_link("T2012", "ST42"));

            links.Add(new track_link("ST33", "T2013"));
            links.Add(new track_link("ST34", "T2014"));
            links.Add(new track_link("T2013", "T2015"));
            links.Add(new track_link("T2014", "T2015"));
            links.Add(new track_link("T2015", "T2016"));
            links.Add(new track_link("T2016", "T2017"));
            links.Add(new track_link("T2017", "T2018"));
            links.Add(new track_link("T2018", "T2019"));
            links.Add(new track_link("T2019", "T2020"));
            links.Add(new track_link("T2020", "T2021"));
            links.Add(new track_link("T2021", "T2022"));
            links.Add(new track_link("T2022", "T2023"));
            links.Add(new track_link("T2022", "T2024"));
            links.Add(new track_link("T2023", "ST43"));
            links.Add(new track_link("T2024", "ST44"));

            links.Add(new track_link("T2003", "T2025"));
            links.Add(new track_link("T2025", "T2017"));

            links.Add(new track_link("T2017", "T2026"));
            links.Add(new track_link("T2026", "T2007"));

            links.Add(new track_link("T2007", "T2027"));
            links.Add(new track_link("T2027", "T2021"));

            // PKC 174 to NRS 252
            track_input.Add(new physical_track_input("T3000", "T3000", 100000, 174000, 175000, 50, 50, 50));
            track_input.Add(new physical_track_input("T3001", "T3001", 100000, 174000, 175000, 50, 50, 0));
            track_input.Add(new physical_track_input("T3002", "T3002", 200000, 175000, 177000, 80, 50, 0));
            track_input.Add(new physical_track_input("T3003", "T3003", 200000, 177000, 179000, 150, 50, 0));
            track_input.Add(new physical_track_input("T3036", "T3036", 200000, 177000, 179000, 50, 50, 50));
            track_input.Add(new physical_track_input("T3004", "T3004", 200000, 179000, 181000, 150, 50, 0));
            track_input.Add(new physical_track_input("T3005", "T3005", 200000, 181000, 183000, 150, 50, 0));
            track_input.Add(new physical_track_input("T3037", "T3037", 200000, 181000, 183000, 50, 50, -50));
            track_input.Add(new physical_track_input("T3006", "T3006", 200000, 183000, 185000, 150, 50, 0));
            track_input.Add(new physical_track_input("T3007", "T3007", 200000, 185000, 187000, 150, 50, 0));
            track_input.Add(new physical_track_input("T3038", "T3038", 200000, 185000, 187000, 50, 50, 50));
            track_input.Add(new physical_track_input("T3008", "T3008", 200000, 187000, 189000, 150, 50, 0));
            track_input.Add(new physical_track_input("T3009", "T3009", 200000, 187000, 189000, 150, 50, 0));
            track_input.Add(new physical_track_input("T3039", "T3039", 200000, 187000, 189000, 150, 50, -50));
            track_input.Add(new physical_track_input("T3010", "T3010", 400000, 189000, 192000, 150, 50, 0));
            track_input.Add(new physical_track_input("T3011", "T3011", 3200000, 192000, 224000, 150, 200, 0));
            track_input.Add(new physical_track_input("T3012", "T3012", 200000, 224000, 226000, 250, 50, 0));
            track_input.Add(new physical_track_input("T3013", "T3013", 2000000, 226000, 246000, 150, 150, 0));
            track_input.Add(new physical_track_input("T3014", "T3014", 200000, 246000, 248000, 60, 50, 0));
            track_input.Add(new physical_track_input("T3040", "T3040", 200000, 246000, 248000, 60, 50, 50));

            track_input.Add(new physical_track_input("T3015", "T3015", 200000, 248000, 250000, 120, 50, 0));
            track_input.Add(new physical_track_input("T3016", "T3016", 200000, 250000, 252000, 60, 50, -50));
            track_input.Add(new physical_track_input("T3017", "T3017", 200000, 250000, 252000, 60, 50, 0));

            track_input.Add(new physical_track_input("T3019", "T3019", 100000, 174000, 175000, 50, 50, -50));
            track_input.Add(new physical_track_input("T3018", "T3018", 100000, 174000, 175000, 50, 50, 0));
            track_input.Add(new physical_track_input("T3020", "T3020", 200000, 175000, 177000, 80, 50, 0));
            track_input.Add(new physical_track_input("T3021", "T3021", 200000, 177000, 179000, 150, 50, 0));
            track_input.Add(new physical_track_input("T3022", "T3022", 200000, 179000, 181000, 150, 50, 0));
            track_input.Add(new physical_track_input("T3023", "T3023", 200000, 181000, 183000, 150, 50, 0));
            track_input.Add(new physical_track_input("T3024", "T3024", 200000, 183000, 185000, 150, 50, 0));
            track_input.Add(new physical_track_input("T3025", "T3025", 200000, 185000, 187000, 150, 50, 0));
            track_input.Add(new physical_track_input("T3026", "T3026", 200000, 187000, 189000, 150, 50, 0));
            track_input.Add(new physical_track_input("T3027", "T3027", 200000, 187000, 189000, 150, 50, 0));
            track_input.Add(new physical_track_input("T3028", "T3028", 400000, 189000, 192000, 150, 50, 0));
            track_input.Add(new physical_track_input("T3029", "T3029", 3200000, 192000, 224000, 150, 200, 0));
            track_input.Add(new physical_track_input("T3030", "T3030", 200000, 224000, 226000, 250, 50, 0));
            track_input.Add(new physical_track_input("T3031", "T3031", 2000000, 226000, 246000, 150, 150, 0));
            track_input.Add(new physical_track_input("T3032", "T3032", 200000, 246000, 248000, 60, 50, 0));
            track_input.Add(new physical_track_input("T3033", "T3033", 200000, 248000, 250000, 120, 50, 0));
            track_input.Add(new physical_track_input("T3034", "T3034", 200000, 250000, 252000, 60, 50, 50));
            track_input.Add(new physical_track_input("T3035", "T3035", 200000, 250000, 252000, 60, 50, 0));

            track_input.Add(new physical_track_input("ST51", "ST51", 100000, 251000, 252000, 60, 100, 0));
            track_input.Add(new physical_track_input("ST52", "ST52", 100000, 251000, 252000, 60, 100, 0));
            track_input.Add(new physical_track_input("ST53", "ST53", 100000, 251000, 252000, 60, 100, 0));
            track_input.Add(new physical_track_input("ST54", "ST54", 100000, 251000, 252000, 60, 100, 0));

            links.Add(new track_link("ST41", "T3000"));
            links.Add(new track_link("ST42", "T3001"));
            links.Add(new track_link("T3000", "T3002"));
            links.Add(new track_link("T3001", "T3002"));
            links.Add(new track_link("T3002", "T3003"));
            links.Add(new track_link("T3003", "T3004"));
            links.Add(new track_link("T3004", "T3005"));
            links.Add(new track_link("T3005", "T3006"));
            links.Add(new track_link("T3006", "T3007"));
            links.Add(new track_link("T3007", "T3008"));
            links.Add(new track_link("T3008", "T3009"));
            links.Add(new track_link("T3009", "T3010"));
            links.Add(new track_link("T3010", "T3011"));
            links.Add(new track_link("T3011", "T3012"));
            links.Add(new track_link("T3012", "T3013"));
            links.Add(new track_link("T3013", "T3014"));
            links.Add(new track_link("T3014", "T3015"));
            links.Add(new track_link("T3015", "T3016"));
            links.Add(new track_link("T3015", "T3017"));
            links.Add(new track_link("T3016", "ST51"));
            links.Add(new track_link("T3017", "ST52"));

            links.Add(new track_link("ST43", "T3018"));
            links.Add(new track_link("ST44", "T3019"));
            links.Add(new track_link("T3018", "T3020"));
            links.Add(new track_link("T3019", "T3020"));
            links.Add(new track_link("T3020", "T3021"));
            links.Add(new track_link("T3021", "T3022"));
            links.Add(new track_link("T3022", "T3023"));
            links.Add(new track_link("T3023", "T3024"));
            links.Add(new track_link("T3024", "T3025"));
            links.Add(new track_link("T3025", "T3026"));
            links.Add(new track_link("T3026", "T3027"));
            links.Add(new track_link("T3027", "T3028"));
            links.Add(new track_link("T3028", "T3029"));
            links.Add(new track_link("T3029", "T3030"));
            links.Add(new track_link("T3030", "T3031"));
            links.Add(new track_link("T3031", "T3032"));
            links.Add(new track_link("T3032", "T3033"));
            links.Add(new track_link("T3033", "T3034"));
            links.Add(new track_link("T3033", "T3035"));
            links.Add(new track_link("T3034", "ST53"));
            links.Add(new track_link("T3035", "ST54"));

            links.Add(new track_link("T3002", "T3036"));
            links.Add(new track_link("T3036", "T3022"));

            links.Add(new track_link("T3022", "T3037"));
            links.Add(new track_link("T3037", "T3006"));

            links.Add(new track_link("T3006", "T3038"));
            links.Add(new track_link("T3038", "T3026"));

            links.Add(new track_link("T3026", "T3039"));
            links.Add(new track_link("T3039", "T3010"));

            links.Add(new track_link("T3011", "T3040"));
            links.Add(new track_link("T3040", "T3031"));

            links.Add(new track_link("T3031", "T3050"));
            links.Add(new track_link("T3050", "T3015"));


            //777fullscale.verify_track("ST11", 50, 100, 0);
        }

        
        public void print_file()
        {
            //XmlSerializer serializer = new XmlSerializer(typeof(physical_track[]), new XmlRootAttribute() { ElementName = "track" });
            XmlSerializer serializer = new XmlSerializer(typeof(List<physical_track_input>));
            using (var stream = File.Open("physical_tracks.xml", FileMode.Create))
            {
                try
                {
                    //serializer.Serialize(stream, tracks.Select(kv => new physical_track() { id = kv.Key, value = kv.Value }).ToArray());
                    serializer.Serialize(stream, track_input);
                    stream.Close();
                }
                catch(Exception e)
                {
                    MessageBox.Show(e.ToString());
                }
            }

            XmlSerializer serializer2 = new XmlSerializer(typeof(List<track_link>));
            using (var stream = File.Open("track_links.xml", FileMode.Create))
            {
                serializer2.Serialize(stream, links);
                stream.Close();
            }
            /*
            XElement rootElement = XElement.Parse("<root><key>value</key></root>");
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (var el in rootElement.Elements())
            {
                dict.Add(el.Name.LocalName, el.Value);
            }*/
        }

        private void button1_Click(object sender, EventArgs e)
        {
            print_file();
        }
    }
}
