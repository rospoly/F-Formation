using System;
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

        public Result(List<int> temp, int IdFrame)
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
            return "Result: Id = " + refIDFrame + ", prec = " + precision + ", Rec = " + recall + ", f1 = " + f1;
        }
    }

    class CollectorResult
    {
        public List<Result> l;
        public double precisionMean { get; set; }
        public double recallMean { get; set; }
        public double fMean { get; set; }

        public CollectorResult()
        {
            l = new List<Result>();
        }

        public void addResult(Result r) {
            l.Add(r);
        }

        public void computeMeans()
        {
            foreach(Result r in l)
            {
                precisionMean += r.precision;
                recallMean += r.recall;
                fMean += r.f1;
            }
            precisionMean /= l.Count;
            recallMean /= l.Count;
            fMean /= l.Count;
        }

        public string getAllResultsStrings()
        {
            string s = "";
            foreach (Result r in l)
                s = s + r.ToString() + "\n";
            return s;
        }

        public override string ToString()
        {
            return "CollectorResult: precision = " + precisionMean + ", recall = " + recallMean + ", fmeasure = " + fMean;
        }
    }
}
