using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace fFormations
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string dataFile = @"input/features.txt";
        public string gtFile = @"input/gt.txt";

        public MainWindow()
        {
            InitializeComponent();

            /* PARSER TEST */

            //get singleton and set paths
            Parser P = Parser.getParser();
            P.setDataFile(dataFile);
            P.setGTFile(gtFile);
            List<Frame> frames = P.readData();
            Debug.WriteLine("Frames correctly read");
            List<Group> groups = P.readGT(frames);
            Debug.WriteLine("GT correctly read");


            /* DATA MANAGER */
           // DataManager dm = new DataManager(dataFile, gtFile);

            foreach(Frame f in frames)
            {
                Affinity aff = new Proximity(f);
                ModularityCut mc = new ModularityCut();
                mc.Initialize(aff);
                Group g = mc.ComputeGroup();
                Console.WriteLine(g);
            }
            


            Console.ReadLine();
        }
    }
}
