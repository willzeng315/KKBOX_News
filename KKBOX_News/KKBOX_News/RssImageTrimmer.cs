using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Data;

namespace KKBOX_News
{
    public class RssImageTrimmer : IValueConverter
    {
        // Clean up text fields from each SyndicationItem. 
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;

            string ImageSource = "";

            int startIndex = value.ToString().IndexOf("img src='");
            int EndIndex = value.ToString().IndexOf("jpg");
            int srcLen = EndIndex - startIndex - 6;

            if (startIndex > EndIndex || srcLen <0)
                return null;

            ImageSource = value.ToString().Substring(startIndex + 9, srcLen);
            Debug.WriteLine(ImageSource);

            return ImageSource;
        }

        // This code sample does not use TwoWay binding, so we do not need to flesh out ConvertBack.  
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
