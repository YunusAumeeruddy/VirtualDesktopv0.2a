   M S B A M L   `   `   `  ����I  EVirtualDesktop, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:VirtualDesktopVirtualDesktop  5   	   *    $VirtualDesktop.ManageCategoryFileExt   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   .localclr-namespace:VirtualDesktop     5   	   -   5        $	��378��5   	   $	��297��6     � 
ResizeMode$  NoResize=�5	   	    � WindowStyle$
 None=�5
   	    � WindowStartupLocation$ CenterOwner=�5   	    � ShowInTaskbar .  .��5      � ���#003566 � RowDefinitions 5   
   �� 5      $��28 �6   �� 5      $��31 �6   �� 5      $��29 �6   �� 5      $	��180 �6   �� 5      $��29 �6    � ColumnDefinitions 5   
   �� 5      $	��209 �6   �� 5      $	��169 �6   .+�5   
   �� -   5        $��lblTitle��5      $��2��6    $	��374��5      $��28��6   $��Center=�5      $��Center=�64   
�������$
��Bold"�5   #   $.��(Add/Remove file extensions for category ��5       �� 5"   
   -   5        $��	lblStatus��5"      $��1��6!   $��2��6.   $	��374��5#      $��27��6   
�������$P��JDo not include the dot(.) for the file extension (e.g. 'exe' , NOT '.exe')��5%      �� 5&   
   -   5        $��
txtFileExt��5&      $��2��6$   $��0��61   $	��205��5'      $��25��6   5)      �� 5*   
   -   5        $��	btnAddExt��5*      $��2��6"   $��1��6/   $	��165��5+      $��25��6   $��Add extension��5,      5-      �� 5.   
   -   5        $��lstBxFileExt��5.      $��3��6&   $��0��63   $	��205��5/      $	��180��6    �� SelectionMode$ Multiple=�50      51      �� 52   
   -   5        $��btnRemoveExt��52      $��3��6%   $��1��62   $	��165��53      $��25��6   $	��Top=�54      {�.  $"��Remove selected extension(s)��56      57      �� 58   
   -   5        $��btnSave��58      $��4��6    $��0��6-   $��75��59      $��25��6   $��Right=�5:      $��Center=�6-   $��0,0,5,0q�5;      $
��Save��5<      {�.  5>      �� 5?   
   -	   5        $��btnClose��5?      $��4��6!   $��1��6.   $��75��5@      $��25��6   $��5,0,0,0q�5A      $
��Left=�5B      $��Center=�6,   $��Close��5C      5D      5E      5F      