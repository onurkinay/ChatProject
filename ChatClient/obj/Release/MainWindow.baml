   M S B A M L   `   `   `  ���� E  AChatClient, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'clr-namespace:ChatClient
ChatClient  D @Emoji.Wpf, Version=0.3.0.0, Culture=neutral, PublicKeyToken=null8*clr-namespace:Emoji.Wpf;assembly=Emoji.Wpf	Emoji.Wpf     ChatClient.MainWindow   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     *localclr-namespace:ChatClient     <emoji*clr-namespace:Emoji.Wpf;assembly=Emoji.Wpf     -     � Title$  Chat Client��$��554.4��$��965.6�� � 
ResizeMode$ CanMinimize=� � 	Resources  
  server �� Padding �� ElementName 	 karsi  Emoji.Wpf.TextBlock   Text 	 benim  dosya_karsi
 � Tag �� StringFormat   ChatClient.MyClass   
MyProperty	 �Name
 � Minimum � Maximum  dosya_karsi_gonderilmis  dosya_benim  dosya_benim_gonderilmis,   &ChatClient.MessageListTemplateSelector  MessageTemplate��%!  &	        &	 �     &	 �    &	 �    &	 t    &	 �    &	 �    &	 �    �� .R��� ���	#FF760505.+��� $ 5q�._��� .+��� $��10(�
����������� *Content.uye.nickname+�� -   $��Disabled=�$��0 0 20 0q����� *ActualWidth+$ 
lbMesajlar��.���� 
����������� *Content.mesaj+�� .R��� ���	#FFDADAA4.+��� $ 5q�._��� .+��� $��10(����� *Content.uye.nickname+�� -   $��Disabled=����� *ActualWidth+$ 
lbMesajlar��.��  $
��Wrap=� �� *Content.mesaj+�� .R��� ���	#FF7FA7CB.+��� $ 5q�._��� .+��� $��10(�$��Right=����� *Content.uye.nickname+�� -   $��Disabled=����� *ActualWidth+$ 
lbMesajlar��.��  $
��Wrap=�$��Right=� �� *Content.mesaj+�� .R��� ���	#FFDADAA4$ karsi��.+��� $ 5q�._��� .+��� $��10(����� *Content.uye.nickname+$ {0} dosya gönderdi���� -   $��Disabled=����� *ActualWidth+$ 
lbMesajlar��.���� $
��Wrap=����� *Content.mesaj+$ Dosya adı: {0}���� -   $��Kabul et��$��0q�$��5q� �� *	Content+ �� $ progress������ � $	 progress��$
 0Q�$	 100Q�$��15��$y�	Collapsed=��� .R��� ���	#FFDADAA4$ karsi��.+��� $ 5q�._��� .+��� $��10(����� *Content.uye.nickname+$ {0} dosya göndermiş���� -   $��Disabled=����� *ActualWidth+$ 
lbMesajlar��.���� $
��Wrap=����� *Content.mesaj+$ Dosya adı: {0}���� .R��� ���	#FF7FA7CB.+��� $ 5q�._��� .+��� $��Dosya gönderdiniz��$��10(�$��Right=��� -   $��Disabled=����� *ActualWidth+$ 
lbMesajlar��.���� $
��Wrap=�$��Right=����� *Content.mesaj+$ Dosya adı: {0}��� $	 progress��-	   $
 0Q�$	 100Q�$��15��$y�Visible=��� .R��� ���	#FF7FA7CB.+��� $ 5q�._��� .+��� $��Dosya gönderdiniz��$��10(�$��Right=��� -
   $��Disabled=����� *ActualWidth+$ 
lbMesajlar��.���� $
��Wrap=�$��Right=����� *Content.mesaj+$ Dosya adı: {0}��  .��]� .+�o� $	��Top=�.@�m� -    m�Name$ 
btnConnect��$��_Sunucuya Bağlan��$ baglan��m� -   $ btnOdaOlustur��$��_Oda Oluştur���  � ColumnDefinitions �� �� $��0* �.+�  Emoji.Wpf.RichTextBox  -     Name$ txtMesaj��{�.  $
��Left=�$��19��$��	0,478,0,0q�$	��Top=�$	��614��. �/� (   "ChatClient.FlowDocumentPagePadding   PagePadding$ 0q�.T�J� .*�.*��� .�text�� -    ��Name$ 
lbMesajlar����.  $
��Left=�$	��473��$	��Top=�$	��752��#��[ $��Hidden=�$��Hidden=�����  �� 
TargetType" ��.���  �� Property � �� �� Value$ Stretch=���  � |� .  �� -    ��Name$ 	btnGonder��{�.  $��Dosya Gönder��$
��Left=�$��620,478,0,0q�$	��Top=�$	��132���� -    ��Name$ 	txtOdalar��$
��Left=�$��757,252,0,0q�$
��Wrap=�$��Aktif Odalar��$	��Top=�$��17��$	��116���� -   $ lbOdalar��$
��Left=�$	��223��$��757,274,0,0q�$	��Top=�$	��198���� -   $ 
lblClients��$
��Left=�$	��226��$��
757,21,0,0q�$	��Top=�$	��198������ " �� �� Triggers ��  �� Binding �� *	DoBlink+ �� Value True N� EnterActions �� .����  g� AutoReverse .   g� RepeatBehavior$ 3x��.����  g� Duration$ 0:0:0.6D� �� TargetProperty$7 1(ListViewItem.Background).(SolidColorBrush.Color)�	 �� To$ SkyBlueX�:�  :� RoutedEvent$% Control.PreviewMouseDoubleClick��[��� .���� .���� $ 0:0:0.0D�$7 1(ListViewItem.Background).(SolidColorBrush.Color)�$ WhiteX��� -   $ txtClientlar��$
��Left=�$��	757,0,0,0q�$
��Wrap=�$��Bağlı Üyeler��$	��Top=�$��16��$��63���� -   $ txtId��$
��Left=�$��	10,58,0,0q�$
��Wrap=�$	��Top=�$	��194��$��16��$y�	Collapsed=�r� -     r�Name$  blink��! r� LoadedBehavior$! Manual=�" r� UnloadedBehavior$
" Stop=�$y�Hidden=�$
��Left=�$	��100��$��
32,141,0,0q�$	��Top=�$	��100��# r� Source$# when-604.wav?�