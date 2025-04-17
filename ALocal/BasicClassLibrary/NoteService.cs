using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicClassLibrary
{
    public class NoteService
    {
        private NoteManager NoteManager;
        public NoteService(NoteManager noteManager)
        {
            this.NoteManager = noteManager;
        }
    }
}
