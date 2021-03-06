﻿using FlaUI.Core;
using FlaUI.Core.Identifiers;
using FlaUI.Core.Patterns;
using FlaUI.Core.Patterns.Infrastructure;
using UIA = System.Windows.Automation;

namespace FlaUI.UIA2.Patterns
{
    public class InvokePattern : PatternBase<UIA.InvokePattern>, IInvokePattern
    {
        public static readonly PatternId Pattern = PatternId.Register(AutomationType.UIA2, UIA.InvokePattern.Pattern.Id, "Invoke");
        public static readonly EventId InvokedEvent = EventId.Register(AutomationType.UIA2, UIA.InvokePattern.InvokedEvent.Id, "Invoked");

        public InvokePattern(BasicAutomationElementBase basicAutomationElement, UIA.InvokePattern nativePattern) : base(basicAutomationElement, nativePattern)
        {
        }

        public IInvokePatternEvents Events => Automation.EventLibrary.Invoke;

        public void Invoke()
        {
            NativePattern.Invoke();
        }
    }

    public class InvokePatternEvents : IInvokePatternEvents
    {
        public EventId InvokedEvent => InvokePattern.InvokedEvent;
    }
}
