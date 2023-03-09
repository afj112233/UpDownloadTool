using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ICSStudio.Interfaces.DataType;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Tags;

namespace ICSStudio.QuickWatchPackage.View.Models
{
    internal class MemberTag:Tag
    {
        public MemberTag(ITag parentTag) : base((TagCollection)parentTag.ParentCollection)
        {
            ParentTag = parentTag;
            DisplayStyle = parentTag.DisplayStyle;
            ChildDescription = ((Tag) parentTag).ChildDescription;
        }

        public bool IsInCreate { set; get; } = true;
        
        public void AddListen()
        {
            WeakEventManager<Tag, PropertyChangedEventArgs>.AddHandler((Tag)ParentTag, "PropertyChanged",
                ParentTag_PropertyChanged);
        }

        public void RemoveListen()
        {

            WeakEventManager<Tag, PropertyChangedEventArgs>.RemoveHandler((Tag)ParentTag, "PropertyChanged",
                ParentTag_PropertyChanged);
        }

        private void ParentTag_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.RaisePropertyChanged(e.PropertyName);
        }

        public ITag ParentTag { get; }

        public int Index { set; get; }

        public Hashtable Transform { set; get; }
        public override void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            if(IsInCreate)return;
            (ParentTag as Tag)?.RaisePropertyChanged(propertyName);
            base.RaisePropertyChanged(propertyName);
        }
    }
}
