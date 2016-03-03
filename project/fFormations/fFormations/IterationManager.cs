﻿using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace fFormations
{
    class IterationManager
    {
        public DataManager DM;
        public List<Group> computed;

        public IterationManager(DataManager DM) {
            this.DM = DM;
            computed = new List<Group>();
        }

        /// <summary>
        /// Non Implementato da sistemare
        /// </summary>
        /// <param name="m"></param>
        /// <param name="a"></param>
        public void computeMethod(Method m, Affinity a)
        {
            foreach (Frame f in DM.getAllFrames())
            {
                a.computeAffinity(f); //calcolo affinità su questo frame
                m.Initialize(a);
                computed.Add(m.ComputeGroup());
            }
        }

        /*public void computeMethod(Method m, List<Affinity> a)
        {
            foreach (Affinity affinity in a) {
                affinity.computeAffinity();
                m.Initialize(affinity);
                computed.Add(m.ComputeGroup());
            }
        }*/

        public CollectorResult comparison() {
            Console.WriteLine("GroupList computed has size: " + computed.Count);
            CollectorResult rs = new CollectorResult();
            foreach (Group g in computed) {
                List<int> temp = Group.Compare(DM.getGTById(g.IdFrame.IdFrame), g);
                rs.addResult(new Result(temp, g.IdFrame.IdFrame));
            }
            //lista di tutti i risultati
            rs.ToString();
            //funzione di riduzione su tutti i risultati (ex.average)
            //aggiunta alla fine di rs
            //ritorno rs
            rs.computation();
            return rs;
        }

        static void Main(string[] args) {
            string dataFile = @"input/features.txt";
            string gtFile = @"input/gt.txt";
            /* PARSER TEST */
            //get singleton and set paths
            DataManager dm = new DataManager(dataFile,gtFile);
            foreach (Frame frame in dm.getAllFrames())
            {
                Affinity a = new ProxOrient(frame);
                Method m = new LocalDominantSet(1E-10);
                //GlobalDominantSet(1E-10);
                m.Initialize(a);
                Group my=m.ComputeGroup();
                Result t=new Result(Group.Compare(my,dm.getGTById(frame.IdFrame)));
                Console.WriteLine(t);
                Console.WriteLine(my);
                Console.WriteLine(dm.getGTById(frame.IdFrame));
                Console.ReadLine();
            }
        }
    }
}
