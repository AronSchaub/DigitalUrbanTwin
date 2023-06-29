using System;
using System.IO;
using Godot;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using FileAccess = Godot.FileAccess;

namespace Leipzig3DGodot.Scripts;

///<author email="aron.schaub@stud.htwk-leipzig.de$">Aron Schaub</author>
public partial class LogConfigurator : Node
{
    public string Name { get; set; }

    public override void _Ready()
    {
        BasicConfigurator.Configure(new GdAppender());
        // FileAccess? fileAccess = FileAccess.Open("res://Resource/log4net.Xml", FileAccess.ModeFlags.Read);
        // XmlConfigurator.Configure(new MemoryStream(fileAccess.GetBuffer((long)fileAccess.GetLength())));
    }
}

class GdAppender : AppenderSkeleton
{
    protected override void Append(LoggingEvent loggingEvent)
    {
        switch (loggingEvent.Level.Name)
        {
            case "DEBUG":
            case "INFO":
                GD.Print($"[{loggingEvent.LoggerName}] {loggingEvent.RenderedMessage}");
                break;
            case "WARN":
                GD.PushWarning($"[{loggingEvent.LoggerName}] {loggingEvent.RenderedMessage}");
                break;
            case "ERROR":
            case "FATAL":
                GD.PushError($"[{loggingEvent.LoggerName}] {loggingEvent.RenderedMessage}");
                break;
        }
    }
}