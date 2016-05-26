using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/*
    temp[0] sono i Correct, cioè i gruppi che corrispondono
    temp[1] sono FalsePositive cioè gruppi tirati fuori dal mio algoritmo ma che in realtà non esistono
    temp[2] sono FalseNegative cioè gruppi che esistono nel ground truth ma che il mio algoritmo non ha trovato

    La precisione(in inglese precision) è la proporzione di documenti pertinenti fra quelli recuperati:
    P = (numero di gruppi pertinenti recuperati) / (numero di gruppi recuperati)= temp[0]/temp[0] + temp[1]
    
    Il recupero o richiamo (in inglese recall) è la proporzione fra il numero di documenti pertinenti recuperati 
    e il numero di tutti i documenti pertinenti disponibili nella collezione considerata:
    R = (numero di documenti pertinenti recuperati) / (numero di documenti pertinenti)=temp[0]/temp[0]+temp[2]

    F1 è la media armonica di P e R. E' un valore bilanciato che pesa in modo uguale sia P che R.
*/
namespace fFormations
{
    public struct InfoRetrivial
    {
        public int correct, falsePositive, falseNegative, diffNumGroups;
        public double precision, recall, f1;
        public InfoRetrivial(int correct,int falsePositive,int falseNegative,int diffNumGroups)
        {
            this.correct = correct;
            this.falsePositive = falsePositive;
            this.falseNegative = falseNegative;
            this.diffNumGroups = diffNumGroups;

            precision = (double)correct / (double)(correct + falsePositive); //precision = tp / (tp + fp)
            if (double.IsNaN(precision)) precision = 1;

            recall = (double)correct / (double)(correct + falseNegative); //recall = tp / (tp + fn)
            if (double.IsNaN(recall)) recall = 1;

            if (precision + recall != 0)
                f1 = (2 * precision * recall) / (precision + recall);
            else f1 = 0;
        }
    }
    public class Result
    {
        public double precision { get; set; }
        public double recall { get; set; }
        public double f1 { get; set; }
        public double correct { get; set; }
        public double falsePositive { get; set; }
        public double falseNegative { get; set; }
        int refIDFrame;

        public Result(InfoRetrivial ir, int IdFrame)
        {
            refIDFrame = IdFrame;

            correct = ir.correct;
            falsePositive = ir.falsePositive;
            falseNegative = ir.falseNegative;

            precision = ir.precision;
            recall = ir.recall;
            f1 = ir.f1;

        }

        public override string ToString()
        {
            return "Result: Id = " + refIDFrame + ", prec = " + precision + ", Rec = " + recall + ", f1 = " + f1 +"\n Correct = "+ correct+ ", FN = "+falseNegative+", FP = "+falsePositive;
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

        public double getSumPrec()
        {
            double p = 0;
            foreach (Result r in l)
            {
                p += r.precision;
            }
            return p;
        }

        public void computeMeans()
        {
            precisionMean = 0;
            recallMean = 0;
            fMean = 0;
            if (l.Count != 0)
            {
                foreach (Result r in l)
                {
                    precisionMean += r.precision;
                    recallMean += r.recall;
                    fMean += r.f1;
                }
                precisionMean /= l.Count;
                recallMean /= l.Count;
                fMean /= l.Count;
            }
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
