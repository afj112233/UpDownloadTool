using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell;

namespace ICSStudio.OrganizerPackage.HistoryFile
{
    internal class HistoryFileItemMenuCommand:OleMenuCommand
    {
        public HistoryFileItemMenuCommand( EventHandler invokeHandler, EventHandler beforeQueryStatus, CommandID id,string file) : base(invokeHandler, null, beforeQueryStatus, id)
        {
            File = file;
        }

        public string File { get; set; }

        //private bool _isMatched = false;

        //public override bool DynamicItemMatch(int cmdId)
        //{
        //    // Call the supplied predicate to test whether the given cmdId is a match.
        //    // If it is, store the command id in MatchedCommandid
        //    // for use by any BeforeQueryStatus handlers, and then return that it is a match.
        //    // Otherwise clear any previously stored matched cmdId and return that it is not a match.
        //    //if (!_isMatched&&_matches(this))
        //    //{
        //    //    this.MatchedCommandId = cmdId;
        //    //    _isMatched = true;
        //    //    return true;
        //    //}
        //    //_isMatched = true;
        //    //this.MatchedCommandId = 0;
        //    return false;
        //}
    }
}
