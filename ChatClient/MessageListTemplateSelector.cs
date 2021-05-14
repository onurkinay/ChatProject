using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ChatClient
{  
    public class MessageListTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate
            SelectTemplate(object item, DependencyObject container)
        {
            item = ((ListBoxItem)item).Content as Message;
            if (item != null && item is Message)
            {
                var taskitem = (Message)item;
                var window = Application.Current.MainWindow;

                if(taskitem.uye.id == "-1")
                {
                    return window.FindResource("server") as DataTemplate;
                } 
                if (taskitem.uye.id == ((MainWindow)window).myId)
                {
                    if (taskitem.dosyaMi) return window.FindResource("dosya_benim") as DataTemplate;
                   
                    return window.FindResource("benim") as DataTemplate;
                }
                if (taskitem.dosyaMi)
                {
                    if(taskitem.gonderilmisMi) return window.FindResource("dosya_karsi_gonderilmis") as DataTemplate;
                    return window.FindResource("dosya_karsi") as DataTemplate;
                }
              
                return  window.FindResource("karsi") as DataTemplate;
            }

            return null;
        }
    }
}
