﻿#pragma checksum "..\..\MainWindow.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "2EB433972FD5F6572DC043C64BAE3106B65ACA3363E3A16EC92A2A095D50C6C9"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using ChatClient;
using Emoji.Wpf;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace ChatClient {
    
    
    /// <summary>
    /// MainWindow
    /// </summary>
    public partial class MainWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector, System.Windows.Markup.IStyleConnector {
        
        
        #line 121 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem btnConnect;
        
        #line default
        #line hidden
        
        
        #line 122 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem btnOdaOlustur;
        
        #line default
        #line hidden
        
        
        #line 132 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Emoji.Wpf.RichTextBox txtMesaj;
        
        #line default
        #line hidden
        
        
        #line 139 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListView lbMesajlar;
        
        #line default
        #line hidden
        
        
        #line 147 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnGonder;
        
        #line default
        #line hidden
        
        
        #line 150 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock txtOdalar;
        
        #line default
        #line hidden
        
        
        #line 151 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListView lbOdalar;
        
        #line default
        #line hidden
        
        
        #line 152 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListView lblClients;
        
        #line default
        #line hidden
        
        
        #line 180 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock txtClientlar;
        
        #line default
        #line hidden
        
        
        #line 181 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock txtId;
        
        #line default
        #line hidden
        
        
        #line 182 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MediaElement blink;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/ChatClient;component/mainwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\MainWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal System.Delegate _CreateDelegate(System.Type delegateType, string handler) {
            return System.Delegate.CreateDelegate(delegateType, this, handler);
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 10 "..\..\MainWindow.xaml"
            ((ChatClient.MainWindow)(target)).Closing += new System.ComponentModel.CancelEventHandler(this.Window_Closing);
            
            #line default
            #line hidden
            return;
            case 11:
            this.btnConnect = ((System.Windows.Controls.MenuItem)(target));
            
            #line 121 "..\..\MainWindow.xaml"
            this.btnConnect.Click += new System.Windows.RoutedEventHandler(this.btnBaglan_Click);
            
            #line default
            #line hidden
            return;
            case 12:
            this.btnOdaOlustur = ((System.Windows.Controls.MenuItem)(target));
            
            #line 122 "..\..\MainWindow.xaml"
            this.btnOdaOlustur.Click += new System.Windows.RoutedEventHandler(this.btnOdaOlustur_Click);
            
            #line default
            #line hidden
            return;
            case 13:
            this.txtMesaj = ((Emoji.Wpf.RichTextBox)(target));
            
            #line 132 "..\..\MainWindow.xaml"
            this.txtMesaj.KeyDown += new System.Windows.Input.KeyEventHandler(this.txtMesaj_KeyDown);
            
            #line default
            #line hidden
            
            #line 132 "..\..\MainWindow.xaml"
            this.txtMesaj.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.txtMesaj_TextChanged);
            
            #line default
            #line hidden
            return;
            case 14:
            this.lbMesajlar = ((System.Windows.Controls.ListView)(target));
            return;
            case 15:
            this.btnGonder = ((System.Windows.Controls.Button)(target));
            
            #line 147 "..\..\MainWindow.xaml"
            this.btnGonder.Click += new System.Windows.RoutedEventHandler(this.btnGonder_Click);
            
            #line default
            #line hidden
            return;
            case 16:
            this.txtOdalar = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 17:
            this.lbOdalar = ((System.Windows.Controls.ListView)(target));
            
            #line 151 "..\..\MainWindow.xaml"
            this.lbOdalar.MouseDoubleClick += new System.Windows.Input.MouseButtonEventHandler(this.lbOdalar_MouseDoubleClick);
            
            #line default
            #line hidden
            return;
            case 18:
            this.lblClients = ((System.Windows.Controls.ListView)(target));
            
            #line 152 "..\..\MainWindow.xaml"
            this.lblClients.MouseDoubleClick += new System.Windows.Input.MouseButtonEventHandler(this.lblClients_MouseDoubleClick);
            
            #line default
            #line hidden
            return;
            case 19:
            this.txtClientlar = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 20:
            this.txtId = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 21:
            this.blink = ((System.Windows.Controls.MediaElement)(target));
            return;
            }
            this._contentLoaded = true;
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        void System.Windows.Markup.IStyleConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 2:
            
            #line 19 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.ScrollViewer)(target)).Initialized += new System.EventHandler(this.ScrollViewer_Initialized);
            
            #line default
            #line hidden
            break;
            case 3:
            
            #line 32 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.ScrollViewer)(target)).PreviewMouseWheel += new System.Windows.Input.MouseWheelEventHandler(this.HandlePreviewMouseWheel);
            
            #line default
            #line hidden
            
            #line 32 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.ScrollViewer)(target)).Initialized += new System.EventHandler(this.ScrollViewer_Initialized);
            
            #line default
            #line hidden
            break;
            case 4:
            
            #line 45 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.ScrollViewer)(target)).PreviewMouseWheel += new System.Windows.Input.MouseWheelEventHandler(this.HandlePreviewMouseWheel);
            
            #line default
            #line hidden
            
            #line 45 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.ScrollViewer)(target)).Initialized += new System.EventHandler(this.ScrollViewer_Initialized);
            
            #line default
            #line hidden
            break;
            case 5:
            
            #line 59 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.ScrollViewer)(target)).PreviewMouseWheel += new System.Windows.Input.MouseWheelEventHandler(this.HandlePreviewMouseWheel);
            
            #line default
            #line hidden
            
            #line 59 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.ScrollViewer)(target)).Initialized += new System.EventHandler(this.ScrollViewer_Initialized);
            
            #line default
            #line hidden
            break;
            case 6:
            
            #line 62 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.downloadFile);
            
            #line default
            #line hidden
            break;
            case 7:
            
            #line 77 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.ScrollViewer)(target)).PreviewMouseWheel += new System.Windows.Input.MouseWheelEventHandler(this.HandlePreviewMouseWheel);
            
            #line default
            #line hidden
            
            #line 77 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.ScrollViewer)(target)).Initialized += new System.EventHandler(this.ScrollViewer_Initialized);
            
            #line default
            #line hidden
            break;
            case 8:
            
            #line 92 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.ScrollViewer)(target)).PreviewMouseWheel += new System.Windows.Input.MouseWheelEventHandler(this.HandlePreviewMouseWheel);
            
            #line default
            #line hidden
            
            #line 92 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.ScrollViewer)(target)).Initialized += new System.EventHandler(this.ScrollViewer_Initialized);
            
            #line default
            #line hidden
            break;
            case 9:
            
            #line 97 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.ProgressBar)(target)).Loaded += new System.Windows.RoutedEventHandler(this.progress_Loaded);
            
            #line default
            #line hidden
            break;
            case 10:
            
            #line 109 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.ScrollViewer)(target)).PreviewMouseWheel += new System.Windows.Input.MouseWheelEventHandler(this.HandlePreviewMouseWheel);
            
            #line default
            #line hidden
            
            #line 109 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.ScrollViewer)(target)).Initialized += new System.EventHandler(this.ScrollViewer_Initialized);
            
            #line default
            #line hidden
            break;
            }
        }
    }
}

