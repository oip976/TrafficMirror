using SHDocVw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TrafficMirror.Classes
{
    static class IEZoomScroll
    {
        public static void SetZoom(this System.Windows.Controls.WebBrowser wb, int zoom)
        {
            try
            {
                FieldInfo webBrowserInfo = wb.GetType().GetField("_axIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);

                object comWebBrowser = null;
                object zoomPercent = zoom;

                if (webBrowserInfo != null)
                    comWebBrowser = webBrowserInfo.GetValue(wb);
                if (comWebBrowser != null)
                {
                    InternetExplorer ie = (InternetExplorer)comWebBrowser;
                    ie.ExecWB(OLECMDID.OLECMDID_OPTICAL_ZOOM, OLECMDEXECOPT.OLECMDEXECOPT_DONTPROMPTUSER, ref zoomPercent, IntPtr.Zero);

                    if (wb.Document is mshtml.HTMLDocument htmlDoc)
                    {
                        htmlDoc.parentWindow.scrollTo(0, 0);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
