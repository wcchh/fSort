   M S B A M L   `   `   `  ���� @  <fSort, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:fSortfSort  O Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem     fSort.MainWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     %localclr-namespace:fSort     9System&clr-namespace:System;assembly=mscorlib     -     � Title$(  "fSort : A Media Files Sorting Tool��$	��493��$	��490�� � Icon$ Open-file-icon.png�� � 	Resources    RoundButtton �� 
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
 ��ح�.��� -   ����  �� EndPoint$ 0.108,0.565&� �� 
StartPoint$ 0.952,0.942&�.���  � Color$ 	#FF292B07�� � Offset$ 0.079Q�� $ 	#FFEBEEB6��$ 0.907Q�.+��� $
��Left=�$	��255��$��348,193,0,0q�$	��Top=�$	��111��$��logo.jpg��$
��Fill=� J� Opacity$
 0.15Q��� $��設定: ��$
��Left=�$��	256,6,0,0q�$	��Top=� J� RenderTransformOrigin$ 
0.51,0.725&��� -    ��Name$ BTN_SaveConfig��$��儲存��$
��Left=�$��	296,9,0,0q�$	��Top=�$��39���� -   $ BTN_ResetConfig��$��清除��$
��Left=�$��	340,9,0,0q�$	��Top=�$��31��
���  ���� -   $ BTN_OpenLogFolder��$��
Log 位置��$
��Left=�$��	395,9,0,0q�$	��Top=�$��56��$��19���� $
��Left=�$��	21,13,0,0q�$
��Wrap=�$ ��檔案來源路徑(來源)��$	��Top=�$	��107���� -   $ BTN_SelectSourceFolder��$��瀏覽��#��[  $
��Left=�$��	21,33,0,0q�$	��Top=�$��31��$��24���� -    ��Name$ TBX_SourceFolder��$
��Left=�$��23��$��	57,33,0,0q� �� TextWrapping$
 Wrap=�$	��C:\��$	��Top=�$	��358���� -   $ BTN_OpenSourceFolder��$��開啟��$
��Left=�$��
420,33,0,0q�$	��Top=�$��31��$��24���� $
��Left=�$��	21,61,0,0q�$
��Wrap=�$ ��檔案目的路徑(結果)��$	��Top=�$	��107���� -	   $ BTN_SelectTargetFolder��$��瀏覽��#��[  $
��Left=�$��	21,81,0,0q�$	��Top=�$��31��$��24���� -
   $ TBX_TargetFolder��$
��Left=�$��23��$��	57,81,0,0q�$
 Wrap=�$��C:\New��$	��Top=�$	��358���� -   $ BTN_OpenTargetFolder��$��開啟��$
��Left=�$��
420,81,0,0q�$	��Top=�$��31��$��24���� $��檔案類型��$
��Left=�$��79��$��
21,109,0,0q�$	��Top=�$	��430��.��� .+��� $
��Left=�$��	10,10,0,0q�$
��Wrap=�$��照片:��$	��Top=�$��89���� -   $ TBX_ImageExtends��$
��Left=�$��23��$��46,4,0,0q�$
 Wrap=�$��jpg, jpeg, png��$	��Top=�$	��362���� $
��Left=�$��	10,32,0,0q�$
��Wrap=�$��影片:��$	��Top=�$��89���� -   $ TBX_VideoExtends��$
��Left=�$��23��$��	46,32,0,0q�$
 Wrap=�$��avi, mts, mpg, mpeg, mov��$	��Top=�$	��362���� $��檔案處理��$
��Left=�$	��131��$��
21,189,0,0q�$	��Top=�$	��430��.��� $��	0,0,-12,0q�.+��� $��處理方式: ��$
��Left=�$��6,0,0,0q�$	��Top=�$ 
0.51,0.725&��� -    ��Name$ CBX_FileAct��$
��Left=�$��70,2,0,0q�$	��Top=�$	��120�� �� SelectedIndex$ 0��.@��� $��ListOnly���� $
��Move���� $
��Copy���� -    ��Name$ CBX_FileRenameAsCopyMove��$8��2當 Copy/Move時，修改新檔名: YYYYMMDDHHmmss��$
��Left=�$��	10,26,0,0q�$	��Top=�{�.  �� -   $ CBX_CHKBinary��$L��F當 Copy/Move前，為確認重複檔案，執行內容比對(較慢).��$
��Left=�$��	10,41,0,0q�$	��Top=�{�.  �� -   $ CBX_ReCHKBinary��$<��6當 Copy/Move後，執行內容比對確認(非常慢)��$
��Left=�$��	10,56,0,0q�$	��Top=�{�.  �� -   $ CBX_AddInfoFileAsCopyMove��$=��7當 Copy/Move時，將來源目錄名加入Info檔(慢)��$
��Left=�$��
10,72,0,-3q�$	��Top=�{�.  �� $M��G** Move : 重複的檔案 將被移至[檔案目的路徑\Dup]下面。��$
��Left=�$��	10,85,0,0q�$	��Top=��� $��新檔案結構設定��$
��Left=�$	��123��$��
21,325,0,0q�$	��Top=�$	��327��.��� .+�R� -    R�Name$ TV_Structure��$
��Left=�$��77��$��	10,10,0,0q�$	��Top=�$	��141��.@�Q� $
��year�� Q� 
IsExpanded . .@�Q� $��month�� . .@�Q� $
��date�� . �� $	��Lv1��$
��Left=�$��
152,10,0,0q�$	��Top=��� $	��Lv2��$
��Left=�$��
152,36,0,0q�$	��Top=��� $	��Lv3��$
��Left=�$��
152,62,0,0q�$	��Top=��� -   $ 
CBX_Level1��$
��Left=�$��
186,10,0,0q�$	��Top=�$	��120��$ 0��.@��� $��西元年分���� $��月份���� $��日期���� -   $ 
CBX_Level2��$
��Left=�$��
186,36,0,0q�$	��Top=�$	��120��$ 1��.@��� $��西元年分���� $��月份���� $��日期���� -   $ 
CBX_Level3��$
��Left=�$��
186,62,0,0q�$	��Top=�$	��120��$ 2��.@��� $��西元年分���� $��月份���� $��日期���� $��(無)���� -   $ BTN_Execution��$��執行��#��[  $
��Left=�$��376,364,0,0q�$	��Top=�$��75��$��62��