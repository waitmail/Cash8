using System;
using System.Collections.Generic;
using System.Text;

namespace Cash8
{
    class CrmCabinetInfo
    {
        public List<Card> cards { get; set; }
        public List<Balance> balances { get; set; }
        public Buyer buyer { get; set; }
        public int res { get; set; }
        public string requestId { get; set; }

        public class Card
        {
            public string cardNum { get; set; }
            public string state { get; set; }
            public string cardStatus { get; set; }
        }

        public class Balance
        {
            public string balanceTypeId { get; set; }
            public string balanceTypeName { get; set; }
            public int balance { get; set; }
            public int inactiveBalance { get; set; }
        }

        public class Subscription
        {
            public string subscriptionId { get; set; }
            public string listId { get; set; }
            public string dateCreated { get; set; }
            public string sendEmail { get; set; }
            public string sendSMS { get; set; }
        }

        public class Buyer
        {
            public string email { get; set; }
            public string lastName { get; set; }
            public string firstName { get; set; }
            public string middleName { get; set; }
            public string sex { get; set; }
            public string birthDate { get; set; }
            public string registrationDate { get; set; }
            public string lastAuthDate { get; set; }
            public string addrZIP { get; set; }
            public string addrRegion { get; set; }
            public string addrCity { get; set; }
            public string addrStreet { get; set; }
            public string addrBuilding { get; set; }
            public string addrHousing { get; set; }
            public string addrFlat { get; set; }
            public string cellphone { get; set; }
            public string sendSMS { get; set; }
            public string sendEmail { get; set; }
            public string preferredContact { get; set; }
            public string keyword { get; set; }
            public string cards { get; set; }
            public object buyerStatus { get; set; }
            public string phoneConfirmed { get; set; }
            public string emailConfirmed { get; set; }
            public string spendAllowed { get; set; }
            public string passportSeries { get; set; }
            public string passportNumber { get; set; }
            public string passportIssueDate { get; set; }
            public string passportIssueAuthority { get; set; }
            public object oddMoneyBalance { get; set; }
            public object oddMoneyFlags { get; set; }
            public string buyerId { get; set; }
            public string fullRegistrationDate { get; set; }
            public string bonusReceived { get; set; }
            public string fullProfile { get; set; }
            public string title { get; set; }
            public string printName { get; set; }
            public string correctName { get; set; }
            public string spendAllowedLog { get; set; }
            public string registrarUserId { get; set; }
            public List<Subscription> subscriptions { get; set; }
            public List<object> offers { get; set; }
        }

    }
}
