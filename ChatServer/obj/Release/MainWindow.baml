   M S B A M L   `   `   `  ���� E  AChatServer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'clr-namespace:ChatServer
ChatServer      ChatServer.MainWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     *localclr-namespace:ChatServer     -     � Title$  Chat Server��$	��450��$��801.6�� � 
ResizeMode$ CanMinimize=�.���  � ColumnDefinitions �� �� $��0* �.+��� -    ��Name$ 	btnServer��$��Sunucuyu Başlat��$
��Left=�$��	10,19,0,0q�$	��Top=�$	��210��$��19���� -    ��Name$ 	txtOdalar��$
��Left=�$��
525,12,0,0q�$
��Wrap=�$��Aktif Odalar��$	��Top=�$��17��$	��116���� -    ��Name$ lbOdalar��$
��Left=�$	��376��$��
525,34,0,0q�$	��Top=�$	��259������  �� 
TargetType" ��.�-    � ContextMenu �� .@�m� -   $��Remove���� -   $ 
lblClients��$
��Left=�$	��376��$��
254,34,0,0q�$	��Top=�$	��259���� -   $ txtClientlar��$
��Left=�$��
254,12,0,0q�$
��Wrap=�$��Bağlı Üyeler��$	��Top=�$��16��$��63���� -	    ��Name$ 
cbInternet��$��Internete açılsın mı��$
��Left=�$��	10,43,0,0q�$	��Top=�$	��210��