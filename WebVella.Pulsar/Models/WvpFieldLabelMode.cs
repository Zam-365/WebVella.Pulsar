﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace WebVella.Pulsar.Models
{
    public enum WvpFieldLabelMode
    {
		[Description("Stacked")]
		Stacked = 1,
		[Description("Horizontal")]
		Horizontal = 2,
		[Description("Hidden")]
		Hidden = 3
    }
}
