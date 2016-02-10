using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Globalization;

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

        public void start()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

            //read lines from data file
            try
            {
                string[] dataLines = File.ReadAllLines(DataFile);
            }
            catch(FileNotFoundException e)
            {
                Console.Write("Errore file data: ");
                Console.WriteLine(e.Message);
            }

            


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


        }

    }
}
