using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ICSStudio.Interfaces.DataType;
using ICSStudio.Interfaces.Notification;
using ICSStudio.SimpleServices.DataType;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;
using Task = System.Threading.Tasks.Task;

namespace ICSStudio.AvalonEdit.Variable
{
    public partial class VariableInfo
    {
        private readonly SemaphoreSlim _semaphoreSlim=new SemaphoreSlim(1);
        public void Consume(MessageData message)
        {
            if (message.Type == MessageData.MessageType.PullFinished
                || message.Type == MessageData.MessageType.Restored)
            {
                if (_semaphoreSlim.Wait(100))
                {
                    ThreadHelper.JoinableTaskFactory.RunAsync(
                        async delegate
                        {
                            await TaskScheduler.Default;

                            if (_dimTags.Any())
                                _field = null;

                            await SetValue();

                            _semaphoreSlim.Release();
                        });
                }
            }
        }

        internal System.Threading.Tasks.Task SetValue()
        {
            return Task.Run(()=> {
                if (!IsDisplay) return;
                if (IsInstr || !IsCorrect) return;
                //GC.Collect();
                if (_value == "{...}") return;
                if (_value == "??")
                {
                    _field = null;
                }

                try
                {
                    if (_field != null)
                    {
                        if (DataType.FamilyType == FamilyType.StringFamily)
                        {
                            //IsEnabled = false;
                            _value = GetStringData();
                            OnPropertyChanged("Value");
                            return;
                        }

                        if (_index > -1)
                        {
                            var array = _field as BoolArrayField;
                            if (array != null)
                            {
                                _value = array.Get(_index) ? "1" : "0";
                            }
                            else
                            {
                                _value = _field.GetBitValue(_index) ? "1" : "0";
                            }
                        }
                        else
                        {
                            _value = FormatOp.ConvertField(_field, DisplayStyle);
                        }
                    }
                    else
                    {
                        var value = GetTagValue(Name);
                        if ("error".Equals(value))
                        {
                            return;
                        }
                        if (value == null)
                        {
                            _value = "{...}";
                            IsEnabled = false;
                        }
                        else if (value == "??")
                        {
                            _value = "??";
                            IsEnabled = false;
                        }
                        else if (value == "{...}")
                        {
                            _value = "{...}";
                            IsEnabled = false;
                        }
                        else
                        {
                            _value = value;
                            IsEnabled = true;
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.StackTrace);
                    _value = "??";
                    IsEnabled = false;
                }
                OnPropertyChanged("Value");
            });
        }
    }
}
