﻿#pragma checksum "..\..\Ozel.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "8DABFC7049BA07F1816CF1B21390CD05BA78940A14A11E401B37B46CC51A30B3"
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
    /// Ozel
    /// </summary>
    public partial class Ozel : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 17 "..\..\Ozel.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Emoji.Wpf.RichTextBox txtMesaj;
        
        #line default
        #line hidden
        
        
        #line 24 "..\..\Ozel.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnGonder;
        
        #line default
        #line hidden
        
        
        #line 25 "..\..\Ozel.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListView lbMesajlar;
        
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
            System.Uri resourceLocater = new System.Uri("/ChatClient;component/ozel.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\Ozel.xaml"
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
            
            #line 9 "..\..\Ozel.xaml"
            ((ChatClient.Ozel)(target)).Closing += new System.ComponentModel.CancelEventHandler(this.Window_Closing);
            
            #line default
            #line hidden
            
            #line 9 "..\..\Ozel.xaml"
            ((ChatClient.Ozel)(target)).Loaded += new System.Windows.RoutedEventHandler(this.Window_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.txtMesaj = ((Emoji.Wpf.RichTextBox)(target));
            
            #line 17 "..\..\Ozel.xaml"
            this.txtMesaj.KeyDown += new System.Windows.Input.KeyEventHandler(this.txtMesaj_KeyDown);
            
            #line default
            #line hidden
            
            #line 17 "..\..\Ozel.xaml"
            this.txtMesaj.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.txtMesaj_TextChanged);
            
            #line default
            #line hidden
            return;
            case 3:
            this.btnGonder = ((System.Windows.Controls.Button)(target));
            
            #line 24 "..\..\Ozel.xaml"
            this.btnGonder.Click += new System.Windows.RoutedEventHandler(this.Button_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.lbMesajlar = ((System.Windows.Controls.ListView)(target));
            
            #line 25 "..\..\Ozel.xaml"
            this.lbMesajlar.MouseDoubleClick += new System.Windows.Input.MouseButtonEventHandler(this.lbMesajlar_MouseDoubleClick);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

