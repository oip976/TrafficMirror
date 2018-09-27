using System;
using System.Linq;
using TrafficMirror.Enums;
using Leap;

namespace TrafficMirror.Classes
{
    class LeapListener : Listener
    {
        #region  Class Variables
        // minimaler Abstand zum Sensor
        private const double MIN_LEAP_DISTANCE = 200.0f;
        private Vector handVelo;
        private Vector handPalmPosition;
        private double handSphereRadius;
        private Vector handSphereCenter;
        private int handFingersCount;
        private int handPointablesCount;
        private double sphereDiameter;

        #endregion Class Variables

        #region OnFrame
        public override void OnFrame(Controller ctrl)
        {
            // aktuelles Frame
            Frame currentFrame = ctrl.Frame();
            // vorangegangenes Frame
            Frame previousFrame = ctrl.Frame(1);

            if (ctrl.Config.SetFloat("Gesture.Swipe.MinLength", 1.0f) && ctrl.Config.SetFloat("Gesture.Swipe.MinVelocity", 1.0f))
            {
                ctrl.Config.Save();
            }

            // virtuelle Interaktionsbox in der Gesten detektiert werden können
            InteractionBox iBox = currentFrame.InteractionBox;

            ctrl.EnableGesture(Gesture.GestureType.TYPE_SWIPE);

            // Hand im aktuellen Frame detektiert?
            if (!currentFrame.Hands.IsEmpty)
            {
                // Speicherung der detektierten Hände des aktuellen und vorangegangen Frames
                Hand firstHand = currentFrame.Hands[0];
                Hand secondHand = currentFrame.Hands[1];
                Hand previousHand = previousFrame.Hands[0];
                Hand previousSecondHand = previousFrame.Hands[1];

                // Empfangene Daten zur Anzeige im OutputParameter-Window (WPF) und zur Detektierung der Gesten
                handVelo = firstHand.PalmVelocity;
                handPalmPosition = firstHand.PalmPosition;
                handSphereRadius = firstHand.SphereRadius;
                handSphereCenter = firstHand.SphereCenter;
                handFingersCount = firstHand.Fingers.Count();
                handPointablesCount = firstHand.Pointables.Count();

                // Durchmesser der virtuell konstruierten Kugel der Handfläche
                sphereDiameter = 2 * firstHand.SphereRadius;

                // Distanz zwischen Interaktionsbox und der Position der Handfläche im aktuellen Frame
                double vDistance = Math.Sqrt(Math.Pow((handPalmPosition.x - iBox.Center.x), 2) + Math.Pow((handPalmPosition.y - iBox.Center.y), 2) + Math.Pow((handPalmPosition.z - iBox.Center.z), 2));

                // Speicherung der detektierten Finger des aktuellen und vorangegangen Hand sowie der zweiten Hand
                FingerList fingers = firstHand.Fingers;
                FingerList fingers_second_hand = secondHand.Fingers;
                GestureList gestures = currentFrame.Gestures();

                int countSwipeLeft = 0;
                int countSwipeRight = 0;
                int countSwipeUp = 0;
                int countSwipeDown = 0;
                int countSwipeBackward = 0;
                int countSwipeForward = 0;

                //for(int i = 0; i < gestures.Count; i++)
                foreach (Gesture gesture in gestures)
                {
                    //Gesture gesture = gestures[i];

                    switch (gesture.Type)
                    {
                        case Gesture.GestureType.TYPE_SWIPE:
                            SwipeGesture swipe = new SwipeGesture(gesture);

                            float iAbsX = Math.Abs(swipe.Direction.x);
                            float iAbsY = Math.Abs(swipe.Direction.y);
                            float iAbsZ = Math.Abs(swipe.Direction.z);

                            if(iAbsX > iAbsY && iAbsX > iAbsZ)
                            {
                                if(swipe.State == Gesture.GestureState.STATESTART && swipe.Direction.x > 0)
                                {
                                    countSwipeRight++;
                                }
                                else if(swipe.State == Gesture.GestureState.STATESTART && swipe.Direction.x < 0) 
                                {                                 
                                    countSwipeLeft++;
                                }
                            }
                            else if(iAbsY > iAbsX && iAbsY > iAbsZ)
                            {
                                if(swipe.State == Gesture.GestureState.STATESTART && swipe.Direction.y > 0)
                                {
                                    countSwipeUp++;
                                }
                                else if (swipe.State == Gesture.GestureState.STATESTART && swipe.Direction.y < 0)
                                {
                                    countSwipeDown++;
                                }
                            }
                            else // Z was the greatest.
                            {
                                
                                if (firstHand.Fingers.Count() == 2 && swipe.State == Gesture.GestureState.STATESTOP && swipe.Direction.z > 0 )
                                {
                                    
                                    //countSwipeBackward++;
                                    MouseInput.ScrollWheel(-1);
                                }
                                else if (firstHand.Fingers.Count() == 2 && swipe.State == Gesture.GestureState.STATESTOP && swipe.Direction.z < 0)
                                {
                                    
                                    //countSwipeForward++;
                                    MouseInput.ScrollWheel(1);
                                }
                            }
                            //Console.WriteLine("CounSwipe: {0}", countSwipeLeft);

                            switch (countSwipeLeft % 4)
                            {
                                case 2:
                                    {
                                        KeyboardInput.LeftArrowKey();
                                        KeyboardInput.LeftArrowKeyReleased();
                                        break;
                                    }
                            }

                            switch (countSwipeRight % 4)
                            {
                                case 2:
                                    {
                                        KeyboardInput.RightArrowKey();
                                        KeyboardInput.RightArrowKeyReleased();
                                        break;
                                    }
                            }

                            switch (countSwipeUp % 6)
                            {
                                case 3:
                                    {
                                       KeyboardInput.UpArrowKey();
                                       KeyboardInput.UpArrowKeyReleased();
                                        break;
                                    }
                            }


                            switch (countSwipeDown % 6)
                            {
                                case 3:
                                    {
                                        KeyboardInput.DownArrowKey();
                                        KeyboardInput.DownArrowKeyReleased();
                                        break;
                                    }
                            }

                            switch (countSwipeForward % 12)
                            {
                                case 1:
                                    {
                                        MouseInput.ScrollWheel(1);
                                        break;
                                    }
                            }

                            switch (countSwipeBackward % 12)
                            {
                                case 1:
                                    {
                                        MouseInput.ScrollWheel(-1);
                                        break;
                                    }
                            }


                            break;


                    }
                }

                #region Gesture Recognition
                // Ein-Hand-Detektion - die Detektion der Gesten zur Manipulation erfolgt nur wenn eine Hand im aktuellen Frame detektiert wurde
                if (!fingers.IsEmpty && fingers_second_hand.IsEmpty)
                {
                    // Event Handling für die Ausgabe spezifischer Parameter des Sensors im OutputParameter-Window
                    // Handgeschwindigkeit, Position der Handfläche, Mittelpunkt der virtuellen Kugel, Radius der virtuellen Kugel, Anzahl der detektierten Finger
                    HandVelocityAction(fingers, handVelo);
                    HandPalmPosAction(firstHand);
                    HandSphereCenterAction(handSphereCenter);
                    HandSphereRadiusAction(handSphereRadius);
                    HandFingersCountAction(handFingersCount);
                    HandPointablesCountAction(handPointablesCount);

                    // Speicherung der Fingerlänge des weitesten links und rechts befindlichen Fingers
                    // Damit kann festgestellt werden, ob der Daumen und der Mittelfinger der rechten Hand detektiert wurden
                    double lmLength = firstHand.Fingers.Leftmost.Length;
                    double rmLength = firstHand.Fingers.Rightmost.Length;

                    // Geschwindigkeit der detektierten Hand aus dem aktuellen Frame
                    double palmVeloX = firstHand.PalmVelocity.x;
                    double palmVeloY = firstHand.PalmVelocity.y;
                    double palmVeloZ = firstHand.PalmVelocity.z; 

                    // Detektion der Geste für die Translation
                    #region PanGesture
                    // detailierte Erläuterung siehe Kapitel 5.2.2
                    if (firstHand.Pointables.Count() == 3 && lmLength < rmLength && vDistance <= 150 && sphereDiameter > 120)
                    {
                        // Horizontale Auslenkung der Hand mit einer Mindestgeschwindigkeit von >20mm/s
                        if (Math.Abs(firstHand.PalmVelocity.x) > Math.Abs(firstHand.PalmVelocity.y) && Math.Abs(palmVeloX) > 30)
                        {
                            // auf der +X-Achse => Volumen nach rechts verschieben
                            if (firstHand.PalmVelocity.x > 0)
                            {
                                // Event-Auslösung zur Ausgabe im MainWindow (WPF)
                                GestureRecAction("Pan" + PanDirection.Right);
                                // Event-Auslösung zur Parmeter-Ausgabe in MainWindow (WPF)
                                // Parameterauswertung bei der Entwicklung - im MainWindow (WPF) nicht sichtbar da ausgeblendet ("collapsed")
                                PanAction(PanDirection.Right);
                            }
                            // auf der -X-Achse => Volumen nach links verschieben
                            else if (firstHand.PalmVelocity.x < 0)
                            {
                                GestureRecAction("Pan" + PanDirection.Left);
                                PanAction(PanDirection.Left);
                            }
                        }
                        // Vertikale Auslenkung der Hand
                        else if (Math.Abs(palmVeloY) > 30)
                        {
                            // auf der +Y-Achse => Volumen nach oben verschieben
                            if (firstHand.PalmVelocity.y > 0)
                            {
                                GestureRecAction("Pan" + PanDirection.Up.ToString());
                                PanAction(PanDirection.Up);
                            }
                            // auf der -Y-Achse => Volumen nach unten verschieben
                            else if (firstHand.PalmVelocity.y < 0)
                            {
                                GestureRecAction("Pan" + PanDirection.Down.ToString());
                                PanAction(PanDirection.Down);
                            }
                        }
                    }
                    #endregion PanGesture

                    // Detektion der Geste für das Skalieren
                    #region ZoomGesture
                    // detailierte Erläuterung siehe Kapitel 5.2.2
                    if (firstHand.Fingers.Count() == 2 && vDistance <= 150 && Math.Abs(palmVeloZ) > 40 && sphereDiameter <= 120)
                    {
                        // Bewegung der Hand zum Anwender mit einem Mindesabstand von 200mm zum Controller => Karte vergrößern
                        if (palmVeloZ >= 0 && firstHand.PalmPosition.y >= MIN_LEAP_DISTANCE)
                        {
                            ZoomAction(ZoomInOut.backwardsDisplay);
                            GestureRecAction("ZoomIn");
                        }
                        // Bewegung der Hand zum Display mit einem Mindesabstand von 200mm zum Controller => Karte verkleinern
                        else if (palmVeloZ < 0 && firstHand.PalmPosition.y >= MIN_LEAP_DISTANCE)
                        {
                            ZoomAction(ZoomInOut.towardsDisplay);
                            GestureRecAction("ZoomOut");
                        }

                    }
                    #endregion ZoomGesture

                }
                #endregion GestureRecognition
            }
            else
            {
                // wenn keine Geste detektiert werden konnte, MainWindow (WPF) benachrichtigen
                GestureRecAction("NoGesture");
            }
        }
        #endregion OnFrame

        #region events

        public delegate void HandFingersCount(int fingersCount);
        public event HandFingersCount LeapHandFingersCount;

        // wird aufgerufen wenn sich die Anzahl der detektierten Finger verändert
        private void HandFingersCountAction(int fingersCount)
        {
            // Abonennten benachrichtigen über Event-Auslösung
            LeapHandFingersCount?.Invoke(fingersCount);
        }

        public delegate void HandPointablesCount(int pointablesCount);
        public event HandPointablesCount LeapPointablesCount;

        // wird aufgerufen wenn sich die Anzahl der detektierten fingerähnlichen Gegenstände verändert
        private void HandPointablesCountAction(int pointablesCount)
        {
            // Abonennten benachrichtigen über Event-Auslösung
            LeapPointablesCount?.Invoke(pointablesCount);
        }

        public delegate void HandSphereCenter(Vector handSphereCenter);
        public event HandSphereCenter LeapHandSphereCenter;

        // wird aufgerufen wenn sich der Mittelpunkt der virtuelle konstruierten Kugel der Handfläche verändert
        private void HandSphereCenterAction(Vector handSphereCenter)
        {
            // Abonennten benachrichtigen über Event-Auslösung
            LeapHandSphereCenter?.Invoke(handSphereCenter);
        }


        public delegate void HandSphereRadius(double handSphereRadius);
        public event HandSphereRadius LeapHandSphereRadius;

        // wird aufgerufen wenn sich der Radius (entspricht Handkrümmung) der virtuell konstruierten Kugel der Handfläche verändert 
        private void HandSphereRadiusAction(double handSphereRadius)
        {
            // Abonennten benachrichtigen über Event-Auslösung
            LeapHandSphereRadius?.Invoke(handSphereRadius);
        }

        public delegate void HandVeloEvent(Vector handVelocity);
        public event HandVeloEvent LeapHandVelo;

        // wird aufgerufen wenn sich die Handgeschwindigkeit verändert
        private void HandVelocityAction(FingerList fingers, Vector handVelocity)
        {
            // Abonennten benachrichtigen über Event-Auslösung
            LeapHandVelo?.Invoke(handVelo);
        }

        public delegate void HandPalmPosition(Hand firstHand);
        public event HandPalmPosition LeapPalmPosition;

        // wird aufgerufen wenn sich die Position der Handfläche verändert
        private void HandPalmPosAction(Hand firstHand)
        {
            // Abonennten benachrichtigen über Event-Auslösung
            LeapPalmPosition?.Invoke(firstHand);
        }

        public delegate void GestureRecEvent(string gestureDesc);
        public event GestureRecEvent LeapGestureRec;

        // wird aufgerufen wenn eine Handgeste zur Manipulation des 3D-Volumens detektiert wurde
        private void GestureRecAction(string gestureDesc)
        {
            // Abonennten benachrichtigen über Event-Auslösung
            LeapGestureRec?.Invoke(gestureDesc);
        }

        public delegate void PanEvent(PanDirection sd);
        public event PanEvent LeapPan;

        // wird aufgerufen wenn eine Translationsgeste detektiert wurde
        private void PanAction(PanDirection sd)
        {
            switch (sd)
            {
                case PanDirection.Left:
                    // Abonennten benachrichtigen über Event-Auslösung
                    LeapPan?.Invoke(PanDirection.Left);
                    break;
                case PanDirection.Right:
                    LeapPan?.Invoke(PanDirection.Right);
                    break;
                case PanDirection.Up:
                    LeapPan?.Invoke(PanDirection.Up);
                    break;
                case PanDirection.Down:
                    LeapPan?.Invoke(PanDirection.Down);
                    break;
            }
        }

        public delegate void ZoomEvent(ZoomInOut hd);
        public event ZoomEvent LeapZoom;

        // wird aufgerufen wenn eine Zoom-Geste detektiert wurde
        private void ZoomAction(ZoomInOut hd)
        {
            switch (hd)
            {
                case ZoomInOut.backwardsDisplay:
                    // Abonennten benachrichtigen über Event-Auslösung
                    LeapZoom?.Invoke(ZoomInOut.backwardsDisplay);
                    break;
                case ZoomInOut.towardsDisplay:
                    LeapZoom?.Invoke(ZoomInOut.towardsDisplay);
                    break;
            }
        }
        #endregion Events
    }
}
