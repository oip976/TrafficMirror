   M S B A M L   `   `   `  ����H  DTrafficMirror, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:TrafficMirrorTrafficMirror  5   	   c _System.Windows.Interactivity, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35qPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.InteractivitySystem.Windows.Interactivity 5   	       TrafficMirror.MainWindow   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   -localclr-namespace:TrafficMirror     5   	   ^iPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.Interactivity     5   	   h dMicrosoft.Expression.Interactions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35Bei9http://schemas.microsoft.com/expression/2010/interactions  5   	   -   5          � Title$  TrafficMirror��5   	   $
��Auto��6   $
��Auto��6-    � WindowStartupLocation$ CenterScreen=�5   	    � WindowState$ 	Maximized=�6.   .  (System.Windows.Interactivity.Interaction   Triggers 5      /  )System.Windows.Interactivity.EventTrigger  5    
      	EventName$ Closing��6   .  (System.Windows.Interactivity.TriggerBase   Actions. 5!      >  8Microsoft.Expression.Interactivity.Core.CallMethodAction     
MethodName$ DisposeListener��6F      TargetObject 6"   �� *
sensorData+6F   5"       � 	Resources 5%      	 �� 
TargetType
 �� Property �� Value��%w   '��       �� 5&   
   "	 ��6   .�5'      �� 
 � ��$
 10,3q�6(   �� 5(      
 � ��$ 5q�6'   5)      5*      .��5-      �  � ColumnDefinitions 5.   
   �� 5/      $	��200 �6   �� 50       � RowDefinitions 52   
   �� 53      $��70 �6   �� 54      .+�57      �� -   5        $��
RequestURL��57      $��2��6+   $	��0,5q�6<   .+�58      �� $��Serial Port:��6   $
��Bold"�60   $
��Left=�6B   $��14(�6]   �� 59      -   5        $��numCom��59      $	��5,0q�6)   �� 5:      -   5        $��conBtn��5:      $��Connect Serial Port��6'   6E   �� 5;      -   5        $��disBtn��5;      $��Disconnect Serial Port��6'   6H   �� 5<      -   5        $��SPStatus��5<      $��SP Not Connected!��6,   $
��Bold"�6E   $��Center=�6W   $��14(�6t   
���  ��6�   �� 5=      $��Stretch=�6   $��1��6=   $	��0,5q�6H   �� 5>      -   5        $��CbManualMode��5>      $��Drive Car with Keyboard��6/   6p   �� 5?      $��Stretch=�6   $��1��6=   $	��0,5q�6H   �� 5A      -   5        $��TbDirections��5A      $��
Directions��60   $
��Bold"�6B   $��Center=�6T   $��14(�6q   �� 5B      -	   5        $��inp_FROM��5B      .��6*   ��5C      O Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem 5D   5   �� 6   0sys&clr-namespace:System;assembly=mscorlib  65   "	 ��6     �� 	Resources 5E         CueBannerBrush j� 
AlignmentX j� 
AlignmentY j� Stretch !� Visual��%�   &	        !� 5F   "   $
 Left=�6E   $ Center=�6W   $
 None=�6k    5G   &   �� 5H   *   $
��FROM��60   
�����p�6?   5J   #   5K       �� Triggers 5L      P� 5M   "    P� Property � �� �� Empty P� Value# Z 6:   . �5N   &   �� 
 � ��# [  6C   5O   #   P� 5P   "    � �� 6:   Z� . �5Q   &   �� 
 � ��# [  6C   5R   #   P� 5S   "    �@J�IsKeyboardFocused . . �5T   &   �� 
 � ��
 �����6C   5U   #   5W      .��5X   %   5Y      �� 5Z      -
   5        $��inp_TO��5Z      .��6(   ��5[      1&clr-namespace:System;assembly=mscorlibSystem 5\   5   �� 6   0sys&clr-namespace:System;assembly=mscorlib  65   "	 ��6     5]      ��%�   &	        !� 5^   "   $
 Left=�6E   $ Center=�6W   $
 None=�6k    5_   &   �� 5`   *   $��TO��60   
�����p�6=   5b   #   5c       5d      P� 5e   "    � ��# Z 6:   . �5f   &   �� 
 � ��# [  6C   5g   #   P� 5h   "    � �� 6:   Z� . �5i   &   �� 
 � ��# [  6C   5j   #   P� 5k   "    �@J�IsKeyboardFocused . . �5l   &   �� 
 � ��
 �����6C   5m   #   5o      .��5p   %   5q      �� 5r      -   5        $��btnGo��5r      $��Go for Warnings��6&   6@   �� 5s      $��Stretch=�6   $��1��6=   $	��0,5q�6H   �� 5t      -   5        $��KeyDown_Events��5t      $��Serial Port Commands��62   $
��Bold"�6N   $��Center=�6`   $��14(�6}   �� 5u      -   5        $��ImgKeyEvents��5u      $��75��6,   $��75��67   $	��0,5q�6C   �� 5v      -   5        $��tbSpeed��5v      $��Speed��6+   $
��Bold"�68   $��Center=�6J   $��14(�6g   �� 5w      -   5        $��CarSpeed��5w      $	��0,5q�6,   $��Center=�69   $��16(�6V   ���	#FF00E4156d   �� 5x      
����  �.��6,   PalmPositionX6;   �� 5y      $	��100��6   $��23��6&   ��62   �� *sensorData.PalmPositionX+�� 5z      
����  �.��6*   Scaling-Factor6:   �� 5{      $	��100��6   $��23��6&   ��62   �� *sensorData.Scale+�� 5|      $	��0,5q�6�   ��6   �� *sensorData.ImgWidth+��6>   �� *sensorData.ImgHeight+��6f   �� *sensorData.ImgPath+6�   �� 5}      -   5        $��Zoom by Mouse Wheel Abuse��5}      $y�Hidden=�6S   5~      �� 5�   
   -   5        $��RequestPanel��5�      $��2��6*   $��1��6;   .+�5�      � $��5,10,5,5q�6   .+�5�      �� $��Request URL:��6   �� 5�       �� Orientation$ 
Horizontal=�6   $��Right=�66   $	��170��6R   .+�5�      �� $��Processing Time: ��6    �� 5�      -   5        $��ProcessingTimeTbx��5�       5�      5�      �� 5�      -   5        $��RequestUrlTbx��5�      $��25��6+   $
��10,0q�67   5�      �� 5�   
   -   5        $��BingMapsPanel��5�      $��2��6*   $��1��6;   $y�	Collapsed=�6K   .+�5�      � $��5,10,5,5q�6   .+�5�      �� $��BingMaps URL:��6   5�      �� 5�      -   5        $��BingMapsURLTbx��5�      $��25��6,   $
��10,0q�68   5�      �� 5�   
   -   5        $��1��5�      $��1��6"   .@�5�      �� -   5        $��CongestionTab��5�      $��Traffic Congestions��6+   $��22��6H   $	��Top=�6T   .��5�      �� -   5        $��SeverityRatings_panel��5�      $��Center=�6:   $��Center=�6W   5�      �� 5�      -   5        $��BingMapsTab��5�      $��	Bing Maps��6)   $��22��6<   $	��Top=�6H   .��5�      �� -   5        $��	SPBingTab��5�      $ 
Horizontal=�6.   .+�5�      (  "System.Windows.Controls.WebBrowser  -   5        $��BingMapsWebPage��5�   !   ��68   �� *ActualWidth+ �� ElementName$ 	SPBingTab����6m   �� *ActualHeight+$ 	SPBingTab��5�      5�      �� 5�      -   5        $��ResponseTab��5�      $��Response- Trimmed��6)   .��5�      R� -   5        $��ResultTreeView��5�      $��0q�61    5�       �� DataType   TrafficMirror.ObjectNode �� ItemsSource �� Path��%+  (��       5�   3   *, +)�� 6   "  63    6X   �� $ Children�.R�5�      Q� ��5�   "   �� 5�   &   $ 
Horizontal=�61   $��	-10,0,0,0q�6J   .+�5�   *   �� ��64   �� $
 Name��� 5�   *   $	�� : ��64   �� 5�   *   ��64   �� $ Value�5�   '   5�      5�      5�      5�      5�      �� 5�      -   5        $��ResponseFullTab��5�      $��Response - Complete��6-   $y�Hidden=�6J   .��5�      R� -   5        $��ResultTreeViewFull��5�      $��0q�65    5�      ��%+  (��       5�   3   *, +)�� 6   "  63    6X   �� $ Children�.R�5�      Q� ��5�   "   �� 5�   &   $ 
Horizontal=�61   $��	-10,0,0,0q�6J   .+�5�   *   �� ��64   �� $
 Name��� 5�   *   $	�� : ��64   �� 5�   *   ��64   �� $ Value�5�   '   5�      5�      5�      5�      5�      5�      � 5�   
   -    5        $��RequestProgressBar��5�      $��1��6)   $��1��66   $��146,93,146,25q�6F   $	��300��6]   $��35��6i   $y�	Collapsed=�6u   .+�5�      �  � IsIndeterminate . 6   �� 5�      -!   5        $��RequestProgressBarText��5�      $��Center=�66   $��Center=�6S   Processing...6}   5�      5�      5�      