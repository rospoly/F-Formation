﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fFormations
{
    class Result
    {
        public double precision { get; set; }
        public double recall { get; set; }
        public double f1 { get; set; }
        int refIDFrame;

        public Result(List<int> temp, int IdFrame = -1)
        {
            refIDFrame = IdFrame;
            precision = temp[0] / (double)(temp[0] + temp[1]); //precision = tp / (tp + fp)
            if (double.IsNaN(precision)) precision = 1;

            recall=temp[0]/(double)(temp[0]+temp[2]); //recall = tp / (tp + fn)
            if (double.IsNaN(recall)) recall = 1;

            f1 = (2*precision* recall) / (precision+recall);
        }

        public override string ToString()
        {
            String s = (refIDFrame == -1 ? "Resoconto: \n" : "IdFrame: " + refIDFrame+"\n");
            s = s + "precision: "+precision+"; Recall: "+recall+"; f1: "+f1+";\n";
            return s;
        }
    }

    class CollectorResult
    {
        public List<Result> l;
        public CollectorResult()
        {
            l = new List<Result>();
        }

        public void addResult(Result r) {
            l.Add(r);
        }

        public void computation() {
        }

        public override string ToString()
        {
            foreach (Result r in l) {
                return r.ToString();
            }
            return "";
        }
    }
}
