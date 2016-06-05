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

            //for (double e = 1E-6; e < 1E-3; e = e + 2E-6)
            //{
            //    IterationManager im = new IterationManager(dm);
            //    Method MC = new ModularityCut(e, true);
            //    Affinity Aff = new SMEFO(200, Math.PI/2);
            //    //  Affinity Aff = new ProxOrient();
            //    im.computeMethod(MC, Aff);

            //    res = im.comparison();
            //    Console.WriteLine(e);
            //    Console.WriteLine(im.getComputationType());
            //    Console.WriteLine(res);

            //    val.Add(e + " " + res.precisionMean + " " + res.recallMean + " " + res.fMean);
            //   // Console.ReadLine();
            //}

            //string[] values = val.ToArray();
            //System.IO.File.WriteAllLines(@"output/modcut_smefo_epsilonTest.txt", values);

            for (double i = 1; i < 31; i = i +1)
            {
                IterationManager im = new IterationManager(dm);
                Method MC = new ModularityCut(1E-5, true);
                Affinity Aff = new SMEFO(i, Math.PI/2);
                //  Affinity Aff = new ProxOrient();
                im.computeMethod(MC, Aff);

                res = im.comparison();
                Console.WriteLine(im.getComputationType());
                Console.WriteLine(res);

                val.Add(i + " " + res.precisionMean + " " + res.recallMean + " " + res.fMean);
                // Console.ReadLine();
            }

            string[] values = val.ToArray();
            System.IO.File.WriteAllLines(@"output/SMEFO-MC-Sigma.txt", values);


            ////////////////////////////////////////
            //////////////////Rocco////////////////
            ///////////////////////////////////////

            /*
            string dataFile = @"input/features.txt";
            string gtFile = @"input/gt.txt";
            DataManager dm = new DataManager(dataFile, gtFile);

            CollectorResult res = new CollectorResult();
            Method m = new GlobalDominantSet(1E-1, 1E-2);
            foreach (Frame frame in dm.getAllFrames())
            {
                Affinity a = new SMEFO(15, Math.PI / 2.0);
                //Affinity a = new ProxOrient(15,Math.PI/2.0);
                a.computeAffinity(frame);
                //Method m = new ModularityCut();
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

                //Console.ReadLine();
            }
            Console.Write(res.getSumPrec());
            */
            /*
            DataManager dm = new DataManager(dataFile, gtFile);
            IterationManager im = new IterationManager(dm);
            int scalarDistance = 15;
            double windowSize = Math.PI / 2.0;
            double deltaValue = 0.1;
            double deltaZero = 0.1;
            Affinity a;
            Method m;
            double fMeasure = 0;
            
            for (int i = 1; i <= 5; i++)
            {
                for (int j = 1; j <= 5; j++)
                {
                    for (int val = 1; val <= 5; val = val + 4)
                    {
                        deltaValue = val * Math.Pow(10, -i);
                        deltaZero = val * Math.Pow(10, -j);
                        for (int scalar = 10; scalar <= 200; scalar = scalar + 10)
                        {
                            scalarDistance = scalar;
                            for (int pi = 0; pi <= 3; pi++)
                            {
                                windowSize = Math.PI / (2.0 + pi);
                                a = new ProxOrient(scalarDistance, windowSize);
                                m = new LocalDominantSet(deltaZero, deltaValue);
                                im.computeMethod(m, a);
                                CollectorResult cr = im.comparison();
                                cr.computeMeans();
                                if (cr.fMean > fMeasure)
                                {
                                    Console.WriteLine();
                                    String s = "deltaZero=" + deltaZero + ", deltaValue=" + deltaValue + ", scalarDistance=" + scalarDistance + ", windowSize=" + windowSize;
                                    Console.WriteLine(s);
                                    Console.WriteLine(cr);
                                    Console.WriteLine();
                                    fMeasure = cr.fMean;
                                    System.IO.File.AppendAllText(@"C:\Users\Roky\Desktop\fileTExt", s+"\n"+cr);
                                }
                                else
                                    Console.Write("*");
                            }
                        }
                        //}
                    }
                }
            }
            Console.WriteLine("FINITO");
            /*
            for (int i = 1; i < 10; i++)
            {
               for (int j=1;j<10;j++)
                {
                   // Method m = new LocalDominantSet(1E-2, 1E-7);
                    Method m = new GlobalDominantSet(deltaZero, deltaValue);
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
