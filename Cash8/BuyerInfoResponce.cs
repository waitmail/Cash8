using System;
using System.Collections.Generic;
using System.Text;

namespace Cash8
{
    class BuyerInfoResponce
    {
        public Cards cards { get; set; }
        public Buyer buyer { get; set; }
        public Balance balance { get; set; }
        public string requestId { get; set; }
        public int res { get; set; }


        public BuyerInfoResponce()
        {
            cards = new Cards();
            buyer = new Buyer();
            balance = new Balance();
        }

        public class CardStatus
        {

        }

        public class Card
        {
            public string cardNum { get; set; }
            public string state { get; set; }
            public string mode { get; set; }
            public string dateIssued { get; set; }
            public string dateExpired { get; set; }
            public string dateActivated { get; set; }
            public string dateRegistered { get; set; }
            public string discount { get; set; }
            public string value { get; set; }
            public CardStatus cardStatus { get; set; }
        }

        public class Cards
        {
            public List<Card> card { get; set; }
        }

        public class Buyer
        {
            public string uid { get; set; }
            public string firstName { get; set; }
            public string middleName { get; set; }
            public string lastName { get; set; }
            public string phone { get; set; }
            public string spendAllowed { get; set; }
            public string phoneConfirmed { get; set; }
        }

        public class Balance
        {
            public string balance { get; set; }
            public string activeBalance { get; set; }
            public string inactiveBalance { get; set; }
            public string oddMoneyBalance { get; set; }
            public string oddMoneyFlags { get; set; }
        }

    }
}
