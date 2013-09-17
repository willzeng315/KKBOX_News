using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml.Linq;
namespace KKBOX_News
{
    class SelectedTopicsXmlParser
    {
        //ObservableCollection<SelectedTopics> students = new ObservableCollection<SelectedTopics>();
        //SelectedTopics SelectedTopics;
        public SelectedTopicsXmlParser()
        {
            Uri uri = new Uri("http://www.charlespetzold.com/Students/students.xml"); // , UriKind.Relative);

            WebClient webClient = new WebClient();
            webClient.DownloadStringCompleted += OnDownloadStringCompleted;
            webClient.DownloadStringAsync(uri);
        }

        void OnDownloadStringCompleted(object sender, DownloadStringCompletedEventArgs args)
        {
            XDocument Xdocument = XDocument.Parse(args.Result);


            //var data = from query in loadedData.Descendants("person")
            //           select new Person
            //           {
            //               FirstName = (string)query.Element("firstname"),
            //               LastName = (string)query.Element("lastname"),
            //               Age = (int)query.Element("age")
            //           };
            //listBox.ItemsSource = data;
            //List<XDocument> xmlElements = Xdocument..Elements("Topic");
            //foreach(XmlElement xe in xmlElements)
            //{
            //    String sIcon = xe.GetElement("icon");
            //    String sTitle = xe.GetElement("title");
            //    String sURL = xe.GetElement("url");
            //    SimpleItem si = new SimpleItem() {Title = sTitle, Description = sURL};
            //    Items = new List<SimpleItem>();
            //    Items.Add(si);
            //}
            
            //StringReader reader = new StringReader(args.Result);
            //XmlSerializer xml = new XmlSerializer(typeof(SelectedTopics));
            //SelectedTopics = xml.Deserialize(reader) as SelectedTopics;

            //DispatcherTimer tmr = new DispatcherTimer();
            //tmr.Tick += TimerOnTick;
            //tmr.Interval = TimeSpan.FromMilliseconds(100);
            //tmr.Start();
        }
    }
}
