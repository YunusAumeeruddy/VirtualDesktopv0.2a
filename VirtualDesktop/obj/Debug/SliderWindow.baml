   M S B A M L   `   `   `  ����I  EVirtualDesktop, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:VirtualDesktopVirtualDesktop  5   	   !    VirtualDesktop.SliderWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   .localclr-namespace:VirtualDesktop     5   	     � Title$  SliderWindow��5   	   $
��1366��6   $��30��6+    � 
ResizeMode$ NoResize=�5	   	    � WindowStyle$
 None=�6    � ShowInTaskbar .  
���   � � 	Resources 5       �� 
TargetType   mainMenuNavBtn �� Property �� Value � Color	 � Offset
 �� 
TargetType ��Name �� Triggers P� Property P� Value ]� 
Conditions �� Property �� Value��%  &	        �� 5   
   " ��6   .�5      ��  �@�OverridesDefaultStyle . 66   �� 5       � �� 5      �� 5      .��5      � $ #24C6DC��6'   $	 0Q�67   � 5      $ #514A9D��6'   $	 1Q�67   5      5      �� 5       � �� 5      �� 5      "
 ��6&   .R�5      � #��z��6   .+�5      �� $ MyContentPresenter��6/   #��z��5   +   $��Center=�5   +   $��Center=�5    +   5!      5"      5$       5&      P� 5'       � {� .  . �5(      ��  � �� 5)      �� 5*      .��5+   "   � $ #283048��6/   $	 0Q�6?   � 5,   "   $ #859398��6/   $	 1Q�6?   5-      5/      50      ]� 51       52      �� 53       �@J�IsMouseOver . 6;   �� 54       � {� . 69   /�56      �� 57       � ��
 �����67   59      ]� 5:       5;      �� 5<       �@J�IsMouseOver . 6;   �� 5=       � {� .  69   /�5?      �� 5@       � ��
 �   �67   5B      5D      5E      .��5G      �� -   5        $��	MainPanel��5G      
���   �.+�5H   
   �� -   5        $��MenuContentPanel��5H   &    K� ZIndex$ 2��6    �� Orientation$ Vertical=�6>   $
��1366��6U   $��Center=�5I      $	��Top=�62   $��	0,-83,0,0q�6J   .+�5J      �� -   5        $��SlideMenuPanel��5J      $ Vertical=�6/   .+�5K      ]� -   5         ]�Name$ topDockPanel��5K      ��.  $	��Top=�5M      
���   �$��38��5O      .+�5Q      �� -   5         ��Name$ btnCategoryMenuNavPrev��5Q      $��<��5R      $��Arial)�5T      $��20(�5U      $��3,3,0,0q�5V      $��24��5W      $��32��5X      $
��Left=�5Y      $��Center=�5Z      #��[  5[      5\      �� 5]      -   5         ��Name$ scroll_CategoryMenuPanel��5]   #   ��. $��0,3,0,0q�5_      $��32��5`      $
��Left=�5a      $	��Top=�5b      $��Hidden=�5c      $��Disabled=�5d      .��5f      �� -   5         ��Name$ CategoryMenuPanel��5f   %   $ 
Horizontal=�5g   !   $��32��5h   !   $	��200��5i   !   5k      5l      �� 5m      -   5        $ btnCategoryMenuNavNext��5m      $	��Top=�5n      $��0,3,0,0q�5o      $��>��5p      $��Arial)�5r      $��20(�5s      $��24��5t      $��32��5u      $
��Left=�5v      $��Center=�5w      #��[  5x      5y      5z      ]� 5{      -	   5        $ separatorDockPanel��5{      $	��Top=�5|      $��1��5}      
�������5�      ]� 5�      -
   5        $ bottomDockPanel��5�      ��.  $	��Top=�5�      $��42��5�      
���   �.+�5�      � -   5        $��bottomDockPanelGrid��5�       � RowDefinitions 5�      �� 5�      $��42 �6,    � ColumnDefinitions 5�      �� 5�   ,   $
��Auto �6=   �� 5�   ,   $��90 �6=   �� 5�   ,   $��5 �6=   �� 5�   ,   $��90 �6=   �� 5�   ,   $��5 �6=   �� 5�   ,   $��90 �6=   �� 5�   ,   $��5 �6=   �� 5�   ,   $��90 �6=   �� 5�   ,   $��5 �6=   �� 5�   ,   $	��171 �6=   �� 5�   -   $��5 �6>   �� 5�   -   $	��196 �6>   �� 5�   -   $��2 �6>   �� 5�   -   $��7 �6>   �� 5�   -   $��95 �6>   �� 5�   -   $��7 �6>   �� 5�   -   $��56 �6>   �� 5�   -   $��2 �6>   �� 5�   -   $	��101 �6>   �� 5�   -   $��7 �6>   �� 5�   -   $��92 �6>   �� 5�   -   $
��Auto �6>   .+�5�      �� -   5        $��btnAdd��5�      $��1��5�   !   $��90��5�      $��38��5�      $��Center=�5�      $��Bottom=�5�       � ContextMenu 5�      �� 5�   "   .@�5�   &   m� -   5        $)��#btnAdd_ContextMenuItem_ShortcutIcon��5�   !   $��New shortcut icon��5�   !   5�   !   m� 5�   &   -   5        $%��btnAdd_ContextMenuItem_Category��5�   !   $��New category��5�   !   5�   !   5�   #   .��5�      �� $ 
Horizontal=�6)   .+�5�   "   �� -   5        $��38��5�      $��38��5�      $��Center=�5�      $��Center=�5�      ._�5�   &   �� $��32��6,   $��32��5�   $   $��Center=�5�   $   $
��Left=�5�   $   $6��0pack://application:,,,/Resources/Images/plus.png��5�   $   5�   #   �� 5�   "   $��Center=�5�   !   $��Center=�5�   !    �� TextAlignment$
 Left=�5�   !   $	��Add��5�   !   5�      5�      �� 5�      -   5        $��	btnRemove��5�      $��3��5�   !   $��90��5�      $��38��5�      $��Center=�5�      $��Bottom=�5�       5�      �� 5�   "   .@�5�   &   m� -   5        $,��&btnRemove_ContextMenuItem_ShortcutIcon��5�   !   {�.  $��Selected shortcut icon(s)��5�   !   5�   !   m� 5�   &   -   5        $(��"btnRemove_ContextMenuItem_Category��5�   !   {�.  $��Selected category��5�   !   5�   !   5�   #   .��5�      �� $ 
Horizontal=�6)   .+�5�   "   �� $��38��5�      $��38��5�      $��Center=�5�      $��Center=�5�      ._�5�   &   �� $��32��6,   $��32��5�   $   $��Center=�5�   $   $
��Left=�5�   $   $7��1pack://application:,,,/Resources/Images/minus.png��5�   $   5�   #   �� 5�   "   $��Center=�5�   !   $��Center=�5�   !   $
 Left=�5�   !   $��Remove��5�   !   5�      5�      �� 5�      -   5        $��btnUndo��5�   !   $��5��5�   !   $��90��5�   !   $��38��5�   !   {�.  $��Center=�5�   !   $��Bottom=�5�   !   .��5     �� $ 
Horizontal=�6)   .+�5  "   �� $��38��6)   $��38��64   $��Center=�5  -   $��Center=�5  -   ._�5  &   �� $��32��6,   $��32��5  0   $��Center=�5  0   $
��Left=�5  0   $6��0pack://application:,,,/Resources/Images/undo.png��5	  0   5
  #   �� 5  "   $��Center=�6,   $
��Left=�5  0   $
��Undo��5  0   5     5     �� 5     -   5        $��btnRedo��5     $��7��5  !   $��90��5     $��38��5     {�.  $��Center=�5     $��Bottom=�5     .��5     �� $ 
Horizontal=�6)   .+�5  "   �� $��38��5  %   $��38��5  %   $��Center=�5  %   $��Center=�5  %   ._�5  &   �� $��32��6,   $��32��5   0   $��Center=�5!  0   $
��Left=�5"  0   $6��0pack://application:,,,/Resources/Images/redo.png��5#  0   5$  #   �� 5%  "   $��Center=�5&  %   $
��Left=�5'  %   $
��Redo��5(  %   5)     5*     �� 5+     -   5        $��btnMove��5,     $��9��5+  !   $	��171��5-     $��38��5.     {�.  $��Center=�50     $��Bottom=�51     .��55     �� $ 
Horizontal=�6)   .+�56  "   �� $��38��57     $��38��58     $��Center=�59     $��Center=�5:     ._�5;  &   �� $��32��6,   $��32��5<  $   $��Center=�5=  $   $
��Left=�5>  $   $6��0pack://application:,,,/Resources/Images/move.png��5?  $   5@  #   �� 5A  "   $ Vertical=�6-   .+�5B  &   �� $	��130��5C  !   $��Center=�5D  !   $
��Left=�5E  !   $ Center=�5F  !   $��Move shortcut icon(s)��5G  !   �� 5H  &   $	��130��5I  !   $��Center=�5J  !   $
��Left=�5K  !   $ Center=�5L  !   $��under another category��5M  !   5N  #   5O     5P     �� 5Q     -   5        $��btnMoveCopy��5R     $��11��5Q  !   $	��196��5S     $��38��5T     {�.  $��Center=�5V     $��Bottom=�5W     .��5[     �� $ 
Horizontal=�6)   .+�5\  "   �� $��38��5]     $��38��5^     $��Center=�5_     $��Center=�5`     ._�5a  &   �� $��32��6,   $��32��5b  $   $��Center=�5c  $   $
��Left=�5d  $   $6��0pack://application:,,,/Resources/Images/copy.png��5e  $   5f  #   �� 5g  "   $ Vertical=�6-   .+�5h  &   �� $	��155��5i  !   $��Center=�5j  !   $
��Left=�5k  !   $ Center=�5l  !   $"��Add copy of shortcut icon(s)��5m  !   �� 5n  &   $	��155��5o  !   $��Center=�5p  !   $
��Left=�5q  !   $ Center=�5r  !   $��under another category��5s  !   5t  #   5u     5v     �� 5w     -   5        $��13��5w  !   $��1��5x     $��38��5y     $��Center=�5z     $��Center=�5{     
������ $	��0.5q�5}     
�������5     �� 5�     -   5        $��14��5�  %   $ Vertical=�5�     $��95��5�     $��42��5�     
������ .+�5�     �� $ Center=�6(   $��16��5�      
�������$��Auto-Categorised��5�      �� 5�     -   5        $��chkbx_Auto_Categorised��5�  '    d� 	IsChecked$ FalseY�5�     $��Center=�5�     $��Center=�5�      � LayoutTransform 5�  "   �� 5�  &    �� ScaleX$	 1.5Q�65    �� ScaleY$	 1.5Q�6B   5�       � ToolTip  5�     �� 5�  "   Auto-Categorised ON:�� 5�     rpWhen you add a new shortcut icon through this application and the shortcut icon represents a file(not a folder),�� 5�     dbthe category to which the shortcut icon should be assigned to is determined by the file extension.�� 5�     ��A file extension can only be associated to one category, but a category may have multiple file extensions associated to it as well as not having any.�� 5�     ��If none of the available categories has the associated file extension, the application will add the shortcut icon to the current selected category grid if the selected category is of mixed type.�� 5�     ZXIf not of mixed type, the shortcut icon will be added to the current main type category.�� 5�     �� 5�     Notes:�� 5�     OM- This mode only effects new shortcut icon(s) added through this application.�� 5�     ;9- Having a Main type category is compulsory in this mode.�� 5�     ��- Any Main type category is also Mixed type. If you mark a category as Main type, it will mark it as Mixed type if it was not Mixed type.�� 5�     ��- There can only be 1 Main type category. Marking a non Main type category as Main type will no more consider the previous Main type as of Main type.�� 5�     �� 5�     Auto-Categorised OFF:�� 5�     trWhen you add a new shortcut icon through this application, it will be added to the current selected category grid.�� 5�     5�  #   5�     �� 5�     -   5        $��15��5�  !   $��1��5�     $��38��5�     $��Center=�5�     $��Center=�5�     
������ $	��0.5q�5�     
�������5�     �� 5�     -   5        $��16��5�  %   $ Vertical=�5�     $��56��5�     $��42��5�     .+�5�     �� $ Center=�6(   $��16��5�      
�������$��	Auto-Sort��5�      �� 5�     -   5        $��chkbx_Auto_Sort��5�  '   $ FalseY�5�     $��Center=�5�     $��Center=�5�      5�  "   �� 5�  &   $	 1.5Q�65   $	 1.5Q�6B   5�       5�     �� 5�  "   Auto-Sort ON:�� 5�     ��It keeps the shortcut icons in the grid to be sorted and positioned in ascending order based on the selected item in "Sort by" drop-down listbox.�� 5�     YWThe way the shortcut icon(s) in the grid is/are positioned is row-wise per each column.�� 5�     
Example:�� 5�     ><1st shortcut icon will be positioned on 1st row & 1st column�� 5�     ><2nd shortcut icon will be positioned on 2nd row & 1st column�� 5�     EC3rd shortcut icon will be positioned on 3rd row & 1st column etc...�� 5�     �� 5�     sqIf all rows have been taken by other shortcut icons in a column, shortcut icons will be added to the next column.�� 5�     zxEventually, additional rows will be added if there are no more spaces left on the grid to position the shortcut icon(s).�� 5�     &$(ALSO APPLIES WHEN THIS MODE IS OFF)�� 5�     �� 5�     ><Note: You cannot drag and drop a shortcut icon in this mode.�� 5�     �� 5�     Auto-Sort OFF:�� 5�     OMYou can drag and drop shortcut icon(s) to any positions you like on the grid.�� 5�     5�  #   5�     �� 5�     -   5        $��18��5�  %   $ Vertical=�5�     $��Center=�5�     $	��101��5�     $��42��5�     .+�5�     �� $��Sort by��6(   $	��100��5�      $��16��5�      
�������$ Center=�5�      �� 5�     -   5        $��cmbx_sortBy��5�  '   $	��100��5�  #   $��24��5   #   ! �� SelectedValuePath$! Content��5  #   .@�5  "   �� -   5        $��cmbx_sortBy_Item_Name��5  /   " �� 
IsSelected" . .��5  9   Name6?   5     5     �� 5     -    5        $��19��5  !   $��1��5     $��38��5     $��Center=�5	     $��Center=�5
     
������ $	��0.5q�5     
�������5     �� 5     -!   5        $��	btnSearch��5     $��20��5  !   $��90��5     $��38��5     {�. $��Center=�5     $��Bottom=�5     .��5     �� $ 
Horizontal=�6)   .+�5  "   �� $��38��5     $��38��5     $��Center=�5     $��Center=�5     ._�5  &   �� $��32��6,   $��32��5  $   $��Center=�5   $   $
��Left=�5!  $   $8��2pack://application:,,,/Resources/Images/search.png��5"  $   5#  #   �� 5$  "   $��Center=�5%     $
��Left=�5&     $��Search��5'     5(     5)     5*     5,     5-     5.     � 5/  
   .+�50     �� -"   5        $��TopMenuHidePanel��50     $
��1366��61   $��30��6>   $ 
Horizontal=�51     
������ $y�Hidden=�52  2   .+�53     �� -#   5        $��lblTopMenuHidePanel��53     $
��1366��63   $��30��6@   $��Center=�54     $��Center=�6<   
������ 
�������$��^��56     57     �� 58     -$   5        $��TopMenuShowPanel��58     $
��1366��61   $��30��6>   $ 
Horizontal=�59     
������ .+�5;     �� -%   5        $��lblTopMenuShowPanel1��5;     $��266.67��64   $��30��6C   $��Center=�5<     
������ 
�������$�� ��5>     �� 5?     -&   5        $��lblTopMenuShowPanel2��5?     $��266.67��64   $��30��6C   $��Center=�5@     $��Center=�6:   
������ 
�������$��v��5B     �� 5C     -'   5        $��TopMenuShowPanelRight��5C     $ 
Horizontal=�5D     
������ .+�5E     �� -(   5        $��lblTopMenuShowPanel3_0��5E     $	��104��6:   $��30��6F   $��Center=�5F     
������ 
�������$��Auto-Categorized Mode:��5H     �� 5I     -)   5        $��lblTopMenuShowPanel3_1��5I     $��30��6:   $��30��6E   $��Center=�5J     
������ 
�������$	��OFF��5L     �� 5M     -*   5        $��lblTopMenuShowPanel3_2��5M     $��96��6:   $��30��6E   $��Center=�5N     
������ 
�������$��Auto-Sort Mode:��5P     �� 5Q     -+   5        $��lblTopMenuShowPanel3_3��5Q     $��30��6:   $��30��6E   $��Center=�5R     $��Right=�5S     
������ 
�������$	��OFF��5U     �� 5V     -,   5        $��lblTopMenuShowPanel3_4��5V     $	��103��6:   $��30��6F   $��Center=�5W     
������ 
�������$��(Sorted by Name)��5Y     5Z     5[     5\     5]     5^     