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

        //public string dataFile = @"input/coffeeBreak_features.txt";
        //public string gtFile = @"input/coffeeBreak_gt.txt";

        //public string dataFile = @"input/coffeeBreak2_features.txt";
        //public string gtFile = @"input/coffeeBreak2_gt.txt";

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
            Debug.WriteLine("GT correctly read"); */

            /* MODULARITY CUT TEST 

            foreach(Frame f in frames)
            {
                Affinity aff = new Proximity(f);
                //     Affinity aff = new ProxOrient(f);
                ModularityCut mc = new ModularityCut();
                mc.Initialize(aff);
                Group g = mc.ComputeGroup();
        //        List<int> res = Group.Compare(g, groups[f.IdFrame-1]);
                
                Debug.Write("Frame " + f.IdFrame + ": ");
                Debug.WriteLine(g);
          //      Debug.WriteLine("Correct = " + res[0] + " fp = " + res[1] + " fn = " + res[2]);
            } */


            /* DATA MANAGER */

            DataManager dm = new DataManager(dataFile, gtFile);


            /* ITERATION MANAGER TEST */

            CollectorResult res = new CollectorResult();
            List<string> val = new List<string>();

            for (double e = 1E-6; e < 5E-4; e = e + 2E-6)
            {
                IterationManager im = new IterationManager(dm);
                Method MC = new ModularityCut(e);
                Affinity Aff = new Proximity(200);
              //  Affinity Aff = new ProxOrient();
                im.computeMethod(MC, Aff);

                res = im.comparison();
                Console.WriteLine(e);
                Console.WriteLine(im.getComputationType());
                Console.WriteLine(res);

                val.Add(e + " " + res.precisionMean + " " + res.recallMean + " " + res.fMean);
                //Console.ReadLine();
            }

            string[] values = val.ToArray();
            System.IO.File.WriteAllLines(@"output/modcut_prox_epsilonTest.txt", values);




            ////////////////////////////////////////
            //////////////////Rocco////////////////
            ///////////////////////////////////////
            /*      
                  string dataFile = @"input/features.txt";
                  string gtFile = @"input/gt.txt";
                  DataManager dm = new DataManager(dataFile, gtFile);

                  CollectorResult res = new CollectorResult();
               //   Method m = new GlobalDominantSet(1E-1, 5E-2);
                  foreach (Frame frame in dm.getAllFrames())
                  {
                      //Affinity a = new SMEFO(frame);
                      //   Affinity a = new ProxOrient();
                      Proximity a = new Proximity(200);
                      a.computeAffinity(frame);
                      Method m = new ModularityCut(1E-4);
                      //new AllSingleton(); 
                      //new GlobalDominantSet(1E-2, 1E-7);
                      //con 200 affinity ok
                      //new LocalDominantSet(1E-3, 1E-4);
                      m.Initialize(a);
                      Group my = m.ComputeGroup();
                      Result t = Group.Compare(my, dm.getGTById(frame.IdFrame), 3.0 / 5.0);
                      res.addResult(t);
                      res.computeMeans();
                      Console.WriteLine(my);
                      Console.WriteLine(dm.getGTById(frame.IdFrame));

                      Console.WriteLine(t);
                      Console.WriteLine(res);

                      Console.ReadLine();
                  }
                  Console.Write(res.getSumPrec()); */

            /*
            IterationManager im = new IterationManager(dm);
            //Method m = new ModularityCut();
            double deltaValue = 0.1;
            double deltaZero = 0.1;
            for (int i = 1; i < 10; i++)
            {
               for (int j=1;j<10;j++)
                {
                   // Method m = new LocalDominantSet(1E-2, 1E-7);
                    Method m = new GlobalDominantSet(deltaZero, deltaValue);
                    Affinity Aff = new Proximity();
                    im.computeMethod(m, Aff);
                    CollectorResult res = im.comparison();
                    Console.WriteLine(res);
                    Console.WriteLine("i:"+i+", j:"+j);

                    //Console.ReadLine();
                    deltaZero = Math.Pow(10, -j);
               }
               deltaValue = Math.Pow(10, -i);
            } */
        }
    }
}
