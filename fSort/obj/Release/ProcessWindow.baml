   M S B A M L   `   `   `  ���� @  <fSort, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:fSortfSort      fSort.ProcessWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     %localclr-namespace:fSort     -     � Title$  WindowProcess��$	��355��$	��663�� � Icon$ Open-file-icon.png�� � 	Resources    RoundButtton �� 
TargetType �� Property �� Value �� 
TargetType �� CornerRadius � Cursor	 �� Triggers
 P� Property P� Value �� 
TargetName��%<  &	        �� " ��.���  � �� �� " ��.R�� .+��� $��ButtonBorder��$ 5��
�������$��1q�
�������._��� $
 Hand��$��Center=�$��Center=�	 P� 
 � �� . . ��� $ ButtonBorder�� � ��
 � ���P� 
 �@J�IsMouseOver . . ��� $ ButtonBorder�� � ��
 ��ح�����  �� EndPoint$ 0.5,1&� �� 
StartPoint$ 0.5,0&� �� RelativeTransform V� .~���  �� CenterY$	 0.5Q� �� CenterX$	 0.5Q���  �� CenterY$	 0.5Q� �� CenterX$	 0.5Q���  �� Angle$ 90Q� �� CenterY$	 0.5Q� �� CenterX$	 0.5Q�T� .���  � Color$ Black�� � Offset$ 0Q�� $ White��$ 1Q����� $ 0.5,1&� � MappingMode$ RelativeToBoundingBox=�$ 0.5,0&� V� .~��� $	 0.5Q�$	 0.5Q��� $	 0.5Q�$	 0.5Q��� $ 137.121Q�$	 0.5Q�$	 0.5Q�T� .��� $ Black��$ 0.179Q�� $ 	#FFEECE04��$ 1Q�.��� .+��� -    ��Name$ SettingsListBox��$
��Left=�$��44��$��	10,31,0,0q�$	��Top=�$	��634���� -   $ ProcessListBox��$
��Left=�$	��155��$��
10,104,0,0q�$	��Top=�$	��634��   ElementMenu��%�   &	       '�� N     7  �� .@�m� -   $��Open Source Folder��m� -   $��Open Target Folder���� " ��.���  �@�ContextMenu8   �� $��設定:��$
��Left=�$��	10,10,0,0q�$	��Top=��� $��進程:��$
��Left=�$��	10,79,0,0q�$	��Top=��� -    ��Name$ LB_ProcessCount��$��0��$
��Left=�$��	52,79,0,0q�$	��Top=��� -    ��Name$ BTN_FlowControl��$��暫停��#��[  $
��Left=�$��
569,75,0,0q�$	��Top=�$��75��$��29���� $��報告:��$
��Left=�$��295,267,0,0q�$	��Top=����	#FFF9EE02�� $��耗時:��$
��Left=�$��
10,267,0,0q�$	��Top=�
��������� -   $ LB_SpendTime��$��0 sec��$
��Left=�$��
52,267,0,0q�$	��Top=�
��������� -	   $ BTN_ReportFileDamaged��$��損毀檔案��$
��Left=�$��335,270,0,0q�$	��Top=�$��60���� -
   $ BTN_ReportFileIgnored��$��忽略檔案��$
��Left=�$��446,270,0,0q�$	��Top=�$��60���� -   $ BTN_ReportCopyFailed��$��失敗檔案��$
��Left=�$��545,270,0,0q�$	��Top=�$��60�� J� RenderTransformOrigin$ 0.317,0.684&��� -   $ LB_DamagedFileCount��$��0��$
��Left=�$��337,287,0,0q�$	��Top=�
��������� -   $ LB_IgnoredFileCount��$��0��$
��Left=�$��446,287,0,0q�$	��Top=�
��������� -   $ LB_CopyFailedFileCount��$��0��$
��Left=�$��545,287,0,0q�$	��Top=�
�������