   M S B A M L   `   `   `  ���� I  EVirtualDesktop, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:VirtualDesktopVirtualDesktop  /clr-namespace:VirtualDesktopVirtualDesktop      VirtualDesktop.MainWindow   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     .localclr-namespace:VirtualDesktop     4MyNamespaceclr-namespace:VirtualDesktop       � Title$  
MainWindow��$	��768��$
��1366�� � AllowsTransparency .  � ShowInTaskbar .   � WindowStyle$
 None=� � 
ResizeMode$ NoResize=� � 	Resources  �� 
TargetType   mainMenuNavBtn �� Property �� Value	 � Color
 � Offset �� 
TargetType ��Name �� Triggers P� Property P� Value ]� 
Conditions �� Property �� Value��%  &	        �� " ��.���  �@�OverridesDefaultStyle . ��  � �� �� .��� $	 #24C6DC��$
 0Q�� $	 #514A9D��$
 1Q���  � �� �� " ��.R�� #��z��.+��� $ MyContentPresenter��#��z��$��Center=�$��Center=� P�  � {� .  . ���  � �� �� .��� $	 #283048��$
 0Q�� $	 #859398��$
 1Q�]�  ��  �@J�IsMouseOver . ��  � {� . /���  � ��
 �����]�  ��  �@J�IsMouseOver . ��  � {� .  /���  � ��
 �   �.���� -   $��MainStackPanel�� �� Orientation$ Vertical=�
���   �.+�]� -    ]�Name$ topDockPanel����.  $	��Top=�
���   �$��38��.+��� -    ��Name$ btnCategoryMenuNavPrev��$��<��$��Arial)�$��20(�$��3,3,0,0q�$��24��$��32��$
��Left=�$��Center=�#��[  �� -    ��Name$ scroll_CategoryMenuPanel����. $��0,3,0,0q�$��32��$
��Left=�$	��Top=�$��Hidden=�$��Disabled=�.���� -    ��Name$ CategoryMenuPanel��$ 
Horizontal=�$��32��$	��200���� -   $ btnCategoryMenuNavNext��$	��Top=�$��0,3,0,0q�$��>��$��Arial)�$��20(�$��24��$��32��$
��Left=�$��Center=�#��[  ]� -   $ separatorDockPanel��$	��Top=�$��1��
�������]� -   $ middleDockPanel����.  $	��Top=�$��42��
���   �.+�� -	   $��middleDockPanelGrid�� � RowDefinitions �� $��42 � � ColumnDefinitions �� $
��Auto ��� $��90 ��� $��5 ��� $��90 ��� $��5 ��� $��90 ��� $��5 ��� $��90 ��� $��5 ��� $	��171 ��� $��5 ��� $	��196 ��� $��2 ��� $��7 ��� $��95 ��� $��7 ��� $��56 ��� $��2 ��� $	��101 ��� $��7 ��� $��92 ��� $
��Auto �.+��� -
   $��btnAdd��$��1��$��90��$��38��$��Center=�$��Bottom=� � ContextMenu �� .@�m� -   $)��#btnAdd_ContextMenuItem_ShortcutIcon��$��New shortcut icon��m� -   $%��btnAdd_ContextMenuItem_Category��$��New category��.���� $ 
Horizontal=�.+��� -   $��38��$��38��$��Center=�$��Center=�._��� $��32��$��32��$��Center=�$
��Left=�$6��0pack://application:,,,/Resources/Images/plus.png���� $��Center=�$��Center=� �� TextAlignment$
 Left=�$	��Add���� -   $��	btnRemove��$��3��$��90��$��38��$��Center=�$��Bottom=� �� .@�m� -   $,��&btnRemove_ContextMenuItem_ShortcutIcon��{�.  $��Selected shortcut icon(s)��m� -   $(��"btnRemove_ContextMenuItem_Category��{�.  $��Selected category��.���� $ 
Horizontal=�.+��� $��38��$��38��$��Center=�$��Center=�._��� $��32��$��32��$��Center=�$
��Left=�$7��1pack://application:,,,/Resources/Images/minus.png���� $��Center=�$��Center=�$
 Left=�$��Remove���� -   $��btnUndo��$��5��$��90��$��38��{�.  $��Center=�$��Bottom=�.���� $ 
Horizontal=�.+��� $��38��$��38��$��Center=�$��Center=�._��� $��32��$��32��$��Center=�$
��Left=�$6��0pack://application:,,,/Resources/Images/undo.png���� $��Center=�$
��Left=�$
��Undo���� -   $��btnRedo��$��7��$��90��$��38��{�.  $��Center=�$��Bottom=�.���� $ 
Horizontal=�.+��� $��38��$��38��$��Center=�$��Center=�._��� $��32��$��32��$��Center=�$
��Left=�$6��0pack://application:,,,/Resources/Images/redo.png���� $��Center=�$
��Left=�$
��Redo���� -   $��btnMove��$��9��$	��171��$��38��{�.  $��Center=�$��Bottom=�.���� $ 
Horizontal=�.+��� $��38��$��38��$��Center=�$��Center=�._��� $��32��$��32��$��Center=�$
��Left=�$6��0pack://application:,,,/Resources/Images/move.png���� $ Vertical=�.+��� $	��130��$��Center=�$
��Left=�$ Center=�$��Move shortcut icon(s)���� $	��130��$��Center=�$
��Left=�$ Center=�$��under another category���� -   $��btnMoveCopy��$��11��$	��196��$��38��{�.  $��Center=�$��Bottom=�.���� $ 
Horizontal=�.+��� $��38��$��38��$��Center=�$��Center=�._��� $��32��$��32��$��Center=�$
��Left=�$6��0pack://application:,,,/Resources/Images/copy.png���� $ Vertical=�.+��� $	��155��$��Center=�$
��Left=�$ Center=�$"��Add copy of shortcut icon(s)���� $	��155��$��Center=�$
��Left=�$ Center=�$��under another category���� -   $��13��$��1��$��38��$��Center=�$��Center=�
������ $	��0.5q�
��������� -   $��14��$ Vertical=�$��95��$��42��
������ .+��� $ Center=�$��16��
�������$��Auto-Categorised���� -   $��chkbx_Auto_Categorised�� d� 	IsChecked$ FalseY�$��Center=�$��Center=� � LayoutTransform ��  �� ScaleX$	 1.5Q� �� ScaleY$	 1.5Q�  � ToolTip  �� Auto-Categorised ON:�� rpWhen you add a new shortcut icon through this application and the shortcut icon represents a file(not a folder),�� dbthe category to which the shortcut icon should be assigned to is determined by the file extension.�� ��A file extension can only be associated to one category, but a category may have multiple file extensions associated to it as well as not having any.�� ��If none of the available categories has the associated file extension, the application will add the shortcut icon to the current selected category grid if the selected category is of mixed type.�� ZXIf not of mixed type, the shortcut icon will be added to the current main type category.�� �� Notes:�� OM- This mode only effects new shortcut icon(s) added through this application.�� ;9- Having a Main type category is compulsory in this mode.�� ��- Any Main type category is also Mixed type. If you mark a category as Main type, it will mark it as Mixed type if it was not Mixed type.�� ��- There can only be 1 Main type category. Marking a non Main type category as Main type will no more consider the previous Main type as of Main type.�� �� Auto-Categorised OFF:�� trWhen you add a new shortcut icon through this application, it will be added to the current selected category grid.�� �� -   $��15��$��1��$��38��$��Center=�$��Center=�
������ $	��0.5q�
��������� -   $��16��$ Vertical=�$��56��$��42��.+��� $ Center=�$��16��
�������$��	Auto-Sort���� -   $��chkbx_Auto_Sort��$ FalseY�$��Center=�$��Center=� �� $	 1.5Q�$	 1.5Q�  �� Auto-Sort ON:�� ��It keeps the shortcut icon in the grid to be sorted and positioned in ascending order based on the selected item in "Sort by" drop-down listbox.�� YWThe way the shortcut icon(s) in the grid is/are positioned is row-wise per each column.�� 
Example:�� ><1st shortcut icon will be positioned on 1st row & 1st column�� ><2nd shortcut icon will be positioned on 2nd row & 1st column�� EC3rd shortcut icon will be positioned on 3rd row & 1st column etc...�� �� sqIf all rows have been taken by other shortcut icons on a column, shortcut icons will be added to the next column.�� zxEventually, additional rows will be added if there are no more spaces left on the grid to position the shortcut icon(s).�� &$(ALSO APPLIES WHEN THIS MODE IS OFF)�� �� ><Note: You cannot drag and drop a shortcut icon in this mode.�� �� Auto-Sort OFF:�� OMYou can drag and drop shortcut icon(s) to any positions you like on the grid.�� �� -   $��18��$ Vertical=�$��Center=�$	��101��$��42��.+��� $��Sort by��$	��100��$��16��
�������$ Center=��� -   $��cmbx_sortBy��$	��100��$��24��.@��� -   $��cmbx_sortBy_Item_Name��! �� 
IsSelected! . .��Name�� -   $��19��$��1��$��38��$��Center=�$��Center=�
������ $	��0.5q�
��������� -   $��	btnSearch��$��20��$��90��$��38��{�. $��Center=�$��Bottom=�.���� $ 
Horizontal=�.+��� $��38��$��38��$��Center=�$��Center=�._��� $��32��$��32��$��Center=�$
��Left=�$8��2pack://application:,,,/Resources/Images/search.png���� $��Center=�$
��Left=�$��Search��]� -    $ bottomDockPanel��
������ ��.  $��Bottom=�$��0,3,0,0q�$	��326�� �� .@�m� -!   $��Set background image��m� -"   $��Remove background image��.+��� -#   $# scroll_ShortcutIconGridCanvas����. $��3,0,0,0q�$
��Left=�$	��Top=�$��Disabled=�$��Visible=�