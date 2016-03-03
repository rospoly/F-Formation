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

            /* PARSER TEST 

            //get singleton and set paths
            Parser P = Parser.getParser();
            P.setDataFile(dataFile);
            P.setGTFile(gtFile);
            List<Frame> frames = P.readData();
            Debug.WriteLine("Frames correctly read");
            List<Group> groups = P.readGT(frames);
            Debug.WriteLine("GT correctly read"); 
            */


            /* MODULARITY CUT TEST 

            foreach(Frame f in frames)
            {
                Affinity aff = new Proximity(f);
                //     Affinity aff = new ProxOrient(f);
                ModularityCut mc = new ModularityCut(true);
                mc.Initialize(aff);
                Group g = mc.ComputeGroup();
                List<int> res = Group.Compare(g, groups[f.IdFrame-1]);
                
                Debug.Write("Frame " + f.IdFrame + ": ");
                Debug.WriteLine(g);
                Debug.WriteLine("Correct = " + res[0] + " fp = " + res[1] + " fn = " + res[2]);
            }

            // DATA MANAGER 

            DataManager dm = new DataManager(dataFile, gtFile);


            // ITERATION MANAGER TEST 

            IterationManager im = new IterationManager(dm);
            Method MC = new ModularityCut();
            //  Affinity Aff = new Proximity();
            Affinity Aff = new ProxOrient();
            im.computeMethod(MC, Aff);
            CollectorResult res = im.comparison();

            double[] mean = new double[3];

            foreach (Result r in res.l)
            {
                mean[0] += r.precision;
                mean[1] += r.recall;
                mean[2] += r.f1;
              //  Debug.WriteLine(r);
            }

            mean[0] /= res.l.Count;
            mean[1] /= res.l.Count;
            mean[2] /= res.l.Count;

            Console.WriteLine("Precision = " + mean[0] + ", Recall = " + mean[1] + ", F1 = " + mean[2]);
            */
            

            string dataFile = @"input/features.txt";
            string gtFile = @"input/gt.txt";
            /* PARSER TEST */
            //get singleton and set paths
            DataManager dm = new DataManager(dataFile, gtFile);
            IterationManager im = new IterationManager(dm);
            //Method MC = new ModularityCut();
            //  Affinity Aff = new Proximity();
            Affinity Aff = new Proximity();
            Method m = new LocalDominantSet(1E-3, 1E-3);
            im.computeMethod(m, Aff);
            CollectorResult res = im.comparison();
            Console.WriteLine(res.ToString());
            /* (null == null)
                Console.WriteLine("ciao");
                Affinity a = new Proximity(frame);
                Method m = new LocalDominantSet(1E-2,1E-6);
                //GlobalDominantSet(1E-10);
                m.Initialize(a);
                Group my = m.ComputeGroup();
                Result t = new Result(Group.Compare(my, dm.getGTById(frame.IdFrame)));
                Console.WriteLine(t);
                Console.WriteLine(my);
                Console.WriteLine(dm.getGTById(frame.IdFrame));
                Console.ReadLine();
            }*/
            Console.ReadLine();
        }
    }
}
