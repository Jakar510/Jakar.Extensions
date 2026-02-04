using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using Jakar.Extensions;
using Newtonsoft.Json;
using Jakar.Extensions.UserLong;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Serilog.Context;
using Serilog.Events;
using ZLinq;
using ZLinq.Linq;
using static Jakar.Extensions.Validate;
using static Jakar.Extensions.Constants; 
using Debug = System.Diagnostics.Debug;
using PureAttribute = System.Diagnostics.Contracts.PureAttribute;


namespace Jakar.Extensions;


public static class DrawingSizeExtensions
{
    [RequiresDynamicCode(nameof(GetScaledSizes))] public static IEnumerable<(TEnum, Size)> GetScaledSizes<TEnum>( this int baseSize, params TEnum[] scales )
        where TEnum : unmanaged, Enum => new Size(baseSize, baseSize).GetScaledSizes(scales);


    extension( Size baseSize )
    {
        [RequiresDynamicCode(nameof(GetScaledSizes))] public IEnumerable<(TEnum, Size)> GetScaledSizes<TEnum>( params TEnum[] scales )
            where TEnum : unmanaged, Enum
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach ( TEnum scale in scales )
            {
                int value = scale.GetEnumValue<int, TEnum>();
                yield return ( scale, baseSize.Scaled(value) );
            }
        }
        public Size Scaled( int value ) => new(baseSize.Width * value, baseSize.Height * value);
    }
}
