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

        private String framePattern = @"(\d+)\s+(\d+)"; //frame header: id and number of elements
        private String personPattern = @"(\d+)\s+(\-?\d*\.?\d+)\s+(\-?\d*\.?\d+)\s+(\-?\d*\.?\d+)";

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
            List<Group> groups = new List<Group>();
            string[] gtLines = null;
            string[] dataLines = null;
            
            //read lines from gt file
            try
            {
                gtLines = File.ReadAllLines(DataFile);
            }
            catch (FileNotFoundException e)
            {
                Console.Write("Error file gt: ");
                Console.WriteLine(e.Message);
            }
            
            //read lines from data file
            try
            {
                dataLines = File.ReadAllLines(DataFile);
            }
            catch(FileNotFoundException e)
            {
                Console.Write("Errore file data: ");
                Console.WriteLine(e.Message);
            }

            int i = 0; //counter on data lines
            int j = 0; //counter on gt lines
            while (i < dataLines?.Length && j<gtLines?.Length)
            {
                //first line has to be a frame header in both files
                Match dataMatch = Regex.Match(dataLines[i], framePattern);
                Match gtMatch = Regex.Match(gtLines[j], framePattern);
                int id = checkMatch(dataMatch, gtMatch);//frame id or -1
                if ( id >= 0)
                {
                    int n_people = Int32.Parse(dataMatch.Groups[2].Value); //frame people
                    int n_groups = Int32.Parse(gtMatch.Groups[2].Value); //groups in the frame
                    Frame newFrame = FactoryFrame.createEmptyFrame(id); //create empty frame

                    for (int k = 1; k <= n_people; j++)
                    {
                        Match p = Regex.Match(dataLines[i + k], personPattern);
                        if (p.Success)
                        {
                            int pId = Int32.Parse(p.Groups[1].Value);
                            double x = Double.Parse(p.Groups[2].Value);
                            double y = Double.Parse(p.Groups[3].Value);
                            double theta = Double.Parse(p.Groups[4].Value);
                            newFrame.addPerson(FactoryPerson.createPerson(pId, x, y, theta)); //add the person to the list
                        }
                    }

                    //create the frame
                    frames.Add(newFrame);

                    //update i
                    i = i + n_people;
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
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

            List<Group> groups = new List<Group>();
            string[] gtLines = null;

            //read lines from gt file
            try
            {
                gtLines = File.ReadAllLines(DataFile);
            }
            catch (FileNotFoundException e)
            {
                Console.Write("Error file gt: ");
                Console.WriteLine(e.Message);
            }

            int i = 0;
            while (i < gtLines?.Length)
            {
                //first line has to be a frame header
                Match m = Regex.Match(gtLines[i], framePattern);
                if (m.Success)
                {
                    int id = Int32.Parse(m.Groups[1].Value); //frame id
                    int n = Int32.Parse(m.Groups[2].Value); //frame groups
                    //Group newGroup = new Group();
                    for (int j = 1; j <= n; j++)
                    {
                        string[] elements = gtLines[i + j].Split(new Char[] { ' ' }); //space is the separator

                        
                    }

                    //create the group
                    

                    //update i
                    i = i + n;
                }
                else
                {
                    i++;
                }
            }

            return groups;
        }

        //returns -1 if not valid, frame id if correct
        private int checkMatch(Match m1, Match m2)
        {
            if (m1.Success && m2.Success && (m1.Groups[1].Value == m2.Groups[1].Value))
                return Int32.Parse(m1.Groups[1].Value);
            else
                return -1;
        }
    }
}
