// Guids.cs
// MUST match guids.h
using System;

namespace ICSStudio.AboutBoxPackage
{
    static class GuidList
    {
        public const string GuidAboutBoxPackagePkgString = "babd0d66-291a-4434-b14b-926af678588f";
        public const string GuidAboutBoxPackageCmdSetString = "ca47ff76-0a77-405e-bc7a-f0426d0a27d9";

        public static readonly Guid GuidAboutBoxPackageCmdSet = new Guid(GuidAboutBoxPackageCmdSetString);

    };
}