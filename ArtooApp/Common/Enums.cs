﻿using System.ComponentModel;

namespace Artoo.Common
{
    public enum MistakeEnum
    {
        ManualMistake = 0,
        DeviceMistake = 1
    }

    public enum InspectionResultEnum
    {
        Reject = 2,
        Accept = 1
    }

    public enum OrderTypeEnum
    {
        Replenishment = 0,
        Implantation = 1
    }

    public enum ApplicationRoleEnum
    {
        [Description("Administrator")]
        Administrator = 0,
        [Description("Factory Manager")]
        Factory_Manager = 1,
        [Description("QPL")]
        QPL = 2,
        [Description("Decathlon Manager")]
        Decathlon_Manager = 3,
        [Description("Manager")]
        Manager = 4,
    }

    public enum TenantEnum
    {
        dpc = 1,
        garmex = 2,
        tng = 3
    }

    public enum ConfigurationEnum
    {
        sendAllMail = 1
    }
}
