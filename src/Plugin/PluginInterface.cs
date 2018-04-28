using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;
using System.IO;
using System.Reflection;
using Fan.Plugin.Application;

namespace Fan.Plugin.Interface
{
    #region ����ӿڶ���

    //��ܲ���ӿڵĻ��ӿ�
    public interface IPlugin
    {
        string Name { get; }
        string Caption { get; }
        bool Visible { get; }
        bool Enabled { get; }
        void OnCreate(IApplicationRef hook);

    }
    //���ť�ӿڲ��
    public interface ICommandRef : IPlugin
    {
        //����ƶ�����ť��ʱ���ֵ�����
        string Tooltip { get;}
        //ͼ��
        Image Image { get;}
        //�������
        string Category { get;}
        //�Ƿ�ѡ��
        bool Checked { get;}
        //����ƶ�����ť��ʱ״̬�����ֵ�����
        string Message { get;}
        bool WriteLog { get; set; } 
        //���Ӱ�ť������ʱ���״̬������
        void ClearMessage();
        //��������
        void OnClick();
    }
    //���������ť�ӿڲ��
    public interface IToolRef : IPlugin
    {
        //����ƶ�����ť��ʱ���ֵ�����
        string Tooltip { get;}
        //ͼ��
        Image Image { get;}
        //�������
        string Category { get;}
        //�Ƿ�ѡ��
        bool Checked { get;}
        //����ƶ�����ť��ʱ״̬�����ֵ�����
        string Message { get;}
        //���Ӱ�ť������ʱ���״̬������
        void ClearMessage();
        //������ʾ��ʽ
        Cursor Cursor { get;}
        //����״̬����
        bool Deactivate { get;}
        //������Ӧ�¼�
        void OnClick();
        //˫����Ӧ�¼�
        void OnDblClick();
        //�Ҽ��˵�������Ӧ�¼�
        bool OnContextMenu(int x, int y);
        //����ƶ���Ӧ�¼�
        void OnMouseMove(int button, int shift, int x, int y);
        //��갴����Ӧ�¼�
        void OnMouseDown(int button, int shift, int x, int y);
        //��굯����Ӧ�¼�
        void OnMouseUp(int button, int shift, int x, int y);
        //ˢ����Ӧ�¼�
        void Refresh(int hDc);
        //���̰�ť������Ӧ�¼�
        void OnKeyDown(int keycode, int shift);
        //���̰�ť������Ӧ�¼�
        void OnKeyUp(int keycode, int shift);
        bool WriteLog { get; set; }    
       
    }
    //�˵����ӿڲ��
    public interface IMenuRef : IPlugin
    {
        //�˵����ϵ�������
        long ItemCount { get;}
        //�������
        void GetItemInfo(int pos, GetITemRef itemref);
    }
    //�������ӿڲ��
    public interface IToolBarRef : IPlugin
    {
        //ͣ�����ӿؼ�
        Control ChildHWND { get;}
        //�������ϵ�������
        long ItemCount { get;}
        //�������
        void GetItemInfo(int pos, GetITemRef itemref);
    }
    #region �˵����򹤾����ϵ���
    public interface IITemRef
    {
        //�Ƿ���һ����
        bool Group { get;set;}
        //ID
        string ID { get;set;}
        //����
        long SubType { get;set;}

    }
    public class GetITemRef : IITemRef
    {
        bool v_Group;
        string v_Id;
        long v_SubType;
        public bool Group
        {
            get
            {
                return this.v_Group;
            }
            set
            {
                this.v_Group = value;
            }
        }
        public string ID
        {
            get
            {
                return this.v_Id;
            }
            set
            {
                this.v_Id = value;
            }
        }
        public long SubType
        {
            get
            {
                return this.v_SubType;
            }
            set
            {
                this.v_SubType = value;
            }
        }
    }
    #endregion
    //��������ӿڲ��
    public interface IDockableWindowRef : IPlugin
    {
        //ͣ�����ӿؼ�
        Control ChildHWND { get;}
        //�ر���Ӧ�¼�
        void OnDestroy();
    }
    //�û��Զ���ؼ��ӿڲ��
    public interface IControlRef : IPlugin
    {
        //�������ݷ���
        void LoadData();
    }
    #endregion
    #region ���ʵ�ֳ����ඨ��
    public abstract class CommandRefBase : ICommandRef
    {
        protected string _Name;
        protected string _Caption;
        protected string _Tooltip;
        protected Image _Image;
        protected string _Category;
        protected bool _Checked;
        protected bool _Visible;
        protected bool _Enabled;
        protected string _Message;
        protected bool _Writelog;
        #region ICommandRef ��Ա
        public virtual string Name
        {
            get{ return _Name;}
        }
        public virtual string Caption
        {
            get{return _Caption;}
        }
        public virtual string Tooltip
        {
            get{return _Tooltip;}
        }
        public virtual Image Image
        {
            get{return _Image;}
        }
        public virtual string Category
        {
            get{return _Category;}
        }
        public virtual bool Checked
        {
            get{return _Checked;}
        }
        public virtual bool Visible
        {
            get{return _Visible;}
        }
        public virtual bool Enabled
        {
            get{return _Enabled;}
        }
        public virtual string Message
        {
            get{return _Message;}
        }
        public virtual bool WriteLog
        {
            get {return _Writelog;}
            set{_Writelog = value;}
        }
        public virtual void ClearMessage()
        {
        }
        public virtual void OnClick()
        {
        }
        public virtual void OnCreate(IApplicationRef hook)
        {
        }
        #endregion
    }
    public abstract class ToolRefBase : IToolRef
    {
        protected string _Name;
        protected string _Caption;
        protected string _Tooltip;
        protected Image _Image;
        protected string _Category;
        protected bool _Checked;
        protected bool _Visible;
        protected bool _Enabled;
        protected string _Message;
        protected Cursor _Cursor;
        protected bool _Deactivate;
        protected bool _Writelog;

        #region IToolRef ��Ա
        public virtual string Name
        {
            get
            {
                return _Name;
            }
        }
        public virtual string Caption
        {
            get
            {
                return _Caption;
            }
        }
        public virtual string Tooltip
        {
            get
            {
                return _Tooltip;
            }
        }
        public virtual Image Image
        {
            get
            {
                return _Image;
            }
        }
        public virtual string Category
        {
            get
            {
                return _Category;
            }
        }
        public virtual bool Checked
        {
            get
            {
                return _Checked;
            }
        }
        public virtual bool Visible
        {
            get
            {
                return _Visible;
            }
        }
        public virtual bool Enabled
        {
            get
            {
                return _Enabled;
            }
        }
        public virtual string Message
        {
            get
            {
                return _Message;
            }
        }
        public virtual void ClearMessage()
        {
        }
        public virtual Cursor Cursor
        {
            get
            {
                return _Cursor;
            }
        }
        public virtual bool Deactivate
        {
            get
            {
                return _Deactivate;
            }
        }
        public virtual void OnClick()
        {

        }
        public virtual void OnCreate(IApplicationRef hook)
        {

        }
        public virtual void OnDblClick()
        {

        }
        public virtual bool OnContextMenu(int x, int y)
        {
            return false;
        }
        public virtual void OnMouseMove(int button, int shift, int x, int y)
        {

        }
        public virtual void OnMouseDown(int button, int shift, int x, int y)
        {

        }
        public virtual void OnMouseUp(int button, int shift, int x, int y)
        {

        }
        public virtual void Refresh(int hDc)
        {

        }
        public virtual void OnKeyDown(int keycode, int shift)
        {

        }
        public virtual void OnKeyUp(int keycode, int shift)
        {

        }
        public virtual bool WriteLog
        {
            get
            {
                return _Writelog;
            }
            set 
            {
                _Writelog = value;
            }
        }
        #endregion
    }
    public abstract class MenuRefBase : IMenuRef
    {
        protected string _Name;
        protected string _Caption;
        protected bool _Visible;
        protected bool _Enabled;
        protected long _ItemCount;
        #region IMenuRef ��Ա
        public virtual string Name
        {
            get
            {
                return _Name;
            }
        }
        public virtual string Caption
        {
            get
            {
                return _Caption;
            }
        }
        public virtual bool Visible
        {
            get
            {
                return _Visible;
            }
        }
        public virtual bool Enabled
        {
            get
            {
                return _Enabled;
            }
        }
        public virtual long ItemCount
        {
            get
            {
                return _ItemCount;
            }
        }
        public virtual void GetItemInfo(int pos, GetITemRef itemref)
        {

        }
        public virtual void OnCreate(IApplicationRef hook)
        {

        }
        #endregion
    }
    public abstract class ToolBarRefBase : IToolBarRef
    {
        protected string _Name;
        protected string _Caption;
        protected Control _ChildHWND;
        protected bool _Visible;
        protected bool _Enabled;
        protected long _ItemCount;
        #region IToolBarRef ��Ա
        public virtual string Name
        {
            get
            {
                return _Name;
            }
        }
        public virtual string Caption
        {
            get
            {
                return _Caption;
            }
        }
        public virtual bool Visible
        {
            get
            {
                return _Visible;
            }
        }
        public virtual bool Enabled
        {
            get
            {
                return _Enabled;
            }
        }
        public virtual long ItemCount
        {
            get
            {
                return _ItemCount;
            }
        }
        public virtual Control ChildHWND
        {
            get
            {
                return _ChildHWND;
            }
        }
        public virtual void GetItemInfo(int pos, GetITemRef itemref)
        {

        }
        public virtual void OnCreate(IApplicationRef hook)
        {

        }
        #endregion
    }
    public abstract class DockableWindowRefBase : IDockableWindowRef
    {
        protected string _Name;
        protected string _Caption;
        protected Control _ChildHWND;
        protected object _UserData;
        protected bool _Visible;
        protected bool _Enabled;
        #region IDockableWindowRef ��Ա
        public virtual bool Visible
        {
            get { return _Visible; }
        }
        public virtual bool Enabled
        {
            get { return _Enabled; }
        }
        public virtual string Name
        {
            get
            {
                return _Name;
            }
        }
        public virtual string Caption
        {
            get
            {
                return _Caption;
            }
        }
        public virtual Control ChildHWND
        {
            get
            {
                return _ChildHWND;
            }
        }
        public virtual object UserData
        {
            get
            {
                return _UserData;
            }
        }
        public virtual void OnCreate(IApplicationRef hook)
        {

        }
        public virtual void OnDestroy()
        {

        }
        #endregion
    }
    public abstract class ControlRefBase : IControlRef
    {
        protected string _Name;
        protected string _Caption;
        protected bool _Visible;
        protected bool _Enabled;
        #region IControlRef ��Ա
        public virtual string Name
        {
            get
            {
                return _Name;
            }
        }
        public virtual string Caption
        {
            get
            {
                return _Caption;
            }
        }
        public virtual bool Visible
        {
            get
            {
                return _Visible;
            }
        }
        public virtual bool Enabled
        {
            get
            {
                return _Enabled;
            }
        }
        public virtual void OnCreate(IApplicationRef hook)
        {

        }
        public virtual void LoadData()
        {

        }
        #endregion
    }
    #endregion
}
