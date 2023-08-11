using System;
using System.Collections.Generic;
using System.Text;

namespace Cash8
{
    class NonFiscallDocument
    {

        public string type { get; set; }
        public List<Item> items { get; set; }

        public class Item
        {
            public string type { get; set; }
            public string text { get; set; }
            public string alignment { get; set; }
            public string barcode { get; set; }
            public string barcodeType { get; set; }
            public int? scale { get; set; }
        }
    }
}
