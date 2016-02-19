using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fFormations
{
    public class DataManager
    {
        //parser singleton
        Parser P = Parser.getParser();
        private static int idCounter = 0;

        private int id {get;}
        private readonly List<Frame> frames;
        private readonly List<Group> groups;

        private Dictionary<int, Tuple<Frame, Group>> data { get; }

        public DataManager(string framesPath, string GTPath)
        {
            id = idCounter;
            idCounter++;

            P.setDataFile(framesPath);
            frames = P.readData();
            P.setGTFile(GTPath);
            groups = P.readGT(frames);

            setDictionary();
        }


        //if frame id exists, returns the frame, otherwise null
        public Frame getFrameById(int id)
        {
            if (data == null)
                return null;

            Tuple<Frame, Group> pair;
            if (data.TryGetValue(id, out pair))
                return pair.Item1;
            else return null;
        }

        //if frame id exists, returns the gt, otherwise null
        public Group getGTById(int id)
        {
            if (data == null)
                return null;

            Tuple<Frame, Group> pair;
            if (data.TryGetValue(id, out pair))
                return pair.Item2;
            else return null;
        }

        //associates frames and groups in an unique disctionary
        private void setDictionary()
        {
            foreach(Frame f in frames)
            {      
                Group g = groups.Find(x => x.IdFrame.IdFrame == f.IdFrame);
                data.Add(f.IdFrame, new Tuple<Frame, Group>(f, g));
            }
        }
    }
}
