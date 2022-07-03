using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFapp1TGbot
{
    struct MessageLog
    {
        public string Time { get; set; }

        public long ID { get; set; }

        public string Msg { get; set; }

        public string FirstName { get; set; }

        public MessageLog(string Time, string Msg, string FirstName, long ID)
        {
            this.Time = Time;
            this.Msg = Msg;
            this.FirstName = FirstName;
            this.ID = ID;
        }

        //public override string ToString()
        //{
        //    return $"{Time} {Msg} {FirstName}";
        //}
    }
}
