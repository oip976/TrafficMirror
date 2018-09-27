using System.Windows;
using System.ComponentModel;
using TrafficMirror.Enums;
using System;
using System.Windows.Media.Imaging;
using TrafficMirror.Classes;
using Leap;

namespace TrafficMirror
{
    class SensorDataWPF : INotifyPropertyChanged
    {
        #region Class Variables
        private LeapListener listener;
        private Controller ctrl;
        

        // Dient lediglich als Ausgabewert für das MainWindow - nicht sichtbar da "collapsed"
        private const double ROTATION_SHIFT = 2;
        private const double TRANSL_SHIFT = 1;
        private const double SCALE_SHIFT = 0;

        // Faktor zur Konvertierung von Radiant nach Grad
        const double RAD_TO_DEG = 57.295779513f;
        #endregion Class Variables

        #region Constructor
        public SensorDataWPF()
        {
            listener = new LeapListener();
            ctrl = new Controller();

            ctrl.SetPolicyFlags(Controller.PolicyFlag.POLICYBACKGROUNDFRAMES);

            // Der Controller informiert allen Abonennten über Auslösung eines Events in der LeapListener-Klasse
            ctrl.AddListener(listener);
            // Abonnenten der ausgelösten Events
            listener.LeapPan += PanAction;
            listener.LeapZoom += ZoomAction;
            listener.LeapHandVelo += HandVelocityAction;
            listener.LeapGestureRec += HandGestureRec;
            listener.LeapPalmPosition += HandPalmPosAction;
            listener.LeapHandSphereCenter += HandSphereCenterAction;
            listener.LeapHandSphereRadius += HandRadiusAction;
            listener.LeapHandFingersCount += HandFingersCount;
            listener.LeapPointablesCount += PointablesCountAction;

            // Default-Wert der Skalierung => 100%
            // während der Entwicklungsphase: diente lediglich als Parameterwert zur Anzeige im MainWindow (WPF) - nicht sichtbar da "collapsed"
            Scale = 11;
        }
        #endregion Constructor

        #region PanBingMapsWPF
        private double _panX;

        public double PanX
        {
            get { return _panX; }
            set
            {
                _panX = value;
                // View benachrichtigen über Wertveränderung (legt die Tranlationsrichtung fest)
                // während der Entwicklungsphase: diente lediglich als Parameterwert zur Anzeige im MainWindow (WPF) - nicht sichtbar da "collapsed"
                OnPropertyChanged("PanX");
            }
        }

        private double _panY;

        public double PanY
        {
            get { return _panY; }
            set
            {
                _panY = value;
                OnPropertyChanged("PanY");
            }
        }

        private double _panZ;

        public double PanZ
        {
            get { return _panZ; }
            set
            {
                _panZ = value;
                OnPropertyChanged("PanZ");
            }
        }

        // wird benachrichtigt wenn Translationsgeste in der LeapListener-Klasse detektiert wurde
        private void PanAction(PanDirection sd)
        {
            // Properties der View (MainWindow) werden entsprechend der detektierten Translationsrichtung angepasst
            // während der Entwicklungsphase: diente lediglich als Parameterwert zur Anzeige im MainWindow (WPF) - nicht sichtbar da "collapsed"
            switch (sd)
            {
                case PanDirection.Up:
                    PanY += TRANSL_SHIFT;
                    //KeyboardInput.UpArrowKey();
                    //KeyboardInput.UpArrowKeyReleased();
                    break;
                case PanDirection.Down:
                    PanY -= TRANSL_SHIFT;
                   // KeyboardInput.DownArrowKey();
                   // KeyboardInput.DownArrowKeyReleased();
                    break;
                case PanDirection.Left:
                    PanX -= TRANSL_SHIFT;
                   // KeyboardInput.LeftArrowKey();
                   // KeyboardInput.LeftArrowKeyReleased();

                    break;
                case PanDirection.Right:
                    //KeyboardInput.RightArrowKey();
                    //KeyboardInput.RightArrowKeyReleased();
                    PanX += TRANSL_SHIFT;
                    break;
            }
        }
        #endregion

        #region ZoomBingMapsWPF
        private double _scale;

        public double Scale
        {
            get { return _scale; }
            set
            {
                _scale = value;
                // View benachrichtigen über Wertveränderung (legt den Skalierungsfaktor fest)
                // während der Entwicklungsphase: diente lediglich als Parameterwert zur Anzeige im MainWindow (WPF) - nicht sichtbar da "collapsed"
                OnPropertyChanged("Scale");
            }
        }

        // is notified when a zooming gesture was detected in the LeapListener class
        private void ZoomAction(ZoomInOut hd)
        {
            // Property of the view (MainWindow) is adapted according to the scaling factor
            //Triggers Mouse-Events dependent on the detected zoom gesture
            switch (hd)
            {
                case ZoomInOut.backwardsDisplay:
                        Scale -= 1;
                        //MouseInput.ScrollWheel(-1);
                    break;
                case ZoomInOut.towardsDisplay:
                        Scale += 1;
                        //MouseInput.ScrollWheel(1);
                        break;
            }
        }
        #endregion

        #region TextboxRecGesture
        private string _recGesture;

        public string RecGesture
        {
            get { return _recGesture; }
            set
            {
                _recGesture = value;
                // View benachrichtigen über Wertveränderung (legt den Inhalt der TextBox entsprechend der detektierten Geste fest)
                OnPropertyChanged("RecGesture");
            }
        }

        private double _imgWidth;

        public double ImgWidth
        {
            get { return _imgWidth; }
            set
            {
                _imgWidth = value;
                // View benachrichtigen über Wertveränderung (legt die Bildbreite des definierten Bildpfades fest)
                OnPropertyChanged("ImgWidth");
            }
        }

        private double _imgHeight;

        public double ImgHeight
        {
            get { return _imgHeight; }
            set
            {
                _imgHeight = value;
                // View benachrichtigen über Wertveränderung (legt die Bildhöhe des definierten Bildpfades fest)
                OnPropertyChanged("ImgHeight");
            }
        }

        BitmapImage img;
        public BitmapImage ImgPath
        {
            get { return img; }
            set
            {
                img = value;
                // View benachrichtigen über Wertveränderung (legt den Bildpfad (GestIcons) für die visuelle Beschreibung der Geste fest)
                OnPropertyChanged("ImgPath");
            }
        }

        // wird benachrichtigt wenn GestureRecAction der LeapListener-Klasse ausgelöst wird
        private void HandGestureRec(string gestureDesc)
        {
            // Bildpfad definieren entsprechend der gestureDesc
            //keine Geste detektiert => Gesamtübersicht der implementierten Gesten anzeigen
            if (gestureDesc == "NoGesture")
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri("pack://application:,,,/Images/overviewGest.png");
                bitmap.EndInit();
                // Die DependencySource muss auf dem selben Thread wie das DependencyObject ertellt werden, andernfalls kommt es zur Exception
                bitmap.Freeze();
                ImgHeight = 350;
                ImgWidth = 350;
                ImgPath = bitmap;
            }
            // Geste detektiert => View entsprechend der detektierten Geste anpassen
            else if (gestureDesc == "PanLeft" || gestureDesc == "PanRight" || gestureDesc == "PanUp" || gestureDesc == "PanDown")
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri("pack://application:,,,/Images/pan_all_dir.png");
                bitmap.EndInit();
                bitmap.Freeze();
                ImgHeight = 200;
                ImgWidth = 200;
                ImgPath = bitmap;
            }
            else if (gestureDesc == "ZoomIn")
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri("pack://application:,,,/Images/zoom_in.png");
                bitmap.EndInit();
                bitmap.Freeze();
                ImgHeight = 200;
                ImgWidth = 200;
                ImgPath = bitmap;
            }
            else if (gestureDesc == "ZoomOut")
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri("pack://application:,,,/Images/zoom_out.png");
                bitmap.EndInit();
                bitmap.Freeze();
                ImgHeight = 200;
                ImgWidth = 200;
                ImgPath = bitmap;
            }
            else
            {
                ImgHeight = 0;
                ImgWidth = 0;
                ImgPath = null;
            }
            RecGesture = gestureDesc;
        }
        #endregion

        #region TextboxPalmVelocity
        private double _palmVelocityX;

        public double PalmVelocityX
        {
            get { return _palmVelocityX; }
            set
            {
                _palmVelocityX = value;
                OnPropertyChanged("PalmVelocityX");
            }
        }

        private double _palmVelocityY;

        public double PalmVelocityY
        {
            get { return _palmVelocityY; }
            set
            {
                _palmVelocityY = value;
                OnPropertyChanged("PalmVelocityY");
            }
        }

        private double _palmVelocityZ;

        public double PalmVelocityZ
        {
            get { return _palmVelocityZ; }
            set
            {
                _palmVelocityZ = value;
                OnPropertyChanged("PalmVelocityZ");
            }
        }

        private void HandVelocityAction(Leap.Vector handVelo)
        {
            PalmVelocityX = handVelo.x;
            PalmVelocityY = handVelo.y;
            PalmVelocityZ = handVelo.z;
        }
        #endregion

        #region TextboxPitchYawRoll/PalmPosition
        private double _pitch;

        public double Pitch
        {
            get { return _pitch; }
            set
            {
                _pitch = value;
                OnPropertyChanged("Pitch");
            }
        }

        private double _yaw;

        public double Yaw
        {
            get { return _yaw; }
            set
            {
                _yaw = value;
                OnPropertyChanged("Yaw");
            }
        }

        private double _roll;

        public double Roll
        {
            get { return _roll; }
            set
            {
                _roll = value;
                OnPropertyChanged("Roll");
            }
        }

        private double _palmPositionX;

        public double PalmPositionX
        {
            get { return _palmPositionX; }
            set
            {
                _palmPositionX = value;
                OnPropertyChanged("PalmPositionX");
            }
        }

        private double _palmPositionY;

        public double PalmPositionY
        {
            get { return _palmPositionY; }
            set
            {
                _palmPositionY = value;
                OnPropertyChanged("PalmPositionY");
            }
        }

        private double _palmPositionZ;

        public double PalmPositionZ
        {
            get { return _palmPositionZ; }
            set
            {
                _palmPositionZ = value;
                OnPropertyChanged("PalmPositionZ");
            }
        }

        private void HandPalmPosAction(Hand firstHand)
        {
            Pitch = firstHand.Direction.Pitch * RAD_TO_DEG;
            Yaw = firstHand.Direction.Yaw * RAD_TO_DEG;
            Roll = firstHand.PalmNormal.Roll * RAD_TO_DEG;
            PalmPositionX = firstHand.PalmPosition.x;
            PalmPositionY = firstHand.PalmPosition.y;
            PalmPositionZ = firstHand.PalmPosition.z;

        }
        #endregion

        #region TextboxPalmSphereCenter
        private double _palmSphereCenterX;

        public double SphereCenterX
        {
            get { return _palmSphereCenterX; }
            set
            {
                _palmSphereCenterX = value;
                OnPropertyChanged("SphereCenterX");
            }
        }

        private double _palmSphereCenterY;

        public double SphereCenterY
        {
            get { return _palmSphereCenterY; }
            set
            {
                _palmSphereCenterY = value;
                OnPropertyChanged("SphereCenterY");
            }
        }

        private double _palmSphereCenterZ;

        public double SphereCenterZ
        {
            get { return _palmSphereCenterZ; }
            set
            {
                _palmSphereCenterZ = value;
                OnPropertyChanged("SphereCenterZ");
            }
        }

        private void HandSphereCenterAction(Leap.Vector palmSphereCenter)
        {
            SphereCenterX = palmSphereCenter.x;
            SphereCenterY = palmSphereCenter.y;
            SphereCenterZ = palmSphereCenter.z;
        }
        #endregion

        #region TextboxPalmSphereRadius
        private double _palmSphereRadius;

        public double SphereRadius
        {
            get { return _palmSphereRadius; }
            set
            {
                _palmSphereRadius = value;
                OnPropertyChanged("SphereRadius");
            }
        }

        private void HandRadiusAction(double handSphereRadius)
        {
            SphereRadius = handSphereRadius;
        }
        #endregion

        #region TextboxFingersCount
        private int _fingersCount;

        public int FingersCount
        {
            get { return _fingersCount; }
            set
            {
                _fingersCount = value;
                OnPropertyChanged("FingersCount");
            }
        }

        private void HandFingersCount(int fingersCount)
        {
            FingersCount = fingersCount;
        }
        #endregion

        #region TextboxPointablesCount
        private int _pointablesCount;

        public int PointablesCount
        {
            get { return _pointablesCount; }
            set
            {
                _pointablesCount = value;
                OnPropertyChanged("PointablesCount");
            }
        }

        private void PointablesCountAction(int pointablesCount)
        {
            PointablesCount = pointablesCount;
        }
        #endregion

        // Listener verwerfen
        #region DisposeListener
        public void DisposeListener()
        {
            ctrl.RemoveListener(listener);
            ctrl.Dispose();
            Application.Current.Shutdown();
        }
        #endregion

        // Stellt die Methode für die Behandlung des PropertyChanged-Ereignisses dar, das beim Ändern einer Eigenschaft einer Komponente ausgelöst wird.
        #region OnPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

    }
}
