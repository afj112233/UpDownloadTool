using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Globalization;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.SourceProtection;
using Microsoft.VisualStudio.Shell;

namespace ICSStudio.OrganizerPackage.Model
{
    internal class AddOnInstructionInfo : BaseSimpleInfo
    {
        private readonly AoiDefinition _aoiDefinition;

        public AddOnInstructionInfo(AoiDefinition aoiDefinition, ObservableCollection<SimpleInfo> infoSource)
            : base(infoSource)
        {
            _aoiDefinition = aoiDefinition;

            if (_aoiDefinition != null)
            {
                CreateInfoItems();

                PropertyChangedEventManager.AddHandler(_aoiDefinition,
                    OnAoiPropertyChanged, string.Empty);

                if (_aoiDefinition.datatype != null)
                {
                    PropertyChangedEventManager.AddHandler(_aoiDefinition.datatype,
                        OnAoiDataTypeChanged, string.Empty);
                }
            }
        }

        private void OnAoiDataTypeChanged(object sender, PropertyChangedEventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                if (e.PropertyName == "ByteSize")
                {
                    SetSimpleInfo("Data Type Size", $"{_aoiDefinition.datatype.ByteSize.ToString()} bytes");
                }
            });
        }

        private void OnAoiPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                if (e.PropertyName == "Description")
                {
                    SetSimpleInfo("Description", _aoiDefinition.Description);
                }

                if (e.PropertyName == "Revision")
                {
                    SetSimpleInfo("Revision", $"v{_aoiDefinition.Revision}");
                }

                if (e.PropertyName == "RevisionNote")
                {
                    SetSimpleInfo("Revision note", _aoiDefinition.RevisionNote);
                }

                if (e.PropertyName == "Vendor")
                {
                    SetSimpleInfo("Vendor", _aoiDefinition.Vendor);
                }

                if (e.PropertyName == "CreatedDate")
                {
                    SetSimpleInfo("Created", _aoiDefinition.CreatedDate.ToString(CultureInfo.CurrentCulture));
                }

                if (e.PropertyName == "CreatedBy")
                {
                    SetSimpleInfo("Created By", _aoiDefinition.CreatedBy);
                }

                if (e.PropertyName == "EditedDate")
                {
                    SetSimpleInfo("Edited", _aoiDefinition.EditedDate.ToString(CultureInfo.CurrentCulture));
                }

                if (e.PropertyName == "EditedBy")
                {
                    SetSimpleInfo("Edited By", _aoiDefinition.EditedBy);
                }

                if (e.PropertyName == "IsSealed" ||
                    e.PropertyName == "SignatureID" ||
                    e.PropertyName == "EditedDate")
                {
                    string signatureID = "<none>";
                    if (_aoiDefinition.IsSealed)
                    {
                        signatureID = _aoiDefinition.SignatureID + ", " + _aoiDefinition.EditedDate;
                    }

                    SetSimpleInfo("Signature ID", signatureID);
                }

                if (e.PropertyName == "IsEncrypted")
                {
                    if (_aoiDefinition.IsEncrypted)
                    {
                        CreateProtectionInfoItems();
                    }
                    else
                    {
                        RemoveProtectionInfoItems();
                    }
                }
            });
        }

        private void CreateInfoItems()
        {
            if (InfoSource != null)
            {
                CreateDefaultInfoItems();

                if (_aoiDefinition.IsEncrypted)
                {
                    CreateProtectionInfoItems();
                }
            }
        }

        private void CreateDefaultInfoItems()
        {
            string signatureID = "<none>";
            if (_aoiDefinition.IsSealed)
            {
                signatureID = _aoiDefinition.SignatureID + ", " + _aoiDefinition.EditedDate;
            }

            InfoSource.Add(new SimpleInfo { Name = "Description", Value = _aoiDefinition.Description });
            InfoSource.Add(new SimpleInfo { Name = "Revision", Value = $"v{_aoiDefinition.Revision}" });
            InfoSource.Add(new SimpleInfo { Name = "Revision Note", Value = _aoiDefinition.RevisionNote });
            InfoSource.Add(new SimpleInfo { Name = "Vendor", Value = _aoiDefinition.Vendor, Key = "AoiVendor" });
            InfoSource.Add(new SimpleInfo
                { Name = "Data Type Size", Value = $"{_aoiDefinition.datatype.ByteSize.ToString()} bytes" });
            InfoSource.Add(new SimpleInfo
                { Name = "Created", Value = _aoiDefinition.CreatedDate.ToString(CultureInfo.CurrentCulture) });
            InfoSource.Add(new SimpleInfo { Name = "Created By", Value = _aoiDefinition.CreatedBy });
            InfoSource.Add(new SimpleInfo
                { Name = "Edited", Value = _aoiDefinition.EditedDate.ToString(CultureInfo.CurrentCulture) });
            InfoSource.Add(new SimpleInfo { Name = "Edited By", Value = _aoiDefinition.EditedBy });
            InfoSource.Add(new SimpleInfo { Name = "Signature Id", Value = signatureID });
        }

        private void CreateProtectionInfoItems()
        {
            var controller = _aoiDefinition.ParentController as Controller;
            var manager = controller?.SourceProtectionManager;
            if (manager != null)
            {
                // Type, Name, Permissions
                string protectionType = "Source Key";
                string protectionName = string.Empty;
                string protectionPermissions = string.Empty;

                var permission = manager.GetPermission(_aoiDefinition);
                if (permission == SourcePermission.Use)
                {
                    protectionName = "";
                    protectionPermissions = "Use";
                }
                else if (permission == SourcePermission.All)
                {
                    protectionName = manager.GetDisplayNameByKey(manager.GetKeyBySource(_aoiDefinition));
                    protectionPermissions = "Protect, Edit, Copy, Export, View, Use";
                }
                else
                {
                    Contract.Assert(false);
                }

                InfoSource.Add(new SimpleInfo { Name = "Protection Type", Value = protectionType });
                InfoSource.Add(new SimpleInfo { Name = "Protection Name", Value = protectionName });
                InfoSource.Add(new SimpleInfo { Name = "Protection Permissions", Value = protectionPermissions });
            }
        }

        private void RemoveProtectionInfoItems()
        {
            InfoSource.Remove(GetSimpleInfo("Protection Type"));
            InfoSource.Remove(GetSimpleInfo("Protection Name"));
            InfoSource.Remove(GetSimpleInfo("Protection Permissions"));
        }
    }
}
