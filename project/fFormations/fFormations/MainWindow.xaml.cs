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
            DataManager dm = new DataManager(dataFile, gtFile);
            
            IterationManager im = new IterationManager(dm);

            int scalarDistance = 200;
            double windowSize = 0;
            double deltaValue = 0.01;
            double deltaZero = 0.01;

            Affinity aff;
            Method m;

            String path = @"output\TestPerGrafico\Angle\SMEFO-Modularity-Angle.txt";
            String s = "deltaZero=" + deltaZero + ", deltaValue=" + deltaValue + "Sigma="+scalarDistance;
            String graph = "";
            Console.WriteLine(s);
            System.IO.File.AppendAllText(path, s + Environment.NewLine);
            //epsilon 1E-5, -6
            for (int pi = 0; pi <= 10; pi++)
            {
                windowSize = Math.PI / (2.0 + pi);
                aff = new SMEFO(scalarDistance, windowSize);
                m = new ModularityCut(1E-2);
                im.computeMethod(m, aff);
                CollectorResult cr = im.comparison(true, 2.0/3.0);
                String str=cr.computeMeans();
                Console.WriteLine();
                Console.WriteLine("Window Size: " + windowSize+" "+ cr + " "+str);
                Console.WriteLine();
                System.IO.File.AppendAllText(path, "Window Size: " + windowSize + " " + cr + " N° Risposte: "+str+ Environment.NewLine);
                graph = graph + Environment.NewLine + windowSize + " " + cr.fMean;
            }
            System.IO.File.AppendAllText(path, graph);

            /*

            int scalarDistance = 10;
            double windowSize = 0;
            double deltaValue = 0.01;
            double deltaZero = 0.0001;

            Affinity aff;
            Method m;
            
            String path = @"output\TestPerGrafico\SMEFO-DS-LC-Sigma.txt";
            String s = "deltaZero=" + deltaZero + ", deltaValue=" + deltaValue + ", angle=PI/2";
            String graph = "";
            Console.WriteLine(s);
            System.IO.File.AppendAllText(path, s + Environment.NewLine);

            for (int var = 1; var <= 30; var=var+1)
            {
                scalarDistance = var;
                windowSize = Math.PI /2.0 ;
                aff = new SMEFO(scalarDistance, windowSize);
                m = new LocalDominantSet(deltaZero, deltaValue);
                im.computeMethod(m, aff);
                CollectorResult cr = im.comparison(true, 2.0 / 3.0);
                String str = cr.computeMeans();
                Console.WriteLine();
                Console.WriteLine("Scalar Distance: " + scalarDistance + " " + cr + " " + str);
                Console.WriteLine();
                System.IO.File.AppendAllText(path, "Scalar Distance" + scalarDistance + " " + cr + " N° Risposte: " + str + Environment.NewLine);
                graph = graph + Environment.NewLine + scalarDistance + " " + cr.fMean;
            }
            System.IO.File.AppendAllText(path,graph);

            /*
            foreach (Frame frame in dm.getAllFrames())
            {
                aff = new SMEFO(scalarDistance, windowSize);
                //Affinity a = new ProxOrient(15,Math.PI/2.0);
                aff.computeAffinity(frame);
                //Method m = new ModularityCut();
                //new AllSingleton(); 
                //new GlobalDominantSet(1E-2, 1E-7);
                //con 200 affinity ok
                //new LocalDominantSet(1E-3, 1E-4);
                m.Initialize(aff);
                Group my = m.ComputeGroup();
                Result t = Group.Compare(my, dm.getGTById(frame.IdFrame),0.66);
                res.addResult(t);
                res.computeMeans();
                Console.WriteLine(my);
                Console.WriteLine(dm.getGTById(frame.IdFrame));

                Console.WriteLine(t);
                Console.WriteLine(res);

                Console.ReadLine();
            }
            
            

            /*
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
                                a = new SMEFO(scalarDistance,windowSize);
                                m = new LocalDominantSet(deltaZero, deltaValue);
                                im.computeMethod(m, a);
                                CollectorResult cr = im.comparison(true);
                                cr.computeMeans();
                                if (cr.fMean > fMeasure)
                                {
                                    Console.WriteLine();
                                    String s = "deltaZero=" + deltaZero + ", deltaValue=" + deltaValue + ", scalarDistance=" + scalarDistance + ", windowSize=" + windowSize;
                                    Console.WriteLine(s);
                                    Console.WriteLine(cr);
                                    Console.WriteLine();
                                    fMeasure = cr.fMean;
                                    System.IO.File.AppendAllText(@"output\SMEFO-DS-LC.txt", s + Environment.NewLine + cr + Environment.NewLine);
                                }
                                else
                                    Console.Write("*");
                            }
                        }
                    }
                }
            }
            Console.WriteLine("FINITO");
            */

            /*
            
            */
        }
    }
}
