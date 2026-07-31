using System;
using UnityEngine;

namespace PiRhoSoft.Utilities
{
    public static class ApplicationHelper
    {
        public static Func<bool> IsPlayingOverride { private get; set; }

        public static bool IsPlaying => IsPlayingOverride?.Invoke() ?? Application.isPlaying;
    }
}
