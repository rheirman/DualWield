using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DualWield.Settings
{
    public class Record
    {
        public bool isSelected = false;
        public String label = "";
        public int extraRotation = 0;
        public Record()
        {

        }
        public Record(bool isSelected, String label)
        {
            this.isSelected = isSelected;
            this.label = label;
        }
        public override string ToString()
        {
            return this.isSelected + "," + this.label;
        }
    }

}
