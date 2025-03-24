using System.Collections.Generic;
using System;

namespace Game.Custom.Debug;


public class DebugCommands
{
    private readonly Dictionary<string, Action> actions = [];
    private readonly List<string> ConsoleHistory = [];

    public bool RunCommand(string command)
    {
        ConsoleHistory.Add(command);
        if (actions.TryGetValue(command, out Action action))
        {
            action.Invoke();
            return true;
        }
        return false;
    }

    public void RemoveCommand(string command) => actions.Remove(command);
    public void AddCommand(string command, Action action) => actions.Add(command, action);
}
