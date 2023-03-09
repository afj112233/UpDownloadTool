using System;
using System.ComponentModel;
using ICSStudio.Cip.Objects;
using ICSStudio.Database.Database;

namespace ICSStudio.Cip.Other
{
    public class IdentityDescriptor
    {
        public IdentityDescriptor(CIPIdentity identity)
        {
            Identity = identity;
        }

        public CIPIdentity Identity { get; }

        public string Revision => Identity.Revision.ToString();

        public string Vendor
        {
            get
            {
                DBHelper dbHelper = new DBHelper();
                return dbHelper.GetVendorName(Convert.ToUInt16(Identity.VendorID));
            }
        }

        public string ProductType
        {
            get
            {
                CipDeviceType deviceType = (CipDeviceType) Convert.ToUInt16(Identity.DeviceType);
                return TypeDescriptor.GetConverter(deviceType).ConvertToString(deviceType);
            }
        }

        public string ProductCode
        {
            get
            {
                DBHelper dbHelper = new DBHelper();
                return dbHelper.GetCatalogNumber(
                    Convert.ToUInt16(Identity.VendorID),
                    Convert.ToUInt16(Identity.DeviceType),
                    Convert.ToUInt16(Identity.ProductCode));
            }
        }

        public string SerialNumber => Convert.ToUInt32(Identity.SerialNumber).ToString("X8");

        public string ProductName => Identity.ProductName?.ToString();


        /*
         * from 5000 help
         * Displays whether there is a major and minor fault.
         * For a digital module, these options are available:
            * EEPROM fault
            * Backplane fault
            * None
         * For an analog module, these options are available:
            * Comm. Lost with owner
            * Channel fault
            * None
         * For other modules, these options are available:
            * None
            * Unrecoverable
            * Recoverable
         */

        public string MajorFault
        {
            get
            {
                ushort status = Convert.ToUInt16(Identity.Status);

                if ((status & (ushort) IdentityStatusBitmap.MajorUnrecoverableFault) > 0)
                {
                    return "Unrecoverable";
                }

                if ((status & (ushort) IdentityStatusBitmap.MajorRecoverableFault) > 0)
                {
                    return "Recoverable";
                }

                return "None";
            }
        }

        public string MinorFault
        {
            get
            {
                ushort status = Convert.ToUInt16(Identity.Status);

                if ((status & (ushort) IdentityStatusBitmap.MinorUnrecoverableFault) > 0)
                {
                    return "Unrecoverable";
                }

                if ((status & (ushort) IdentityStatusBitmap.MinorRecoverableFault) > 0)
                {
                    return "Recoverable";
                }

                return "None";
            }
        }

        public string InternalState
        {
            get
            {
                // from PSDIOAdapterCommonCIPObjects.xml
                //<Attribute Name="Internal State" DataType="BITFIELD" HiBit="7" LowBit="4" Target="Status" EnumID="IntrnlStateCode" SupportsRefresh="True"/>

                ushort status = Convert.ToUInt16(Identity.Status);

                ushort internalState = (ushort) ((status >> 4) & 0xF);

                switch (internalState)
                {
                    case 0:
                        return "Self-test";
                    case 1:
                        return "Flash update";
                    case 2:
                        return "Communication fault";
                    case 3:
                        return "Unconnected";
                    case 4:
                        return "Flash configuration bad";
                    case 5:
                        return "Major fault";
                    case 6:
                        return "Run mode";
                    case 7:
                        return "Program mode";

                    default:
                        return $"(16#{internalState:X4}) unknown";
                }

            }
        }

        public string Configured
        {
            get
            {
                ushort status = Convert.ToUInt16(Identity.Status);

                return (status & (ushort) IdentityStatusBitmap.Configured) > 0 ? "Configured" : "No";
            }
        }

        public string Owned
        {
            get
            {
                ushort status = Convert.ToUInt16(Identity.Status);

                return (status & (ushort) IdentityStatusBitmap.Owned) > 0 ? "Owned" : "No";
            }
        }

        public string ProtectionMode
        {
            get
            {
                ushort mode = Convert.ToUInt16(Identity.ProtectionMode);

                if ((mode & 1) > 0)
                    return "Implicit";

                if ((mode & (1 << 3)) > 0)
                    return "Explicit";

                return "None";
            }
        }

        public string GetModuleIdentity(int vendor, int productCode, int productType, int majorRevision)
        {
            // Match
            // Vendor, Module Type(Product Type and Code), Major Revision

            if (Convert.ToUInt16(Identity.VendorID) == vendor
                && Convert.ToUInt16(Identity.ProductCode) == productCode
                && Convert.ToUInt16(Identity.DeviceType) == productType
                && Identity.Revision.Major == majorRevision)

                return "Match";

            return "Mismatch";


        }
    }
}
