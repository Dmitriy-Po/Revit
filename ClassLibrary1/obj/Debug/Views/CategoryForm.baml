   M S B A M L   `   `   `  ����  ClassLibrary1     ClassLibrary1.CategoryForm   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5        � WindowStartupLocation$  CenterScreen=�5   7    � WindowStyle$ 
ToolWindow=�6\   .��5
      � 
����� � � RowDefinitions 5   
   �� 5      $��70 �6   �� 5      �� 5      $��70 �6    � ColumnDefinitions 5   
   �� 5      .+�5   
   &   System.Windows.Controls.DataGrid  -   5        $��dgViewNames��5      $��1��5      $��2��5         AutoGenerateColumns .     CanUserAddRows .     CanUserDeleteRows .     CanUserReorderColumns .  ��5      ��  �� Path$! SelectedCategori.UsersViews�	   Columns	 5      0  *System.Windows.Controls.DataGridTextColumn  5      ,  &System.Windows.Controls.DataGridColumn
   Header$
 Имя вида��5          Width5  /System.Windows.Controls.DataGridLengthConverter$ * 5!         
IsReadOnly . 1  +System.Windows.Controls.DataGridBoundColumn   Binding 5      �� $ 
NameUIView�5#      4  .System.Windows.Controls.DataGridCheckBoxColumn  5$      $
 Видимость��5&      $ 70 5'       .   5%      �� $ StatusOfVisible�5(      5*      �� 5,   
   -   5        $��cbNameCategories��5.       �� SelectedIndex$ 0��5-      $��30��5/      $��10q�50       �� MaxDropDownHeight$	 100��6   ��51      �� *ListCategory+ �� SelectedItem 52      �� *SelectedCategori+��53      �� 54      .R�55      �� ��6    �� *NameCategori+56      58      �� 5:   
   -   5        $��btnApplyParametrs��5;      $��2��5<      $	��200��5=      $��40��5>      $��Center=�5?      $��Center=�5@      $��Go��5A      $��20(�5B      ��5C      �� $ ApplyChanges�5D      5E      