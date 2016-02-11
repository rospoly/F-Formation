using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Globalization;
using System.Text.RegularExpressions;

namespace fFormations
{
    public class Parser
    {
        //parser singleton object
        private static Parser singParser = new Parser();

        //input files paths
        public string DataFile { get; private set; }
        public string GtFile { get; private set; }

        //private constructur
        private Parser() { }

        //returns the singleton
        public static Parser getParser()
        {
            return singParser;
        }

        //sets the input files, throw exception if one does not exists
        public void setFiles(string dataFile, string gtFile)
        {
            //check data file path
            if (!File.Exists(dataFile))
                throw new FileNotFoundException("Data file does not exixts");
            else
                DataFile = dataFile;

            //check GT file path
            if (!File.Exists(gtFile))
                throw new FileNotFoundException("GT file does not exixts");
            else
                GtFile = gtFile;

        }

        public List<Frame> readData()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

            List<Frame> frames = new List<Frame>();

            String framePattern = @"(\d+)\s+(\d+)"; //frame id and number of elements
           // String groupPattern = @"(\d+)\s+([-+*/])\s+(\d+)";
            String personPattern = @"(\d+)\s+(\d+)\s+(\d+)\s+(\d+)";

            //read lines from data file
            string[] dataLines = null;
            try
            {
                dataLines = File.ReadAllLines(DataFile);
            }
            catch(FileNotFoundException e)
            {
                Console.Write("Errore file data: ");
                Console.WriteLine(e.Message);
            }

            int i = 0;
            while (i < dataLines?.Length)
            {
                //first line has to be a frame header
                Match m = Regex.Match(dataLines[i], framePattern);
                if (m.Success)
                {
                    int id = Int32.Parse(m.Groups[1].Value); //frame id
                    int n = Int32.Parse(m.Groups[2].Value); //frame elements
                    List < Person > people = new List<Person>();
                    for (int j = 1; j <= n; j++)
                    {
                        Match p = Regex.Match(dataLines[i + j], personPattern);
                        if (p.Success)
                        {
                            int pId = Int32.Parse(p.Groups[1].Value);
                            double x = Double.Parse(p.Groups[2].Value);
                            double y = Double.Parse(p.Groups[3].Value);
                            double theta = Double.Parse(p.Groups[4].Value);
                            people.Add(FactoryPerson.createPerson(pId, x, y, theta)); //add the person to the list
                        }
                    }

                    //create the frame
                    frames.Add(FactoryFrame.createFrame(id, people));

                    //update i
                    i = i + n;
                }
                else
                {
                    i++;
                }
            }



            return frames;
        }

        public List<Group> readGT()
        {
            List<Group> groups = new List<Group>();

            //read lines from gt file
            try
            {
                string[] gtLines = File.ReadAllLines(DataFile);
            }
            catch (FileNotFoundException e)
            {
                Console.Write("Errore file data: ");
                Console.WriteLine(e.Message);
            }

            return groups;
        }

    }
}
