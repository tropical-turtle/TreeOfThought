using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tree_Of_Thought
{
    public class FileState
    {
        public bool IsFileChanged;
        public bool IsFileSaved;

        public FileState()
        {
            IsFileChanged = false;
            IsFileSaved = false;
        }
    }
}
