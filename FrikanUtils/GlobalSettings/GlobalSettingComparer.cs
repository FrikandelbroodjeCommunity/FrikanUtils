using System;
using System.Collections.Generic;

namespace FrikanUtils.GlobalSettings;

public class GlobalSettingComparer : IComparer<IGlobalSetting>
{
    public int Compare(IGlobalSetting x, IGlobalSetting y)
    {
        if (ReferenceEquals(x, y)) return 0;
        if (ReferenceEquals(null, y)) return 1;
        if (ReferenceEquals(null, x)) return -1;
        return string.Compare(x.Label, y.Label, StringComparison.Ordinal);
    }
}