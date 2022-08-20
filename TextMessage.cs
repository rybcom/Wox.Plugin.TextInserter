using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace TextInserter
{
    public class TextMessage
    {
        public string Key { get; set; }

        public string Value { get; set; }
    }


    public class TextMessageManager
    {
        private readonly List<TextMessage> _messageList = new List<TextMessage>();

        public void LoadMessages(string filepath)
        {
            _messageList.Clear();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(File.ReadAllText(filepath));

            XmlNodeList items = doc.DocumentElement.SelectNodes("/text_list/item");

            foreach (XmlNode node in items)
            {
                string key = node.Attributes["key"].Value;
                string value = node.Attributes["value"].Value;

                _messageList.Add(new TextMessage() { Key = key, Value = value });
            }
        }

        public List<TextMessage> GetMessages() => _messageList;

    }


}
