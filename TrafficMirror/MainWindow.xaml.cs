/*//////////////////////////////////////////////////////////////////////////////////////////////////////////////////*/
/*                                                                                                                  */
/*    (c) Cooperative Media Lab, Bamberg, Germany                                                                   */
/*                                                                                                                  */
/*     This file is part of the Master Thesis "TrafficMirror: An Ambient Zoomable Display for Traffic Information." */
/*                                                                                                                  */
/*     Chair for Human-Computer Interaction                                                                         */
/*                                                                                                                  */
/*     Friedemann Dürrbeck                                                                                          */
/*     1766526                                                                                                      */
/*     friedemann-thomas.duerrbeck@stud.uni-bamberg.de                                                              */
/*                                                                                                                  */
/*//////////////////////////////////////////////////////////////////////////////////////////////////////////////////*/

using BingMapsRESTToolkit;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Globalization;
using System.IO.Ports;
using TrafficMirror.Classes;
using Microsoft.Maps.MapControl.WPF;
using System.Reflection;
using SHDocVw;
using System.Windows.Navigation;
using static NativeMethods;

namespace TrafficMirror
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Private Properties

        //Progress Bar Properties
        private DispatcherTimer _timer;
        private TimeSpan _time;

        //BingMaps Authentication Key
        private readonly string BingMapsKey = System.Configuration.ConfigurationManager.AppSettings.Get("BingMapsKey");

        //speed of the car, PWM value from 0-255 => 0 = STOP; 255 = full speed
        private readonly byte car_speed = 255;
        private readonly byte speedOnSeriousTraffic = 10;
        private readonly byte speedOnModerateTraffic = 150;
        private readonly byte speedOnLowTraffic = 255;

        //retrieve Serial Ports for displaying in ComboBox
        private readonly string[] ports = SerialPort.GetPortNames();

        //
        private TMController tMController = new TMController();

        //State of the buttons being pressed
        private byte keyState = 0;

        //LED colour dependent on traffic serverity
        private byte LEDState = 0;

        private static List<ObjectNode> nodes = new List<ObjectNode>();

        private readonly KeyValuePair<string, string> lowTraffic = new KeyValuePair<string, string>("Severity", "\"LowImpact\"");
        private readonly KeyValuePair<string, string> minorTraffic = new KeyValuePair<string, string>("Severity", "\"Minor\"");
        private readonly KeyValuePair<string, string> moderateTraffic = new KeyValuePair<string, string>("Severity", "\"Moderate\"");
        private readonly KeyValuePair<string, string> seriousTraffic = new KeyValuePair<string, string>("Severity", "\"Serious\"");

        #endregion

        public MainWindow()
        {
            InitializeComponent();

            DataContext = new
            {
                sensorData = new SensorDataWPF(),
            };

            numCom.ItemsSource = ports;
            numCom.SelectedIndex = 0;
            disBtn.IsEnabled = false;
            CbManualMode.IsEnabled = false;

            inp_FROM.Text = "Krackhardtstr. 2, Bamberg";
            inp_TO.Text = "Hugo-Junkers-Str. 9, Nürnberg";

            _timer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate
            {
                if (_time != null)
                {
                    RequestProgressBarText.Text = string.Format("Time remaining: {0}", _time);

                    if (_time == TimeSpan.Zero)
                    {
                        _timer.Stop();
                    }

                    _time = _time.Add(TimeSpan.FromSeconds(-1));
                }
            }, Application.Current.Dispatcher);

        }

        #region Traffic Request
        /// <summary>
        /// Creates a Traffic Request.
        /// </summary>
        /*private void TrafficBtn_Clicked(object sender, RoutedEventArgs e)
        {

            var r = new TrafficRequest()
            {
                Culture = "de-de",
                TrafficType = new List<TrafficType>()
                {
                    TrafficType.Accident,
                    TrafficType.Congestion
                },
                //Severity = new List<SeverityType>()
                //{
                //    SeverityType.LowImpact,
                //    SeverityType.Minor
                //},
                MapArea = new BoundingBox()
                {
                    SouthLatitude = 46,
                    WestLongitude = -124,
                    NorthLatitude = 50,
                    EastLongitude = -117
                },
                IncludeLocationCodes = true,
                BingMapsKey = BingMapsKey
            };

            ProcessRequest(r);
        }*/
        #endregion

        #region Route Request
        /// <summary>
        /// Creates a Driving Route Request.
        /// </summary>
        private void RouteBtn_Clicked(object sender, RoutedEventArgs e)
        {
            //if (tMController.CheckSerialPort())
            //{
                try
                {
                    var r = new RouteRequest()
                    {
                        RouteOptions = new RouteOptions()
                        {
                            Avoid = new List<AvoidType>()
                        {
                            AvoidType.MinimizeTolls
                        },
                            TravelMode = TravelModeType.Driving,
                            DistanceUnits = DistanceUnitType.Kilometers,
                            Heading = 45,
                            RouteAttributes = new List<RouteAttributeType>()
                            {
                                RouteAttributeType.RoutePath
                            },
                            Optimize = RouteOptimizationType.TimeWithTraffic
                        },
                        Waypoints = new List<SimpleWaypoint>()
                        {
                            new SimpleWaypoint()
                            {
                                Address = inp_FROM.Text
                            },
                            new SimpleWaypoint()
                            {
                                Address = inp_TO.Text
                            }
                        },
                        BingMapsKey = BingMapsKey
                    };

                    ProcessRequest(r);
                    LoadBingMapsInBrowser(inp_FROM, inp_TO);
                    BingMapsTab.IsSelected = true;
            }
                catch (Exception ex)
                {
                    throw new Exception("Exception thrown: {0}", ex);
                }
           /* }
            else
            {
                MessageBox.Show("Please first connect to Arduino Board", "Not Connected!");
            }*/
        }
        #endregion

        #region Process Request
        /// <summary>
        /// Method triggers the request by calling the execute method of the BingMaps RESTToolKit.
        /// <param name="request"></param>
        private async void ProcessRequest(BaseRestRequest request)
        {
            try
            {
                RequestProgressBar.Visibility = Visibility.Visible;
                RequestProgressBarText.Text = string.Empty;

                ResultTreeView.ItemsSource = null;

                var start = DateTime.Now;

                //Executes the request.
                var response = await request.Execute((remainingTime) =>
                {
                    if (remainingTime > -1)
                    {
                        _time = TimeSpan.FromSeconds(remainingTime);

                        RequestProgressBarText.Text = string.Format("Time remaining {0} ", _time);

                        _timer.Start();
                    }
                });

                RequestUrlTbx.Text = request.GetRequestUrl();

                var end = DateTime.Now;
                var processingTime = end - start;
                var nodes_warnings = new List<ObjectNode>();
                var nodes_all = new List<ObjectNode>();
                //Takes any Object and generates an Object Tree.
                var tree = await ObjectNode.ParseAsync("result", response);
                //List of warnings for trim function and visualisation in the view
                var list_warnings = new List<KeyValuePair<string, string>>();
                string filter_severity = "Severity";

                ProcessingTimeTbx.Text = string.Format(CultureInfo.InvariantCulture, "{0:0} ms", 
                    processingTime.TotalMilliseconds);
                RequestProgressBar.Visibility = Visibility.Collapsed;

                nodes_all.Add(tree);

                SeverityRatings_panel.Children.Clear();

                nodes_warnings.Add(tree);
                ResultTreeViewFull.ItemsSource = nodes_all;

                TrimTree(nodes_warnings, filter_severity, list_warnings);
              
                ResultTreeView.ItemsSource = nodes_warnings;

                foreach (var element in list_warnings)
                {
                    switch (element.Value)
                    {
                        case "\"Minor\"":
                        case "\"LowImpact\"":
                            TextBlock Minor_Low_sev = new TextBlock
                            {
                                Text = element.ToString(),
                                Background = Brushes.LightGreen,
                                FontSize = 20
                            };
                            SeverityRatings_panel.Children.Add(Minor_Low_sev);
                            break;
                        case "\"Moderate\"":
                            TextBlock Moderate_sev = new TextBlock
                            {
                                Text = element.ToString(),
                                Background = Brushes.Orange,
                                FontSize = 20
                            };
                            SeverityRatings_panel.Children.Add(Moderate_sev);
                            break;
                        case "\"Serious\"":
                            TextBlock Serious_sev = new TextBlock
                            {
                                Text = element.ToString(),
                                Background = Brushes.PaleVioletRed,
                                FontSize = 20
                            };
                            SeverityRatings_panel.Children.Add(Serious_sev);
                            break;
                        default:
                            break;
                    }
                }

                ProcessWarnings(list_warnings);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            _timer.Stop();
            RequestProgressBar.Visibility = Visibility.Collapsed;
        }
        #endregion

        #region Connect and Disconnect
        /// <summary>
        /// Handles events of connect & disconnect buttons
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                tMController.ConnectSerialPort(numCom.SelectedValue.ToString());

                SPStatus.Foreground = Brushes.ForestGreen;
                SPStatus.Text = "Arduino Connected!";
                disBtn.IsEnabled = true;
                conBtn.IsEnabled = false;
                numCom.IsHitTestVisible = false;
                numCom.Focusable = false;
                CbManualMode.IsEnabled = true;
            }
            catch
            {
                MessageBoxResult result = MessageBox.Show("Serial Port Not Found. Try Again", "Error");
            }
        }

        private void DisBtn_Click(object sender, RoutedEventArgs e)
        {
            if(keyState == 9)
            {
                keyState -= (byte)(tMController.FORWARD_BIT + tMController.LEFT_BIT);
                tMController.SendDirectionCommand(keyState);
                KeyDown_Events.Text = "STOP DRIVING FL";

            }
            else if (keyState == 5)
            {
                keyState -= (byte)(tMController.FORWARD_BIT + tMController.RIGHT_BIT);
                tMController.SendDirectionCommand(keyState);
                KeyDown_Events.Text = "STOP DRIVING FR";
            }
            tMController.DisconnectSerialPort();
            SPStatus.Foreground = Brushes.Red;
            SPStatus.Text = "Arduino Disconnected!";
            numCom.IsHitTestVisible = true;
            numCom.Focusable = true;
            disBtn.IsEnabled = false;
            conBtn.IsEnabled = true;
            CbManualMode.IsEnabled = false;


        }
        #endregion

        #region KeyDown & KeyUp Events
        /// <summary>
        /// Handles Keyboard events for manual control of the toy car (just for electronical testing purposes)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if(tMController.CheckSerialPort() && (CbManualMode.IsChecked == true))
            {
                switch (e.Key)
                {
                    case Key.Up:
                        Console.WriteLine("Key up");
                        KeyDown_Events.Text = "Drive Forward";
                        ImgKeyEvents.Source = new BitmapImage(new Uri("images/up_on.png", UriKind.Relative));

                        if ((keyState & tMController.FORWARD_BIT) == 0)
                        {
                            keyState += tMController.FORWARD_BIT;
                            tMController.SendDirectionCommand(keyState);
                        }
                        break;
                    case Key.Down:
                        KeyDown_Events.Text = "Drive Backward";
                        ImgKeyEvents.Source = new BitmapImage(new Uri("images/down_on.png", UriKind.Relative));

                        if ((keyState & tMController.BACKWARD_BIT) == 0)
                        {
                            keyState += tMController.BACKWARD_BIT;
                            tMController.SendDirectionCommand(keyState);
                        }
                        break;
                    case Key.Right:
                        KeyDown_Events.Text = "Drive to the Right";
                        ImgKeyEvents.Source = new BitmapImage(new Uri("images/right_on.png", UriKind.Relative));

                        if ((keyState & tMController.RIGHT_BIT) == 0)
                        {
                            keyState += tMController.RIGHT_BIT;
                            tMController.SendDirectionCommand(keyState);
                        }
                        break;
                    case Key.Left:
                        KeyDown_Events.Text = "Drive to the Left";
                        ImgKeyEvents.Source = new BitmapImage(new Uri("images/left_on.png", UriKind.Relative));

                        if ((keyState & tMController.LEFT_BIT) == 0)
                        {
                            keyState += tMController.LEFT_BIT;
                            tMController.SendDirectionCommand(keyState);
                        }
                        break;
                }
                CarSpeed.Text = car_speed.ToString();
            }
            return; 
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if(tMController.CheckSerialPort() && (CbManualMode.IsChecked == true))
            {
                switch (e.Key)
                {
                    case Key.Up:
                        if((keyState & tMController.FORWARD_BIT) > 0)
                        {
                            KeyDown_Events.Text = "Key Released";
                      

                        ImgKeyEvents.Source = null;
                        keyState -= tMController.FORWARD_BIT;
                        tMController.SendDirectionCommand(keyState);
                        }
                        break;
                    case Key.Down:
                        KeyDown_Events.Text = "Key Released";
                        ImgKeyEvents.Source = null;
                        keyState -= tMController.BACKWARD_BIT;
                        tMController.SendDirectionCommand(keyState);
                        break;
                    case Key.Right:
                        KeyDown_Events.Text = "Key Released";
                        ImgKeyEvents.Source = null;
                        keyState -= tMController.RIGHT_BIT;
                        tMController.SendDirectionCommand(keyState);
                        break;
                    case Key.Left:
                        KeyDown_Events.Text = "Key Released";
                        ImgKeyEvents.Source = null;
                        keyState -= tMController.LEFT_BIT;
                        tMController.SendDirectionCommand(keyState);
                        break;
                }
            }
            return;
           
        }
        #endregion

        #region Trim Method
        /// <summary>
        /// Trim Result of Bing Maps response
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="filter_severity"></param>
        /// <param name="list_warnings"></param>
        private void TrimTree(List<ObjectNode> nodes, string filter_severity, 
            List<KeyValuePair<string, string>> list_warnings)
        {
            ObjectNode node = null;
            for (int ndx = nodes.Count; ndx > 0; ndx--)
            {
                node = nodes[ndx - 1];
                TrimTree(node.Children, filter_severity, list_warnings);
                if (node.Children.Count == 0 && node.Name != filter_severity)
                {
                    nodes.Remove(node);
                }
                else if(node.Name == filter_severity)
                {
                    KeyValuePair<string, string> severity_rating = 
                        new KeyValuePair<string, string>(node.Name, node.Value.ToString());
                    if (!list_warnings.Contains(severity_rating))
                    {
                        list_warnings.Add(severity_rating); 
                    }
                }
            }
        }
        #endregion

        #region Process Warnings
        private void ProcessWarnings(List<KeyValuePair<string,string>> list_warnings)
        {

            if (list_warnings.Contains(seriousTraffic))
            {
                LEDState = tMController.RED_LED_BIT;
                keyState = (byte)(tMController.FORWARD_BIT + tMController.LEFT_BIT);
                tMController.SendDirectionCommand(keyState, speedOnSeriousTraffic, LEDState);
                CarSpeed.Text = speedOnSeriousTraffic.ToString();
                
                return;
            }
            else if (list_warnings.Contains(moderateTraffic))
            {
                LEDState = tMController.AMBER_LED_BIT;
                keyState = (byte)(tMController.FORWARD_BIT + tMController.LEFT_BIT);
                tMController.SendDirectionCommand(keyState, speedOnModerateTraffic, LEDState);
                CarSpeed.Text = speedOnModerateTraffic.ToString();
                return;
            }
            else if (list_warnings.Contains(minorTraffic) || list_warnings.Contains(lowTraffic))
            {
                LEDState = tMController.GREEN_LED_BIT;
                keyState = (byte)(tMController.FORWARD_BIT + tMController.LEFT_BIT);
                tMController.SendDirectionCommand(keyState, speedOnLowTraffic, LEDState);
                CarSpeed.Text = speedOnLowTraffic.ToString();
                return;
            }
        }
        #endregion

        #region Checkbox for manual mode
        /// <summary>
        /// Handles checkbox events for checked or unchecked
        /// Checked: Manual control of the toy car possible => Disables control elements of the Ambient Display
        /// Unchecked: Disables manual controls and enables control elements of the Ambient Display
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbManualMode_Checked(object sender, RoutedEventArgs e)
        {
            TbDirections.Visibility = Visibility.Collapsed;
            inp_FROM.Visibility = Visibility.Collapsed;
            inp_TO.Visibility = Visibility.Collapsed;
            btnGo.Visibility = Visibility.Collapsed;
            KeyDown_Events.Visibility = Visibility.Collapsed;
        }

        private void CbManualMode_Unchecked(object sender, RoutedEventArgs e)
        {
            TbDirections.Visibility = Visibility.Visible;
            inp_FROM.Visibility = Visibility.Visible;
            inp_TO.Visibility = Visibility.Visible;
            btnGo.Visibility = Visibility.Visible;
            ImgKeyEvents.Visibility = Visibility.Collapsed;
        }
        #endregion

        #region Bing Maps Browser Call
        /// <summary>
        /// Loads Bing Maps request in Browser based on user input
        /// </summary>
        /// <param name="inp_FROM"></param>
        /// <param name="inp_TO"></param>
        private void LoadBingMapsInBrowser(TextBox inp_FROM, TextBox inp_TO)
        {
            if (inp_FROM.Text != "" && inp_TO.Text != "")
            {
                string BingMapsURL = "https://www.bing.com/maps?Rtp=adr." + inp_FROM.Text + "~adr." + inp_TO.Text + "&toWww=1&redig=EAFF30EB56FC40C287FC6F3963862630&trfc=1";
                string BingMapgsEscapedURL = Uri.EscapeUriString(BingMapsURL);
                BingMapsWebPage.Navigate(BingMapgsEscapedURL);
                BingMapsURLTbx.Text = BingMapgsEscapedURL;
            }
            else
            {
                MessageBox.Show("Please first enter your Start (FROM) and Destination (TO)", "FROM => TO not defined!");
            }
        }

        #endregion

        #region TabControls
        /// <summary>
        /// Hides the Textbox of the Rest-Response URL and show instead the Bing Maps Browser URL
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BingMapsTab.IsSelected)
            {
                BingMapsPanel.Visibility = Visibility.Visible;
                RequestPanel.Visibility = Visibility.Collapsed;
               
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MouseInput.MoveMouse(new System.Drawing.Point(50,100));
        }
        #endregion
    }
}

