using System;
using System.Collections.Generic;
using System.Text;

namespace GSM
{
	public class ShortMessage
    {

        #region Private Variables
        private string index;
		private string status;
		private string sender;
		private string alphabet;
		private DateTime sentDate;
		private string message;
        private string recipient;
        public bool IsRead { get; set; }
        #endregion

        #region Public Properties
        public string Index
		{
			get { return index;}
			set { index = value;}
		}
		public string Status
		{
			get { return status;}
			set { status = value;}
		}
		public string Sender
		{
			get { return sender;}
			set { sender = value;}
		}
		public string Alphabet
		{
			get { return alphabet;}
			set { alphabet = value;}
		}
        public DateTime SentDate
		{
			get { return sentDate;}
			set { sentDate = value;}
		}
		public string Text
		{
			get { return message;}
			set { message = value;}
        }
        public string Recipient
        {
            get { return recipient; }
            set { recipient = value; }
        }
        #endregion

    }

    public class ShortMessageCollection : List<ShortMessage>
    {
    }
}
