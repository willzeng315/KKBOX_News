using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Xml.Linq;
using System.Diagnostics;
using KKBOX_News.ViewModels;

namespace KKBOX_News
{
    public partial class MainPage : PhoneApplicationPage
    {
        // 建構函式
        public MainPage()
        {
            InitializeComponent();

            // 將清單方塊控制項的資料內容設為範例資料
            DataContext = App.ViewModel;
        }

        // 載入 ViewModel 項目的資料
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.SelectTopicParser();
            }
        }

        private void LongListSelector_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            
            LongListSelector box = sender as LongListSelector;
            //SimpleItem item = box.SelectedItem as SimpleItem;
            ItemViewModel n = box.SelectedItem as ItemViewModel;

            string destination = "/TopicSelected.xaml";

                destination += String.Format("?XML={0}",n.LineThree);
           

            this.NavigationService.Navigate(new Uri(destination, UriKind.Relative));
            //Debug.WriteLine(n.LineOne);
        }

        //private void PhoneApplicationPage_Loaded_1(object sender, RoutedEventArgs e)
        //{
        //    Uri uri = new Uri("https://mail.kkbox.com.tw/~willzeng/SelectTopic.xml"); 

        //    WebClient webClient = new WebClient();
        //    webClient.DownloadStringCompleted += OnDownloadStringCompleted;
        //    webClient.DownloadStringAsync(uri);
        //}

        //void OnDownloadStringCompleted(object sender, DownloadStringCompletedEventArgs args)
        //{
            
        //    XDocument doc = XDocument.Parse(args.Result);
        //    Console.WriteLine(doc.ToString());
        //    IEnumerable<XElement> eles = doc.Element("kkbox_news").Elements("topic");
        //   foreach (XElement el in eles) {
        //       Debug.WriteLine(el.Element("title").Value); 
        //      // + " , " + el.Element("title").Value 
        //      // + " , " + el.Attribute("publisher").Value);
        //   }

        //}
    }
}