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
        public void setDataFile(string dataFile)
        {
            //check data file path
            if (!File.Exists(dataFile))
                throw new FileNotFoundException("Data file does not exixts");
            else
                DataFile = dataFile; 
        }

        public void setGTFile(string gtFile)
        {
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
            string[] dataLines = null;
            
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
            while (i < dataLines?.Length)
            {
                //first line has to be a frame header in both files
                Match dataMatch = Regex.Match(dataLines[i], framePattern);
                if (dataMatch.Success)
                {
                    int id = Int32.Parse(dataMatch.Groups[1].Value); //frame id
                    int n_people = Int32.Parse(dataMatch.Groups[2].Value); //people inside frame
                    List<Person> people = new List<Person>();
                   // Frame newFrame = FactoryFrame.createEmptyFrame(id); //create empty frame

                    for (int k = 1; k <= n_people; k++)
                    {
                        Match p = Regex.Match(dataLines[i + k], personPattern);
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
                    i = i + n_people;
                }
                else
                {
                    i++;
                }
            }

            return frames;
        }

        public List<Group> readGT(List<Frame> frames)
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

                    // trova il frame con ID == id
                    Frame currentFrame = frames.Find(x => x.IdFrame == id);
                    Group newGroup = FactoryGroup.createGroup(currentFrame);

                    for (int j = 1; j <= n; j++) //for each subgroup
                    {
                        List < Person > peopleGroup = new List<Person>();
                        string[] elements = gtLines[i + j].Split(new Char[] { ' ' }); //space is the separator
                        foreach(string s in elements)
                        {
                            int personId = Int32.Parse(s);
                            Person p = currentFrame.getPersonById(personId);
                            peopleGroup.Add(p);
                        }

                        newGroup.addSubGroup(peopleGroup);
                    }
                    //add the new grouping
                    groups.Add(newGroup);
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

    }
}
